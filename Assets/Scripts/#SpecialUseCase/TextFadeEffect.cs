using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFadeEffect : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshProUGUI;
    private Animator textAnimator;
    private bool textHidden = true;
    public float delaySeconds = 4f;
    // Start is called before the first frame update
    void Start()
    {
        if(PlatformerMovement2D.blocked)
        {
            Debug.Log("Player Movement Blocked");
        }
        //PlatformerMovement2D.blocked = true;
        m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        textAnimator = GetComponent<Animator>();
        textAnimator.SetBool("textHidden", false);
        StartCoroutine(HideText());
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(delaySeconds);
        textAnimator.SetBool("textHidden", true);
        //PlatformerMovement2D.blocked = false;
    }
}
