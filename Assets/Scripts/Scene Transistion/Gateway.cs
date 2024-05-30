using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private string sceneName;
    private BoxCollider2D boxCollider2D;
    private bool sceneNullReported = false;

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (string.IsNullOrEmpty(sceneName) && !sceneNullReported)
    {
        Debug.LogError("Scene name is null or empty");
        sceneNullReported = true;
        boxCollider2D.enabled = false;
        return;
    }     
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            levelLoader.TriggerLoading(sceneName);
        }
    }
}
