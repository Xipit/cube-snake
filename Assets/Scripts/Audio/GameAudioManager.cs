using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("General Audio")]
    public AudioSource EatSnackAudioSource;
    public AudioSource GameOverAudioSource;

    [Header("Music Audio")]
    public AudioSource BackgroundAudioSource;

    // Backgroundmusic specific for every CubeSide
    public AudioClip FrontAudioClip;
    public AudioClip BackAudioClip;
    public AudioClip RightAudioClip;
    public AudioClip LeftAudioClip;
    public AudioClip UpAudioClip;
    public AudioClip DownAudioClip;

    private AudioSource CurrentAudioSource;
    private AudioSource LastSideAudioSource;

    private void Start()
    {
        BackgroundAudioSource.Play();

        CurrentAudioSource = gameObject.AddComponent<AudioSource>();
        LastSideAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void GameOver()
    {
        BackgroundAudioSource.Stop();
        GameOverAudioSource.Play();
    }

    public void SwitchCubeSide(CubeSideCoordinate cubeSide)
    {
        switch (cubeSide)
        {
            case CubeSideCoordinate.Front:
                SetNewAudioClipAndFadeIn(FrontAudioClip);
                break;

            case CubeSideCoordinate.Back:
                SetNewAudioClipAndFadeIn(BackAudioClip);
                break;

            case CubeSideCoordinate.Right:
                SetNewAudioClipAndFadeIn(RightAudioClip);
                break;

            case CubeSideCoordinate.Left:
                SetNewAudioClipAndFadeIn(LeftAudioClip);
                break;

            case CubeSideCoordinate.Up:
                SetNewAudioClipAndFadeIn(UpAudioClip);
                break;

            case CubeSideCoordinate.Down:
                SetNewAudioClipAndFadeIn(DownAudioClip);
                break;

            default:
                break;
        }
    }

    private void SetNewAudioClipAndFadeIn(AudioClip newCubeSideAudioClip)
    {
        if (CurrentAudioSource.clip == null)
        {
            CurrentAudioSource.clip = newCubeSideAudioClip;
        }

        LastSideAudioSource.clip = CurrentAudioSource.clip;
        CurrentAudioSource.clip = newCubeSideAudioClip;

        LastSideAudioSource.Play();
        CurrentAudioSource.Play();

        // fade in and out
        CurrentAudioSource.volume = 0;
        StartCoroutine(AudioHelper.StartFade(CurrentAudioSource, 1, 1));

        LastSideAudioSource.volume = 1;
        StartCoroutine(AudioHelper.StartFade(LastSideAudioSource, 1, 0));
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
