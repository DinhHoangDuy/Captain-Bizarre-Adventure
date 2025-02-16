using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class RoomTransitionAnimation : MonoBehaviour
{
    [Tooltip("Enable this option if you want to use the animation for the room transition. (Should be used to debug the room transition)")]
    [SerializeField] private bool useAnimation = true;
    public static RoomTransitionAnimation instance;
    internal Transform targetTransform = null;
    internal CinemachineCamera cinemachineCameraRoomA = null;
    internal CinemachineCamera cinemachineCameraRoomB = null;
    internal float characterGravityScale;
    internal AutoMoveDirection autoMoveDirection;
    private Animator crossfadeAnimator;
    private Animator playerUIAnimator;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        crossfadeAnimator = GetComponent<Animator>();
        playerUIAnimator = GameObject.Find("Player UI").GetComponent<Animator>();
    }


    internal void SendTransitionInformation(Transform targetTransform, CinemachineCamera cinemachineCameraRoomA, CinemachineCamera cinemachineCameraRoomB, AutoMoveDirection autoMoveDirection, float characterGravityScale)
    {
        this.targetTransform = targetTransform;
        this.cinemachineCameraRoomA = cinemachineCameraRoomA;
        this.cinemachineCameraRoomB = cinemachineCameraRoomB;
        this.autoMoveDirection = autoMoveDirection;
        this.characterGravityScale = characterGravityScale;

        if (useAnimation)
        {
            crossfadeAnimator.SetTrigger("Start");
            playerUIAnimator.SetTrigger("Start");
        }
        else
        {
            Room_Transition_Animation();
        }

    }

    // Attach to the Animation Event
    public void Room_Transition_Animation()
    {
        if (targetTransform == null || cinemachineCameraRoomA == null || cinemachineCameraRoomB == null)
        {
            Debug.LogWarningFormat("Transition information is missing! The game is not transitioning to the target room. GameObject:" + gameObject.name);
            return;
        }
        // Transitioning the player to the target room
        PlatformerMovement2D.instance.transform.position = targetTransform.position;
        // Switching the Cinemachine Cameras
        CameraManager.instance.SwitchCamera(cinemachineCameraRoomA, cinemachineCameraRoomB);
        // CameraManager.instance.UseThisCamera(cinemachineCameraRoomB);

        // Delete all the informations
        targetTransform = null;
        cinemachineCameraRoomA = null;
        cinemachineCameraRoomB = null;


        if (autoMoveDirection == AutoMoveDirection.BottomUp)
        {
            // Shoot the character upwards and horizontally
            PlatformerMovement2D.instance.rb.gravityScale = characterGravityScale;
            PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(0, 0);
            PlatformerMovement2D.instance.rb.AddForce(new Vector2(0, PlatformerMovement2D.instance.jumpingPower * 1f), ForceMode2D.Impulse);
            // TODO: Try to throw the character horizontally, too. Make it throw the character upwards and horizontally based on the direction the character is facing.
            PlatformerMovement2D.instance.inputBlocked = false;
        }
        else if (autoMoveDirection == AutoMoveDirection.LeftToRight)
        {
            PlatformerMovement2D.instance.rb.gravityScale = characterGravityScale;
            PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(PlatformerMovement2D.instance.rb.linearVelocityX, PlatformerMovement2D.instance.rb.linearVelocity.y);
            // PlatformerMovement2D.instance.inputBlocked = false;
        }
        else if (autoMoveDirection == AutoMoveDirection.RightToLeft)
        {
            PlatformerMovement2D.instance.rb.gravityScale = characterGravityScale;
            PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(-PlatformerMovement2D.instance.rb.linearVelocityX, PlatformerMovement2D.instance.rb.linearVelocity.y);
            // PlatformerMovement2D.instance.inputBlocked = false;
        }
        else if (autoMoveDirection == AutoMoveDirection.TopDown)
        {
            PlatformerMovement2D.instance.rb.gravityScale = characterGravityScale;
            PlatformerMovement2D.instance.rb.linearVelocity = new Vector2(0, 0);
            PlatformerMovement2D.instance.inputBlocked = false;
        }

    }
}