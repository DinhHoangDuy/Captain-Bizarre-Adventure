using UnityEngine;
using Yarn.Unity;
using System.Linq;


public class YarnScriptScheduler : MonoBehaviour
{

    // Define needed variables
    [Header("Dialogue Componenets")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private YarnDialogueSkipper dialogueSkipper;
    [SerializeField] private YarnDialogueBackground dialogueBackground;
    public string yarnNodeName;
    void Awake()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("No Runner found");
        }
        // dialogueRunner.onDialogueComplete.AddListener(() => dialogueRunner.gameObject.SetActive(false));
        dialogueRunner.onDialogueComplete.AddListener(() => EndDialogue());
    }

    public void ReadNode(string nodeName)
    {
        // Check if the node exists in the Yarn Project
        if (dialogueRunner.YarnProject.NodeNames.Contains(nodeName) == false)
        {
            Debug.LogWarning($"The Yarn Project {dialogueRunner.name} does not contain a node named \"{nodeName}\"", dialogueRunner.YarnProject);
            return;
        }
        Debug.Log($"The Yarn Project {dialogueRunner.name} contains a node named \"{nodeName}\"", dialogueRunner.YarnProject);
        // Start the dialogue with the specified node
        dialogueRunner.StartDialogue(nodeName);
    }

    private void EndDialogue()
    {
        dialogueBackground.ResetAudioSources();

        // Finally, diactivate the dialogue runner (by SetActive(false))
        dialogueRunner.gameObject.SetActive(false);
    }

}