using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRT_Translation
{
    public partial class TranslationForm : Form
    {
        public string[] SrtFiles;

        public TranslationForm() => InitializeComponent();

        private async void TranslationForm_Load(object sender, EventArgs e)
        {
            FileTranslationControl ftc;
            string output;
            foreach (string s in SrtFiles)
            {
                if (s.EndsWith(" en.srt") || s.EndsWith("_en.srt") || s.EndsWith("-en.srt"))
                    output = s.Substring(0, s.Length - 6) + "ar.srt";
                else if (!s.EndsWith(" ar.srt") && !s.EndsWith("_ar.srt") && !s.EndsWith("-ar.srt"))
                    output = s.Substring(0, s.Length - 4) + "_ar.srt";
                else
                    continue;

                ftc = new FileTranslationControl(s, Path.GetFileName(s), output, GetColor(flowLayoutPanel1));
                flowLayoutPanel1.Controls.Add(ftc);
            }
            
            await TranslateFile();
            Close();
        }

        public async Task TranslateFile()
        {
            string[] lines;
            FileTranslationControl ftc;
            while (flowLayoutPanel1.Controls.Count > 0)
            {
                ftc = (FileTranslationControl)flowLayoutPanel1.Controls[0];
                if (File.Exists(ftc.FilePath))
                {
                    lines = File.ReadAllLines(ftc.FilePath);
                    await TranslateAsync(lines, "en", "ar", ftc);
                    File.WriteAllLines(ftc.OutputFilePath, lines);
                    flowLayoutPanel1.Controls.Remove(ftc);
                }
            }
        }

        public static async Task TranslateAsync(string[] text, string sourceLang, string targetLang, FileTranslationControl ftc = null)
        {
            string s;
            ftc.LinesCountText = text.Length;
            for (int i = 0; i < text.Length; i++)
            {
                s = text[i];
                if (s.Trim() != "" && !int.TryParse(s, out _) && (!s.Contains(" --> ") || !s.Contains(":") || !s.Contains(",") && !s.Contains(".")))
                {
                    text[i] = await Translator.TranslateAsync(s, sourceLang, targetLang);
                    if (ftc != null) ftc.PrecentText = i;
                }
            }
        }

        private Color GetColor(Control control)
        {
            switch (control.Controls.Count % 8)
            {
                case 1: return Color.FromArgb(250, 200, 200);
                case 2: return Color.FromArgb(250, 250, 200);
                case 3: return Color.FromArgb(200, 250, 200);
                case 4: return Color.FromArgb(200, 200, 200);
                case 5: return Color.FromArgb(200, 250, 250);
                case 6: return Color.FromArgb(200, 200, 250);
                case 7: return Color.FromArgb(250, 200, 250);
                default: return Color.FromArgb(250, 250, 250);
            }
        }
    }
}
