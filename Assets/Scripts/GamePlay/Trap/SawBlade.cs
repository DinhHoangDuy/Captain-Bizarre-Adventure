using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    - This script will be used to handle the SawBlade trap
    - It will automatically attach Animator and CircleCollider2D to the GameObject
    - The trap will deal damage to the player when the player enters the damage area
*/


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
public class SawBlade : MonoBehaviour
{
    private CircleCollider2D SawDamageArea;
    private void Awake()
    {
        SawDamageArea = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if(player.CompareTag("Player"))
        {
            player.GetComponent<TakeDMG>().HitPlayer(1);
        }
    }
}
