using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalDialogueManager : MonoBehaviour
{
    private static ExternalDialogueManager instance;
    
    [Header("Dialogue Canvas")]
    [SerializeField] private GameObject dialogueCanvas;
    /*
    [Header("Player Controller Canvas to hide (From Scene only)")]
    [SerializeField] private Canvas playerControllerCanvas;
    */
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one External Dialogue Manager in the scene");
        }
        instance = this;
    }
    public static ExternalDialogueManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {        
        dialogueCanvas.SetActive(false);
    }
    
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        dialogueCanvas.SetActive(true);
        PlatformerMovement2D.blocked = true;
        //Hide player controller
        //playerControllerCanvas.enabled = false;
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
    public void ExitDialogueMode()
    {
        PlatformerMovement2D.blocked = false;
        //Show player controller
        //playerControllerCanvas.enabled = true;
        dialogueCanvas.SetActive(false);
        Debug.Log("Dialogue Exited");
    }
}
