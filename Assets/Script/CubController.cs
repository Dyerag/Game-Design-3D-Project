using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CubController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum State { Idle, Wander, Follow, Flee, Home }
    public State currentState = State.Idle;

    private NavMeshAgent agent;

    [Header("References")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float fleeRange = 10f;
    public float minFollowRange = 4f;
    public float maxFollowRange = 10f;

    private float idleTimer = 0f;
    public float idleTime = 2f;

    public Animator animator;


    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //TODO Every second, the AI should check for closest player in multiplayer. And set the transform player field. 
        switch (currentState)
        {
            case State.Idle: Idle(); break;
            case State.Wander: Wander(); break;
            case State.Flee: Flee(); break;
            case State.Follow: Follow(); break;
            case State.Home: Home(); break;
        }


        if (currentState != State.Home)
            return;
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            if (distToPlayer > maxFollowRange && currentState != )
            {
                currentState = State.Wander;
            }


            if (distToPlayer < fleeRange && currentState != State.Follow)
            {
                currentState = State.Flee;
            }


        

        animator.SetFloat("Move", agent.velocity.magnitude);

    }

    private void Home()
    {
        throw new NotImplementedException();
    }

    private void Flee()
    {
        throw new NotImplementedException();
    }

    // -------------------- STATES --------------------
    void Idle()
    {
        idleTimer += Time.deltaTime + Random.Range(0, 10);
        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            currentState = State.Wander;
        }
    }
    void Wander()
    {

    }

    void Follow()
    {

        if (player == null) return;
        agent.SetDestination(player.position);
        //MoveTowards(player.position, chaseSpeed);
    }

    // -------------------- HELPER --------------------

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        // Face the movement direction
        if (dir.magnitude > 0.1f)
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Den"))
        {
            currentState = State.Home;
        }
    }
}
