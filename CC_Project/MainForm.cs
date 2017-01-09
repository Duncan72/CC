/* 
 * The code for running Connection Cartographer.
 * Please contact Michael Duncan <duncan.72@wright.edu> if you have any questions.
 */

using SharpPcap;
using SharpPcap.WinPcap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System.Timers;
using System.Diagnostics;

namespace ConnectionCartographer
{
    public partial class MainForm : Form
    {
        #region Private Variables and Delegates
        //----------------------------------------------------------------------------

        private static bool running = true; // Is the application currently running?
                                            // This is used to end packet capture after the application is closed.
        private static bool initialized = false;    // Has the packet capture initialization process been completed?

        private static List<string> interfaceIPs = new List<string>();      // The list of the interface IP addresses
        private static List<string> interfaceIPsV6 = new List<string>();    // The list of the interface IPv6 addresses

        // Lookup services for obtaining authoritative service and city data from an IP address
        private static LookupService asLookup;
        private static LookupService cityLookup;
        private static LookupService asLookupIPv6;
        private static LookupService cityLookupIPv6;

        // Locks for handling multi-threading
        private static object lookupLock = new object();
        private static object connectionLinesLock = new object();
        private static object fadeoutLock = new object();
        private static object packetQueueLock = new object();

        private static List<ConnectionCartographer.NetStatPortProcess.Port> openSockets;    // The list of the currently open network sockets
        private static List<Connection> connections = new List<Connection>();   // The list of witnessed network connections
        private static Dictionary<int, ConnectionLine> connectionLines = new Dictionary<int, ConnectionLine>(); // A dictionary of the connection lines to display
        private static Dictionary<string, int> flagDictionary = new Dictionary<string, int>();  // A dictionary used to identify country flags
        private static Dictionary<int, Connection> activeConnections = new Dictionary<int, Connection>();   // A dictionary of the recently active connections
        private static HashSet<String> seenIPs = new HashSet<String>(); // A hashset of the IP addresses witnessed
        private static List<RawCapture> packetQueue = new List<RawCapture>(); // The queue of raw packet data

        private static int previousSelectedMarkerIndex = -1;    // The index of the previously selected marker
        private static string hostIP = "";  // The public IP Address of the user
        private static int packetQueueSize = 0; // The current network packet queue size

        // The map and overlays
        private static GMapControl map;
        private static GMapOverlay markerOverlay;   // Overlay for the markers
        private static GMapOverlay lineOverlay;     // Overlay for the lines

        // Timers used for periodically updating the application
        private static System.Timers.Timer lineTimer;
        private static System.Timers.Timer lineAnimationTimer;
        private static System.Timers.Timer transparencyTimer;
        private static System.Timers.Timer socketReaderTimer;
        private static System.Timers.Timer dataQueueTextTimer;

        // Constant values
        private const double LINETIMERINTERVAL = 500; // How often should connection lines be updated? (milliseconds)
        private const double LINEANIMATIONTIMERINTERVAL = 50; // How often do the connection lines update their animation? (milliseconds)
        private const double LINEDURATION = 2001; // How long does a connection line stay visible? (milliseconds)
        private const float LINEANIMATIONSPEED = 1; // How quickly do the connection lines animate? (1 is base)
        private const double FADEOUTINTERVAL = 200; // How often should the fadout occur? (milliseconds)
        private const double FADEOUTDURATION = 60000; // How long does it take for an object to reach max fade out? (milliseconds)
        private const double SOCKETREADINTERVAL = 5000; // How often should the open sockets be read? (milliseconds)
        private const double DATAQUEUETEXTINTERVAL = 500; // How often should the data queue text update? (milliseconds)
        private const int MAXPACKETQUEUESIZE = 500; // What is the maximum number of network packets to queue before non-essential packets are dropped?
      
        private static HelpWindow helpWindow = new HelpWindow();    // The help window
        private static System.Threading.Thread packetThread;    // The thread for packet capture
        private static MainForm thisForm;

        // Delegates used for updating the application with multi-threading
        delegate void DisplayConnectionCallback(Connection connection);
        delegate void DisplayDetailsCallback();
        delegate void UpdateConnectionCallback(Connection connection, int index);
        delegate void UpdateDetailsCallback();
        delegate void UpdateListFadeoutCallback(int id);
        delegate void UpdateMarkerCallback(int id, float alpha);
        delegate void UpdateDataQueueCallback(int queueSize);

        //----------------------------------------------------------------------------
        #endregion

        #region Constructor
        //----------------------------------------------------------------------------

        /**
         * The MainForm constructor enables double buffering to remove
         * flicker from the list view, and it also starts the packet
         * capture worker thread and adds gmap overlays.
         */
        public MainForm()
        {
            InitializeComponent();

            // Set the private form variable to this form
            thisForm = this;

            // Declare a worker thread for packet capture
            Thread workerThread = null;

            // Remove flickering from the list views
            DoubleBuffering.doubleBuffering(listViewConnections, true);
            DoubleBuffering.doubleBuffering(listViewDetails, true);

            // Add overlays
            lineOverlay = new GMapOverlay("lineOverlay");
            gmap.Overlays.Add(lineOverlay);
            markerOverlay = new GMapOverlay("markerOverlay");
            gmap.Overlays.Add(markerOverlay);
            map = gmap;

            // Initialise and start the packet capture worker thread
            workerThread = new Thread(new ThreadStart(this.packetCapture));
            workerThread.Start();
        }

        //----------------------------------------------------------------------------
        #endregion

        #region Packet Capture
        //----------------------------------------------------------------------------

