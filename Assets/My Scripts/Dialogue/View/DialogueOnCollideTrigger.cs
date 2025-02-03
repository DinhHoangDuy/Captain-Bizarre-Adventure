using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(BoxCollider2D))]
public class DialogueOnCollideTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    private BoxCollider2D boxCollider2D;
    public bool isUsed = false;

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Set the "isTrigger" property to true
        boxCollider2D.isTrigger = true;
    }

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
