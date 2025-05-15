using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Yarn.Unity;

public class YarnDialogueBackground : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoCanvas;

    void Start()
    {
        if (videoCanvas == null)
        {
            Debug.LogError("Video Canvas component is not assigned!");
            return;
        }
        else videoCanvas.gameObject.SetActive(false);
    }

    [YarnCommand("UseImage")]
    public void UseImage(string imageName)
    {
        if (backgroundImage == null)
        {
            Debug.LogError("Background Image component is not assigned!");
            return;
        }

        // Try to load the sprite from Resources/background folder
        Sprite sprite = Resources.Load<Sprite>("Background/" + imageName);

        // If not found, try without the folder prefix (in case full path was provided)
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>(imageName);
        }

        if (sprite == null)
        {
            Debug.LogWarning($"Could not find sprite with name '{imageName}'");
            return;
        }

        // Set the new background image
        backgroundImage.sprite = sprite;
    }
    #region Video Player
    [YarnCommand("PlayVideo")]
    public void PlayVideo(string videoName)
    {
        videoCanvas.SetActive(true);
        if (!videoCanvas.activeSelf)
        {
            Debug.LogError("Video Canvas is not active!");
            return;
        }
        if (videoPlayer == null)
        {
            Debug.LogError("Video Player component is not assigned!");
            return;
        }
        // Try to load the video clip from Resources/background folder
        VideoClip videoClip = Resources.Load<VideoClip>("Video/" + videoName);

        // If not found, try without the folder prefix (in case full path was provided)
        if (videoClip == null)
        {
            videoClip = Resources.Load<VideoClip>(videoName);
        }

        if (videoClip == null)
        {
            Debug.LogWarning($"Could not find video clip with name '{videoName}'");
            return;
        }
        // Register the event handler to deactivate canvas when video ends
        videoPlayer.loopPointReached += OnVideoFinished;
        // Set the new video clip
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
    }
    // This method will be called when the video finishes playing
    private void OnVideoFinished(VideoPlayer player)
    {
        // Deactivate the video canvas
        videoCanvas.SetActive(false);
        videoPlayer.clip = null; // Clear the video clip

        // Unsubscribe from the event to prevent memory leaks
        // (especially important if the video might be played multiple times)
        player.loopPointReached -= OnVideoFinished;

        Debug.Log("Video finished playing, canvas deactivated");
    }
    #endregion
}
