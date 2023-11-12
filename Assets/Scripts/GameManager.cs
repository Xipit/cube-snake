using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

/// <summary>
/// This class handles GameModes and general game tasks, such as start, pause, win, etc.
/// <br/> see: https://stackoverflow.com/questions/69978315/unity-game-modes-as-state-machine
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode Mode;

    public GameState GameState = GameState.Active;

    public int SnakeLength { get; private set; } = 3;
    public int MaxSnakeLength { get; private set; } = 1;

    public void SetGameMode(GameMode mode)
    {
        this.Mode = mode;
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        Cube? cube = CubeSpawner.Instance.SpawnCube(Mode);

        if (cube is not null)
        {
            MaxSnakeLength = cube.Dimension.GetFieldAmount();
        }
    }

    public void GameOver()
    {
        GameState = GameState.GameOver;

        GameAudioManager.Instance.GameOver();
        GameMenuManager.Instance.OpenGameOverPanel(CalculateScore(), CalculateFieldCoverage());
    }

    public void PauseGame()
    {
        if(GameState != GameState.Active)
        {
            return;
        }
        
        Time.timeScale = 0;
        GameMenuManager.Instance.OpenPausePanel(CalculateScore(), CalculateFieldCoverage());

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

    public void AddSnakeLength()
    {
        SnakeLength++;
    }

    private int CalculateScore()
    {
        return (SnakeLength - 3) * 10;
    }

    private float CalculateFieldCoverage()
    {
        return SnakeLength / MaxSnakeLength;
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