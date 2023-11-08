using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Include the necessary using directive

public class MainMenuManager : MonoBehaviour
{
    public GameObject PanelSettings;
    public GameObject PanelTutorial;
    public GameObject PanelFreeMode;
    public GameObject PanelDead;
    public GameObject PanelPause;
    public TextMeshProUGUI TextDimensions; 
    public Slider Slider_Dimensions;
    public TextMeshProUGUI TextSpeed;
    public Slider Slider_Speed;
    public TMP_InputField InputField_X;
    public TMP_InputField InputField_Y;
    public TMP_InputField InputField_Z;
    public TextMeshProUGUI Dscore;
    public TextMeshProUGUI Dcover;
    public TextMeshProUGUI Pscore;
    public TextMeshProUGUI Pcover;

    private int minValue = 1;
    private int maxValue = 50;
    public int scoreValue = 300;
    public float coverValue = 69;

    public void StartGame()
    {
        // Load the game scene when the "Start" button is clicked
        // SceneManager.LoadScene("GameScene");
    }

    public void Tutorial()
    {
        DeactivateOtherPanels();
        // Load the Tutorial when the "Tutorial" button is clicked
        PanelTutorial.SetActive(!PanelTutorial.activeSelf);
    }
       
    public void Free_Mode()
    {
        DeactivateOtherPanels();
        // Load the Free Mode when the "Free Mode" button is clicked
        PanelFreeMode.SetActive(!PanelFreeMode.activeSelf);
    }

    private void Start()
    {
        // Attach a function to the "onValueChanged" event for each InputField
        InputField_X.onValueChanged.AddListener(OnInputFieldValueChanged);
        InputField_Y.onValueChanged.AddListener(OnInputFieldValueChanged);
        InputField_Z.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    private void OnInputFieldValueChanged(string newValue)
    {
        // Check if the input is a valid integer
        if (int.TryParse(newValue, out int value))
        {
            // Ensure the value is within the specified range
            value = Mathf.Clamp(value, minValue, maxValue);
            // Update the TMP_InputField text with the valid value
            TMP_InputField inputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            inputField.text = value.ToString();
        }
        else
        {
            // If the input is not a valid integer, reset it to the minimum value
            TMP_InputField inputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            inputField.text = minValue.ToString();
        }
    }

    public void Change_Speed()
    {
        // Get the slider's value and update the Text component
        TextSpeed.text = "x " + Slider_Speed.value.ToString("F1") + " Speed"; // Adjust the format as needed
    }

    public void Options()
    {
        DeactivateOtherPanels();
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

    public void back()
    {
        DeactivateOtherPanels();
    }

    public void showScore()
    {
        // Get the slider's value and update the Text component
        Dscore.text = "Score: " + scoreValue; 
        Dcover.text = "Covert of the Cube: " + coverValue + "%";
        Pscore.text = "Score: " + scoreValue;
        Pcover.text = "Covert of the Cube: " + coverValue + "%";
    }

    private void DeactivateOtherPanels()
    {
        PanelFreeMode.SetActive(false);
        PanelSettings.SetActive(false);
        PanelTutorial.SetActive(false);
        PanelDead.SetActive(false);
        PanelPause.SetActive(false);
    }
}
