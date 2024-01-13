using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterSelectButton : MonoBehaviour
{
    [Header("Chapter Preview")]
    [SerializeField] private Animator backgroundController;
    private string Chapter;

    private void Start()
    {
        backgroundController.Play("default");
    }
    public void ChangeImage(string name)
    {
        Chapter = name;
        backgroundController.Play(name);
    }
    public void ToChapter()
    {
        Change_Scene.ToLevelSelect(Chapter);
    }
}
