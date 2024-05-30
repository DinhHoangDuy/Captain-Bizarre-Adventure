using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Animator transitionAnim;
    [SerializeField] private TextMeshProUGUI loadingText;
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
        loadingText.text = "Moving to " + levelName + "...";
        transitionAnim.SetTrigger("Trigger");
    }
    public void BlockInput()
    {
        PlatformerMovement2D.blocked = true;
        CaptainMoonBlade.blocked = true;
    }
    public void UnblockInput()
    {
        PlatformerMovement2D.blocked = false;
        CaptainMoonBlade.blocked = false;
    }
}
