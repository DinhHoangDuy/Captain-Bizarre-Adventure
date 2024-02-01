using UnityEngine;

public class BasicAttackBehaviour : StateMachineBehaviour
{
    private PlayerStats playerStats;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerStats == null)
        {
            playerStats = animator.GetComponent<PlayerStats>();
        }

        playerStats.UnblockAllMovement();
    }
}
