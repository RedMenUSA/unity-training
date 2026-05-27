using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectilePrediction : MonoBehaviour
{

    [SerializeField] private Material lineRendererMaterial;
    private int shaderID;
    private int shaderIDMin;
    private int shaderIDMax;
    [Header("References")]
    public Transform firePoint;

    [Header("Projectile Settings")]
    public float launchForce = 15f;

    [Tooltip("Horizontal rotation")]
    public float yaw = 0f;

    [Tooltip("Vertical launch angle")]
    public float launchAngle = 45f;
    public float launchAngleMin = 5f;
    public float launchAngleMax = 85f;

    [Header("Prediction Settings")]
    public int maxSteps = 50;
    public float timeStep = 0.1f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRendererMaterial = gameObject.GetComponent<LineRenderer>().material;
        shaderID = Shader.PropertyToID("_Angle");
        shaderIDMin = Shader.PropertyToID("_MinAngle");
        shaderIDMax = Shader.PropertyToID("_MaxAngle");
        lineRendererMaterial.SetFloat(shaderIDMin, launchAngleMin);
        lineRendererMaterial.SetFloat(shaderIDMax, launchAngleMax);
    }

    private void Update()
    {
        HandleInput();
        DrawPrediction();
        UpdateLineRenderer();
    }

    private void HandleInput()
    {
        // Left / Right arrows = horizontal aiming
        yaw += Input.GetAxis("Horizontal") * 60f * Time.deltaTime;

        // Up / Down arrows = launch angle
        launchAngle += Input.GetAxis("Vertical") * 60f * Time.deltaTime;

        // Clamp angle so we don't flip upside down
        launchAngle = Mathf.Clamp(launchAngle, launchAngleMin, launchAngleMax);

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
    private void UpdateLineRenderer()
    {
        lineRendererMaterial.SetFloat(shaderID, launchAngle);
    }
}