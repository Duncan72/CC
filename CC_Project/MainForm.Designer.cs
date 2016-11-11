namespace ConnectionCartographer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBoxConnections = new System.Windows.Forms.GroupBox();
            this.listViewConnections = new System.Windows.Forms.ListView();
            this.columnHeaderProcessIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderApplication = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDestination = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.listViewDetails = new System.Windows.Forms.ListView();
            this.columnHeaderFlag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCountry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRegion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBytesDown = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBytesUp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLatitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLongitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAreaCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPostalCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMetroCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimeLast = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimeFirst = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.groupBoxMap = new System.Windows.Forms.GroupBox();
            this.gmap = new GMap.NET.WindowsForms.GMapControl();
            this.imageListProcessIcons = new System.Windows.Forms.ImageList(this.components);
            this.imageListFlags = new System.Windows.Forms.ImageList(this.components);
            this.labelDataQueueDescription = new System.Windows.Forms.Label();
            this.labelDataQueue = new System.Windows.Forms.Label();
            this.groupBoxConnections.SuspendLayout();
            this.groupBoxDetails.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.groupBoxMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConnections
            // 
            this.groupBoxConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxConnections.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxConnections.Controls.Add(this.listViewConnections);
            this.groupBoxConnections.Location = new System.Drawing.Point(13, 13);
            this.groupBoxConnections.Name = "groupBoxConnections";
            this.groupBoxConnections.Size = new System.Drawing.Size(520, 250);
            this.groupBoxConnections.TabIndex = 0;
            this.groupBoxConnections.TabStop = false;
            this.groupBoxConnections.Text = "Connections";
            // 
            // listViewConnections
            // 
            this.listViewConnections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderProcessIcon,
            this.columnHeaderApplication,
            this.columnHeaderDestination});
            this.listViewConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewConnections.FullRowSelect = true;
            this.listViewConnections.GridLines = true;
            this.listViewConnections.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewConnections.HideSelection = false;
            this.listViewConnections.Location = new System.Drawing.Point(3, 16);
            this.listViewConnections.MultiSelect = false;
            this.listViewConnections.Name = "listViewConnections";
            this.listViewConnections.Size = new System.Drawing.Size(514, 231);
            this.listViewConnections.TabIndex = 0;
            this.listViewConnections.TabStop = false;
            this.listViewConnections.UseCompatibleStateImageBehavior = false;
            this.listViewConnections.View = System.Windows.Forms.View.Details;
            this.listViewConnections.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewConnections_ItemSelectionChanged);
            this.listViewConnections.Enter += new System.EventHandler(this.listViewConnections_Enter);
            // 
            // columnHeaderProcessIcon
            // 
            this.columnHeaderProcessIcon.Text = "";
            this.columnHeaderProcessIcon.Width = 38;
            // 
            // columnHeaderApplication
            // 
            this.columnHeaderApplication.Text = "Application";
            this.columnHeaderApplication.Width = 154;
            // 
            // columnHeaderDestination
            // 
            this.columnHeaderDestination.Text = "Destination";
            this.columnHeaderDestination.Width = 317;
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxDetails.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxDetails.Controls.Add(this.listViewDetails);
            this.groupBoxDetails.Location = new System.Drawing.Point(13, 269);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Size = new System.Drawing.Size(520, 86);
            this.groupBoxDetails.TabIndex = 1;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "Details";
            // 
            // listViewDetails
            // 
            this.listViewDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFlag,
            this.columnHeaderCountry,
            this.columnHeaderRegion,
            this.columnHeaderCity,
            this.columnHeaderBytesDown,
            this.columnHeaderBytesUp,
            this.columnHeaderLatitude,
            this.columnHeaderLongitude,
            this.columnHeaderAreaCode,
            this.columnHeaderPostalCode,
            this.columnHeaderMetroCode,
            this.columnHeaderTimeLast,
            this.columnHeaderTimeFirst});
            this.listViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDetails.Location = new System.Drawing.Point(3, 16);
            this.listViewDetails.Name = "listViewDetails";
            this.listViewDetails.Size = new System.Drawing.Size(514, 67);
            this.listViewDetails.TabIndex = 0;
            this.listViewDetails.TabStop = false;
            this.listViewDetails.UseCompatibleStateImageBehavior = false;
            this.listViewDetails.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderFlag
            // 
            this.columnHeaderFlag.Text = "";
            this.columnHeaderFlag.Width = 25;
            // 
            // columnHeaderCountry
            // 
            this.columnHeaderCountry.Text = "Country";
            this.columnHeaderCountry.Width = 102;
            // 
            // columnHeaderRegion
            // 
            this.columnHeaderRegion.Text = "Region";
            this.columnHeaderRegion.Width = 87;
            // 
            // columnHeaderCity
            // 
            this.columnHeaderCity.Text = "City";
            this.columnHeaderCity.Width = 87;
            // 
            // columnHeaderBytesDown
            // 
            this.columnHeaderBytesDown.Text = "Bytes Downloaded";
            this.columnHeaderBytesDown.Width = 102;
            // 
            // columnHeaderBytesUp
            // 
            this.columnHeaderBytesUp.Text = "Bytes Uploaded";
            this.columnHeaderBytesUp.Width = 102;
            // 
            // columnHeaderLatitude
            // 
            this.columnHeaderLatitude.Text = "Latitude";
            this.columnHeaderLatitude.Width = 80;
            // 
            // columnHeaderLongitude
            // 
            this.columnHeaderLongitude.Text = "Longitude";
            this.columnHeaderLongitude.Width = 80;
            // 
            // columnHeaderAreaCode
            // 
            this.columnHeaderAreaCode.Text = "Area Code";
            this.columnHeaderAreaCode.Width = 80;
            // 
            // columnHeaderPostalCode
            // 
            this.columnHeaderPostalCode.Text = "Postal Code";
            this.columnHeaderPostalCode.Width = 80;
            // 
            // columnHeaderMetroCode
            // 
            this.columnHeaderMetroCode.Text = "Metro Code";
            this.columnHeaderMetroCode.Width = 80;
            // 
            // columnHeaderTimeLast
            // 
            this.columnHeaderTimeLast.Text = "Time of Last Update";
            this.columnHeaderTimeLast.Width = 110;
            // 
            // columnHeaderTimeFirst
            // 
            this.columnHeaderTimeFirst.Text = "Time of First Update";
            this.columnHeaderTimeFirst.Width = 110;
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxOptions.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxOptions.Controls.Add(this.labelDataQueue);
            this.groupBoxOptions.Controls.Add(this.labelDataQueueDescription);
            this.groupBoxOptions.Controls.Add(this.buttonHelp);
            this.groupBoxOptions.Controls.Add(this.buttonReset);
            this.groupBoxOptions.Location = new System.Drawing.Point(13, 358);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(520, 73);
            this.groupBoxOptions.TabIndex = 2;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Options/Other";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(182, 20);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(170, 47);
            this.buttonHelp.TabIndex = 1;
            this.buttonHelp.TabStop = false;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(7, 20);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(169, 47);
            this.buttonReset.TabIndex = 0;
            this.buttonReset.TabStop = false;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // groupBoxMap
            // 
            this.groupBoxMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMap.AutoSize = true;
            this.groupBoxMap.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxMap.Controls.Add(this.gmap);
            this.groupBoxMap.Location = new System.Drawing.Point(540, 13);
            this.groupBoxMap.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxMap.Name = "groupBoxMap";
            this.groupBoxMap.Padding = new System.Windows.Forms.Padding(4, 3, 4, 4);
            this.groupBoxMap.Size = new System.Drawing.Size(72, 418);
            this.groupBoxMap.TabIndex = 3;
            this.groupBoxMap.TabStop = false;
            this.groupBoxMap.Text = "Map";
            // 
            // gmap
            // 
            this.gmap.AutoSize = true;
            this.gmap.Bearing = 0F;
            this.gmap.CanDragMap = true;
            this.gmap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmap.GrayScaleMode = false;
            this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmap.LevelsKeepInMemmory = 5;
            this.gmap.Location = new System.Drawing.Point(4, 16);
            this.gmap.Margin = new System.Windows.Forms.Padding(0);
            this.gmap.MarkersEnabled = true;
            this.gmap.MaxZoom = 18;
            this.gmap.MinZoom = 0;
            this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gmap.Name = "gmap";
            this.gmap.NegativeMode = false;
            this.gmap.PolygonsEnabled = true;
            this.gmap.RetryLoadTile = 0;
            this.gmap.RoutesEnabled = true;
            this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmap.ShowTileGridLines = false;
            this.gmap.Size = new System.Drawing.Size(64, 398);
            this.gmap.TabIndex = 1;
            this.gmap.TabStop = false;
            this.gmap.Zoom = 2D;
            this.gmap.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gmap_OnMarkerClick);
            // 
            // imageListProcessIcons
            // 
            this.imageListProcessIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListProcessIcons.ImageStream")));
            this.imageListProcessIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListProcessIcons.Images.SetKeyName(0, "question_icon.png");
            this.imageListProcessIcons.Images.SetKeyName(1, "na_icon.png");
            // 
            // imageListFlags
            // 
            this.imageListFlags.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListFlags.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListFlags.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // labelDataQueueDescription
            // 
            this.labelDataQueueDescription.AutoSize = true;
            this.labelDataQueueDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDataQueueDescription.Location = new System.Drawing.Point(358, 32);
            this.labelDataQueueDescription.Name = "labelDataQueueDescription";
            this.labelDataQueueDescription.Size = new System.Drawing.Size(100, 20);
            this.labelDataQueueDescription.TabIndex = 2;
            this.labelDataQueueDescription.Text = "Data Queue:";
            // 
            // labelDataQueue
            // 
            this.labelDataQueue.AutoSize = true;
            this.labelDataQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDataQueue.Location = new System.Drawing.Point(464, 29);
            this.labelDataQueue.Name = "labelDataQueue";
            this.labelDataQueue.Size = new System.Drawing.Size(20, 24);
            this.labelDataQueue.TabIndex = 3;
            this.labelDataQueue.Text = "0";
            this.labelDataQueue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.groupBoxMap);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.groupBoxConnections);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection Cartographer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxConnections.ResumeLayout(false);
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            this.groupBoxMap.ResumeLayout(false);
            this.groupBoxMap.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConnections;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.GroupBox groupBoxMap;
        private System.Windows.Forms.ListView listViewConnections;
        private System.Windows.Forms.ListView listViewDetails;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.ColumnHeader columnHeaderApplication;
        private System.Windows.Forms.ColumnHeader columnHeaderDestination;
        private System.Windows.Forms.ColumnHeader columnHeaderBytesDown;
        private System.Windows.Forms.ColumnHeader columnHeaderBytesUp;
        private System.Windows.Forms.ColumnHeader columnHeaderCountry;
        private System.Windows.Forms.ColumnHeader columnHeaderRegion;
        private System.Windows.Forms.ColumnHeader columnHeaderCity;
        private System.Windows.Forms.ColumnHeader columnHeaderLatitude;
        private System.Windows.Forms.ColumnHeader columnHeaderLongitude;
        private System.Windows.Forms.ColumnHeader columnHeaderAreaCode;
        private System.Windows.Forms.ColumnHeader columnHeaderPostalCode;
        private System.Windows.Forms.ColumnHeader columnHeaderMetroCode;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeLast;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeFirst;
        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.ImageList imageListProcessIcons;
        private System.Windows.Forms.ColumnHeader columnHeaderProcessIcon;
        private System.Windows.Forms.ImageList imageListFlags;
        private System.Windows.Forms.ColumnHeader columnHeaderFlag;
        private System.Windows.Forms.Label labelDataQueue;
        private System.Windows.Forms.Label labelDataQueueDescription;

    }
}

