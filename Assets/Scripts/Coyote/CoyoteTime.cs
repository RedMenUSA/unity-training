using UnityEngine;
using UnityEngine.Events;

public class CoyoteTime : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float coyoteJumpForce = 15f;
    public float coyotePushBack = 3f;

    [Header("Physics")]
    public float gravity = -30f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimer;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.1f;
    private float jumpBufferTimer;

    [Header("World")]
    public float platformY = 0f;
    public float platformEdgeX = 5f;
    public float floorY = -5f;

    private Vector3 velocity;
    private Vector3 startPosition;

    private bool isJumping;

    [Header("Events")]
    public UnityEvent onBallFellOnFloor;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // =========================
        // INPUT BUFFER
        // =========================
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= dt;
        }

        // =========================
        // MOVE FORWARD
        // =========================
        Vector3 pos = transform.position;
        pos.x += speed * dt;

        // =========================
        // GRAVITY
        // =========================
        velocity.y += gravity * dt;
        pos.y += velocity.y * dt;

        // =========================
        // PLATFORM CONSTRAINT
        // =========================
        bool onPlatform = pos.x <= platformEdgeX;

        if (onPlatform)
        {
            if (pos.y <= platformY)
            {
                pos.y = platformY;
                velocity.y = 0f;

                isJumping = false;
                coyoteTimer = coyoteTime;
            }
        }

        // =========================
        // COYOTE TIMER
        // =========================
        if (!onPlatform || pos.y > platformY + 0.01f)
        {
            coyoteTimer -= dt;
        }

        // =========================
        // JUMP LOGIC
        // =========================
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            if (onPlatform)
            {
                // Normal jump
                velocity.y = jumpForce;
                Debug.Log("Normal jump!");
            }
            else
            {
                velocity.y = coyoteJumpForce;
                Debug.Log("Coyote jump!");
            }

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            isJumping = true;
        }

        // =========================
        // FLOOR RESET
        // =========================
        if (pos.y <= floorY)
        {
            // Invoke Unity Event
            onBallFellOnFloor?.Invoke();

            ResetBall();
            return;
        }

        // =========================
        // APPLY POSITION
        // =========================
        transform.position = pos;
    }

    void ResetBall()
    {
        transform.position = startPosition;
        velocity = Vector3.zero;
        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
        isJumping = false;
    }
}