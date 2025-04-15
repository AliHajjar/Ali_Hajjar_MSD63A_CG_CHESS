using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using System.Threading.Tasks;
using System.IO;

public class SkinDownloader : MonoBehaviour
{
    [SerializeField] private RawImage targetImage; // or UnityEngine.UI.Image
    private FirebaseStorage storage;

    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance;

        // Example: download pfp1
        DownloadAndApplySkin("dlc_skins/pfp1.png");
    }

    public void DownloadAndApplySkin(string cloudPath)
    {
        string localPath = Path.Combine(Application.persistentDataPath, Path.GetFileName(cloudPath));

        // Skip download if already saved
        if (File.Exists(localPath))
        {
            Debug.Log("Using cached skin: " + localPath);
            ApplyTexture(File.ReadAllBytes(localPath));
            return;
        }

        // Otherwise, download from Firebase
        storage.GetReference(cloudPath).GetBytesAsync(1024 * 1024).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to download: " + task.Exception);
                return;
            }

            // Save locally
            File.WriteAllBytes(localPath, task.Result);
            Debug.Log("Downloaded and saved to: " + localPath);

            ApplyTexture(task.Result);
        });
    }

    private void ApplyTexture(byte[] data)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        targetImage.texture = tex; // for RawImage; use .sprite for Image
    }
}
