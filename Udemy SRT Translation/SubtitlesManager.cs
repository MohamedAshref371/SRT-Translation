using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ChatGPT4o
{
    internal static class SubtitlesManager
    {
        public static string[] GetMergedSubtitle(string[] lines)
        {
            var list = ReadSubtitles(lines);
            list = MergeSubtitles(list);
            return WriteSubtitles(list);
        }

        static List<SubtitleEntry> ReadSubtitles(string[] lines)
        {
            var subtitles = new List<SubtitleEntry>();

            SubtitleEntry current = null;
            Regex timePattern = new Regex(@"(\d{2}:\d{2}:\d{2},\d{3}) --> (\d{2}:\d{2}:\d{2},\d{3})");

            foreach (string line in lines)
            {
                if (int.TryParse(line, out _)) // رقم الترجمة
                {
                    if (current != null) subtitles.Add(current);
                    current = new SubtitleEntry();
                }
                else if (timePattern.IsMatch(line)) // التوقيت
                {
                    var match = timePattern.Match(line);
                    current.StartTime = match.Groups[1].Value;
                    current.EndTime = match.Groups[2].Value;
                }
                else if (!string.IsNullOrWhiteSpace(line)) // النص
                    current.Text += (current.Text.Length > 0 ? " " : "") + line;
                
            }
            if (current != null) subtitles.Add(current);
            return subtitles;
        }

        static List<SubtitleEntry> MergeSubtitles(List<SubtitleEntry> subtitles)
        {
            var merged = new List<SubtitleEntry>();
            SubtitleEntry current = null;

            foreach (var sub in subtitles)
            {
                if (current == null)
                    current = sub;
                else if (current.EndTime == sub.StartTime && !current.Text.Trim().EndsWith("."))
                {
                    current.EndTime = sub.EndTime;
                    current.Text += " " + sub.Text;
                }
                else
                {
                    merged.Add(current);
                    current = sub;
                }
            }
            if (current != null) merged.Add(current);
            return merged;
        }

        static string[] WriteSubtitles(List<SubtitleEntry> subtitles)
        {
            int index = 1;
            List<string> lines = new List<string>();
            foreach (var sub in subtitles)
            {
                lines.Add(index++.ToString());
                lines.Add($"{sub.StartTime} --> {sub.EndTime}");
                lines.Add(sub.Text);
                lines.Add("\n");
            }
            return lines.ToArray();
        }

        class SubtitleEntry
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Text { get; set; } = "";
        }
    }
}
