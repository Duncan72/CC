// See MainForm.cs for the main program details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ConnectionCartographer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
