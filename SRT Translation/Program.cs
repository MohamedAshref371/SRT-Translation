using System;
using System.IO;
using System.Windows.Forms;

namespace SRT_Translation
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!File.Exists("Newtonsoft.Json.dll"))
            {
                MessageBox.Show("Newtonsoft.Json.dll file is missing.");
                return;
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "إختر مجلد ملفات الترجمة\nSelect the SRT files folder",
                ShowNewFolderButton = false
            };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            var tf = new TranslationForm
            {
                SrtFiles = Directory.GetFiles(fbd.SelectedPath, "*.srt", SearchOption.AllDirectories)
            };

            var result = MessageBox.Show(new Form { WindowState = FormWindowState.Maximized, TopMost = true }, "إضغط 'نعم' إذا اردت الترجمة إلى العربية و'لا' للإنجليزية ", "الترجمة إلى ؟", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.RtlReading);
            if (result == DialogResult.No)
                tf.SwapLang();
            if (result != DialogResult.Cancel)
                Application.Run(tf);
        }
    }
}
