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
    public float damageTaken = 0f;

    private float timeToReset = 5f;
    private Coroutine resetCoroutine;

    public void TakeDamage(float damage)
    {
        RecordDamage(damage);
        Debug.Log("Dummy took " + damage + " damage.");

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(ResetDamageAfterTime());
    }

    public void RecordDamage(float damage)
    {
        damageTaken += damage;
        Debug.Log("Dummy has taken " + damageTaken + " damage.");
    }

    public void ResetDamage()
    {
        damageTaken = 0f;
        Debug.Log("Dummy has reset its damage.");
    }

    private IEnumerator ResetDamageAfterTime()
    {
        yield return new WaitForSeconds(timeToReset);
        ResetDamage();
    }

    void Update()
    {

    }
}