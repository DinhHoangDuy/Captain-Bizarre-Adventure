using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    private Vector3 cameraPosition;
    [SerializeField] private float offset;
    [SerializeField] private float offsetSmoothing;

    private void LateUpdate()
    {
        cameraPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        if(player.transform.localScale.x > 0f)
        {
            cameraPosition = new Vector3(player.transform.position.x + offset, player.transform.position.y, transform.position.z);
        }
        else
        {
            cameraPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        }
        //Chuyển vị trí camera từ vị trí ban đầu của nó đến chỗ player
        transform.position = Vector3.Lerp(transform.position, cameraPosition, offsetSmoothing);
    }
}
