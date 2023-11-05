using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake;

public class BackgroundSFX : MonoBehaviour
{
    public Snake.Snake snakeScript; // Reference to the Snake script

    private CubeSideCoordinate currentSideCoordinate;
    private string currentSide;
    private AudioSource currentAudioSource;
    private float lastSideChangeTime;

    // Add MP3 audio files here.
    public AudioClip frontAudioClip;
    public AudioClip backAudioClip;
    public AudioClip rightAudioClip;
    public AudioClip leftAudioClip;
    public AudioClip upAudioClip;
    public AudioClip downAudioClip;

    private AudioSource frontAudio;
    private AudioSource backAudio;
    private AudioSource rightAudio;
    private AudioSource leftAudio;
    private AudioSource upAudio;
    private AudioSource downAudio;

    public float sideChangeTimeout = 3.0f; // Time in seconds to stop the audio

    private void Start()
    {
        // Perform assignments in the Start method.
        frontAudio = gameObject.AddComponent<AudioSource>();
        backAudio = gameObject.AddComponent<AudioSource>();
        rightAudio = gameObject.AddComponent<AudioSource>();
        leftAudio = gameObject.AddComponent<AudioSource>();
        upAudio = gameObject.AddComponent<AudioSource>();
        downAudio = gameObject.AddComponent<AudioSource>();

        // Load the MP3 files and assign them to the corresponding AudioSources.
        frontAudio.clip = frontAudioClip;
        backAudio.clip = backAudioClip;
        rightAudio.clip = rightAudioClip;
        leftAudio.clip = leftAudioClip;
        upAudio.clip = upAudioClip;
        downAudio.clip = downAudioClip;

        // Initialize the current audio source.
        currentAudioSource = frontAudio;
    }

    private void Update()
    {
        // Update the snake's position in every frame.
        currentSideCoordinate = snakeScript.CurrentSideCoordinate;
        string newSide = currentSideCoordinate.ToString();

        // Check if the side has changed and the audio is not already playing.
        if (newSide != currentSide)
        {
            StopCurrentAudio();
            PlayAudioBySide(newSide);
            lastSideChangeTime = Time.time;
        }
        else
        {
            // Check if the time since the last side change has exceeded the timeout.
            if (Time.time - lastSideChangeTime > sideChangeTimeout)
            {
                StopCurrentAudio();
            }
        }
        Debug.Log(currentSide);
    }

    public void PlayAudioBySide(string side)
    {
        // Play the audio depending on the side.
        currentSide = side;
        switch (side)
        {
            case "Front":
                currentAudioSource = frontAudio;
                break;
            case "Back":
                currentAudioSource = backAudio;
                break;
            case "Right":
                currentAudioSource = rightAudio;
                break;
            case "Left":
                currentAudioSource = leftAudio;
                break;
            case "Up":
                currentAudioSource = upAudio;
                break;
            case "Down":
                currentAudioSource = downAudio;
                break;
        }

        currentAudioSource.Play();
    }

    private void StopCurrentAudio()
    {
        // Stop the current audio.
        currentAudioSource.Stop();
    }
}
