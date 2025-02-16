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
            Application.Run(tf);
        }
    }
}
