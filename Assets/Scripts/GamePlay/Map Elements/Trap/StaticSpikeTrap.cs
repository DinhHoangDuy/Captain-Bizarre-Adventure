using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class StaticSpikeTrap : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    [SerializeField] private int spikeTrapDamage = 1;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        //Play and loop the Active animation
        animator.Play("Active");
        boxCollider2D.enabled = true;
    }
    
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            // TODO: Implement spike trap logic here
            Debug.Log("Player triggered the spike trap!");
            player.GetComponent<TakeDMG>().HitPlayer(spikeTrapDamage);
        }
    }
}