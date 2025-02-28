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

            var tempForm = new Form { WindowState = FormWindowState.Maximized, TopMost = true };
            bool isUdemy = MessageBox.Show(tempForm, "إضغط 'نعم' إذا كانت ملفات الترجمة من udemy", "Udemy ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading) == DialogResult.Yes;
            DialogResult to_ar = MessageBox.Show(tempForm, "إضغط 'نعم' إذا اردت الترجمة إلى العربية و'لا' للإنجليزية ", "الترجمة إلى ؟", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            tempForm.Dispose();

            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "إختر مجلد ملفات الترجمة\nSelect the SRT files folder",
                ShowNewFolderButton = false
            };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            var tf = new TranslationForm { IsUdemy = isUdemy };
            if (isUdemy)
                tf.Mp4Files = Directory.GetFiles(fbd.SelectedPath, "*.mp4", SearchOption.AllDirectories);
            else
                tf.SrtFiles = Directory.GetFiles(fbd.SelectedPath, "*.srt", SearchOption.AllDirectories);

            if (to_ar == DialogResult.No)
                tf.SwapLang();
            if (to_ar != DialogResult.Cancel)
                Application.Run(tf);
        }
    }
}
