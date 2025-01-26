using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Assets.Scripts.Network
{
    public class PastebinModsSynchronizer : ModsSynchronizer
    {
        private const string SERVICE_URL = "https://pastebin.com/api/api_post.php";
        private const string API_KEY = "";
        private const string PASTE_NAME = "UserMod";

        public override string Upload(string modPath) => _ = UploadModToPastebin(modPath).Result;
        public override void Download(string modUrl, string outputPath) => _ = DownloadModFromPastebin(modUrl, outputPath);

        private async Task<string> UploadModToPastebin(string filePath)
        {
            if (!File.Exists(filePath) || !mLoader.LoadModNative(filePath, true))
                return string.Empty;

            byte[] zipBytes = File.ReadAllBytes(filePath);
            string base64Text = Convert.ToBase64String(zipBytes);

            using (HttpClient client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_dev_key", API_KEY),
                    new KeyValuePair<string, string>("api_option", "paste"),
                    new KeyValuePair<string, string>("api_paste_code", base64Text),
                    new KeyValuePair<string, string>("api_paste_private", "1"),
                    new KeyValuePair<string, string>("api_paste_name", PASTE_NAME),
                    new KeyValuePair<string, string>("api_paste_expire_date", "N")
                });

                HttpResponseMessage response = await client.PostAsync(SERVICE_URL, content);
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode || result.Contains("Bad API request"))
                    throw new Exception("Ошибка загрузки на Pastebin: " + result);

                return result;
            }
        }
        private async Task DownloadModFromPastebin(string pasteUrl, string destinationPath)
        {
            string pasteKey = pasteUrl.Split('/').Last();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"https://pastebin.com/raw/{pasteKey}");
                string base64Text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Ошибка загрузки текста из Pastebin: " + base64Text);

                if (!Directory.Exists(destinationPath))
                    Directory.CreateDirectory(destinationPath);

                byte[] zipBytes = Convert.FromBase64String(base64Text);
                File.WriteAllBytes(destinationPath, zipBytes);

                mLoader.LoadModByPath(destinationPath);
            }
        }
    }
}
