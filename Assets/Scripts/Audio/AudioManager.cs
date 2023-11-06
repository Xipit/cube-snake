using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("General Audio")]
    public AudioSource eatSnackAudioSource;
    public AudioSource gameOverAudioSource;

    [Header("Music Audio")]
    public AudioSource backgroundAudioSource;

    // Backgroundmusic specific for every CubeSide
    public AudioClip frontAudioClip;
    public AudioClip backAudioClip;
    public AudioClip rightAudioClip;
    public AudioClip leftAudioClip;
    public AudioClip upAudioClip;
    public AudioClip downAudioClip;

    private AudioSource currentAudioSource;
    private AudioSource lastSideAudioSource;

    private void Start()
    {
        //backgroundAudioSource.Play();

        currentAudioSource = gameObject.AddComponent<AudioSource>();
        lastSideAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void GameOver()
    {
        backgroundAudioSource.Stop();
        gameOverAudioSource.Play();
    }

    public void SwitchCubeSide(CubeSideCoordinate cubeSide)
    {
        switch (cubeSide)
        {
            case CubeSideCoordinate.Front:
                SetNewAudioClipAndFadeIn(frontAudioClip);
                break;

            case CubeSideCoordinate.Back:
                SetNewAudioClipAndFadeIn(backAudioClip);
                break;

            case CubeSideCoordinate.Right:
                SetNewAudioClipAndFadeIn(rightAudioClip);
                break;

            case CubeSideCoordinate.Left:
                SetNewAudioClipAndFadeIn(leftAudioClip);
                break;

            case CubeSideCoordinate.Up:
                SetNewAudioClipAndFadeIn(upAudioClip);
                break;

            case CubeSideCoordinate.Down:
                SetNewAudioClipAndFadeIn(downAudioClip);
                break;

            default:
                break;
        }
    }

    public void SetNewAudioClipAndFadeIn(AudioClip newCubeSideAudioClip)
    {
        lastSideAudioSource.clip = currentAudioSource.clip;
        currentAudioSource.clip = newCubeSideAudioClip;

        lastSideAudioSource.Play();
        currentAudioSource.Play();

        // fade in and out
        currentAudioSource.volume = 0;
        StartCoroutine(AudioHelper.StartFade(currentAudioSource, 1, 1));

        lastSideAudioSource.volume = 1;
        StartCoroutine(AudioHelper.StartFade(lastSideAudioSource, 1, 0));
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
