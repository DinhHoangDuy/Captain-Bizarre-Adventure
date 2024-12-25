using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVisualCueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;
    public bool isUsed = false;
    private void Start() 
    {
        playerInRange = false;
        isUsed = false;
        visualCue.SetActive(false);
    }

    private void Update() 
    {
        if (!isUsed && playerInRange && !DialogueManager.GetInstance().DialogueIsPlaying) 
        {
            visualCue.SetActive(true);
            //if(Input.GetKeyDown(KeyCode.F))
            //if (InputManager.GetInstance().GetInteractPressed())
            if(Input.GetButtonDown("Interact"))
            {
                ExternalDialogueManager.GetInstance().EnterDialogueMode(inkJSON);
                isUsed = true;
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
        }
    }
}
