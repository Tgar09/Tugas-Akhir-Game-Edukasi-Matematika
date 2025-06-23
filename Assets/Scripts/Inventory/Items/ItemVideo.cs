using UnityEngine;

[CreateAssetMenu(menuName = "Items/Video", fileName = "ItemVideo")]
public class ItemVideo : InventoryItem
{
    [Header("Video Data")]
    public string videoTitle;
    public string videoPath; // Contoh: "Videos/nama_video.mp4"

    public override bool UseItem()
    {
        Debug.Log($"Using video item: {videoTitle} - {videoPath}");

        VideoPlayerManager manager = GameObject.FindObjectOfType<VideoPlayerManager>();
        if (manager != null)
        {
            manager.PlayVideo(videoTitle, videoPath);
        }
        else
        {
            Debug.LogWarning("VideoPlayerManager not found in the scene.");
        }

        return false; // Item tidak habis dipakai
    }

    public override void RemoveItem()
    {
        Debug.Log($"Removed video item: {videoTitle}");
    }
}
