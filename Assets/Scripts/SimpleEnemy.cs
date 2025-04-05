using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SimpleEnemy : Character
{
    
    public Transform target;
    [SerializeField] float speed;
    [SerializeField] float distanceToNextPoint;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D circleCollider;
    [HideInInspector] public float lastAttackTime = -Mathf.Infinity;
    public float detectionRange = 10f;
    private Time timer;
    [SerializeField] Seeker seeker;
    Path path;
    int currentPoint;

    protected override void  Awake()
    {
        base.Awake(); 
        InvokeRepeating(nameof(UpdatePath),0f, 0.5f);
 
    }

    void UpdatePath()
    {
        if(target)
            seeker.StartPath(transform.position, target.position, OnPathComplete);
    }
    // Update is called once per frame
    void Update()
    {
        if(!target) return;
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if(path == null || distanceToPlayer > detectionRange)
        {
            rb.velocity = Vector2.zero;
            return;
        }
            
        if(currentPoint >= path?.vectorPath.Count)
        {
            Debug.Log("arrived");
            currentPoint = 0;
            rb.velocity = Vector2.zero;
            return;
        }
    
        // Calculate the direction and normalize it for smooth movement
        if(path != null)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentPoint] - rb.position).normalized;
            rb.AddForce(direction * speed);


            distanceToNextPoint = Vector2.Distance(rb.position, path.vectorPath[currentPoint]);
            if(distanceToNextPoint <= 0.05f)
                currentPoint++;
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentPoint = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            MainCharacter player = collision.gameObject.GetComponent<MainCharacter>();
            player.TakeDamage(1);
        }
    }
}
