using UnityEngine;

public class SideScrollerCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 3f, -10f);

    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired camera position
        Vector3 desiredPosition =
            target.position + offset;

        // Smooth follow
        Vector3 smoothedPosition =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime
            );

        transform.position = smoothedPosition;
    }
}