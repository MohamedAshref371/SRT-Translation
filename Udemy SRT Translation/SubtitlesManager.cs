using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace ChatGPT4o
{
    internal static class SubtitlesManager
    {
        static readonly Regex timePattern = new Regex(@"(\d{2}:\d{2}:\d{2}[,.]\d{3}) --> (\d{2}:\d{2}:\d{2}[,.]\d{3})");

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
                if (current == null) current = sub;

                // تحقق مما إذا كانت الجملة السابقة تنتهي بنقطة
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
                lines.Add("");
            }
            return lines.ToArray();
        }

        class SubtitleEntry
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Text { get; set; } = "";
        }

        public static string[] SplitLongLine(string[] lines)
        {
            List<string> newLines = new List<string>();

            int index = 1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (timePattern.IsMatch(lines[i]) && i + 1 < lines.Length)
                {
                    string timing = lines[i];
                    string text = lines[i + 1];

                    if (text.Length > 80) // الحد الأقصى لكل سطر - يمكن تعديله
                    {
                        int splitIndex = text.IndexOf(' ', text.Length / 2);
                        if (splitIndex == -1) splitIndex = text.Length / 2;

                        string part1 = text.Substring(0, splitIndex).Trim();
                        string part2 = text.Substring(splitIndex).Trim();

                        Match match = timePattern.Match(timing);
                        TimeSpan startTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
                        TimeSpan endTime = TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));

                        TimeSpan midTime = startTime + TimeSpan.FromTicks((endTime - startTime).Ticks / 2);

                        newLines.Add(index++.ToString());
                        newLines.Add($"{match.Groups[1].Value} --> {midTime:hh\\:mm\\:ss\\,fff}");
                        newLines.Add(part1);
                        newLines.Add("");

                        newLines.Add(index++.ToString());
                        newLines.Add($"{midTime:hh\\:mm\\:ss\\,fff} --> {match.Groups[2].Value}");
                        newLines.Add(part2);
                        newLines.Add("");
                    }
                    else
                    {
                        newLines.Add(index++.ToString());
                        newLines.Add(timing);
                        newLines.Add(text);
                        newLines.Add("");
                    }

                    i++; // تجاوز النص بعد التوقيت
                }
                else if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    newLines.Add(lines[i]);
                }
            }
            return newLines.ToArray();
        }

    }
}
