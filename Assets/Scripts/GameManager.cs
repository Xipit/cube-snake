using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles GameModes and general game tasks, such as start, pause, win, etc.
/// <br/> see: https://stackoverflow.com/questions/69978315/unity-game-modes-as-state-machine
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode Mode;


    public void SetGameMode(GameMode mode)
    {
        this.Mode = mode;
    }

    public void StartGame()
    {
        CubeSpawner.Instance.SpawnCube(Mode);
    }

    public void GameOver()
    {
        GameAudioManager.Instance.GameOver();
    }

    public void Start()
    {
        StartGame();
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
