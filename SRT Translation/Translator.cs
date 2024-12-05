using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRT_Translation
{
    public static class Translator
    {
        public static async Task<string> TranslateAsync(string text, string sourceLang, string targetLang)
        {
            // Example: Space => %20
            string escapedText = Uri.EscapeDataString(text);

            // https://stackoverflow.com/questions/43155233
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={escapedText}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();

                    int firstQuoteIndex = result.IndexOf("\"");
                    int secondQuoteIndex = result.IndexOf("\"", firstQuoteIndex + 1);
                    if (firstQuoteIndex != -1 && secondQuoteIndex != -1)
                    {
                        string translatedText = result.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);
                        return translatedText;
                    }
                }
                catch (Exception) { }
                return "";
            }
        }
    }
}
