using UnityEngine;
using System.Text;

public class DebugOverlay : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public int coinsCollected = 12;
    public int enemiesDefeated = 4;

    [Header("Performance")]
    public bool showFPS = true;

    [Header("Session")]
    public float sessionStartTime;

    private bool showMenu = true;
    private float deltaTime;

    private CoyoteTime coyoteTime;

    private void Awake()
    {
        coyoteTime = FindAnyObjectByType<CoyoteTime>();
    }

    private void Start()
    {
        sessionStartTime = Time.time;
    }

    private void OnEnable()
    {
        coyoteTime.onBallFellOnFloor.AddListener(HandleFall);
    }

    private void OnDisable()
    {
        coyoteTime.onBallFellOnFloor.RemoveListener(HandleFall);
    }

    private void Update()
    {
        // Toggle debug menu with F1
        if (Input.GetKeyDown(KeyCode.F1)) showMenu = !showMenu;
        
        // FPS smoothing
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        if (!showMenu) return;

        GUI.Box(new Rect(10, 10, 320, 260), "Debug Overlay");

        var sb = new StringBuilder();

        // --- Player Telemetry ---
        sb.AppendLine("=== PLAYER ===");

        if (player != null)
        {
            sb.AppendLine($"Position: {player.position}");
            
            // TODO: The GetComponent is expensive, we need to cache it.
            sb.AppendLine($"Speed: {player.GetComponent<Rigidbody>()?.linearVelocity.magnitude:F2}");
        }

        sb.AppendLine($"Coins Collected: {coinsCollected}");
        sb.AppendLine($"Enemies Defeated: {enemiesDefeated}");

        // --- Session Telemetry ---
        sb.AppendLine("\n=== SESSION ===");

        var sessionDuration = Time.time - sessionStartTime;

        sb.AppendLine($"Session Time: {sessionDuration:F1}s");
        sb.AppendLine($"Current Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        // --- Performance Telemetry ---
        sb.AppendLine("\n=== PERFORMANCE ===");

        if (showFPS)
        {
            var fps = 1.0f / deltaTime;
            sb.AppendLine($"FPS: {fps:F1}");
        }

        sb.AppendLine($"Frame Time: {(deltaTime * 1000f):F2} ms");
        sb.AppendLine($"Memory Usage: {(System.GC.GetTotalMemory(false) / 1024 / 1024)} MB");

        GUI.Label(new Rect(20, 40, 300, 220), sb.ToString());
    }

    // TODO: We need to display the number of times the ball falls on the floor per run.
    private void HandleFall()
    {
        Debug.Log("Ball fell (code listener)");
    }
}