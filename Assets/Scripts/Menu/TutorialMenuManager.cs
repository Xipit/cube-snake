using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage UI for Tutorial Popup.
/// <br/> Should be attached to the Tutorial Panel.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public GameMode Tutorial1;
    public GameMode Tutorial2;
    public GameMode Tutorial3;

    public void StartTutorial1()
    {
        GameModeManager.Instance.StartGameWithGameMode(Tutorial1);
    }

    public void StartTutorial2()
    {
        GameModeManager.Instance.StartGameWithGameMode(Tutorial2);
    }

    public void StartTutorial3()
    {
        GameModeManager.Instance.StartGameWithGameMode(Tutorial3);
    }
}
