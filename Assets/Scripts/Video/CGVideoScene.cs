using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CGVideoScene : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;
    private const string VideoPath = "Videos/";
    private string VideoFilePath = VideoPath;
    private bool IsVideoPlaying;
    [Header("Hide UI")]
    [SerializeField] private Canvas uiCanvas;
    [Header("Skip Video Button")]
    [SerializeField] private Button skipButton;
    private Animator skipButtonAnimator;
    private bool IsShowed;

    void Start()
    {
        skipButton.onClick.AddListener(ClickToSkip);
        skipButtonAnimator = skipButton.gameObject.GetComponent<Animator>();
        videoPlayer.loopPointReached += SkipVideo;
        IsShowed = false;
        IsVideoPlaying = false;
    }

    void Update()
    {
        /*
        if(IsVideoPlaying)
        {
            if(!IsShowed)
            {
                skipButtonAnimator.SetBool("IsShowed", true);
            }
            else
            {
                skipButtonAnimator.SetBool("IsShowed", false);
            }
        }
        */
    }
    public void ReadyToPlayVideo(string name)
    {
        //Initiate the Video clip
        VideoFilePath += name;
        videoPlayer.clip = Resources.Load<VideoClip>(VideoFilePath);
        //Check if the clip is loaded or not
        if(videoPlayer.clip != null)
        {
            Debug.Log("The Video clip is prepared, the loaded video is " + VideoFilePath);
            PlayVideo();
        }
        else Debug.LogError("The Video clip is NOT prepared");
    }
    private void ClickToSkip()
    {
        SkipVideo(videoPlayer);
    }
    private void PlayVideo()
    {
        IsVideoPlaying = true;
        videoPlayer.Play();
        uiCanvas.enabled = false;
    }
    private void SkipVideo(VideoPlayer vp)
    {
        videoPlayer.Stop();
        uiCanvas.enabled = true;
        IsVideoPlaying = false;
    }
}
