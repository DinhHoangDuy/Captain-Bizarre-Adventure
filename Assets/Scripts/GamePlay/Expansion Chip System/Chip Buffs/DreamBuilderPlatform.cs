using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamBuilderPlatform : MonoBehaviour
{
    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        duration = ExpansionChipStatus.instance.dreamBuilderPlatformDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
