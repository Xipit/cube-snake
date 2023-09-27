using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a singleton
// https://gamedevbeginner.com/singletons-in-unity-the-right-way/
public class InstantiateManager : MonoBehaviour
{

    public static InstantiateManager Instance { get; private set; }

    // probably needs a rotation at some point, if gameobject itself has a direction
    public void InstantiateGameObject(Vector3 position, GameObject gameObject)
    {
        Instantiate(gameObject, position, Quaternion.identity);
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
