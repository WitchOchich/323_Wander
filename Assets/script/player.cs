using UnityEngine;

public class player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    private Vector3 targetPosition;
    private Vector3 velocity;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMouseInput();
        MovePlayer();
        RotatePlayer();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;

            mousePos.z = 10f;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            targetPosition = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

    void MovePlayer()
    {
        Vector3 direction = targetPosition - transform.position;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            direction.Normalize();

            velocity = direction * moveSpeed;

            transform.position += velocity * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    void RotatePlayer()
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}