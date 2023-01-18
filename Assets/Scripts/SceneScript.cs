using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneScript : NetworkBehaviour
{
    [SerializeField] private Text canvasStatusText;
    public Text canvasBulletRemain;
    public Player player;
    [SyncVar(hook = nameof(OnStatusTextChanged))] public string statusText;
    private void OnStatusTextChanged(string oldStr, string newStr) {
        canvasStatusText.text = statusText;
    }

    public void ButtonSendMessage() {
        if(player != null) {
            player.CmdSendPlayerMessage();
        }
    }

    public void ButtonChangeScene() {
        if(isServer) {
            var currentScene = SceneManager.GetActiveScene();
            NetworkManager.singleton.ServerChangeScene (
                currentScene.name == "Scene 1" ? "Scene 2" : "Scene 1"
            );
        }
        else {
            Debug.Log("You are not Server");
        }
    }
}
