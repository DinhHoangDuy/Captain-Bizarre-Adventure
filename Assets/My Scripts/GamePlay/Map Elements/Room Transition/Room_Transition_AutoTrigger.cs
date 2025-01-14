using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class RoomTransitionAutoTrigger : MonoBehaviour
{
    [SerializeField] private Direction movingDirection;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private CinemachineCamera cinemachineCameraRoomA;
    [SerializeField] private CinemachineCamera cinemachineCameraRoomB;
    [SerializeField] private BoxCollider2D camenraConfiner2DRoomA;
    private AutoMoveDirection autoMoveDirection;
    private RoomTransitionAnimation playerUIAnimator;
    private bool isTriggered = false;
    void Start()
    {
        playerUIAnimator = RoomTransitionAnimation.instance;
        if (playerUIAnimator == null)
        {
            Debug.LogError("PlayerUI Animator is not found!");
        }

        // Check both Cinemachine Cameras are not null
        if (cinemachineCameraRoomA == null || cinemachineCameraRoomB == null)
        {
            Debug.LogError("One of the Cinemachine Cameras are not assigned! Check game object: " + gameObject.name);
        }
        else
        {
            // Check if the Cinemachine Cameras are active or not
            if (cinemachineCameraRoomA.enabled)
            {
                Debug.Log("Cinemachine Camera Room A is active");
            }
            if (cinemachineCameraRoomB.enabled)
            {
                Debug.Log("Cinemachine Camera Room B is active");
            }
            // Export an error if both cameras are active
            if (cinemachineCameraRoomA.enabled && cinemachineCameraRoomB.enabled)
            {
                Debug.LogError("Both Cinemachine Cameras are active! This is not allowed.");
            }
        }



    }

    // void OnTriggerEnter2D(Collider2D other)
    void OnTriggerStay2D(Collider2D other)
    {
        float moveSpeed = PlatformerMovement2D.instance.moveSpeed;
        float currentVelocityY = PlatformerMovement2D.instance.rb.linearVelocity.y;
        float jumpForce = PlatformerMovement2D.instance.jumpingPower;
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the room");
            switch (movingDirection)
            {
                case Direction.LeftToRight:
                    if (PlatformerMovement2D.instance.IsLookingRight)
                    {
                        Debug.Log("Player is moving right when entering the room");
                        // Forcing the player to move to the right
                        PlatformerMovement2D.instance.inputBlocked = true;
                        PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(moveSpeed, currentVelocityY);
                        isTriggered = true;
                        autoMoveDirection = AutoMoveDirection.LeftToRight;
                    }
                    break;
                case Direction.RightToLeft:
                    if (!PlatformerMovement2D.instance.IsLookingRight)
                    {
                        Debug.Log("Player is moving left when entering the room");
                        // Forcing the player to move to the left
                        PlatformerMovement2D.instance.inputBlocked = true;
                        PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(-moveSpeed, currentVelocityY);
                        isTriggered = true;
                        autoMoveDirection = AutoMoveDirection.RightToLeft;
                    }
                    break;
                case Direction.TopDown:
                    if (currentVelocityY < 0)
                    {
                        Debug.Log("Player is moving down when entering the room");
                        // Forcing the player to move up
                        PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(0, currentVelocityY);
                        isTriggered = true;
                        autoMoveDirection = AutoMoveDirection.TopDown;
                    }
                    break;
                case Direction.BottomUp:
                    if (currentVelocityY > 0)
                    {
                        Debug.Log("Player is moving up when entering the room");
                        // Forcing the player to move down
                        PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(0, jumpForce);
                        isTriggered = true;
                        PlatformerMovement2D.instance.isTransitingFromTheBottomUp = true;
                        autoMoveDirection = AutoMoveDirection.BottomUp;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Do not trigger the exit event if the trigger is not triggered
            if (!isTriggered)
            {
                PlatformerMovement2D.instance.inputBlocked = false;
                PlatformerMovement2D.instance.isTransitingFromTheBottomUp = false;
                return;
            }
            else
            {
                Debug.Log("Player exited the room");
                playerUIAnimator.SendTransitionInformation(targetTransform, cinemachineCameraRoomA, cinemachineCameraRoomB, autoMoveDirection, PlatformerMovement2D.instance.characterGravityScale);
                isTriggered = false;

                if (autoMoveDirection == AutoMoveDirection.TopDown || autoMoveDirection == AutoMoveDirection.BottomUp)
                {
                    PlatformerMovement2D.instance.inputBlocked = true;
                    if (autoMoveDirection == AutoMoveDirection.BottomUp)
                    {
                        PlatformerMovement2D.instance.rb.AddForce(new Vector2(0, PlatformerMovement2D.instance.jumpingPower * 1f), ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (targetTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetTransform.position, 0.3f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(camenraConfiner2DRoomA.bounds.center, camenraConfiner2DRoomA.bounds.size);
        }
    }

}

public enum AutoMoveDirection
{
    LeftToRight,
    RightToLeft,
    TopDown,
    BottomUp,
}

enum Direction
{
    LeftToRight,
    RightToLeft,
    TopDown,
    BottomUp,
}

// Graphical example
// -------TopDown------------
// |                        |
// |                        |
// RightToLeft <-[     ]-> LeftToRight
// |                        |
// |                        |
// ------BottomUp------------
