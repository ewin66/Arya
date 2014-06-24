using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Arya.Framework.Extensions
{
    using System.Collections.Generic;

    public static class TextBoxExtensions
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public static void SetWatermark(this TextBox textBox, string watermarkText)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }

        public static void SetAutoComplete(this TextBox textBox, IEnumerable<string> enumerable)
        {
            var autoComplete = new AutoCompleteStringCollection();

            if (enumerable != null)
            {
                foreach (var item in enumerable)
                    autoComplete.Add(item);
            }

            textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox.AutoCompleteCustomSource = autoComplete;
        }
    }
}
