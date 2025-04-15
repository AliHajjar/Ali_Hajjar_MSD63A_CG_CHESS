using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileSkinLoader : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    // Call this function with the Firebase image URL
    public void LoadSkinFromURL(string imageUrl)
    {
        StartCoroutine(DownloadSkin(imageUrl));
    }

    private IEnumerator DownloadSkin(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Download failed: " + request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // Convert Texture2D to Sprite
                Sprite skinSprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                targetImage.sprite = skinSprite;

                Debug.Log("Skin loaded and applied.");
            }
        }
    }
}
