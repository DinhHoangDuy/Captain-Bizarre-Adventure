using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpikeTrapTrigger : MonoBehaviour
{
    //Dependecies
    private Animator animator;
    private BoxCollider2D boxCollider2D;

    // Spike Trap Variables
    [SerializeField] private int spikeTrapDamage = 1;
    [SerializeField] private TrapTrigger[] trapTrigger;
    private bool isTriggered = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Run the Animations
    public void Activate()
    {
        animator.SetTrigger("Activate");
    }
    public void Deactivate()
    {
        animator.SetTrigger("Deactivate");
    }
    private void GlowingEffect()
    {
        animator.Play("Glowing");
    }

    // Activate/Deactivate the Collider
    public void ActivateCollider()
    {
        boxCollider2D.enabled = true;
    }
    public void DeactivateCollider()
    {
        boxCollider2D.enabled = false;
    }

    // Damage the Player
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            player.GetComponent<TakeDMG>().HitPlayer(spikeTrapDamage);
        }
    }

    private void Start()
    {
        DeactivateCollider();
    }

    // Check if the trap is triggered
    private void Update()
    {
        foreach (var trigger in trapTrigger)
        {
            if (trigger.TriggerStatus() && !isTriggered)
            {
                Activate();
                isTriggered = true;
            }
            else if (!trigger.TriggerStatus() && isTriggered)
            {
                Deactivate();
                isTriggered = false;
            }
        }
    }
}
