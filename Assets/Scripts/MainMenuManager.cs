using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game scene when the "Start" button is clicked
        //SceneManager.LoadScene("GameScene");
    }

    public void Options()
    {
        // Load the Options Menu scene when the "Option" button is clicked
        SceneManager.LoadScene("Menu Option");
    }

    public void QuitGame()
    {
        // Load the Options Menu scene when the "Menu Quit" button is clicked
        SceneManager.LoadScene("Menu Quit");
    }

    public void CloseGame()
    {
        // Quit the application (works in a standalone build)
        Application.Quit();
    }

    public void BackToMain()
    { 
        // Load the Main Menu scene when the "Zurück" button is cloicked
        SceneManager.LoadScene("Menu");
    }
}
