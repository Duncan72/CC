using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ConnectionCartographer
{
    /**
     * Enable double buffering on the specified control to help prevent flickering.
     */
    class DoubleBuffering
    {
        // Copied from user Oilver at https://stackoverflow.com/questions/87795/how-to-prevent-flickering-in-listview-when-updating-a-single-listviewitems-text
        public static void doubleBuffering(Control control, bool enable)
        {
            var method = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enable });
        }
    }
}
