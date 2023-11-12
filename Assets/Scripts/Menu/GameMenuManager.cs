using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for managing UI Canvas in the "Game" Scene
/// </summary>
public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance { get; private set; }

    [Header("GameOver")]
    public GameObject PanelGameOver;
    public TextMeshProUGUI ScoreGameOver;
    public TextMeshProUGUI CubeCoverGameOver;

    [Header("GameWin")]
    public GameObject PanelGameWin;
    public TextMeshProUGUI ScoreGameWin;
    public TextMeshProUGUI CubeCoverGameWin;

    [Header("Pause")]
    public GameObject PanelPause;
    public TextMeshProUGUI ScorePause;
    public TextMeshProUGUI CubeCoverPause;

    public void Start()
    {
        CloseAllPanels();
    }

    public void OpenPausePanel(int score, float cubeCoverPercentage)
    {
        CloseAllPanels();
        PanelPause.SetActive(!PanelPause.activeSelf);

        UpdateScoreText(score, cubeCoverPercentage);
    }

    public void OpenGameOverPanel(int score, float cubeCoverPercentage)
    {
        CloseAllPanels();
        PanelGameOver.SetActive(!PanelGameOver.activeSelf);

        UpdateScoreText(score, cubeCoverPercentage);
    }

    public void OpenGameWinPanel(int score, float cubeCoverPercentage)
    {
        CloseAllPanels();
        PanelGameWin.SetActive(!PanelGameWin.activeSelf);

        UpdateScoreText(score, cubeCoverPercentage);
    }

    private void UpdateScoreText(int score, float cubeCoverPercentage)
    {
        string scoreText = "Score: " + score;
        string cubeCoverText = "Cube covered: " + cubeCoverPercentage + "%";

        ScoreGameOver.text = scoreText;
        ScoreGameWin.text = scoreText;
        ScorePause.text = scoreText;

        CubeCoverGameOver.text = cubeCoverText;
        CubeCoverGameWin.text = cubeCoverText;
        CubeCoverPause.text = cubeCoverText;
    }

    public void CloseAllPanels()
    {
        PanelGameOver.SetActive(false);
        PanelPause.SetActive(false);
        PanelGameOver.SetActive(false);
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void Retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
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
