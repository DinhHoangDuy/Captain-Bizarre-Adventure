using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class SpearTrap : MonoBehaviour
{
    [SerializeField] private float inActiveDuration = 4f;
    [SerializeField] private float activeDuration = 3f;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Coroutine spearTrapAnimationCoroutine;
    [SerializeField] private int spearTrapDamage = 1;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider2D.enabled = true;

        animator.Play("Inactive");

    }
    private void Update()
    {
        if (spearTrapAnimationCoroutine == null)
        {
            spearTrapAnimationCoroutine = StartCoroutine(SpearTrapController());
        }
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            player.GetComponent<TakeDMG>().HitPlayer(spearTrapDamage);
        }
    }

    private IEnumerator SpearTrapController()
    {
        animator.SetTrigger("Activate");
        yield return new WaitForSeconds(activeDuration);
        animator.SetTrigger("Deactivate");
        yield return new WaitForSeconds(inActiveDuration);
        spearTrapAnimationCoroutine = null;
    }

}
