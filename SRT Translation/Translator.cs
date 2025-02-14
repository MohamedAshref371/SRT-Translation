using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private static void GetTextFromJson(string json)
        {
            JArray data = JArray.Parse(json);

            foreach (var item in data[0])
                arabicTexts.Add(item[0].ToString().Replace("\n", ""));
        }
    }
}
