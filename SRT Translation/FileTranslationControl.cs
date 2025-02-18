using System.Drawing;
using System.Windows.Forms;

namespace SRT_Translation
{
    public class FileTranslationControl : Control
    {
        public static readonly Size ControlSize = new Size(782, 50);

        private readonly Label fileName = new Label
        {
            Font = new Font("Tahoma", 12F),
            Location = new Point(11, 9),
            Size = new Size(530, 32),
            TextAlign = ContentAlignment.MiddleCenter
        };

        private readonly Label precent = new Label
        {
            Font = new Font("Tahoma", 12F),
            Location = new Point(550, 9),
            Size = new Size(100, 32),
            TextAlign = ContentAlignment.MiddleCenter
        };

        private readonly Label linesCount = new Label
        {
            Font = new Font("Tahoma", 12F),
            Location = new Point(670, 9),
            Size = new Size(100, 32),
            TextAlign = ContentAlignment.MiddleCenter
        };

        public int PrecentText { set => precent.Text = value.ToString(); }
        public int LinesCountText { set => linesCount.Text = value.ToString(); }

        public readonly string FilePath;
        public readonly string OutputFilePath;

        public FileTranslationControl(string path, string name, string outputPath, Color backColor)
        {
            FilePath = path;
            fileName.Text = name;
            OutputFilePath = outputPath;
            
            ClientSize = ControlSize;
            Controls.Add(precent);
            Controls.Add(fileName);
            Controls.Add(linesCount);
            if (backColor.A == 255) BackColor = backColor;
        }
    }
}
