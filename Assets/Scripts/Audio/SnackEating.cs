using UnityEngine;

public class SnackEating : MonoBehaviour
{
    public int snackSoundIndex = 0; // This index corresponds to the sound for snack eating in the AudioManager

    public void OnSnackEaten()
    {
        // Play the sound for snack eating by specifying the index
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound(snackSoundIndex);
        }
    }
}
