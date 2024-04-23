using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.gameObject.transform.parent = this.transform.parent;
    }
}
