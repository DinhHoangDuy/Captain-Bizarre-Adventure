using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyResistance))]
public class Dummy : MonoBehaviour
{ 
    [Tooltip("This is meant for Check Damage only, DO NOT EDIT!!")]
    [SerializeField] private float DamageTaken = 0f; // The amount of damage the dummy has taken which will be shown in the inspector
    public void TakeDamage(float damage)
    {
        RecordDamage(damage);
    }
    public void RecordDamage(float damage)
    {
        DamageTaken += damage;
    }
}