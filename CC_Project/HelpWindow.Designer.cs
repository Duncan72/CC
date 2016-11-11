namespace ConnectionCartographer
{
    partial class HelpWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpWindow));
            this.tabControlHelpTabs = new System.Windows.Forms.TabControl();
            this.tabPageIntro = new System.Windows.Forms.TabPage();
            this.tabPageUsage = new System.Windows.Forms.TabPage();
            this.richTextBoxIntroduction = new System.Windows.Forms.RichTextBox();
            this.tabPageFAQ = new System.Windows.Forms.TabPage();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.richTextBoxUsage = new System.Windows.Forms.RichTextBox();
            this.richTextBoxFAQ = new System.Windows.Forms.RichTextBox();
            this.richTextBoxAbout = new System.Windows.Forms.RichTextBox();
            this.tabControlHelpTabs.SuspendLayout();
            this.tabPageIntro.SuspendLayout();
            this.tabPageUsage.SuspendLayout();
            this.tabPageFAQ.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlHelpTabs
            // 
            this.tabControlHelpTabs.Controls.Add(this.tabPageIntro);
            this.tabControlHelpTabs.Controls.Add(this.tabPageUsage);
            this.tabControlHelpTabs.Controls.Add(this.tabPageFAQ);
            this.tabControlHelpTabs.Controls.Add(this.tabPageAbout);
            this.tabControlHelpTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlHelpTabs.Location = new System.Drawing.Point(0, 0);
            this.tabControlHelpTabs.Name = "tabControlHelpTabs";
            this.tabControlHelpTabs.SelectedIndex = 0;
            this.tabControlHelpTabs.Size = new System.Drawing.Size(384, 442);
            this.tabControlHelpTabs.TabIndex = 0;
            // 
            // tabPageIntro
            // 
            this.tabPageIntro.Controls.Add(this.richTextBoxIntroduction);
            this.tabPageIntro.Location = new System.Drawing.Point(4, 22);
            this.tabPageIntro.Name = "tabPageIntro";
            this.tabPageIntro.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIntro.Size = new System.Drawing.Size(376, 416);
            this.tabPageIntro.TabIndex = 0;
            this.tabPageIntro.Text = "Introduction";
            this.tabPageIntro.UseVisualStyleBackColor = true;
            // 
            // tabPageUsage
            // 
            this.tabPageUsage.Controls.Add(this.richTextBoxUsage);
            this.tabPageUsage.Location = new System.Drawing.Point(4, 22);
            this.tabPageUsage.Name = "tabPageUsage";
            this.tabPageUsage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUsage.Size = new System.Drawing.Size(376, 416);
            this.tabPageUsage.TabIndex = 1;
            this.tabPageUsage.Text = "How to Use";
            this.tabPageUsage.UseVisualStyleBackColor = true;
            // 
            // richTextBoxIntroduction
            // 
            this.richTextBoxIntroduction.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxIntroduction.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBoxIntroduction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxIntroduction.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxIntroduction.Name = "richTextBoxIntroduction";
            this.richTextBoxIntroduction.ReadOnly = true;
            this.richTextBoxIntroduction.Size = new System.Drawing.Size(370, 410);
            this.richTextBoxIntroduction.TabIndex = 0;
            this.richTextBoxIntroduction.Text = "Error loading \"Introduction.rtf\". Is it already open in another program?";
            // 
            // tabPageFAQ
            // 
            this.tabPageFAQ.Controls.Add(this.richTextBoxFAQ);
            this.tabPageFAQ.Location = new System.Drawing.Point(4, 22);
            this.tabPageFAQ.Name = "tabPageFAQ";
            this.tabPageFAQ.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFAQ.Size = new System.Drawing.Size(376, 416);
            this.tabPageFAQ.TabIndex = 2;
            this.tabPageFAQ.Text = "Frequently Asked Questions";
            this.tabPageFAQ.UseVisualStyleBackColor = true;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.richTextBoxAbout);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(376, 416);
            this.tabPageAbout.TabIndex = 3;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // richTextBoxUsage
            // 
            this.richTextBoxUsage.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxUsage.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBoxUsage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxUsage.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxUsage.Name = "richTextBoxUsage";
            this.richTextBoxUsage.ReadOnly = true;
            this.richTextBoxUsage.Size = new System.Drawing.Size(370, 410);
            this.richTextBoxUsage.TabIndex = 1;
            this.richTextBoxUsage.Text = "Error loading \"Usage.rtf\". Is it already open in another program?";
            // 
            // richTextBoxFAQ
            // 
            this.richTextBoxFAQ.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxFAQ.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBoxFAQ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxFAQ.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxFAQ.Name = "richTextBoxFAQ";
            this.richTextBoxFAQ.ReadOnly = true;
            this.richTextBoxFAQ.Size = new System.Drawing.Size(370, 410);
            this.richTextBoxFAQ.TabIndex = 1;
            this.richTextBoxFAQ.Text = "Error loading \"FAQ.rtf\". Is it already open in another program?";
            // 
            // richTextBoxAbout
            // 
            this.richTextBoxAbout.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxAbout.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBoxAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxAbout.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxAbout.Name = "richTextBoxAbout";
            this.richTextBoxAbout.ReadOnly = true;
            this.richTextBoxAbout.Size = new System.Drawing.Size(370, 410);
            this.richTextBoxAbout.TabIndex = 1;
            this.richTextBoxAbout.Text = "Error loading \"About.rtf\". Is it already open in another program?";
            // 
            // HelpWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 442);
            this.Controls.Add(this.tabControlHelpTabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "HelpWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Help";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.HelpWindow_Load);
            this.tabControlHelpTabs.ResumeLayout(false);
            this.tabPageIntro.ResumeLayout(false);
            this.tabPageUsage.ResumeLayout(false);
            this.tabPageFAQ.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlHelpTabs;
        private System.Windows.Forms.TabPage tabPageIntro;
        private System.Windows.Forms.RichTextBox richTextBoxIntroduction;
        private System.Windows.Forms.TabPage tabPageUsage;
        private System.Windows.Forms.TabPage tabPageFAQ;
        private System.Windows.Forms.TabPage tabPageAbout;
        private System.Windows.Forms.RichTextBox richTextBoxUsage;
        private System.Windows.Forms.RichTextBox richTextBoxFAQ;
        private System.Windows.Forms.RichTextBox richTextBoxAbout;
    }
}