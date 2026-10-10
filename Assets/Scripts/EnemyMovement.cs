using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float patrolSpeed = 1f;
    [SerializeField] float chaseSpeed = 2f;
    [SerializeField] float chaseDistance = 5f;
    [SerializeField] float sameHeightTolerance = 0.4f;
    [SerializeField] Transform player;

    Rigidbody2D myRigidbody;
    Health health;
    EnemyAttack enemyAttack;

    float patrolDirection = 1f;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        enemyAttack = GetComponent<EnemyAttack>();

        patrolDirection = Mathf.Sign(patrolSpeed);

        if (patrolDirection == 0f)
        {
            patrolDirection = 1f;
        }
    }

    void Update()
    {
        if (health != null && health.IsDead)
        {
            myRigidbody.linearVelocity =
                new Vector2(0f, myRigidbody.linearVelocity.y);

            return;
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        Health playerHealth = player.GetComponent<Health>();

        if (playerHealth != null && playerHealth.IsDead)
        {
            Patrol();
            return;
        }

        if (enemyAttack != null && enemyAttack.PlayerInAttackRange)
        {
            myRigidbody.linearVelocity =
                new Vector2(0f, myRigidbody.linearVelocity.y);

            FacePlayer();
            return;
        }

        float horizontalDistance =
            Mathf.Abs(player.position.x - transform.position.x);

        float verticalDistance =
            Mathf.Abs(player.position.y - transform.position.y);

        bool playerIsSameHeight =
            verticalDistance <= sameHeightTolerance;

        bool playerIsCloseEnough =
            horizontalDistance <= chaseDistance;

        if (playerIsSameHeight && playerIsCloseEnough)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        myRigidbody.linearVelocity =
            new Vector2(
                direction * chaseSpeed,
                myRigidbody.linearVelocity.y
            );

        FaceDirection(direction);
    }

    void Patrol()
    {
        myRigidbody.linearVelocity =
            new Vector2(
                patrolDirection * Mathf.Abs(patrolSpeed),
                myRigidbody.linearVelocity.y
            );

        FaceDirection(patrolDirection);
    }

    void FacePlayer()
    {
        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        FaceDirection(direction);
    }

    void FaceDirection(float direction)
    {
        if (direction == 0f) return;

        transform.localScale =
            new Vector2(Mathf.Sign(direction), 1f);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ground"))
        {
            return;
        }

        patrolDirection = -patrolDirection;

        FaceDirection(patrolDirection);
    }

    void OnDeath()
    {
        myRigidbody.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }
}