        /**
         *  Set up the packet capture to occur in the background.
         */
        private void packetCapture()
        {
            // Open the geoIP databases
            asLookup = new LookupService("./Databases/GeoIPASNum.dat", LookupService.GEOIP_MEMORY_CACHE);
            cityLookup = new LookupService("./Databases/GeoLiteCity.dat", LookupService.GEOIP_MEMORY_CACHE);
            asLookupIPv6 = new LookupService("./Databases/GeoIPASNumv6.dat", LookupService.GEOIP_MEMORY_CACHE);
            cityLookupIPv6 = new LookupService("./Databases/GeoLiteCityv6.dat", LookupService.GEOIP_MEMORY_CACHE);

            // Before we start packet capture, get the host location while showing the loading screen
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.Update();
            // TODO: Handle this better if getting the IP fails
            try
            {
                hostIP = GetPublicIP.getPublicIP(); // Get the host IP address
            }
            catch (System.Exception)
            {
                ErrorExit();
            }
            loadingWindow.Close();
            // Mark the host location on the map
            try
            {
                Location location;

                // Is the IP address v4 or v6?
                if (hostIP.Contains(':'))
                {
                    location = cityLookupIPv6.getLocation(hostIP);
                }
                else
                {
                    location = cityLookup.getLocation(hostIP);
                }

                // Add a marker for the host location
                lock (fadeoutLock)
                {
                    Bitmap icon = new Bitmap("./Images/home_marker.png"); // Icon from https://www.iconfinder.com/icons/68010/favorite_location_pin_star_start_icon
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(location.latitude, location.longitude), icon);
                    markerOverlay.Markers.Add(marker);
                }
            }
            catch (System.Exception)
            {
                // TODO: Handle being unable to get the host location
            }

            // Get the list of open sockets
            openSockets = NetStatPortProcess.getNetStatPorts();

            // Retrieve the network device list
            var devices = CaptureDeviceList.Instance;

            // If no network devices exist, then exit
            if (devices.Count < 1)
            {
                Console.WriteLine("No network device has been found on this machine");
                // TODO: Quit and display a more appropriate error
                ErrorExit();
                return;
            }

            // Setup each network device
            foreach (var device in devices)
            {
                // Add the device IP to the list of interface IPs
                foreach (var ipAddress in ((WinPcapDevice)device).Addresses)
                {
                    if (ipAddress.Addr != null)
                    {
                        // If this is NOT a hardware address
                        if (!ipAddress.Addr.ToString().Contains("HW"))
                        {
                            // If this is IPv6
                            if (ipAddress.Addr.ToString().Contains(':'))
                            {
                                interfaceIPsV6.Add(ipAddress.Addr.ToString());
                            }
                            // Else it is IPv4
                            else
                            {
                                interfaceIPs.Add(ipAddress.Addr.ToString());
                            }
                        }
                    }
                }

                // Register our handler function to the packet arrival event
                device.OnPacketArrival +=
                    new PacketArrivalEventHandler(device_OnPacketArrival);

                // Open the device for capturing
                int readTimeoutMilliseconds = 1000;
                device.Open(DeviceMode.Normal, readTimeoutMilliseconds);

                // tcpdump filter to capture only IP packets
                string filter = "ip or ip6";
                device.Filter = filter;

                // Start capture 'INFINTE' number of packets
                device.StartCapture();
            }

            initialized = true; // The packet capture has been initialized

            // Start up the packet processing thread
            packetThread = new System.Threading.Thread(processPackets);
            packetThread.Priority = ThreadPriority.Highest;
            packetThread.Start();

            // Wait while the form is open
            while (running) ;

            // Close the pcap device(s)
            foreach (var device in devices)
            {
                device.StopCapture();
                device.Close();
            }
            
            // End the packet processing thread
            packetThread.Abort();
        }

