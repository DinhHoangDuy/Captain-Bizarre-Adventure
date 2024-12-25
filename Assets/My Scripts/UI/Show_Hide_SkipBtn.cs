using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show_Hide_SkipBtn : MonoBehaviour
{
    public GameObject skipButton;
    private bool skipShowed;
    // Start is called before the first frame update
    void Start()
    {
        skipShowed = false;
        skipButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(skipShowed)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    void Hide()
    {
        skipButton.SetActive(false);
        skipShowed=false;
    }
    void Show()
    {
        skipButton.SetActive(true);
        skipShowed = true;
    }
}
