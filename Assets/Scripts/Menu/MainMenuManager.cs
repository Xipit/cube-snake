using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MainMenuManager : MonoBehaviour
{
    public GameObject PanelSettings;
    public GameObject PanelTutorial;
    public GameObject PanelFreeMode;
    public GameObject PanelDead;
    public GameObject PanelPause;
    
    public TextMeshProUGUI Dscore;
    public TextMeshProUGUI Dcover;
    public TextMeshProUGUI Pscore;
    public TextMeshProUGUI Pcover;

  
    public int scoreValue = 300;
    public float coverValue = 69;



    public void StartGame()
    {
        // Load the game scene when the "Start" button is clicked
        // SceneManager.LoadScene("GameScene");
    }

    public void OpenTutorialPanel()
    {
        CloseAllPanels();
        // Load the Tutorial when the "Tutorial" button is clicked
        PanelTutorial.SetActive(!PanelTutorial.activeSelf);
    }
       
    public void OpenFreeModePanel()
    {
        CloseAllPanels();
        // Load the Free Mode when the "Free Mode" button is clicked
        PanelFreeMode.SetActive(!PanelFreeMode.activeSelf);
    }    

    public void OpenOptionsPanel()
    {
        CloseAllPanels();
        // Load the Options Menu scene when the "Option" button is clicked
        PanelSettings.SetActive(!PanelSettings.activeSelf);
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

    public void GoBack()
    {
        CloseAllPanels();
    }

    public void showScore()
    {
        // Get the slider's value and update the Text component
        Dscore.text = "Score: " + scoreValue; 
        Dcover.text = "Covert of the Cube: " + coverValue + "%";
        Pscore.text = "Score: " + scoreValue;
        Pcover.text = "Covert of the Cube: " + coverValue + "%";
    }

    private void CloseAllPanels()
    {
        PanelFreeMode.SetActive(false);
        PanelSettings.SetActive(false);
        PanelTutorial.SetActive(false);
        PanelDead.SetActive(false);
        PanelPause.SetActive(false);
    }
}
