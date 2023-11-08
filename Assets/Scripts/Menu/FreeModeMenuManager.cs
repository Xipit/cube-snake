using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FreeModeMenuManager : MonoBehaviour
{
    [Header("Random")]
    public Slider RandomSlider;

    [Header("Speed")]
    public TextMeshProUGUI SpeedText;
    public Slider SpeedSlider;

    [Header("Dimension")]
    public TMP_InputField DimensionXInput;
    public TMP_InputField DimensionYInput;
    public TMP_InputField DimensionZInput;

    private void Start()
    {
        // Attach a function to the "onValueChanged" event for each InputField
        DimensionXInput.onValueChanged.AddListener(OnDimensionInputChanged);
        DimensionYInput.onValueChanged.AddListener(OnDimensionInputChanged);
        DimensionZInput.onValueChanged.AddListener(OnDimensionInputChanged);
    }

    public void StartGame()
    {
        GameMode selectedGameMode = ScriptableObject.CreateInstance<GameMode>();

        // apply values from UI to GameMode
        selectedGameMode.dimension = GetDimensionInputValue();

        selectedGameMode.dimensionsAreRandom =
            RandomSlider.value.ToString() == "1" ? true : false;

        selectedGameMode.sidesAreUnique = true;

        selectedGameMode.cubeIsSquare = false; // TODO add UI to select option


        GameModeManager.Instance.StartGameWithGameMode(selectedGameMode);
    }

    public void SetSpeedUIText()
    {
        // Get the slider's value and update the Text component
        SpeedText.text = "x " + SpeedSlider.value.ToString("F1") + " Speed";

        // TODO add speed to gameMode
    }

    private Vector3 GetDimensionInputValue()
    {
        int.TryParse(DimensionXInput.text, out int x);
        int.TryParse(DimensionYInput.text, out int y);
        int.TryParse(DimensionZInput.text, out int z);

        return new Vector3(x, y, z);
    }

    private void OnDimensionInputChanged(string newValue)
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
}
