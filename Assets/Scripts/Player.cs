using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour {
    [SerializeField] private float moveSpeed = 10f;

    void HandleMovement() {
        if(isLocalPlayer) {
            float moveHorizontal = Input.GetAxis("Horizontal") * Time.deltaTime;
            float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime;
            Vector3 movement = new Vector3(moveHorizontal * moveSpeed, 0, moveVertical * moveSpeed);
            transform.position += movement;
        }
    }

    private void Update() {
        HandleMovement();
    }
}
