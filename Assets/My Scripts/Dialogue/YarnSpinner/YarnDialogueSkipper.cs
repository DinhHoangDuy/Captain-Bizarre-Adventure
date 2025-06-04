using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

// How it should work:
/*
    1. The yarn script should have a variable that contains the name of the next dialogue node.
    2. When the dialogue starts, the script will sent a message to the Dialogueskipper to know the next node
     (or the "EndDialogue" node which means the dialogue is finished after this node. "EndDialogue" will be the default node if no other node is specified).
    3. The script will listen for button clicks on the skip buttons.
    4. When a skip button is clicked, the script will check if the current dialogue node is the final node.
    5. If it is the final node, the script will end the dialogue.
    6. If it is not the final node, the script will skip to the next dialogue node.
*/
public class YarnDialogueSkipper : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;

    [Tooltip("List of buttons that will trigger the skip action when clicked.")]
    [SerializeField] private List<Button> skipButtons = new List<Button>();

    // List of nodes that are considered the next dialogues node
    private string nextDialogueNode = null;
    // We'll use "EndDialogue" as a default final node name.
    private const string DefaultFinalNodeName = "EndDialogue";

    public void Start()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("DialogueRunner is not assigned in YarnDialogueSkipper.");
            return;
        }

        // Add listeners to each skip button
        foreach (var button in skipButtons)
        {
            if (button != null)
            {
                button.onClick.AddListener(SkipDialogue);
            }
            else
            {
                Debug.LogWarning("A skip button is not assigned in the list.");
            }
        }
    }

    public void SkipDialogue()
    {
        // Sent a message to the devs to let them know the dialogue is being skipped
        Debug.Log("SkipDialogue called");

        // Skip Dialogue logic
        if (nextDialogueNode == null || nextDialogueNode == DefaultFinalNodeName)
        {
            // If no next dialogue nodes are set, or if the default final node is set, end the dialogue.
            Debug.Log("Ending dialogue as no next dialogue nodes are set or default final node is active.");
            dialogueRunner.Stop(); // Tell the DialogueRunner to stop the dialogue.
            return;
        }
        else
        {
            // If next dialogue nodes are set, start the next dialogue node.
            Debug.Log($"Skipping to next dialogue node: {nextDialogueNode}");



            // Tell the DialogueRunner to start the next dialogue node.
            // Step 1: Stop the current dialogue.
            dialogueRunner.Stop();
            // Step 2: Start the next dialogue node.
            StartCoroutine(StartDialogueNextFrame(nextDialogueNode));

            // Final step: reset the nextDialogueNode to the default final node.
            nextDialogueNode = null;
            Debug.Log($"The next dialogue node is reseted. The next Skip should end the dialogue, unless a new node is set by the Yarn script.");
        }
    }

    private IEnumerator StartDialogueNextFrame(string nodeName)
    {
        // Wait for the end of the frame to ensure all cleanup is complete
        yield return new WaitForEndOfFrame();

        // Ensure the DialogueRunner is active
        dialogueRunner.gameObject.SetActive(true);

        // Start the new dialogue
        Debug.Log($"Starting dialogue at node: {nodeName}");
        dialogueRunner.StartDialogue(nodeName);
    }

    [YarnCommand("NextDialogueNode")]
    public void SetNextDialogueNode(string nodes)
    {
        if (nextDialogueNode != null)
        {
            Debug.LogError("NextDialogueNode command failed: the 'nextDialogueNode' value is already set." +
                            "Check the yarn script for multiple calls to this command at the same node");
            Debug.Log("Current 'nextDialogueNode' node: " + nextDialogueNode);
            // Debug.LogError("Error node triggered at: " + dialogueRunner.CurrentNodeName);
            return;
        }
        // Set the final dialogue nodes from the Yarn script
        nextDialogueNode = nodes;
        Debug.Log($"The next is dialogue nodes set to: {nextDialogueNode}");
    }
}