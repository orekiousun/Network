using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class HUDExtension : MonoBehaviour
{
    private void OnEnable() {
        SceneManager.activeSceneChanged += HandleSceneChanged;
    }

    private void OnDisable() {
        SceneManager.activeSceneChanged -= HandleSceneChanged;
    }

    private void HandleSceneChanged(Scene arg0, Scene arg1) {
        GetComponent<NetworkManagerHUD>().enabled = arg1.name == "Menu" ? false : true;
    }
}
