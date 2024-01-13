using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedTrigger : MonoBehaviour
{
    [SerializeField] private float delaySecs = 4f;
    [SerializeField] private new Collider2D collider; 
    // Start is called before the first frame update
    private void Start()
    {
        collider.enabled = false;
        StartCoroutine(DelayedEnableTrigger());
    }
    private IEnumerator DelayedEnableTrigger()
    {
        yield return new WaitForSeconds(delaySecs);
        collider.enabled = true;
    }
}
