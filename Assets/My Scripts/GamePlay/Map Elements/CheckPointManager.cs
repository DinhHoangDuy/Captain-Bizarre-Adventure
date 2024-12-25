using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public bool isUsed = false;
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            if (!isUsed)
            {
                // Set the respawn position
                //player.gameObject.GetComponent<Charater_Moving>().respawnPoint = transform.position;
                isUsed = true;
            }
        }
    }
}
