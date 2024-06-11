using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(TakeDMG))]
[RequireComponent(typeof(EnemyResistance))]
[RequireComponent(typeof(EnemyHealth))]
public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDelay = 4.0f;
    private bool attackAllowed = true;
    [SerializeField] private LayerMask playerFriendlyLayer;

    [Header("Stats")]

    [SerializeField] private int attackDamage = 1;

    
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (PlayerDetected())
        {
            if (attackAllowed)
            {
                Attack();
                StartCoroutine(AttackDelay());
            }
        }
    }


    #region Attack Event
    bool PlayerDetected()
    {
        // Detect player in attack range
        Collider2D[] hitFriendly = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerFriendlyLayer);
        if(hitFriendly.Length > 0)
        {
            return true;
        }
        return false;
    }
    void Attack()
    {
        // Attack the player
        animator.Play("Attack 1");
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;    
    }
    IEnumerator AttackDelay()
    {
        attackAllowed = false;
        yield return new WaitForSeconds(attackDelay);
        attackAllowed = true;
    }
    
    /// <summary>
    /// This event is called from the animation event in the attack animation
    /// </summary>
    public void DealDamge()
    {
        // Detect player in attack range during attack
        Collider2D[] hitFriendly = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerFriendlyLayer);
        foreach(Collider2D player in hitFriendly)
        {
            player.GetComponent<TakeDMG>().HitPlayer(attackDamage, transform.position);
            Debug.Log("Player Hit!!!");
        }
    }
    #endregion


    /// <summary>
    /// Draws the attack range gizmo in the scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            Debug.LogError("Attack Point is not assigned");
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
