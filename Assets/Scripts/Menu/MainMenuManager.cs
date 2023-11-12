using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

/// <summary>
/// Responsible for managing UI Canvas in the "Main" Scene
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    public GameObject PanelSettings;
    public GameObject PanelTutorial;
    public GameObject PanelFreeMode;
    
    public void StartRandomGame()
    {
        GameModeManager.Instance.StartGameWithGameMode(GameMode.CreateRandomGameMode());
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
        #if UNITY_EDITOR
        Debug.Log("Quit button has been pressed. This doesnt do anything in the Unity Editor.");
        return;
        #endif

        Application.Quit();
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

    private void CloseAllPanels()
    {
        PanelFreeMode.SetActive(false);
        PanelSettings.SetActive(false);
        PanelTutorial.SetActive(false);
    }
}
