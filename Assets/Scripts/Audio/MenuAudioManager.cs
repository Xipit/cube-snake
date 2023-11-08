using UnityEngine;


public class MenuAudioManager : MonoBehaviour
{
    private static MenuAudioManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip music)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.Play();
    }

    // Add more audio control methods as needed (e.g., Pause, Stop, Volume).
}
