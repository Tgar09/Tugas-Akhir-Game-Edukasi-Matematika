using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    public static VideoPlayerManager Instance;

    public VideoPlayer videoPlayer;
    public GameObject videoUI;
    public GameObject video;
    
    private void Start()
    {
        // Optional: Logika saat start
        Debug.Log("VideoPlayerManager siap digunakan.");

        video.SetActive(false); 
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }// Sembunyikan UI saat awal
    }

    public void PlayVideo(string title, string localPath)
    {
        BackgroundSoundManager.Instance.StopBGM();
        string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, localPath);

        Debug.Log($"Memutar video: {title} dari path: {fullPath}");

        video.SetActive(true);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = fullPath;
        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            vp.Play();
        };

        // Pastikan tidak double subscribe
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video selesai diputar.");
        video.SetActive(false); // Sembunyikan UI
        BackgroundSoundManager.Instance.PlayNormalBGM(); 
    }

}
