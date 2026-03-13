using UnityEngine;

public class Weder : MonoBehaviour
{
    public Transform Inplayer;

    public float maxAcceleration = 10f;
    public float maxSpeed = 5f;

    public float slowRadius = 3f;
    public float timeToTarget = 0.1f;
    public float stopDistance = 1f;

    // Wander
    public float wanderOffset = 2f;
    public float wanderRadius = 1.5f;
    public float wanderRate = 0.5f;

    // Rotation
    public float rotationSpeed = 5f;

    // Pursue
    public float pursuePrediction = 0.5f;

    // ======================
    // COLLISION AVOIDANCE
    // ======================
    public float mainRayLength = 3f;
    public float whiskerLength = 2f;
    public float whiskerAngle = 30f;

    public float avoidForce = 15f;
    public LayerMask obstacleLayer;

    private float wanderOrientation = 0f;

    private Vector2 velocity;
    private Vector2 steering;
    private Vector2 lastPlayerPos;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPlayerPos = Inplayer.position;
    }

    void Update()
    {
        steering = Vector2.zero;

        float distanceToPlayer =
            Vector2.Distance(rb.position, Inplayer.position);

        if (distanceToPlayer > 7f)
            Wander();
        else
            Pursue();

        // ======================
        // CHECK OBSTACLE
        // ======================
        AvoidObstacle();

        velocity += steering * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        rb.MovePosition(rb.position + velocity * Time.deltaTime);

        RotateSmooth();
    }

    // ======================
    // PURSUE
    // ======================
    void Pursue()
    {
        Vector2 playerVelocity =
            ((Vector2)Inplayer.position - lastPlayerPos) / Time.deltaTime;

        lastPlayerPos = Inplayer.position;

        Vector2 playerFront =
            (Vector2)Inplayer.position + (Vector2)Inplayer.up;

        Vector2 predictedPosition =
            playerFront + playerVelocity * pursuePrediction;

        Arrive(predictedPosition);
    }

    // ======================
    // ARRIVE
    // ======================
    void Arrive(Vector2 targetPos)
    {
        Vector2 direction = targetPos - rb.position;

        Vector2 finalTarget =
            targetPos - direction.normalized * stopDistance;

        Vector2 newDirection =
            finalTarget - rb.position;

        float distance = newDirection.magnitude;

        float targetSpeed;

        if (distance > slowRadius)
            targetSpeed = maxSpeed;
        else
            targetSpeed = maxSpeed * distance / slowRadius;

        Vector2 targetVelocity =
            newDirection.normalized * targetSpeed;

        Vector2 accel =
            (targetVelocity - velocity) / timeToTarget;

        accel = Vector2.ClampMagnitude(accel, maxAcceleration);

        steering += accel;
    }

    // ======================
    // WANDER
    // ======================
    void Wander()
    {
        float random = Random.value - Random.value;

        wanderOrientation += random * wanderRate;

        float orientation =
            transform.eulerAngles.z * Mathf.Deg2Rad;

        float targetOrientation =
            wanderOrientation + orientation;

        Vector2 forward =
            transform.up; // +Y คือหัว

        Vector2 circleCenter =
            rb.position + forward * wanderOffset;

        Vector2 wanderTarget =
            circleCenter +
            new Vector2(Mathf.Cos(targetOrientation),
                        Mathf.Sin(targetOrientation)) * wanderRadius;

        Vector2 direction =
            wanderTarget - rb.position;

        Vector2 accel =
            direction.normalized * maxAcceleration;

        steering += accel;
    }

    // ======================
    // COLLISION AVOIDANCE
    // ======================
    void AvoidObstacle()
    {
        Vector2 forward = transform.up;

        // main ray
        RaycastHit2D hitMain =
            Physics2D.Raycast(rb.position, forward, mainRayLength, obstacleLayer);

        // left whisker
        Vector2 leftDir =
            Quaternion.Euler(0, 0, whiskerAngle) * forward;

        RaycastHit2D hitLeft =
            Physics2D.Raycast(rb.position, leftDir, whiskerLength, obstacleLayer);

        // right whisker
        Vector2 rightDir =
            Quaternion.Euler(0, 0, -whiskerAngle) * forward;

        RaycastHit2D hitRight =
            Physics2D.Raycast(rb.position, rightDir, whiskerLength, obstacleLayer);

        if (hitMain.collider != null)
        {
            Vector2 avoidDir = hitMain.normal;
            steering += avoidDir * avoidForce;
        }
        else if (hitLeft.collider != null)
        {
            Vector2 avoidDir = hitLeft.normal;
            steering += avoidDir * avoidForce;
        }
        else if (hitRight.collider != null)
        {
            Vector2 avoidDir = hitRight.normal;
            steering += avoidDir * avoidForce;
        }

        // Debug rays
        Debug.DrawRay(rb.position, forward * mainRayLength, Color.red);
        Debug.DrawRay(rb.position, leftDir * whiskerLength, Color.yellow);
        Debug.DrawRay(rb.position, rightDir * whiskerLength, Color.yellow);
    }

    // ======================
    // ROTATION
    // ======================
    void RotateSmooth()
    {
        if (velocity.magnitude < 0.01f) return;

        float angle =
            Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90;

        Quaternion targetRotation =
            Quaternion.Euler(0, 0, angle);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }
}