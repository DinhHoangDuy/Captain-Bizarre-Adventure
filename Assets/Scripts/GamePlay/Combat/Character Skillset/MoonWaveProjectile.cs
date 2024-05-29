using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MoonWaveProjectile : MonoBehaviour
{
    //Needed components for a projectile
    private Rigidbody2D rb;

    //Variables
    [SerializeField] private  LayerMask wallLayer;
    [SerializeField] private  LayerMask enemyLayer;


    #region Wave Damage
        //Wave Damage
        public float waveDamage { get; private set;}
        private float speed;
        private float lifeTime;
        public DamageType damageType { get; private set;}
        private GameObject[] enemies = new GameObject[0];

        public void SetWaveDamage(float damage, DamageType DMGType)
        {
            waveDamage = damage;
            damageType = DMGType;
        }
        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }
        public void SetDuration(float duration)
        {
            lifeTime = duration;
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
        StartCoroutine(DestroyProjectile());
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
            bool isEnemyInList = false;
            foreach (GameObject enemy in enemies)
            {
                if (hitEnemy.gameObject == enemy)
                {
                    isEnemyInList = true;
                    Debug.Log("Enemy is in the list: " + hitEnemy.gameObject.name + ". The enemy did not take damage in this frame!.");
                    break;
                }
            }
            if(!isEnemyInList)
            {
                enemies = new GameObject[enemies.Length + 1];
                enemies[enemies.Length - 1] = hitEnemy.gameObject;
                hitEnemy.GetComponent<TakeDMG>().TakeRangeDamage(waveDamage, damageType, DamageFromSkill.UltimateSkill);
                Debug.Log("Enemy is not in the list: " + hitEnemy.gameObject.name + ". The enemy took damage in this frame!.");
                Debug.Log("Enemy took: " + waveDamage + " damage of type: " + damageType);
            }

            return;
        }
    }
    private IEnumerator DestroyProjectile()
    {
        Debug.Log("Projectile's life time: " + lifeTime + " seconds.");
        Debug.Log("Projectile's speed: " + speed + " units per second.");

        yield return new WaitForSeconds(lifeTime);
        
        Destroy(gameObject);
        Debug.Log("Projectile Destroyed!");
    }
}
