using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints; // Waypoints for patrolling
    private int currentWaypointIndex = 0; // Index of the current waypoint
    private NavMeshAgent navMeshAgent;
    public float fovAngle = 90f;
    public float viewDistance = 10f;
    public Transform player; // Reference to the player GameObject
    public Animator animator;
    float Speed=1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetNextWaypoint(); // Start patrolling by moving to the first waypoint
        navMeshAgent.speed = Speed;
    }
    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Update()
    {
        if (CanSeePlayer())
        {
            // Player is within FOV, start chasing
            StartChasing();
        }
        else
        {
            // Continue patrolling
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                SetNextWaypoint();
            }
        }
    }

    void SetNextWaypoint()
    {

        // Move to the next waypoint in the array
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);

        animator.SetBool("isWalking", true);
        Debug.Log("Start Chasing: Triggered Walking Animation");

        
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 directionToPlayer = player.position - transform.position;
        if (Vector3.Angle(transform.forward, directionToPlayer) < fovAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, viewDistance))
            {
                if (hit.collider.gameObject == player.gameObject)
                {
                    return true; // Player is within FOV and not obstructed
                }
            }

            RaycastHit rhit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, viewDistance))
            {
                if (hit.collider.gameObject == gameObject.CompareTag("Enemy"))
                {
                    RestartScene();
                }
            }
        }
        return false; // Player is not within FOV or obstructed
    }

    void StartChasing()
    {
        // Implement chasing behavior here
        if (player != null) // Check if player exists
        {
            // Set the destination of the NavMeshAgent to the player's position
            navMeshAgent.SetDestination(player.position);
           
         
         
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            float halfFOV = fovAngle / 2f;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;

            Gizmos.DrawRay(transform.position, leftRayDirection * viewDistance);
            Gizmos.DrawRay(transform.position, rightRayDirection * viewDistance);

            // Draw FOV cone
            Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * viewDistance);
            Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * viewDistance);
            Gizmos.DrawLine(transform.position + leftRayDirection * viewDistance, transform.position + rightRayDirection * viewDistance);
        }

        // Draw waypoints for patrolling
        Gizmos.color = Color.green;
        if (waypoints != null)
        {
            foreach (Transform waypoint in waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, 0.3f);
            }
        }
    }
}
