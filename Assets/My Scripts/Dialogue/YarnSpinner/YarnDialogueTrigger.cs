using UnityEngine;

public class YarnDialogueTrigger : MonoBehaviour
{
    [SerializeField] private YarnScriptScheduler scriptScheduler;

    // public string[] yarnNodes;
    public string yarnNodeName;

    // Debug Only
    public bool autoStart = false;

    private void Start()
    {
        if (!scriptScheduler)
        {
            Debug.LogError("Schedueler not found");
            return;
        }

        if (autoStart)
        {
            StartDialogue();
        }
        // scriptSchedueler = YarnScriptSchedueler.instance;         
    }
    
    private void StartDialogue()
    {
        // Check if the node exists in the Yarn Project
        if (scriptScheduler.yarnNodeName == null)
        {
            Debug.LogWarning($"The Yarn Project {scriptScheduler.name} does not contain a node named \"{yarnNodeName}\"", scriptScheduler);
            return;
        }
        Debug.Log($"The Yarn Project {scriptScheduler.name} contains a node named \"{yarnNodeName}\"", scriptScheduler);
        // Start the dialogue with the specified node
        scriptScheduler.gameObject.SetActive(true);
        scriptScheduler.ReadNode(yarnNodeName);
    }
}
