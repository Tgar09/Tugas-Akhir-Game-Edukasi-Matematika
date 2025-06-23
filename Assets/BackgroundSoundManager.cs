using UnityEngine;
using System.Collections;

public class BackgroundSoundManager : MonoBehaviour
{
    public static BackgroundSoundManager Instance { get; private set; }
    public bool isBoss = false;
    [Header("BGM Clips")]
    [SerializeField] private AudioClip normalBGM;
    [SerializeField] private AudioClip battleBGM;
    [SerializeField] private AudioClip victoryBGM;
    [SerializeField] private AudioClip deathBGM;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip sfxShoot;
    [SerializeField] private AudioClip sfxDeath;
    [SerializeField] private AudioClip sfxHit;
    [SerializeField] private AudioClip sfxCollect;
    [SerializeField] private AudioClip sfxChestOpen;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.1f;
    [SerializeField] private float defaultVolume = 0.8f;
    [SerializeField] private float victoryDuration = 3f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;
    private Coroutine returnToNormalCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // BGM Source
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = defaultVolume;

        // SFX Source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = defaultVolume;
    }
    private void Start()
    {
        if(isBoss){
            PlayBattleBGM();
        }else{
            PlayNormalBGM();
        }
    }


    // === BGM Controls ===
    public void PlayNormalBGM()
    {
        PlayBGM(normalBGM, true, defaultVolume);
    }

    public void PlayBattleBGM()
    {
        PlayBGM(battleBGM, true, defaultVolume);
    }

    public void PlayVictoryBGMThenBackToNormal()
    {
        if (victoryBGM == null) return;

        StopAllCoroutines(); // Hentikan semua coroutine (fade atau sebelumnya)
        PlayBGM(victoryBGM, false, defaultVolume);
        returnToNormalCoroutine = StartCoroutine(ReturnToNormalAfterVictory());
    }

    public void PlayDeathBGM()
    {
        if (deathBGM == null) return;

        StopAllCoroutines();
        PlayBGM(deathBGM, false, defaultVolume);
    }

    private IEnumerator ReturnToNormalAfterVictory()
    {
        yield return new WaitForSeconds(victoryDuration);
        PlayNormalBGM();
    }

    private void PlayBGM(AudioClip newClip, bool loop = true, float volume = 1f)
    {
        if (newClip == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewClip(newClip, volume, loop));
    }

    private IEnumerator FadeToNewClip(AudioClip newClip, float targetVolume, bool loop)
    {
        float startVolume = bgmSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = loop;
        bgmSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();

        // Hentikan coroutine yang akan memutar ulang normal BGM
        if (returnToNormalCoroutine != null)
        {
            StopCoroutine(returnToNormalCoroutine);
            returnToNormalCoroutine = null;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }


    // === SFX Controls ===
    private void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayShootSFX()     => PlaySFX(sfxShoot);
    public void PlayDeathSFX()     => PlaySFX(sfxDeath);
    public void PlayHitSFX()       => PlaySFX(sfxHit);
    public void PlayCollectSFX()   => PlaySFX(sfxCollect);
    public void PlayChestOpenSFX()=> PlaySFX(sfxChestOpen);
}
