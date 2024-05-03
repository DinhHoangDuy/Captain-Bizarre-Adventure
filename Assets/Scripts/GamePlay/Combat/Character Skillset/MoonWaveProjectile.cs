using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MoonWaveProjectile : MonoBehaviour
{
    //Needed components for a projectile
    private Rigidbody2D rb;

    //Variables
    [SerializeField] private float speed;
    [SerializeField] private  LayerMask wallLayer;
    [SerializeField] private  LayerMask enemyLayer;


    #region Wave Damage
        //Wave Damage
        public float waveDamage { get; private set;}
        public DamageType damageType { get; private set;}

        public void SetWaveDamage(float damage, DamageType DMGType)
        {
            waveDamage = damage;
            damageType = DMGType;
        }
    #endregion

    private void Awake()
    {
        //Get the components
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        //Move the projectile
        rb.velocity = transform.right * speed;
    }
    private void Update()
    {
        //Destroy the projectile if it hits a wall
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, transform.right, 0.5f, wallLayer);
        if (hitWall)
        {
            Destroy(gameObject);
            return;
        }

        //Damage the enemy if the projectile hits enemies
        Collider2D hitEnemy = Physics2D.OverlapCircle(transform.position, 0.5f, enemyLayer);
        if (hitEnemy)
        {
            hitEnemy.GetComponent<TakeDMG>().TakeRangeDamage(waveDamage, damageType, DamageFromSkill.UltimateSkill);
            Destroy(gameObject);
            return;
        }
    }
}
