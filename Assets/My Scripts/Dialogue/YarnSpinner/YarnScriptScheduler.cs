using UnityEngine;
using Yarn.Unity;
using System.Linq;
using System.Collections.Generic;
public class YarnScriptScheduler : MonoBehaviour
{
    // Create a singleton instance
    // public static YarnScriptSchedueler instance;

    // Define needed variables
    private DialogueRunner runner;

    public string yarnNodeName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        runner = GetComponent<DialogueRunner>();
        if (runner == null)
        {
            Debug.LogError("No Runner found");
        }

        // // Show Yarn Nodes it have.
        // for (int i = 0; i < yarnNodes.Length; i++)
        // {
        //     Debug.Log(yarnNodes[i]);
        //     if (runner.yarnProject.NodeNames.Contains(yarnNodes[i]) == false) {
        //         Debug.LogWarning($"The Yarn Project {runner.name} does not contain a node named \"{yarnNodes[i]}\"", runner.yarnProject);
        //         return;
        //     }
        // }        

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadNode(string nodeName)
    {
        // Check if the node exists in the Yarn Project
        if (runner.yarnProject.NodeNames.Contains(nodeName) == false)
        {
            Debug.LogWarning($"The Yarn Project {runner.name} does not contain a node named \"{nodeName}\"", runner.yarnProject);
            return;
        }
        Debug.Log($"The Yarn Project {runner.name} contains a node named \"{nodeName}\"", runner.yarnProject);    
        // Start the dialogue with the specified node
        runner.StartDialogue(nodeName);
    }

}