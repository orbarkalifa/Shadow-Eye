using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class EnemyAgent : Agent
{
    public Transform player; // Assign in Unity Inspector
    public Rigidbody2D rb;
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if the enemy is on the ground
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, groundLayer);
    }

    public override void OnEpisodeBegin()
    {
        // Reset enemy position at the start of each episode
        transform.position = new Vector3(Random.Range(-4f, 4f), 1f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the relative position of the player
        sensor.AddObservation(transform.position.x - player.position.x);
        sensor.AddObservation(transform.position.y - player.position.y);

        // Observe enemy velocity
        sensor.AddObservati