        /**
         * Analyze all of the packets currently in the queue.
         * If the queue is empty, then wait until it fills up again.
         */
        private static void processPackets()
        {
            while (running)
            {
                bool shouldSleep = true;

                lock (packetQueueLock)
                {
                    if (packetQueue.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if (shouldSleep)
                {
                    System.Threading.Thread.Sleep(100);
                }
                else
                {
                    List<RawCapture> thisQueue;
                    lock (packetQueueLock)
                    {
                        // swap queues
                        thisQueue = packetQueue;
                        packetQueue = new List<RawCapture>();
                    }

                    int packetsProcessed = 0;

                    foreach (var packetData in thisQueue)
                    {
                        try
                        {
                            var time = packetData.Timeval.Date;
                            var len = packetData.Data.Length;

                            var packet = PacketDotNet.Packet.ParsePacket(packetData.LinkLayerType, packetData.Data);

                            // Get the IP packet details
                            var ipPacket = PacketDotNet.IpPacket.GetEncapsulated(packet);
                            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
                            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;

                            // Determine if this is an outgoing packet
                            bool outgoing = false;
                            if (interfaceIPs.Contains(srcIp.ToString()) || interfaceIPsV6.Contains(srcIp.ToString()))
                            {
                                outgoing = true;
                            }

                            // Determine the host connection IP and the external connection IP
                            string hostIP;
                            string externalIP;

                            if (outgoing)
                            {
                                hostIP = srcIp.ToString();
                                externalIP = dstIp.ToString();
                            }
                            else
                            {
                                hostIP = dstIp.ToString();
                                externalIP = srcIp.ToString();
                            }

                            // Get TCP/UDP information
                            var tcpPacket = PacketDotNet.TcpPacket.GetEncapsulated(packet);
                            var udpPacket = PacketDotNet.UdpPacket.GetEncapsulated(packet);
                            int hostPort = -1;
                            int externalPort = -1;

                            // Determine if this packet is a TCP or UDP packet
                            bool isTCP = false;
                            bool isUDP = false;

                            // Determine the host connection port and the external connection port
                            if (tcpPacket != null)
                            {
                                isTCP = true;

                                if (outgoing)
                                {
                                    hostPort = tcpPacket.SourcePort;
                                    externalPort = tcpPacket.DestinationPort;
                                }
                                else
                                {
                                    hostPort = tcpPacket.DestinationPort;
                                    externalPort = tcpPacket.SourcePort;
                                }
                            }
                            else if (udpPacket != null)
                            {
                                isUDP = true;

                                if (outgoing)
                                {
                                    hostPort = udpPacket.SourcePort;
                                    externalPort = udpPacket.DestinationPort;
                                }
                                else
                                {
                                    hostPort = udpPacket.DestinationPort;
                                    externalPort = udpPacket.SourcePort;
                                }
                            }

                            // Check if this is an existing connection
                            bool exists = false;
                            int index = 0;
                            foreach (Connection connection in connections)
                            {
                                bool exit = false;

                                if (connection.ipList.Contains(externalIP))
                                {
                                    if (connection.hostPorts.Contains(hostPort))
                                    {
                                        exists = true;

                                        // Update the timestamp, opacity, bytes sent/received, and line visibility
                                        updateTOBV(connection, time, outgoing, len);

                                        // Update the duration
                                        updateDuration(connection);

                                        // Update the details view, but only if we need to
                                        if (connection.id == (previousSelectedMarkerIndex - 1))
                                        {
                                            thisForm.updateDetailsView();
                                        }

                                        exit = true;

                                        break;
                                    }
                                }

                                if (exit)
                                {
                                    break;
                                }

                                index++;
                            }

                            // The connection was not found to exist using the port IP pair,
                            // so get the process and check with that
                            string processName = "?";
                            if (isUDP || isTCP)
                            {
                                if (!exists)
                                {
                                    // Get the list of open sockets
                                    openSockets = NetStatPortProcess.getNetStatPorts();

                                    // Get the process name of the process associated with the host port
                                    bool found = false;
                                    foreach (var port in openSockets)
                                    {
                                        if (isTCP)
                                        {
                                            if (port.protocol.Contains("TCP") && hostPort.ToString().Equals(port.port_number))
                                            {
                                                processName = port.process_name;
                                                found = true;
                                                break;
                                            }
                                        }
                                        else if (isUDP)
                                        {
                                            if (port.protocol.Contains("UDP") && hostPort.ToString().Equals(port.port_number))
                                            {
                                                processName = port.process_name;
                                                found = true;
                                                break;
                                            }
                                        }
                                    }

                                    // If the process is "Idle", change it to "?"
                                    if (processName == "Idle")
                                    {
                                        processName = "?";
                                    }

                                    // Use the process to check if this is an existing connection
                                    if (found)
                                    {
                                        index = 0;
                                        foreach (Connection connection in connections)
                                        {
                                            bool exit = false;

                                            if (connection.ipList.Contains(externalIP))
                                            {
                                                if (((connection.processName == processName) || (processName == "?")) ||
                                                   ((connection.processName == "?") && (processName != "?")))
                                                {
                                                    exists = true;

                                                    // Update the timestamp, opacity, bytes sent/received, and line visibility
                                                    updateTOBV(connection, time, outgoing, len);

                                                    // Update the process name if needed
                                                    if ((connection.processName == "?") && (processName != "?"))
                                                    {
                                                        connection.processName = processName;
                                                        thisForm.updateConnectionsView(connection, index);
                                                    }

                                                    // Update the duration
                                                    updateDuration(connection);

                                                    // Add the hostport to the list of ports
                                                    connection.hostPorts.Add(hostPort);

                                                    thisForm.updateDetailsView();
                                                    exit = true;
                                                    break;
                                                }
                                            }

                                            if (exit)
                                            {
                                                break;
                                            }

                                            index++;
                                        }
                                    }
                                }
                            }

                            // Check if this is a new external IP that goes to the same destination AS
                            // or if this is a new connection entirely that needs to be added
                            if (!exists)
                            {
                                Connection connection = new Connection();

                                // Set the connection ID
                                connection.id = connections.Count;

                                // Set the IP, port, and process name
                                connection.externalIP = externalIP;
                                connection.ipList.Add(externalIP);
                                if (hostPort > -1)
                                {
                                    connection.hostPorts.Add(hostPort);
                                }
                                if (!isUDP && !isTCP)
                                {
                                    processName = "? - Indeterminable";
                                }
                                connection.processName = processName;

                                // Set the timestamps
                                connection.timestampRecent = time.ToLocalTime().Hour + ":" +
                                                    time.ToLocalTime().Minute + ":" + time.ToLocalTime().Second;
                                connection.timestampFirst = connection.timestampRecent;
                                connection.duration = FADEOUTDURATION;

                                // Set the opacity
                                connection.opacity = (float)1.0;

                                // Set the bytes sent/received
                                if (outgoing)
                                {
                                    connection.bytesSent += len;
                                    connection.downloading = false;
                                }
                                else
                                {
                                    connection.bytesReceived += len;
                                    connection.downloading = true;
                                }

                                // Get the geolocation information
                                string asName = null;

                                bool ignore = false;
                                try
                                {
                                    if (ipPacket.Version.ToString().Equals("IPv4"))
                                    {
                                        lock (lookupLock)
                                        {
                                            asName = asLookup.getOrg(dstIp.ToString());
                                            Location location = cityLookup.getLocation(dstIp.ToString());

                                            if (asName != null)
                                            {
                                                connection.asName = asName.Substring(asName.IndexOf(' ') + 1);
                                                connection.asNumber = Int32.Parse(asName.Substring(2, asName.IndexOf(' ') - 2));
                                            }
                                            connection.setLocationInformation(location);
                                        }
                                    }
                                    else
                                    {
                                        lock (lookupLock)
                                        {
                                            asName = asLookupIPv6.getOrgV6(dstIp.ToString());
                                            Location location = cityLookupIPv6.getLocationV6(dstIp.ToString());

                                            if (asName != null)
                                            {
                                                connection.asName = asName.Substring(asName.IndexOf(' ') + 1);
                                                connection.asNumber = Int32.Parse(asName.Substring(2, asName.IndexOf(' ') - 2));
                                            }
                                            connection.setLocationInformation(location);
                                        }
                                    }
                                }
                                catch (System.Exception)
                                {
                                    // If we got here, then there was an error in looking up the city.
                                    // This is almost certainly because the IP address was a reserved IP address.
                                    // Therefore this packet is not relevant to us, since we only care about 
                                    // non-multicast communications with the external world (Internet).
                                    ignore = true;
                                }

                                // Check if we already have a connection for this process / AS pair
                                index = 0;
                                foreach (Connection checkConnection in connections)
                                {
                                    if ((checkConnection.asNumber == connection.asNumber) &&
                                        (((checkConnection.processName == processName) || (processName == "?")) ||
                                        ((checkConnection.processName == "?") && (processName != "?"))))
                                    {
                                        exists = true;

                                        // Update the timestamp, opacity, bytes sent/received, and line visibility
                                        updateTOBV(checkConnection, time, outgoing, len);

                                        // Add this IP to the list of IPs
                                        checkConnection.ipList.Add(externalIP);

                                        // Update the process name if needed
                                        if ((checkConnection.processName == "?") && (processName != "?"))
                                        {
                                            checkConnection.processName = processName;
                                            thisForm.updateConnectionsView(connection, index);
                                        }

                                        // Update the duration
                                        updateDuration(checkConnection);

                                        // Add the host port to the list of ports
                                        checkConnection.hostPorts.Add(hostPort);

                                        thisForm.updateDetailsView();

                                        ignore = true;
                                        break;
                                    }

                                    index++;
                                }

                                if (!ignore)
                                {
                                    // Determine the iso3166 code for the country associated
                                    // with this connection's destination
                                    var isoFileLines = File.ReadLines("./Databases/iso3166.csv");
                                    foreach (String isoFileLine in isoFileLines)
                                    {
                                        if (isoFileLine.Contains("\"" + connection.country + "\""))
                                        {
                                            connection.iso3166Code = isoFileLine.Substring(0, 2);
                                            break;
                                        }
                                    }

                                    // See if the flag for this iso3166 code needs to be added to the image list
                                    if (!flagDictionary.ContainsKey(connection.iso3166Code))
                                    {
                                        // Try to get the flag image
                                        try
                                        {
                                            Bitmap flag = new Bitmap("./Images/Flags/" + connection.iso3166Code + ".png");
                                            flagDictionary.Add(connection.iso3166Code, thisForm.imageListFlags.Images.Count);
                                            thisForm.imageListFlags.Images.Add(flag);
                                        }
                                        catch (System.IO.FileNotFoundException)
                                        {
                                            Bitmap flag = new Bitmap("./Images/Flags/_unknown.png");
                                            flagDictionary.Add(connection.iso3166Code, thisForm.imageListFlags.Images.Count);
                                            thisForm.imageListFlags.Images.Add(flag);
                                        }
                                    }

                                    // Add the connection to the list
                                    connections.Add(connection);

                                    // Add this connection as an active connection
                                    lock (fadeoutLock)
                                    {
                                        activeConnections.Add(connection.id, connection);
                                    }

                                    // Add a marker for the location
                                    Bitmap icon = new Bitmap("./Images/standard_marker.png"); // Icon from https://www.iconfinder.com/icons/68010/favorite_location_pin_star_start_icon
                                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(Double.Parse(connection.latitude), Double.Parse(connection.longitude)), icon);

                                    // Add a connection line for this new connection
                                    ConnectionLine line = new ConnectionLine();
                                    line.id = connection.id;
                                    line.duration = LINEDURATION;
                                    line.downloading = connection.downloading;
                                    connectionLines.Add(connection.id, line);

                                    // Draw the line on the map
                                    List<PointLatLng> points = new List<PointLatLng>();
                                    points.Add(markerOverlay.Markers[0].Position);
                                    points.Add(new PointLatLng(Double.Parse(connection.latitude), Double.Parse(connection.longitude)));
                                    GMapPolygon drawLine = new GMapPolygon(points, "lineName");

                                    // Create and set the line Pen
                                    Pen linePen = new Pen(Color.FromArgb(192, Color.Blue), 1);
                                    float[] dashPattern = { 5, 5 };
                                    linePen.DashPattern = dashPattern;
                                    linePen.DashOffset = 5;
                                    drawLine.Stroke = linePen;
                                    lock (connectionLinesLock)
                                    {
                                        lock (fadeoutLock)
                                        {
                                            lineOverlay.Polygons.Add(drawLine);
                                        }
                                    }

                                    lock (fadeoutLock)
                                    {
                                        // See if we need to make this marker invisible
                                        // (if another maker is at the same location)
                                        foreach (var overlayMarker in markerOverlay.Markers)
                                        {
                                            if (overlayMarker.Position == marker.Position)
                                            {
                                                marker.IsVisible = false;
                                            }
                                        }

                                        markerOverlay.Markers.Add(marker);
                                    }

                                    // Print out what we just added
                                    /*Console.WriteLine("{0}   {1}   {2}   [{3}]   {4}, {5}, {6}   ID: {7}",
                                                connection.timestampRecent, connection.processName, connection.externalIP,
                                                connection.asName, connection.city, connection.region, connection.country, connection.id);*/

                                    thisForm.displayConnection(connection);
                                }
                            }

                            packetQueueSize--;
                            packetsProcessed++;
                        }
                        catch (Exception)
                        {
                            // Some unknown exception happened (probably a threading issue after the form has exited)
                            // Just ignore it
                        }
                    }
                }
            }
        }

        /**
         * Update the timestamp, opacity, bytes sent/received, and line visibility values for a connection.
         */
        private static void updateTOBV(Connection connection, DateTime time, bool outgoing, int len)
        {
            // Update the most recent timestamp
            connection.timestampRecent = time.ToLocalTime().Hour + ":" +
                        time.ToLocalTime().Minute + ":" + time.ToLocalTime().Second;

            // Update the opacity
            connection.opacity = (float)1.0;

            // Update the bytes sent/received
            if (outgoing)
            {
                connection.bytesSent += len;
                connection.downloading = false;
            }
            else
            {
                connection.bytesReceived += len;
                connection.downloading = true;
            }

            lock (connectionLinesLock)
            {
                // Is there already a line visible for this connection?
                if (connectionLines.ContainsKey(connection.id))
                {
                    // Update the line's duration and downloading status
                    connectionLines[connection.id].duration = LINEDURATION;
                    connectionLines[connection.id].downloading = connection.downloading;
                }
                // There isn't, so make the line visible
                else
                {
                    ConnectionLine line = new ConnectionLine();
                    line.id = connection.id;
                    line.duration = LINEDURATION;
                    line.downloading = connection.downloading;
                    connectionLines.Add(connection.id, line);
                    if (lineOverlay.Polygons.Count > connection.id)
                    {
                        lineOverlay.Polygons[connection.id].IsVisible = true;
                    }
                }
            }
        }

        /**
         * Update the connection duration, but only if it has degraded more than one eighth from the max.
         */
        private static void updateDuration(Connection connection)
        {
            if (connection.duration < (FADEOUTDURATION - (FADEOUTDURATION / 8)))
            {
                connection.duration = FADEOUTDURATION;
                lock (fadeoutLock)
                {
                    //Is this connection not listed as an active connection?
                    if (!activeConnections.ContainsKey(connection.id))
                    {
                        // Add this connection as an active connection
                        activeConnections.Add(connection.id, connection);
                    }
                }
            }
        }

        //----------------------------------------------------------------------------
        #endregion

        #region Display / Update Views
        //----------------------------------------------------------------------------

        /**
         * Displays the connection view (upper left view) information.
         */
        private void displayConnection(Connection connection)
        {
            if (this.listViewConnections.InvokeRequired)
            {
                DisplayConnectionCallback d = new DisplayConnectionCallback(displayConnection);
                this.Invoke(d, new object[] { connection });
            }
            else
            {
                ListViewItem lvi = new ListViewItem();
                if (connection.processName != "?")
                {
                    try
                    {
                        Icon processIcon = Icon.ExtractAssociatedIcon(Process.GetProcessesByName(connection.processName)[0].MainModule.FileName);
                        imageListProcessIcons.Images.Add(processIcon);
                        lvi.ImageIndex = imageListProcessIcons.Images.Count - 1;
                    }
                    // Access was denied in trying to get the icon
                    catch (Exception)
                    {
                        lvi.ImageIndex = 1;
                    }
                }
                else
                {
                    lvi.ImageIndex = 0;
                }

                lvi.SubItems.Add(connection.processName);
                lvi.SubItems.Add(connection.asName);
                listViewConnections.Items.Add(lvi);
            }
        }

        /**
         * Displays the details view (bottom left view) information.
         */
        private void displayDetailsView()
        {
            try
            {
                if (this.listViewDetails.InvokeRequired)
                {
                    DisplayDetailsCallback d = new DisplayDetailsCallback(displayDetailsView);
                    this.Invoke(d, new object[] { });
                }
                else
                {
                    // Show the details
                    if (listViewConnections.SelectedIndices.Count > 0)
                    {
                        int index = listViewConnections.SelectedItems[0].Index;
                        ListViewItem details = new ListViewItem();
                        details.SubItems.Add(connections.ElementAt(index).country);
                        details.SubItems.Add(connections.ElementAt(index).region);
                        details.SubItems.Add(connections.ElementAt(index).city);
                        details.SubItems.Add(connections.ElementAt(index).bytesReceivedTruncated());
                        details.SubItems.Add(connections.ElementAt(index).bytesSentTruncated());
                        details.SubItems.Add(connections.ElementAt(index).latitude.ToString());
                        details.SubItems.Add(connections.ElementAt(index).longitude.ToString());
                        details.SubItems.Add(connections.ElementAt(index).areaCode);
                        details.SubItems.Add(connections.ElementAt(index).postalCode);
                        details.SubItems.Add(connections.ElementAt(index).metroCode);
                        details.SubItems.Add(connections.ElementAt(index).timestampRecent);
                        details.SubItems.Add(connections.ElementAt(index).timestampFirst);
                        details.ImageIndex = flagDictionary[connections.ElementAt(index).iso3166Code];
                        listViewDetails.Items.Clear();
                        listViewDetails.Items.Add(details);
                    }
                }
            }
            catch (Exception)
            {
                // This catches any odd exceptions that may occur after the form
                // has been closed, but before the packet capture thread has stopped
            }
        }

        /**
         * Updates the connections view information.
         */
        private void updateConnectionsView(Connection connection, int index)
        {
            try
            {
                if (this.listViewConnections.InvokeRequired)
                {
                    UpdateConnectionCallback d = new UpdateConnectionCallback(updateConnectionsView);
                    this.Invoke(d, new object[] {connection, index});
                }
                else
                {
                    // Replace the old list view item
                    ListViewItem lvi = new ListViewItem();
                    if (connection.processName != "?")
                    {
                        try
                        {
                            Icon processIcon = Icon.ExtractAssociatedIcon(Process.GetProcessesByName(connection.processName)[0].MainModule.FileName);
                            imageListProcessIcons.Images.Add(processIcon);
                            lvi.ImageIndex = imageListProcessIcons.Images.Count - 1;
                        }
                        // Access was denied in trying to get the icon
                        catch (Exception)
                        {
                            lvi.ImageIndex = 1;
                        }
                    }
                    else
                    {
                        lvi.ImageIndex = 0;
                    }
                    lvi.SubItems.Add(connection.processName);
                    lvi.SubItems.Add(connection.asName);
                    int tempIndex = previousSelectedMarkerIndex;
                    listViewConnections.Items.Insert(index, lvi);
                    listViewConnections.Items.RemoveAt(index + 1);
                    previousSelectedMarkerIndex = tempIndex;

                    // Re-select the connection
                    if (previousSelectedMarkerIndex > -1)
                    {
                        listViewConnections.Items[previousSelectedMarkerIndex - 1].Selected = true;
                    }
                }
            }
            catch (Exception)
            {
                // This catches any odd exceptions that may occur after the form
                // has been closed, but before the packet capture thread has stopped
            }
        }

        /**
         * Updates the details view information.
         */
        private void updateDetailsView()
        {
            try
            {
                if (this.listViewDetails.InvokeRequired)
                {
                    UpdateDetailsCallback d = new UpdateDetailsCallback(updateDetailsView);
                    this.Invoke(d, new object[] { });
                }
                else
                {
                    // Update the details
                    if (listViewConnections.SelectedIndices.Count > 0)
                    {
                        int index = listViewConnections.SelectedItems[0].Index;
                        listViewDetails.Items[0].SubItems[4].Text = connections.ElementAt(index).bytesReceivedTruncated();
                        listViewDetails.Items[0].SubItems[5].Text = connections.ElementAt(index).bytesSentTruncated();
                        listViewDetails.Items[0].SubItems[11].Text = connections.ElementAt(index).timestampRecent;
                    }
                }
            }
            catch (Exception)
            {
                // This catches any odd exceptions that may occur after the form
                // has been closed, but before the packet capture thread has stopped
            }
        }

        /**
         * Updates the list fadeout in the connections view.
         */
        private void updateListFadeout(int id)
        {
            try
            {
                if (this.listViewConnections.InvokeRequired)
                {
                    UpdateListFadeoutCallback d = new UpdateListFadeoutCallback(updateListFadeout);
                    this.Invoke(d, new object[] { id });
                }
                else
                {
                    double percentDurationRemaining = connections[id].duration / FADEOUTDURATION;
                    int rgb = 255 - ((int)(percentDurationRemaining * 255));
                    int maxGrey = 210;
                    if (rgb > maxGrey)
                    {
                        rgb = maxGrey;
                    }

                    // Update the text color
                    listViewConnections.Items[id].ForeColor = Color.FromArgb(rgb, rgb, rgb);
                }
            }
            catch (Exception)
            {
                // This catches any odd exceptions that may occur after the form
                // has been closed, but before the packet capture thread has stopped
            }
        }

        /**
         * Updates the map markers.
         */
        private void updateMarker(int id, float alpha)
        {
            try
            {
                if (this.gmap.InvokeRequired)
                {
                    UpdateMarkerCallback d = new UpdateMarkerCallback(updateMarker);
                    this.Invoke(d, new object[] { id, alpha });
                }
                else
                {
                    int selectedIndex = -1;

                    if (listViewConnections.SelectedItems.Count != 0)
                    {
                        selectedIndex = listViewConnections.SelectedItems[0].Index;

                        // If the marker to update is already selected
                        if (selectedIndex == id)
                        {
                            return;
                        }
                    }

                    GMarkerGoogle oldMarker = (GMarkerGoogle)markerOverlay.Markers[id + 1];
                    GMarkerGoogle newMarker = new GMarkerGoogle(oldMarker.Position, ChangeImageOpacity.changeOpacity(new Bitmap("./Images/standard_marker.png"), alpha));
                    newMarker.IsVisible = oldMarker.IsVisible;
                    markerOverlay.Markers[id + 1] = newMarker;

                    // If this marker is being refreshed, then see if we need to bring its visibility to the front
                    if (alpha > 0.95)
                    {
                        // If nothing is selected, then make this new marker visible and make
                        // any markers at the same location invisible
                        if (selectedIndex < 0)
                        {
                            markerOverlay.Markers[id + 1].IsVisible = true;

                            foreach (var marker in markerOverlay.Markers)
                            {
                                if ((marker != markerOverlay.Markers[id + 1]) && (marker.Position == newMarker.Position))
                                {
                                    marker.IsVisible = false;
                                }
                            }
                        }
                        // Else do the same as above, but only if the new marker is not at
                        // the same location as the selected marker
                        else
                        {
                            if (markerOverlay.Markers[id + 1].Position != markerOverlay.Markers[selectedIndex + 1].Position)
                            {
                                markerOverlay.Markers[id + 1].IsVisible = true;

                                foreach (var marker in markerOverlay.Markers)
                                {
                                    if ((marker != markerOverlay.Markers[id + 1]) && (marker.Position == newMarker.Position))
                                    {
                                        marker.IsVisible = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // This catches any odd exceptions that may occur after the form
                // has been closed, but before the packet capture thread has stopped
            }
        }

        /**
         * Updates the data queue text and color.
         */
        private void changeDataQueueText(int queueSize)
        {
            if (this.listViewConnections.InvokeRequired)
            {
                UpdateDataQueueCallback d = new UpdateDataQueueCallback(changeDataQueueText);
                try
                {
                    this.Invoke(d, new object[] { queueSize });
                }
                catch (System.Exception)
                {
                    // Unexpected exception on closing
                }
            }
            else
            {
                // Update the text
                labelDataQueue.Text = queueSize.ToString();

                // Update the color
                if (queueSize > (MAXPACKETQUEUESIZE - (MAXPACKETQUEUESIZE / 10)))
                {
                    labelDataQueue.ForeColor = Color.Red;
                }
                else
                {
                    labelDataQueue.ForeColor = Color.Black;
                }
            }
        }

        //----------------------------------------------------------------------------
        #endregion

        #region Events
        //----------------------------------------------------------------------------

        /**
         * Initialize Connection Cartorgapher information. Called when the form loads.
         */
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set the image lists for the connections and details views
            listViewConnections.SmallImageList = imageListProcessIcons;
            listViewDetails.SmallImageList = imageListFlags;
            // http://www.independent-software.com/gmap-net-tutorial-maps-markers-and-polygons/

            // Initialize the map
            //gmap.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            gmap.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.OnMarkerClick += new MarkerClick(gmap_OnMarkerClick);
            //gmap.SetPositionByKeywords("Maputo, Mozambique");

            // Change map dragging to left click
            gmap.DragButton = MouseButtons.Left;

            // Remove the red + from the center
            gmap.ShowCenter = false;

            // Allow mousewheel scrolling while the mouse is over a marker
            gmap.IgnoreMarkerOnMouseWheel = true;

            // Set the default details text
            ListViewItem defaultDetails = new ListViewItem();
            defaultDetails.SubItems.Add("Nothing Selected");
            listViewDetails.Items.Add(defaultDetails);
            listViewDetails.BackColor = Color.Gainsboro;

            // Set the font size for the buttons
            buttonReset.Font = new Font("Sans", 16, FontStyle.Regular);
            buttonHelp.Font = new Font("Sans", 16, FontStyle.Regular);

            // Setup the animation timers
            lineTimer = new System.Timers.Timer();
            lineTimer.Elapsed += new ElapsedEventHandler(updateLineDisplay);
            lineTimer.Interval = LINETIMERINTERVAL;
            lineTimer.Enabled = true;

            lineAnimationTimer = new System.Timers.Timer();
            lineAnimationTimer.Elapsed += new ElapsedEventHandler(lineAnimation);
            lineAnimationTimer.Interval = LINEANIMATIONTIMERINTERVAL;
            lineAnimationTimer.Enabled = true;

            transparencyTimer = new System.Timers.Timer();
            transparencyTimer.Elapsed += new ElapsedEventHandler(updateTransparency);
            transparencyTimer.Interval = FADEOUTINTERVAL;
            transparencyTimer.Enabled = true;

            socketReaderTimer = new System.Timers.Timer();
            socketReaderTimer.Elapsed += new ElapsedEventHandler(updateSocketList);
            socketReaderTimer.Interval = SOCKETREADINTERVAL;
            //socketReaderTimer.Enabled = true;

            dataQueueTextTimer = new System.Timers.Timer();
            dataQueueTextTimer.Elapsed += new ElapsedEventHandler(updateDataQueueText);
            dataQueueTextTimer.Interval = DATAQUEUETEXTINTERVAL;
            dataQueueTextTimer.Enabled = true;
        }

        /**
         * End Connection Cartographer. Called when the form closes.
         */
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Set running to false so that the capture loop ends
            running = false;
        }     

        /**
         * Add relevant packets to the packet queue. Called whenever a packet arrives.
         */
        private void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

            // Get the IP packet details
            var ipPacket = PacketDotNet.IpPacket.GetEncapsulated(packet);
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;

            // Determine if this is an outgoing packet
            bool outgoing = false;
            if (interfaceIPs.Contains(srcIp.ToString()) || interfaceIPsV6.Contains(srcIp.ToString()))
            {
                outgoing = true;
            }

            // Determine the host connection IP and the external connection IP
            string externalIP;

            if (outgoing)
            {
                externalIP = dstIp.ToString();
            }
            else
            {
                externalIP = srcIp.ToString();
            }

            // Only process this packet if it is routable
            if (!IpRoutable.IsNonRoutableIpAddress(externalIP))
            {
                // If the packet queue is not full
                if (packetQueueSize < MAXPACKETQUEUESIZE)
                {
                    lock (packetQueueLock)
                    {
                        packetQueue.Add(e.Packet);
                        packetQueueSize++;
                    }
                }
                else
                {
                    // If the packet queue is full, but we may not have seen this
                    // source or destination before so process it anyway
                    if ((!seenIPs.Contains(externalIP)))
                    {
                        lock (packetQueueLock)
                        {
                            packetQueue.Add(e.Packet);
                            packetQueueSize++;
                        }

                        // Add the newly discovered IPs to the seen list
                        seenIPs.Add(externalIP);
                    }
                }
            }
        }

        /**
         * Make the connections list unfocusable.
         */
        private void listViewConnections_Enter(object sender, EventArgs e)
        {
            gmap.Focus();
        }

        /**
         * Update marker display when a listview item is selected.
         */
        private void listViewConnections_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            GMarkerGoogle oldMarker;
            GMarkerGoogle newMarker;
            HashSet<PointLatLng> markerLocations = new HashSet<PointLatLng>();

            // Make sure all markers are visible
            foreach (var marker in markerOverlay.Markers)
            {
                marker.IsVisible = true;
            }

            // Nothing selected, so show no details
            if (listViewConnections.SelectedItems.Count == 0)
            {
                listViewDetails.Items.Clear();
                ListViewItem defaultDetails = new ListViewItem();
                defaultDetails.SubItems.Add("Nothing Selected");
                listViewDetails.Items.Add(defaultDetails);
                listViewDetails.BackColor = Color.Gainsboro;

                // Revert the display of a previously selected marker
                if (previousSelectedMarkerIndex > -1)
                {
                    oldMarker = (GMarkerGoogle)markerOverlay.Markers[previousSelectedMarkerIndex];
                    newMarker = new GMarkerGoogle(oldMarker.Position,
                        ChangeImageOpacity.changeOpacity(new Bitmap("./Images/standard_marker.png"), connections[previousSelectedMarkerIndex - 1].opacity));
                    markerOverlay.Markers[previousSelectedMarkerIndex] = newMarker;
                    lineOverlay.Polygons[previousSelectedMarkerIndex - 1].Stroke.Color = Color.Blue;
                }
                previousSelectedMarkerIndex = -1;

                // Make it so that only one marker is shown at any given location
                // (i.e. Don't allow multiple transparent markers at one location to overlap)
                foreach (var marker in markerOverlay.Markers)
                {
                    if (marker.IsVisible)
                    {
                        // If there is already a visible marker at this location
                        // then make this marker invisible
                        if (markerLocations.Contains(marker.Position))
                        {
                            marker.IsVisible = false;
                        }
                        else
                        {
                            markerLocations.Add(marker.Position);
                        }
                    }
                }

                return;
            }

            listViewDetails.BackColor = Color.White;

            // Show the details
            displayDetailsView();

            // Update the map view
            int index = listViewConnections.SelectedItems[0].Index;
            index++; // Add 1 to the index, because of the home marker
            oldMarker = (GMarkerGoogle)markerOverlay.Markers[index];
            newMarker = new GMarkerGoogle(oldMarker.Position, new Bitmap("./Images/selected_standard_marker.png"));
            markerOverlay.Markers[index] = newMarker;
            lineOverlay.Polygons[index - 1].Stroke.Color = Color.Orange;

            // Make any marker with the exact same location temporairally invisible
            foreach (var marker in markerOverlay.Markers)
            {
                if ((marker != newMarker) && (marker.Position == newMarker.Position))
                {
                    marker.IsVisible = false;
                }
            }

            // Make it so that only one marker is shown at any given location
            // (i.e. Don't allow multiple transparent markers at one location to overlap)
            foreach (var marker in markerOverlay.Markers)
            {
                if (marker.IsVisible)
                {
                    // If there is already a visible marker at this location
                    // then make this marker invisible
                    if (markerLocations.Contains(marker.Position))
                    {
                        marker.IsVisible = false;
                    }
                    else
                    {
                        markerLocations.Add(marker.Position);
                    }
                }
            }

            // Update the previosly selected marker index
            previousSelectedMarkerIndex = index;
        }

        /**
         * Display the help view when the help button is clicked.
         */
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            // If the help window has not already been disposed, then show it
            if (!helpWindow.IsDisposed)
            {
                helpWindow.Show();
            }
            // Else initiate a new help window and show it
            else
            {
                helpWindow = new HelpWindow();
                helpWindow.Show();
            }

            // Show the help window if it has been minimized
            if (helpWindow.WindowState == FormWindowState.Minimized)
            {
                helpWindow.WindowState = FormWindowState.Normal;
            }
        }

        /**
         * Reset the connections when the reset button is clicked.
         */
        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (initialized)
            {
                // Clear the connection data
                connections.Clear();
                connectionLines.Clear();

                // Clear critical data in a separate thread
                // to prevent UI deadlocking
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    // Clear the line overlay
                    lock (connectionLinesLock)
                    {
                        lineOverlay.Clear();
                    }

                    // Clear the active connections
                    lock (fadeoutLock)
                    {
                        activeConnections.Clear();
                    }
                }).Start();

                // Reset the selected marker index
                previousSelectedMarkerIndex = -1;

                // Get the home marker
                GMarkerGoogle homeMarker = (GMarkerGoogle)markerOverlay.Markers[0];
                // Clear the marker list
                markerOverlay.Clear();
                // Re-add the home marker
                markerOverlay.Markers.Add(homeMarker);

                // Clear the listviews
                listViewConnections.Items.Clear();
                listViewDetails.Items.Clear();
                ListViewItem defaultDetails = new ListViewItem();
                defaultDetails.SubItems.Add("Nothing Selected");
                listViewDetails.Items.Add(defaultDetails);
                listViewDetails.BackColor = Color.Gainsboro;

                // Clear the image list
                Image tempImage1 = imageListProcessIcons.Images[0];
                Image tempImage2 = imageListProcessIcons.Images[1];
                imageListProcessIcons.Images.Clear();
                imageListProcessIcons.Images.Add(tempImage1);
                imageListProcessIcons.Images.Add(tempImage2);

                // Reset the packet capture in a separate thread
                // to prevent the UI locking up
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    /* Retrieve the device list */
                    var devices = CaptureDeviceList.Instance;

                    foreach (var device in devices)
                    {
                        try
                        {
                            // This sometimes throws a pcap error
                            device.StopCapture();
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            device.StartCapture();
                        }
                    }

                    packetThread.Abort();
                    packetQueue.Clear();
                    packetThread = new System.Threading.Thread(processPackets);
                    packetThread.Priority = ThreadPriority.Highest;
                    packetQueueSize = 0;
                    changeDataQueueText(packetQueueSize);
                    seenIPs.Clear();
                    packetThread.Start();
                }).Start();
            }
        }

        /**
         * Change the selected connection when a connection marker is clicked.
         */
        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            int markerIndex = 0;

            foreach (GMarkerGoogle marker in markerOverlay.Markers)
            {
                if (marker.Equals(item))
                {
                    break;
                }

                markerIndex++;
            }

            if (markerIndex > 0)
            {
                // Make sure the index is valid
                if (listViewConnections.Items.Count > (markerIndex - 1))
                {
                    // Change the selected connection
                    listViewConnections.Items[markerIndex - 1].Selected = true;
                }
            }
        }

        #region Periodic Timer Events
        //----------------------------------------------------------------------------

        /**
         * Periodically update the list of socket information.
         */
        private void updateSocketList(object source, ElapsedEventArgs e)
        {
            openSockets = NetStatPortProcess.getNetStatPorts();
        }

        /**
         * Periodically update the connection line display.
         */
        private void updateLineDisplay(object source, ElapsedEventArgs e)
        {
            List<int> removeIDs = new List<int>();
            foreach (ConnectionLine line in connectionLines.Values)
            {
                // Decrease the remaining duration of the line
                line.duration -= LINETIMERINTERVAL;

                // Get the list of lines that are no longer active
                if (line.duration <= 0)
                {
                    removeIDs.Add(line.id);
                }
            }

            // Remove the lines that are no longer active
            foreach (int id in removeIDs)
            {
                lock (connectionLinesLock)
                {
                    connectionLines.Remove(id);
                    if (lineOverlay.Polygons.Count > id)
                    {
                        lineOverlay.Polygons[id].IsVisible = false;
                    }
                }
            }
        }

        /**
         * Periodically update the connection line animation.
         */
        private void lineAnimation(object source, ElapsedEventArgs e)
        {
            lock (connectionLinesLock)
            {
                foreach (ConnectionLine line in connectionLines.Values)
                {
                    if (lineOverlay.Polygons.Count > line.id)
                    {
                        try
                        {
                            if (line.downloading)
                            {
                                lineOverlay.Polygons[line.id].Stroke.DashOffset += LINEANIMATIONSPEED;
                                map.Invalidate();
                            }
                            else
                            {
                                lineOverlay.Polygons[line.id].Stroke.DashOffset -= LINEANIMATIONSPEED;
                                map.Invalidate();
                            }
                        }
                        catch (Exception)
                        {
                            // TODO: I don't know what is causing errors here
                        }
                    }
                }
            }
        }

        /**
         * Periodically update the connection line and marker transparency.
         */
        private void updateTransparency(object source, ElapsedEventArgs e)
        {
            List<int> removeIDs = new List<int>();
            lock (fadeoutLock)
            {
                foreach (Connection connection in activeConnections.Values)
                {
                    // Decrease the remaining duration of the connection fadeout
                    connection.duration -= FADEOUTINTERVAL;

                    // Decrease the connection opacity
                    connection.opacity -= (float)0.0025;

                    updateListFadeout(connection.id);
                    updateMarker(connection.id, connection.opacity);

                    // Get the list of connections that are no longer active
                    if (connection.duration <= 0)
                    {
                        removeIDs.Add(connection.id);
                    }
                }

                // Remove the connections that are no longer active
                foreach (int id in removeIDs)
                {
                    activeConnections.Remove(id);
                }
            }
        }

        /**
         * Periodically update the data queue text.
         */
        private void updateDataQueueText(object source, ElapsedEventArgs e)
        {
            changeDataQueueText(packetQueueSize);
        }

        //----------------------------------------------------------------------------
        #endregion

        //----------------------------------------------------------------------------
        #endregion

        #region Other Functions
        //----------------------------------------------------------------------------

        /** 
         * Exit the app if something bad happened
         */
        private void ErrorExit()
        {
            // Set running to false so that the capture loop ends
            running = false;

            MessageBox.Show("Undefined Error: Please Re-run the Application");
            Application.Exit();
        }

        //----------------------------------------------------------------------------
        #endregion
    }
}
