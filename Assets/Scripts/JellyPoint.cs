using UnityEngine;

public class JellyPoint: MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;

    public JellyPoint(Vector3 startPos)
    {
        position = startPos;
        velocity = Vector3.zero;
    }
}