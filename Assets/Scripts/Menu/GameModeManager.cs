using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// This class stores the reference to the current GameMode and allows other scripts to alter that GameMode.
/// <br/> The current GameMode controls the Game Settings and is referenced inside the "Game" Scene by the Game Manager.
/// </summary>
public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public GameMode Mode;

    public void SetGameMode(GameMode mode)
    {
        Mode.Set(mode);
    }

    public void StartGameWithGameMode(GameMode mode)
    {
        SetGameMode(mode);
        SceneManager.LoadScene("Game");
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
