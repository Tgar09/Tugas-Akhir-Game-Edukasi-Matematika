using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController Instance;

    public GameObject videoPanel;
    public VideoPlayer videoPlayer;

    private void Awake()
    {
        Instance = this;
        videoPanel.SetActive(false);
    }

    public void PlayClip(VideoClip clip)
    {
        videoPanel.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPanel.SetActive(false);
    }
}