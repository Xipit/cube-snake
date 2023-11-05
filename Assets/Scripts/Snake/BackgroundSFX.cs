using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake;

public class BackgroundSFX : MonoBehaviour
{
    public Snake.Snake snakeScript; // Verweis auf das Snake-Skript

    private CubeSideCoordinate currentSideCoordinate;
    private string currentSide;
    private AudioSource currentAudioSource;
    private float lastSideChangeTime;

    // Fügen Sie hier Ihre MP3-Audiodateien hinzu.
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

    public float sideChangeTimeout = 3.0f; // Zeit in Sekunden bis zum Stoppen des Audios

    private void Start()
    {
        // Führen Sie die Zuweisungen im Start-Methode durch.
        frontAudio = gameObject.AddComponent<AudioSource>();
        backAudio = gameObject.AddComponent<AudioSource>();
        rightAudio = gameObject.AddComponent<AudioSource>();
        leftAudio = gameObject.AddComponent<AudioSource>();
        upAudio = gameObject.AddComponent<AudioSource>();
        downAudio = gameObject.AddComponent<AudioSource>();

        // Laden Sie die MP3-Dateien und weisen Sie sie den entsprechenden AudioSources zu.
        frontAudio.clip = frontAudioClip;
        backAudio.clip = backAudioClip;
        rightAudio.clip = rightAudioClip;
        leftAudio.clip = leftAudioClip;
        upAudio.clip = upAudioClip;
        downAudio.clip = downAudioClip;

        // Initialisieren Sie die aktuelle Audioquelle.
        currentAudioSource = frontAudio;
    }

    private void Update()
    {
        // Aktualisieren Sie die Position der Schlange in jeder Frame.
        currentSideCoordinate = snakeScript.CurrentSideCoordinate;
        string newSide = currentSideCoordinate.ToString();

        // Überprüfen Sie, ob sich die Seite geändert hat und das Audio nicht bereits abgespielt wird.
        if (newSide != currentSide)
        {
            StopCurrentAudio();
            PlayAudioBySide(newSide);
            lastSideChangeTime = Time.time;
        }
        else
        {
            // Überprüfen Sie, ob die Zeit seit der letzten Seitenänderung abgelaufen ist.
            if (Time.time - lastSideChangeTime > sideChangeTimeout)
            {
                StopCurrentAudio();
            }
        }
        Debug.Log(currentSide);
    }

    public void PlayAudioBySide(string side)
    {
        // Spielen Sie das Audio abhängig von der Seite.
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
        // Stoppen Sie das aktuelle Audio.
        currentAudioSource.Stop();
    }
}
