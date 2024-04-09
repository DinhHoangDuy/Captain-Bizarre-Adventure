using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed;
    [SerializeField] private float delayTime;
    private int currentPoint;
    private bool isMoving = true;
    private Vector3 target;

    void Start()
    {
        target = points[0].position;
    }

    void Update()
    {
        if(!isMoving)
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
