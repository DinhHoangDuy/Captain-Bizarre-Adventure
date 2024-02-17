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
    public float speed;
    public LayerMask wallLayer;
    public LayerMask enemyLayer;

    #region Wave Damage
        //Wave Damage
        public float waveDamage;

        public void SetWaveDamage(float damage)
        {
            waveDamage = damage;
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
            Debug.Log("Wave Destroyed by Wall");
            Destroy(gameObject);
            return;
        }

        //Damage the enemy if the projectile hits enemies
        Collider2D hitEnemy = Physics2D.OverlapCircle(transform.position, 0.5f, enemyLayer);
        if (hitEnemy)
        {
            hitEnemy.GetComponent<DMGInput>().TakeRangeDamage(waveDamage, DamageType.Lightning);
            Debug.Log("Wave Destroyed by Enemy");
            Destroy(gameObject);
            return;
        }
    }
}
