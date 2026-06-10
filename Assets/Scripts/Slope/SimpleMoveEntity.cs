using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleMoveEntity : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -20f;
    public float jumpForce = 8f;

    private CharacterController controller;

    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move =
            transform.right * h +
            transform.forward * v;

        move = Vector3.ClampMagnitude(move, 1f);

        velocity.x = move.x * moveSpeed;
        velocity.z = move.z * moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }
}