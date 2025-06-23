using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Scene Settings")]
    public string sceneToLoad = "SampleScene"; // Bisa diubah dari Inspector

    private void Start()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            if (fadeImage != null)
            {
                fadeImage.gameObject.SetActive(true);
            }

            StartCoroutine(CrossDissolveAndLoadScene(sceneToLoad));
        }
    }

    private IEnumerator CrossDissolveAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeIn());
        BackgroundSoundManager.Instance.PlayNormalBGM();
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color color = fadeImage.color;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color color = fadeImage.color;
            color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
