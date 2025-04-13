using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikesLogic : MonoBehaviour
{
    public Transform resetPosition;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<MainCharacter>();
        Debug.Log("Trigger");
        ResetPlayer(player);
    }

    private void ResetPlayer(MainCharacter player)
    {
        player.TakeDamage(1);
        Debug.Log("SPIKED");
        player.transform.position = resetPosition.position;
    }
}
