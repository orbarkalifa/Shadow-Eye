using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCharacter : Character
{
    public Rigidbody2D rb;
    [FormerlySerializedAs("FluidMovementParameter")]
    [SerializeField]
    private float fluidMovementParameter = 5f;
    private bool isJumping;
    public MainCharacter(float i_MaxHp)
        : base(i_MaxHp)
    {
    }

    public 
        // Start is called before the first frame update
        void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleJump();
    }

    private void handleMovement()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        if(Horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(Horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        rb.velocity = new Vector2(Horizontal*fluidMovementParameter, rb.velocity.y);
    }

    private void handleJump()
    {
        if(Input.GetButton("Jump"))
        {
            rb.AddForce(Vector2.up * fluidMovementParameter, ForceMode2D.Impulse);
            isJumping = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        isJumping = false;
    }
    
}