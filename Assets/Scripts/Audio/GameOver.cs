using UnityEngine;

public class GameOver : MonoBehaviour
{
    public int gameOverSoundIndex = 1; // This index corresponds to the game over sound in the AudioManager

    public void OnGameOver()
    {

        // Play the sound for game over by specifying the index
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound(gameOverSoundIndex);
        }
    }
}
