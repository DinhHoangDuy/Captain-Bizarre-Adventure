using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldRiftEntry : MonoBehaviour
{
    private Animator textAnimator;
    public float delaySeconds = 4f;
    [SerializeField] private BoxCollider2D firstTrigger;

    //[SerializeField] private GameObject DialogueManager;
    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {        
        PlatformerMovement2D.blocked = true;
        if (PlatformerMovement2D.blocked)
        {
            Debug.Log("Player Movement Blocked");
        }
        textAnimator = GetComponent<Animator>();
        textAnimator.SetBool("textHidden", false);
        firstTrigger.enabled = false;
        //DialogueManager.SetActive(false);
        StartCoroutine(HideText());
    }

    [System.Obsolete]
    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(delaySeconds);
        textAnimator.SetBool("textHidden", true);
        firstTrigger.enabled = true;
    }
}
