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

    public GameState GameState = GameState.Active;

    public void SetGameMode(GameMode mode)
    {
        this.Mode = mode;
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        CubeSpawner.Instance.SpawnCube(Mode);
    }

    public void GameOver()
    {
        GameState = GameState.GameOver;

        GameAudioManager.Instance.GameOver();
        GameMenuManager.Instance.OpenGameOverPanel(3, 40);
    }

    public void PauseGame()
    {
        if(GameState != GameState.Active)
        {
            return;
        }
        
        Time.timeScale = 0;
        GameMenuManager.Instance.OpenPausePanel(3, 40);

        GameState = GameState.Paused;
        
    }

    public void ResumeGame()
    {
        if(GameState != GameState.Paused)
        {
            return;
        }
        
        Time.timeScale = 1;
        GameMenuManager.Instance.CloseAllPanels();

        GameState = GameState.Active;
        
    }

    public void Start()
    {
        StartGame();
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.P))
        {
            switch (GameState)
            {
                case GameState.Active:
                    PauseGame();
                    break;

                case GameState.Paused:
                    ResumeGame();
                    break;

                default:
                    break;
            }
        }
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

public enum GameState
{
    Active = 0,
    Paused = 1,
    GameOver = 2
}