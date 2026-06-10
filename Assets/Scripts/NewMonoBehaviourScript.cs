using UnityEngine;

public class JellyCube : MonoBehaviour
{
    [Header("Structure")]
    public float size = 1f;

    [Header("Spring Settings")]
    public float stiffness = 60f;
    public float damping = 8f;

    private JellyPoint[] points = new JellyPoint[8];

    // cube corner offsets
    private Vector3[] offsets;

    private void Start()
    {
        float s = size * 0.5f;

        offsets = new Vector3[]
        {
            new Vector3(-s, -s, -s),
            new Vector3( s, -s, -s),
            new Vector3(-s,  s, -s),
            new Vector3( s,  s, -s),
            new Vector3(-s, -s,  s),
            new Vector3( s, -s,  s),
            new Vector3(-s,  s,  s),
            new Vector3( s,  s,  s),
        };

        for (int i = 0; i < 8; i++)
        {
            points[i] = new JellyPoint(transform.position + offsets[i]);
        }
    }

    private void Update()
    {
        SimulatePoints();
        DrawCube();
    }

    private void SimulatePoints()
    {
        Vector3 center = transform.position;

        for (int i = 0; i < points.Length; i++)
        {
            JellyPoint p = points[i];

            Vector3 restPos = center + offsets[i];

            Vector3 displacement = p.position - restPos;

            Vector3 springForce = -stiffness * displacement;
            Vector3 dampingForce = -damping * p.velocity;

            Vector3 acceleration = springForce + dampingForce;

            p.velocity += acceleration * Time.deltaTime;
            p.position += p.velocity * Time.deltaTime;
        }
    }

    private void DrawCube()
    {
        Debug.DrawLine(points[0].position, points[1].position);
        Debug.DrawLine(points[0].position, points[2].position);
        Debug.DrawLine(points[1].position, points[3].position);
        Debug.DrawLine(points[2].position, points[3].position);

        Debug.DrawLine(points[4].position, points[5].position);
        Debug.DrawLine(points[4].position, points[6].position);
        Debug.DrawLine(points[5].position, points[7].position);
        Debug.DrawLine(points[6].position, points[7].position);

        Debug.DrawLine(points[0].position, points[4].position);
        Debug.DrawLine(points[1].position, points[5].position);
        Debug.DrawLine(points[2].position, points[6].position);
        Debug.DrawLine(points[3].position, points[7].position);
    }
}