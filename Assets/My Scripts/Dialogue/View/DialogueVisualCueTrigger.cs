using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DialogueVisualCueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    [SerializeField] private bool oneTimeUseOnly = false;
    private BoxCollider2D boxCollider2D;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;
    public bool isUsed = false;
    private void Start() 
    {
        playerInRange = false;
        isUsed = false;
        visualCue.SetActive(false);

        boxCollider2D = GetComponent<BoxCollider2D>();
        // Set the "isTrigger" property to true
        boxCollider2D.isTrigger = true;
    }

    private void Update() 
    {
        if (!isUsed && playerInRange && !DialogueManager.instance.DialogueIsPlaying) 
        {
            visualCue.SetActive(true);
            //if(Input.GetKeyDown(KeyCode.F))
            //if (InputManager.GetInstance().GetInteractPressed())
            // if(Input.GetButtonDown("Interact"))
            if (InputManager.instance.interactionInputTriggered)
            {
                // ExternalDialogueManager.instance.EnterDialogueMode(inkJSON);
                DialogueManager.instance.EnterDialogueMode(inkJSON);
                if(oneTimeUseOnly) isUsed = true;
            }
        }
        else 
        {
            visualCue.SetActive(false);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            if (!oneTimeUseOnly) isUsed = false; // Fallback in case the player leaves the trigger area
        }
    }
}
