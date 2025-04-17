using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikesLogic : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            var player = other.GetComponent<MainCharacter>();
            if (player != null)
            {
                ResetPlayer(player);
            }
        }
    }


    private void ResetPlayer(MainCharacter player)
    {
        player.TakeDamage(1, -1f);
        Debug.Log("SPIKED");
        player.ResetPosition();
    }
}
