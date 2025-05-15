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
            // TODO: Add these to a function.
            scriptScheduler.gameObject.SetActive(true);
            scriptScheduler.ReadNode(yarnNodeName);  
        }
        // scriptSchedueler = YarnScriptSchedueler.instance;         
    }
}
