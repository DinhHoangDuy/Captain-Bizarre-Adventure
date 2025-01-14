using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnCollideTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    public bool isUsed = false;

    private void Update() 
    {
        if (isUsed)
        {
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(!isUsed)
            {
                // PlatformerMovement2D.instance.inputBlocked = true;
                // ExternalDialogueManager.instance.EnterDialogueMode(inkJSON);
                DialogueManager.instance.EnterDialogueMode(inkJSON);
                isUsed = true;
            }
        }
    }
}
