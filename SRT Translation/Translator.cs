using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SRT_Translation
{
    public static class Translator
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string[]> TranslateAsync(string text, string sourceLang, string targetLang)
        {
            string escapedText = Uri.EscapeDataString(text);

            // https://stackoverflow.com/questions/43155233
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={escapedText}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                GetTextFromJson(result);

                string[] texts = arabicTexts.ToArray();
                arabicTexts.Clear();
                return texts;
            }
            catch (Exception) { }
            return null;
        }

        static readonly List<string> arabicTexts = new List<string>();
        static readonly StringBuilder sb = new StringBuilder();
        private static void GetTextFromJson(string json)
        {
            JArray data = JArray.Parse(json);

            foreach (var item in data[0])
            {
                sb.Append(item[0].ToString());
                if (sb[sb.Length - 1] == '\n')
                {
                    sb.Remove(sb.Length - 1, 1);
                    arabicTexts.Add(sb.ToString());
                    sb.Clear();
                }
                else if (sb[sb.Length - 1] != ' ')
                    sb.Append(' ');
            }
            if (sb.Length > 0)
            {
                arabicTexts.Add(sb.ToString());
                sb.Clear();
            }
        }
    }
}
