using System.Collections;
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
        private float waveHitForce;
        private GameObject[] enemies = new GameObject[0];

        public void SetWaveDamage(float damage, float hitForce)
        {
            waveDamage = damage;
            waveHitForce = hitForce;
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
        rb.linearVelocity = transform.right * speed;
        StartCoroutine(DestroyProjectile());
    }
    private void Update()
    {        
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
                    break;
                }
            }
            if(!isEnemyInList)
            {
                enemies = new GameObject[enemies.Length + 1];
                enemies[enemies.Length - 1] = hitEnemy.gameObject;
                // hitEnemy.GetComponent<TakeDMG>().TakeRangeDamage(waveDamage, damageType, DamageFromSkill.UltimateSkill);

                // Push the enemy back
                float hitDirection = transform.right.x;
            }

            return;
        }
    }
    private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(lifeTime);        
        Destroy(gameObject);
    }
}
