using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_Scene : MonoBehaviour
{
    //MainMenu
    public void ToCGPage()
    {
        SceneManager.LoadScene("CG Video");
    }
    public static void ToLevelSelect(string sceneName)
    {
        SceneManager.LoadScene("Scenes/Stages/" + sceneName + "/SelectLevel");
    }        
    public void ToChapterSelect()
    {
        SceneManager.LoadScene("Scenes/MainMenu/ChapterSelect");
    }
    //Exit
    public void ExitToLvSelect()
    {
        SceneManager.LoadScene("SelectLevel"); ;
    }
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu/Welcome");
    }    
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    //Universal
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
