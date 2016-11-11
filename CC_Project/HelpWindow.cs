using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConnectionCartographer
{
    public partial class HelpWindow : Form
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void HelpWindow_Load(object sender, EventArgs e)
        {
            richTextBoxIntroduction.LoadFile("HelpText/Introduction.rtf", RichTextBoxStreamType.RichText);
            richTextBoxUsage.LoadFile("HelpText/Usage.rtf", RichTextBoxStreamType.RichText);
            richTextBoxFAQ.LoadFile("HelpText/FAQ.rtf", RichTextBoxStreamType.RichText);
            richTextBoxAbout.LoadFile("HelpText/About.rtf", RichTextBoxStreamType.RichText);
        }
    }
}
