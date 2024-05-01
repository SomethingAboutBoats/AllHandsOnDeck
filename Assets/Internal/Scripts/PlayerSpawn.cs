using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.gameObject.transform.parent = this.transform.parent;

        RandomizeColor(playerInput.gameObject);
    }

    protected void RandomizeColor(GameObject gameObject)
    {
        Color randomColor = Random.ColorHSV();
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = randomColor;
        }
    }
}
