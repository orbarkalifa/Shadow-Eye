using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCharacter : Character
{
    public Rigidbody2D rb;
    [SerializeField]private float FluidMovementParameter;
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
        rb.velocity = new Vector2(Horizontal*FluidMovementParameter, rb.velocity.y);
    }

    private void handleJump()
    {
        if(Input.GetButton("Jump"))
        {
            rb.AddForce(Vector2.up * FluidMovementParameter, ForceMode2D.Impulse);
            isJumping = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        isJumping = false;
    }
    
}