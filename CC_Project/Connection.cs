using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionCartographer
{
    class Connection
    {
        public int id = -1;
        public bool downloading;    // Was the last packet seen incoming/downloading (true)
                                    // or was it outgoing/uploading (false)
        public string externalIP = "?";
        public HashSet<int> hostPorts = new HashSet<int>();
        public string processName = "?";
        public string asName = "?";
        public int asNumber = -1;
        public long bytesReceived = 0;
        public long bytesSent = 0;
        public string countryCode = "?";
        public string country = "?";
        public string region = "?";
        public string regionName = "?";
        public string city = "?";
        public string latitude = "?";
        public string longitude = "?";
        public string postalCode = "?";
        public string metroCode = "?";
        public string areaCode = "?";
        public string timestampRecent = "?";
        public string timestampFirst = "?";
        public HashSet<string> ipList = new HashSet<string>();
        public string iso3166Code = "_unknown";
        public double duration = 0;
        public float opacity = 1; // 100% visible

        public void setLocationInformation(Location location)
        {
            if (location.countryCode != null)
            {
                country = location.countryCode;
            }
            if (location.countryName != null)
            {
                country = location.countryName;
            }
            if (location.region != null)
            {
                region = location.region;
            }
            if (location.regionName != null)
            {
                regionName = location.regionName;
            }
            if (location.city != null)
            {
                city = location.city;
            }
            if (location.latitude != 0)
            {
                latitude = location.latitude.ToString();
            }
            if (location.longitude != 0)
            {
                longitude = location.longitude.ToString();
            }
            if (location.postalCode != null)
            {
                postalCode = location.postalCode;
            }
            if (location.metro_code != 0)
            {
                metroCode = location.metro_code.ToString();
            }
            if (location.area_code != 0)
            {
                areaCode = location.area_code.ToString();
            }
        }

        // Return the number of bytes sent as a truncated string (e.g. KB, MB, GB)
        public string bytesSentTruncated()
        {
            string truncatedBytes = "";

            // Is this in GB?
            if (bytesSent > 1073741824)
            {
                double bytes = Math.Round(((double)bytesSent / (double)1073741824), 3);
                truncatedBytes = bytes + " GB";
            }
            // Is this in MB?
            else if (bytesSent > 1048576)
            {
                double bytes = Math.Round(((double)bytesSent / (double)1048576), 3);
                truncatedBytes = bytes + " MB";
            }
            // Is this in KB?
            else if (bytesSent > 1024)
            {
                double bytes = Math.Round(((double)bytesSent / (double)1024), 3);
                truncatedBytes = bytes + " KB";
            }
            else
            {
                truncatedBytes = bytesSent + " B";
            }

            return truncatedBytes;
        }

        // Return the number of bytes received as a truncated string (e.g. KB, MB, GB)
        public string bytesReceivedTruncated()
        {
            string truncatedBytes = "";

            // Is this in GB?
            if (bytesReceived > 1073741824)
            {
                double bytes = Math.Round(((double)bytesReceived / (double)1073741824), 3);
                truncatedBytes = bytes + " GB";
            }
            // Is this in MB?
            else if (bytesReceived > 1048576)
            {
                double bytes = Math.Round(((double)bytesReceived / (double)1048576), 3);
                truncatedBytes = bytes + " MB";
            }
            // Is this in KB?
            else if (bytesReceived > 1024)
            {
                double bytes = Math.Round(((double)bytesReceived / (double)1024), 3);
                truncatedBytes = bytes + " KB";
            }
            else
            {
                truncatedBytes = bytesReceived + " B";
            }

            return truncatedBytes;
        }
    }
}
