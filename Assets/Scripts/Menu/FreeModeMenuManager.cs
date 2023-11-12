using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manage UI for Free Mode Popup.
/// <br/> Should be attached to the Free Mode Panel.
/// </summary>
public class FreeModeMenuManager : MonoBehaviour
{
    [Header("Random")]
    public Slider RandomSlider;

    [Header("Square")]
    public Slider SquareSlider;

    [Header("Speed")]
    public TextMeshProUGUI SpeedText;
    public Slider SpeedSlider;

    [Header("Dimension")]
    public TextMeshProUGUI DimensionText;
    public TMP_InputField DimensionXInput;
    public TMP_InputField DimensionYInput;
    public TMP_InputField DimensionZInput;

    private void Start()
    {
        // Attach a function to the "onDeselect" event for each InputField
        DimensionXInput.onDeselect.AddListener(ValidateDimensionInput);
        DimensionYInput.onDeselect.AddListener(ValidateDimensionInput);
        DimensionZInput.onDeselect.AddListener(ValidateDimensionInput);

        RandomSlider.onValueChanged.AddListener(WIPDImensionTextChange);
    }

    public void StartGame()
    {
        GameMode selectedGameMode = ScriptableObject.CreateInstance<GameMode>();

        // apply values from UI to GameMode
        selectedGameMode.dimensionsAreRandom =
            RandomSlider.value == 1 ? true : false;

        selectedGameMode.cubeIsSquare =
            SquareSlider.value == 1 ? true : false;

        selectedGameMode.speedFactor = SpeedSlider.value;

        selectedGameMode.dimension = GetDimensionInputValue();


        GameModeManager.Instance.StartGameWithGameMode(selectedGameMode);
    }

    public void SetSpeedUIText()
    {
        // Get the slider's value and update the Text component
        SpeedText.text = "x " + SpeedSlider.value.ToString("F1") + " Speed";
    }

    private Vector3 GetDimensionInputValue()
    {
        int.TryParse(DimensionXInput.text, out int x);
        int.TryParse(DimensionYInput.text, out int y);
        int.TryParse(DimensionZInput.text, out int z);

        return new Vector3(x, y, z);
    }

    private void ValidateDimensionInput(string newValue)
    {
        // Check if the input is a valid integer
        if (int.TryParse(newValue, out int value))
        {
            // Ensure the value is within the specified range
            value = Mathf.Clamp(value, Dimension3D.MIN, Dimension3D.MAX);

            TMP_InputField inputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            inputField.text = value.ToString();
        }
        else
        {
            // If the input is not a valid integer, reset it to the minimum value
            TMP_InputField inputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            inputField.text = Dimension3D.MIN.ToString();
            
        }
    }

    private void WIPDImensionTextChange(float newValue)
    {
        if(newValue == 1)
        {
            DimensionText.text = "Max Dimension";
        }
        else
        {
            DimensionText.text = "Dimension";
        }
    }
}
