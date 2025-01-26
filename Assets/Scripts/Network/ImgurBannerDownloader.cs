using System.IO;
using System.Net.Http;
using System.Collections;
using Assets.Scripts.Network;
using HtmlAgilityPack;
using UnityEngine;

public class ImgurBannerDownloader : BannerDownloader
{
    public override void Download(string url)
    {
        StartCoroutine(DownloadRoutine(url));
    }

    private IEnumerator DownloadRoutine(string pageUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            var pageTask = client.GetStringAsync(pageUrl);
            while (!pageTask.IsCompleted)
                yield return null;

            string pageContent = pageTask.Result;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);
            var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            string imageUrl = imageNode?.GetAttributeValue("content", null);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                var imageTask = client.GetByteArrayAsync(imageUrl);
                while (!imageTask.IsCompleted)
                    yield return null;

                byte[] imageBytes = imageTask.Result;
                string savePath = Path.Combine(Application.persistentDataPath, "Servers/banner.png");

                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                File.WriteAllBytes(savePath, imageBytes);

                Sprite currentSprite = null;
                Texture2D texture = new Texture2D(2, 2);

                if (texture.LoadImage(imageBytes))
                {
                    Rect spriteRect = new Rect(0, 0, texture.width, texture.height);
                    Vector2 pivot = new Vector2(0.5f, 0.5f);
                    currentSprite = Sprite.Create(texture, spriteRect, pivot);
                }

                Downloaded(currentSprite);
            }
        }
    }
}