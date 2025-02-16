using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FloatingPlatform))]
public class MovingPlatform : MonoBehaviour
{
    // The list of points that the platform will move between
    [Header("Points: Put all the points the platform will move between. If none are assigned, the platform will not move.")]
    public Transform[] points;

    // The speed at which the platform moves
    [Header("Speed: The speed at which the platform moves.")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float delayTime = 1f;

    // The current point the platform is moving towards
    private int currentPoint;
    private bool isMoving = true;
    private Vector3 target;

    void Start()
    {
        if (points.Length <= 1)
        {
            Debug.Log("No points assigned to the moving platform. This platform will not move.");
            return;
        }
        else if (speed <= 0 || delayTime <= 0)
        {
            Debug.LogError("Speed or delay time is less than or equal to 0, while the platform is set to move. Please set a speed and delay time greater than 0.");
            return;
        }

        else
        {
            // Set the target to the first point
            target = points[0].position;
            // Move the platform to the first point by changing the position
            transform.position = target;
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            return;
        }
        if (transform.position == target)
        {
            currentPoint++;
            if (currentPoint >= points.Length)
            {
                currentPoint = 0;
            }
            target = points[currentPoint].position;
            StartCoroutine(DelayMovement(delayTime));
        }
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
    private IEnumerator DelayMovement(float delayTime)
    {
        isMoving = false;
        yield return new WaitForSeconds(delayTime);
        isMoving = true;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (i + 1 < points.Length)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }
}
