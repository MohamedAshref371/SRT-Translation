using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace ChatGPT4o
{
    internal static class SubtitleProcessor
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
                else if (IsTimeClose(current.EndTime, sub.StartTime) && !current.Text.Trim().EndsWith("."))
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

        static bool IsTimeClose(string endTime, string startTime, int thresholdMs = 100)
        {
            TimeSpan end = TimeSpan.Parse(endTime.Replace(',', '.'));
            TimeSpan start = TimeSpan.Parse(startTime.Replace(',', '.'));

            return (start - end).TotalMilliseconds <= thresholdMs;
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


        public static string[] SplitLongLine(string[] lines, int maxLength = 90)
        {
            List<string> newLines = new List<string>();
            int index = 1;

            for (int i = 0; i < lines.Length; i++)
            {
                if (timePattern.IsMatch(lines[i]) && i + 1 < lines.Length)
                {
                    string timing = lines[i];
                    string text = lines[i + 1];

                    Match match = timePattern.Match(timing);
                    if (!match.Success) continue; // التأكد من نجاح التحقق قبل المتابعة

                    TimeSpan startTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
                    TimeSpan endTime = TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));

                    if (text.Length > maxLength) // الحد الأقصى لكل سطر
                    {
                        int splitIndex = FindBestSplit(text);
                        if (splitIndex == -1 || splitIndex >= text.Length - 1)
                        {
                            splitIndex = text.Length / 2; // احتياطيًا إذا لم نجد فاصلًا جيدًا
                        }

                        string part1 = text.Substring(0, splitIndex).Trim();
                        string part2 = text.Substring(splitIndex).Trim();

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
            }
            return newLines.ToArray();
        }

        private static int FindBestSplit(string text)
        {
            int mid = text.Length / 2;
            int leftSpace = text.LastIndexOf(' ', mid);
            int rightSpace = text.IndexOf(' ', mid);

            if (leftSpace == -1) return rightSpace;
            if (rightSpace == -1) return leftSpace;
            return (mid - leftSpace < rightSpace - mid) ? leftSpace : rightSpace;
        }

    }
}
