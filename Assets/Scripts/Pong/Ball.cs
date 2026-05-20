using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 8f;

    public float topBound = 4.5f;
    public float bottomBound = -4.5f;
    public float leftBound = -8f;
    public float rightBound = 8f;

    private Vector2 velocity;

    private void Start()
    {
        Launch();
    }

    void Update()
    {
        Move();
        CheckWallBounce();
    }

    void Launch()
    {
        float angle = Random.Range(-45f, 45f);
        float dirX = Random.value < 0.5f ? -1 : 1;

        velocity = new Vector2(
            dirX,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized * speed;
    }

    void Move()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    void CheckWallBounce()
    {
        // Top / Bottom
        if (transform.position.y >= topBound && velocity.y > 0)
        {
            velocity.y *= -1;
            transform.position = new Vector3(transform.position.x, topBound, 0);
        }

        if (transform.position.y <= bottomBound && velocity.y < 0)
        {
            velocity.y *= -1;
            transform.position = new Vector3(transform.position.x, bottomBound, 0);
        }

        // Left / Right
        if (transform.position.x >= rightBound && velocity.x > 0)
        {
            velocity.x *= -1;
            transform.position = new Vector3(rightBound, transform.position.y, 0);
        }

        if (transform.position.x <= leftBound && velocity.x < 0)
        {
            velocity.x *= -1;
            transform.position = new Vector3(leftBound, transform.position.y, 0);
        }
    }
}