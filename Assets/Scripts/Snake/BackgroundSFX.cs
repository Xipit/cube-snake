using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake;

public class BackgroundSFX : MonoBehaviour
{
    public Snake.Snake snakeScript; // Reference to the Snake script

    // Add MP3 audio files here.
    public AudioClip frontAudioClip;
    public AudioClip backAudioClip;
    public AudioClip rightAudioClip;
    public AudioClip leftAudioClip;
    public AudioClip upAudioClip;
    public AudioClip downAudioClip;

    private AudioSource currentAudioSource;
    private AudioSource lastSideAudioSource;
    

    public float sideChangeTimeout = 3.0f; // Time in seconds to stop the audio

    private void Start()
    {
        currentAudioSource = gameObject.AddComponent<AudioSource>();
        lastSideAudioSource = gameObject.AddComponent<AudioSource>();

        currentAudioSource.Play();
        lastSideAudioSource.Play();
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

        // fade in and out
        currentAudioSource.volume = 0;
        StartCoroutine(AudioHelper.StartFade(currentAudioSource, 1000, 1));

        lastSideAudioSource.volume = 1;
        StartCoroutine(AudioHelper.StartFade(lastSideAudioSource, 1000, 0));
    }
}
