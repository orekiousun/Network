using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour {
    #region Init Camera
    [SerializeField] private Transform cameraTransform;
    private void InitPlayerCamera() {
        // 摄像机绑定
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = cameraTransform.localPosition;
        Camera.main.transform.localRotation = cameraTransform.localRotation; 
    }
    #endregion

    #region Player Movement
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 45f;
    void PlayerMovement() {
        float rotateHor = Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
        float moveVer = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        Vector3 movement = new Vector3(0, 0, moveVer);
        Vector3 rotate = new Vector3(0, rotateHor, 0);
        transform.Translate(movement);
        transform.Rotate(rotate);
    }
    #endregion

    #region Player Init
    [SerializeField] private GameObject floatingInfo;
    [SerializeField] private TextMesh nameText;
    private Material playerMaterialClone;
    [SyncVar(hook = nameof(OnPlayerNameChanged))] private string playerName;    // 当playerName变量被修改时，就会调用OnPlayerNameChanged方法
    [SyncVar(hook = nameof(OnPlayerColorChanged))] private Color playerColor;   // 当playerColor变量被修改时，就会调用OnPlayerColorChanged方法
    private void OnPlayerNameChanged(string oldStr, string newStr) {
        nameText.text = newStr;
    }

    private void OnPlayerColorChanged(Color oldCol, Color newCol) {
        nameText.color = newCol;
        // 修改玩家材质的自发光颜色
        playerMaterialClone = new Material(GetComponent<Renderer>().material);
        playerMaterialClone.SetColor("_EmissionColor", newCol);
        GetComponent<Renderer>().material = playerMaterialClone;
    }

    [Command] private void CmdSetupPlayer(string nameValue, Color colorValue) {
        playerName = nameValue;
        playerColor = colorValue;
        sceneScript.statusText = $"{playerName} joined.";
    }

    private void InitPlayer() {
        // 修改名字出现的位置
        floatingInfo.transform.localPosition = new Vector3(0, -1f, 2.5f);
        floatingInfo.transform.localScale = new Vector3(1f, 1f, 1f  );
        // 生成随机颜色和姓名
        string tempName = $"Player {Random.Range(1, 999)}";
        Color tempColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1
        );
        CmdSetupPlayer(tempName, tempColor);
    }
    #endregion

    #region Send Player Message
    private SceneScript sceneScript;
    [Command]public void CmdSendPlayerMessage() {
        if(sceneScript) {
            sceneScript.statusText = $"{playerName} says hello {Random.Range(1, 99)}";
        }
    }
    #endregion

    #region Weapon
    [SerializeField] GameObject[] weaponArray;
    private Weapon activeWeapon;
    private int currentWeapon = 0;
    private float iterateTime = 0f;   // 用于迭代武器cd
    [SyncVar (hook = nameof(OnWeaponChanged))] private int currentWeaponSynced;
    private void OnWeaponChanged(int oldIndex, int newIndex) {
        weaponArray[oldIndex].SetActive(false);
        weaponArray[newIndex].SetActive(true);
        activeWeapon = weaponArray[newIndex].GetComponent<Weapon>();
    }   

    [Command] private void CmdActiveWeapon(int index) {
        currentWeaponSynced = index;
    }

    private void InitWeapon() {
        currentWeapon = 0;
        foreach(GameObject weapon in weaponArray) {
            weapon.SetActive(false);
        }
        weaponArray[currentWeapon].SetActive(true);
        activeWeapon = weaponArray[currentWeapon].GetComponent<Weapon>();
    }

    private void GetChangeWeaponInput() {
        if(Input.GetMouseButtonDown(1)) {
            currentWeapon = (currentWeapon + 1) % weaponArray.Length;
            CmdActiveWeapon(currentWeapon);
        }
    }

    [Command] private void CmdCallFire() {   // 在客户端调用，服务端执行
        RpcWeaponFire();   // 通过这种方法，可以使客户端调用服务端指令，从而同步所有客户端
    }

    [ClientRpc] private void RpcWeaponFire() {   // 服务端可以向所有连接的客户端发送指令
        GameObject bullet = GameObject.Instantiate(activeWeapon.bullet, activeWeapon.startPos.position, activeWeapon.startPos.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * activeWeapon.bulletSpeed;
        Destroy(bullet, activeWeapon.bulletLife);
    }

    private void GetFireInpt() {
        iterateTime += Time.deltaTime;
        if(Input.GetMouseButtonDown(0) && iterateTime > activeWeapon.bulletCD && activeWeapon.bulletCount > 0) {
            iterateTime = 0;
            activeWeapon.bulletCount--;
            CmdCallFire();
        }
        sceneScript.canvasBulletRemain.text = activeWeapon.bulletCount.ToString();
    }
    #endregion

    private void Awake() {
        sceneScript = FindObjectOfType<SceneScript>();
        InitWeapon();
    }

    public override void OnStartLocalPlayer()
    {
        sceneScript.player = this;
        base.OnStartLocalPlayer();
        InitPlayerCamera();
        InitPlayer();
    }

    private void Update() {
        if(!isLocalPlayer) {
            floatingInfo.transform.LookAt(Camera.main.transform);   // 使其他玩家对象的名称一直朝向本地对象
            return;
        }
            
        PlayerMovement();
        GetChangeWeaponInput();
        GetFireInpt();
        // if(Input.GetKeyDown(KeyCode.X)) {
        //     Debug.Log("Sending Hola to Server");
        //     Hola();
        // }
    }

    #region Hola Test
    // [SyncVar(hook = nameof(OnHolaCountChanged))]private int holaCount = 0;
    // [Command]   // 加上这个标签，表示这个函数只会在服务器上执行，客户端不能也不会执行这个函数
    // private void Hola() {
    //     Debug.Log("Received Hola from Client");
    //     holaCount++;
    //     ReplyHola();
    // }

    // [TargetRpc]
    // private void ReplyHola() {
    //     Debug.Log("Received Hola from Server");
    // }

    // [ClientRpc]
    // void TooFar() {
    //     Debug.Log("Too Far From Origin");
    // }

    // private void OnHolaCountChanged(int oldCount, int newCount) {
    //     Debug.Log($"oldHolaCount: {oldCount}, newHolaCOunt: {newCount}");
    // }
    #endregion
}
