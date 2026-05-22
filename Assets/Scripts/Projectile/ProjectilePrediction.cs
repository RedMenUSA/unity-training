using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectilePrediction : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;

    [Header("Projectile Settings")]
    public float launchForce = 15f;

    [Tooltip("Horizontal rotation")]
    public float yaw = 0f;

    [Tooltip("Vertical launch angle")]
    public float launchAngle = 45f;

    [Header("Prediction Settings")]
    public int maxSteps = 50;
    public float timeStep = 0.1f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        HandleInput();
        DrawPrediction();
    }

    private void HandleInput()
    {
        // Left / Right arrows = horizontal aiming
        yaw += Input.GetAxis("Horizontal") * 60f * Time.deltaTime;

        // Up / Down arrows = launch angle
        launchAngle += Input.GetAxis("Vertical") * 60f * Time.deltaTime;

        // Clamp angle so we don't flip upside down
        launchAngle = Mathf.Clamp(launchAngle, 5f, 85f);

        // Rotate launcher visually
        transform.rotation = Quaternion.Euler(-launchAngle, yaw, 0f);
    }

    private void DrawPrediction()
    {
        _lineRenderer.positionCount = maxSteps;

        // Starting position
        var position = firePoint.position;

        // Build launch direction from angle
        var direction = Quaternion.Euler(-launchAngle, yaw, 0f) * Vector3.forward;

        // Initial velocity
        var velocity = direction.normalized * launchForce;

        for (int i = 0; i < maxSteps; i++)
        {
            _lineRenderer.SetPosition(i, position);

            // Gravity changes velocity
            velocity += Physics.gravity * timeStep;

            // Velocity changes position
            position += velocity * timeStep;
        }
    }
}