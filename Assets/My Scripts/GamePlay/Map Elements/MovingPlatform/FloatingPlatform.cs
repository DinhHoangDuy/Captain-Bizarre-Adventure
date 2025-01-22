using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(MovingPlatform))]
public class FloatingPlatform : MonoBehaviour
{
    private MovingPlatform movingPlatform;
    void Start()
    {
        movingPlatform = GetComponent<MovingPlatform>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered the platform");
            if (movingPlatform.points.Length >= 1)
            {
                // Set the player as the child of the platform
                collision.transform.SetParent(transform);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited the platform");
            // Remove the player as the child of the platform
            collision.transform.SetParent(null);
        }
    }
}
