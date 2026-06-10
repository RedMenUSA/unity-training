using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleSlopeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -20f;
    public float jumpForce = 8f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    public float slideSpeed = 8f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.6f;

    private CharacterController _controller;

    private Vector3 _velocity;
    private bool _isGrounded;
    private RaycastHit _groundHit;

    // slope state
    private float _slopeAngle;
    private Vector3 _slopeDown;
    private bool _isUphill;
    private bool _isDownhill;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _controller.slopeLimit = 90f;
    }

    private void Update()
    {
        GroundCheck();

        Vector3 input = GetInputDirection();

        EvaluateSlope(input);

        HandleMovement(input);
        HandleSliding();
        ApplyGravity();

        _controller.Move(_velocity * Time.deltaTime);
    }

    private Vector3 GetInputDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input =
            transform.right * h +
            transform.forward * v;

        return Vector3.ClampMagnitude(input, 1f);
    }

    private void GroundCheck()
    {
        Vector3 rayOrigin =
            transform.position + _controller.center
            - Vector3.up * (_controller.height * 0.5f - _controller.radius)
            + Vector3.up * 0.1f;

        _isGrounded = Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out _groundHit,
            groundCheckDistance
        );

        if (_isGrounded && _velocity.y < 0f)
            _velocity.y = -10f;
    }

    private void EvaluateSlope(Vector3 input)
    {
        if (!_isGrounded)
        {
            _isUphill = false;
            _isDownhill = false;
            return;
        }

        _slopeAngle =
            Vector3.Angle(_groundHit.normal, Vector3.up);

        _slopeDown =
            Vector3.ProjectOnPlane(Vector3.down, _groundHit.normal).normalized;

        Vector3 moveDir =
            input.sqrMagnitude > 0.001f
                ? input
                : transform.forward;

        float alignment =
            Vector3.Dot(moveDir.normalized, _slopeDown);

        _isDownhill = alignment > 0.3f;
        _isUphill = alignment < -0.3f;

        if (_isDownhill)
            Debug.Log($"Slope Angle: {_slopeAngle:F1} | Uphill: {_isUphill} | Downhill: {_isDownhill}");

        if (_isUphill)
            Debug.Log($"Slope Angle: {_slopeAngle:F1} | Uphill: {_isUphill} | Downhill: {_isDownhill}");
    }

    private void HandleMovement(Vector3 input)
    {
        if (!_isGrounded)
        {
            _velocity.x = input.x * moveSpeed;
            _velocity.z = input.z * moveSpeed;
            return;
        }

        if (_slopeAngle <= maxSlopeAngle)
        {
            Vector3 move =
                Vector3.ProjectOnPlane(input, _groundHit.normal);

            _velocity.x = move.x * moveSpeed;
            _velocity.z = move.z * moveSpeed;
        }
        else
        {
            // steep slope = no climbing force
            _velocity.x = 0f;
            _velocity.z = 0f;
        }

        if (Input.GetButtonDown("Jump"))
            _velocity.y = jumpForce;
    }

    private void HandleSliding()
    {
        if (!_isGrounded)
            return;

        if (_slopeAngle <= maxSlopeAngle)
            return;

        // always downhill direction
        Vector3 slideDir = _slopeDown;

        _velocity.x = slideDir.x * slideSpeed;
        _velocity.z = slideDir.z * slideSpeed;
    }

    private void ApplyGravity()
    {
        if (!_isGrounded)
            _velocity.y += gravity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (_controller == null)
            _controller = GetComponent<CharacterController>();

        if (_controller != null)
        {
            Vector3 rayOrigin =
                transform.position + _controller.center
                - Vector3.up * (_controller.height * 0.5f - _controller.radius)
                + Vector3.up * 0.1f;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                rayOrigin,
                rayOrigin + Vector3.down * groundCheckDistance
            );
        }

        if (_groundHit.collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_groundHit.point, _groundHit.normal);
        }
    }
}