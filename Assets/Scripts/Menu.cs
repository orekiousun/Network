using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void ButtonLoadSceneGameList() {
        SceneManager.LoadScene("GameList");
    }

    public void ButtonLoadSceneMenu() {
        SceneManager.LoadScene("Menu");
    }
}
