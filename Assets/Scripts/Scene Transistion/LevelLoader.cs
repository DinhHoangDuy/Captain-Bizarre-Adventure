using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    private Animator transitionAnim;
    private string levelName;
    private void Start()
    {
        transitionAnim = GetComponent<Animator>();
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }
    public void TriggerLoading(string levelName)
    {
        if(levelName == null)
        {
            Debug.LogError("Level name is null");
            return;
        }
        this.levelName = levelName;
        transitionAnim.SetTrigger("Trigger");
    }
    public void BlockInput()
    {
        PlatformerMovement2D.instance.blocked = true;
        CaptainMoonBlade.blocked = true;
        playerUI.SetActive(false);
    }
    public void UnblockInput()
    {
        PlatformerMovement2D.instance.blocked = false;
        CaptainMoonBlade.blocked = false;
        playerUI.SetActive(true);
    }
}
