using UnityEngine;

public class RoomToRoomTelepoint : MonoBehaviour
{
    [SerializeField] private BoxCollider2D CameraConfiner2DRoomA;
    [SerializeField] private BoxCollider2D CameraConfiner2DRoomB;

    void Awake()
    {
        if (CameraConfiner2DRoomA == null || CameraConfiner2DRoomB == null)
        {
            Debug.LogWarning("Camera Confiner 2D is not assigned! They cannot be seen in the Scene View.");
        }
    }
    void OnDrawGizmosSelected()
    {
        if (CameraConfiner2DRoomA != null && CameraConfiner2DRoomB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(CameraConfiner2DRoomA.bounds.center, CameraConfiner2DRoomA.bounds.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(CameraConfiner2DRoomB.bounds.center, CameraConfiner2DRoomB.bounds.size);
        }

        // Draw gizmos for nearby RoomTransitionAutoTrigger objects
        RoomTransitionAutoTrigger[] roomTriggers = FindObjectsOfType<RoomTransitionAutoTrigger>();
        foreach (var trigger in roomTriggers)
        {
            if (trigger.transform != this.transform)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(trigger.GetComponent<BoxCollider2D>().bounds.center, trigger.GetComponent<BoxCollider2D>().bounds.size);
                // Gizmos.DrawLine(this.transform.position, trigger.transform.position);
            }
        }
    }
}
