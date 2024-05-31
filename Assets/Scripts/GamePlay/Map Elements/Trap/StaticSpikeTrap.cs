using UnityEngine;
using System;
using System.Collections;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class StaticSpikeTrap : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Coroutine spikeTrapAnimationCoroutine;
    [SerializeField] private int spikeTrapDamage = 1;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider2D.enabled = true;

    }
    private void Update()
    {
        if(spikeTrapAnimationCoroutine == null)
        {
            spikeTrapAnimationCoroutine = StartCoroutine(PlayShiningAnimation());
        }
    }
    
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            Debug.Log("Player hit the spike trap!");
            player.GetComponent<TakeDMG>().HitPlayer(spikeTrapDamage, transform.position);
        }
    }

    private IEnumerator PlayShiningAnimation()
    {
        yield return new WaitForSeconds(10f);
        animator.SetTrigger("Shine");
        spikeTrapAnimationCoroutine = null;
    }
}