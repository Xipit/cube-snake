using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource[] audioSources;

    private void Start()
    {
        // Get all AudioSource components in the AudioManager
        audioSources = GetComponents<AudioSource>();

        // Stop all audio sources at the beginning
        foreach (var source in audioSources)
        {
            source.Stop();
        }
    }

    public void PlaySound(int index)
    {
        if (index >= 0 && index < audioSources.Length)
        {
            // Play the sound with the specified index
            audioSources[index].Play();
        }
    }
}
