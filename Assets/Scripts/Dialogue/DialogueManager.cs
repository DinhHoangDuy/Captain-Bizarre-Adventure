using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    //[SerializeField] private GameObject continueIcon;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float continueDelay = 0.05f;

    [Header("Show/Hide Dialogue Button")]
    [SerializeField] private GameObject showHideObj;
    [SerializeField] private GameObject dialogueBox;
    private GameObject dialoguePanel;
    private bool DialogueHidden = false;
    private Button showHideButton;
    private TextMeshProUGUI showHideText;
    [Header("Skip Dialogue Button")]
    [SerializeField] private Button skipDialogueButton;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Background and Portrait Animation")]
    [SerializeField] private Animator backgroundAnimator;
    private string portraitCharacter;
    [SerializeField] private Animator portraitLeftAnimator;
    [SerializeField] private Animator portraitCenterAnimator;
    [SerializeField] private Animator portraitRightAnimator;
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string PORTRAITLOCATION_TAG = "portrait location";
    private const string CLEAR_TAG = "clear";
    private const string BACKGROUND_TAG = "background";

    [Header("Background music")]
    [SerializeField] private AudioSource BGM;
    private const string BGMPath = "Musics/BGM/";
    private const string BGM_TAG = "BGM";
    private string BGMFilePath = "";
    [Header("Voice acting")]
    [SerializeField] private AudioSource voiceActing;
    private const string VoicePath = "Music/VoiceActing/";    
    private const string VOICE_TAG = "voice";
    private const string VOICESOURCE_TAG = "voiceSource";

    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button skipVideoButton;
    [SerializeField] private float autoHideDelay = 3f;
    private bool skipVideoButtonShowed = false;
    private Animator skipVideoButtonAnimator;    
    private const string VideoPath = "Videos/";
    private const string VIDEO_TAG = "video";
    private string VideoFilePath = "";
    private bool IsVideoPlaying = false;   

    private Coroutine skipButtonCoroutine;


    //Others
    private Coroutine displayLineCoroutine;
    private bool CanContinueToNextLine = false;
    private bool isAddingRichTextTag = false;

    //Singeton pattern setup
    private Story currentStory;
    public bool DialogueIsPlaying { get; private set; }

    private static DialogueManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        //Make sure the character can still move when this script is active
        DialogueIsPlaying = false;
        PlatformerMovement2D.blocked = false;

        //Gain access to the Dialogue Panel, which is the parrent of the Dialogue Box
        dialoguePanel = dialogueBox.transform.parent.gameObject;        

        //Initiate Show/Hide Dialogue Button
        showHideButton = showHideObj.GetComponent<Button>();
        showHideButton.onClick.AddListener(ShowHideDialogue);
        showHideText = showHideObj.GetComponent<TextMeshProUGUI>();

        //Initiate Skip Buttons
        skipDialogueButton.onClick.AddListener(SkipDialogue);
        skipVideoButtonAnimator = skipVideoButton.gameObject.GetComponent<Animator>();
        skipVideoButton.onClick.AddListener(SkipVideo);
        skipVideoButton.gameObject.SetActive(false);

        //Initiate Background music source
        BGM = BGM.GetComponent<AudioSource>();
        //Setup video player
        videoPlayer.loopPointReached += CloseVideo;

        //Get all of the choices text 
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        //Make it touch-ready
        Button PanelClick = dialogueBox.GetComponent<Button>();
        PanelClick.onClick.AddListener(ClickToContinue);
        
    }

    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!DialogueIsPlaying)
        {
            PlatformerMovement2D.blocked = false;
            return;
        }
        else
        {
            PlatformerMovement2D.blocked = true;        
        }

        if(Input.GetButtonDown("Submit"))
        //if(InputManager.GetInstance().GetSubmitPressed())
        {
            if(!DialogueHidden)
            {
                ClickToContinue();
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                ShowHideDialogue();
                EventSystem.current.SetSelectedGameObject(null);
            }
        }       
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        DialogueIsPlaying = true;
        Debug.Log("The Dialogue trigger is used");

        //Set default Dialogue portrait and background to check for startup error (Mostly related to the Ink file)
        characterName.text = "???";
        portraitLeftAnimator.Play("default");
        portraitCenterAnimator.Play("default");
        portraitRightAnimator.Play("default");
        backgroundAnimator.Play("default");

        ContinueStory();
    }
    private IEnumerator ExitDialogueMode()
    {
        Debug.Log("The story has ended");
        yield return new WaitForSeconds(0.2f);
        DialogueIsPlaying = false;    
        dialogueText.text = "";
        ExternalDialogueManager.GetInstance().ExitDialogueMode();
    }
    private void ClickToContinue()
    {
        if (CanContinueToNextLine)
        {
            if (currentStory.currentChoices.Count == 0)
            {
                ContinueStory();
            }     
        }
        else if (IsVideoPlaying)
        {
            HandleSkipVideoButton();
        }
        else
        {
            Debug.Log("Can't continue because CanContinueToNextLine == " + CanContinueToNextLine);
        }
    }
    private void ContinueStory()
    {
        if (currentStory.canContinue && videoPlayer.clip == null)
        {
            //Initiate Coroutine for typing effect
            if(displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            //Handle tags
            HandleTag(currentStory.currentTags);
        }
        else if (videoPlayer.clip != null
            && !IsVideoPlaying)
        {
            StartVideo();
        }        
        else
        {            
            StartCoroutine(ExitDialogueMode());
        }
    }

    //========Animation Handling via Ink Tags========
    private void HandleTag(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            //parse tags
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be parsed" + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                //Character name text
                case SPEAKER_TAG:
                    characterName.text = tagValue switch
                    {
                        "Captain" => "Thuyền Trưởng",
                        "Narrator" => "",
                        _ => tagValue,
                    };
                    break;
                //Hide all portraits if the dialogue doesn't want anyone
                case CLEAR_TAG:
                    if (tagValue == "yes")
                    {
                        portraitLeftAnimator.Play("none");
                        portraitRightAnimator.Play("none");
                        portraitCenterAnimator.Play("none");
                    }
                    break;
                case PORTRAIT_TAG:
                    //Bring the portraitCharacter variable to the Location tag handler
                    portraitCharacter = tagValue; 
                    break;
                case PORTRAITLOCATION_TAG:
                    //portraitCharacter from PORTRAIT_TAG is now used to change portrait to desired character
                    switch (tagValue)
                    {
                        case "left":
                            portraitCenterAnimator.Play("none");
                            portraitLeftAnimator.Play(portraitCharacter);
                            break;
                        case "right":
                            portraitCenterAnimator.Play("none");
                            portraitRightAnimator.Play(portraitCharacter);
                            break;
                        case "center":
                            portraitLeftAnimator.Play("none");
                            portraitRightAnimator.Play("none");
                            portraitCenterAnimator.Play(portraitCharacter);
                            break;
                    }                    
                    break;
                    //Change the background of the dialogue
                case BACKGROUND_TAG:
                    backgroundAnimator.Play(tagValue);
                    break;

                //Background Music
                case BGM_TAG:
                    if (tagValue != "stop")
                    {
                        BGMFilePath = BGMPath + tagValue;
                        PlayBGM(BGMFilePath);
                    }
                    else if (tagValue == "stop" || tagValue == "none")
                    {
                        StopBGM();
                        Debug.Log("Audio Stopped because the current BGM_TAG is " + tagValue);
                    }
                    else
                    {
                        Debug.LogError("Wrong BGM Value: " + tagValue +
                            ". It should be a real file name or 'stop' to stop the background music");
                    }
                    break;

                //VoiceActing
                /*
                case VOICESOURCE_TAG:
                    //Debug.Log("voiceSource: " + tagValue);
                    break;
                */
                
                //Video player
                case VIDEO_TAG:
                    VideoFilePath = VideoPath + tagValue;
                    ReadyToPlayVideo(VideoFilePath);                    
                    break;
                default:
                    Debug.LogWarning("Tag came in but not handled yet:" + tag);
                    break;
            }
        }
    }

    //========Typing Effect========
    private IEnumerator DisplayLine(string line)
    {
        //continueIcon.SetActive(false);
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        HideChoices();
        CanContinueToNextLine = false;

        foreach (char letter in line.ToCharArray())
        {
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if(letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        yield return new WaitForSeconds(continueDelay);
        //continueIcon.SetActive(true);
        DisplayChoices();
        CanContinueToNextLine = true;
    }
    //========Dialogue Choices========
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach (Choice choice in currentChoices)
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    public void MakeChoice(int choiceIndex)
    {
        if (CanContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
    }
    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0]);
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }
    //========Show and Hide Dialogue Panel========
    private void ShowHideDialogue()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (DialogueHidden)
        {
            CanContinueToNextLine = true;
            dialogueBox.SetActive(true);
            showHideText.text = "Ẩn";
        }
        else if (!DialogueHidden)
        {
            CanContinueToNextLine = false;
            dialogueBox.SetActive(false);
            showHideText.text = "Hiện";
        }
        DialogueHidden = !DialogueHidden;
    }


    private void SkipDialogue()
    {
        StartCoroutine(ExitDialogueMode());
    }

    //========Music and Video========
    private void PlayBGM(string BGMFilePath)
    {
        Debug.Log("Music file path: " + BGMFilePath);
        BGM.clip = Resources.Load<AudioClip>(BGMFilePath);
        BGM.Play();
    }
    private void StopBGM()
    {
        BGM.Stop();
    }

    private void ReadyToPlayVideo(string VideoFilePath)
    {
        //Initiate the Video clip        
        Debug.Log("Video file path: " + VideoFilePath);
        videoPlayer.clip = Resources.Load<VideoClip>(VideoFilePath);
        if (videoPlayer.clip == null)
        {
            Debug.LogError("The video is NOT prepared to run!");
        }
        else
        {
            Debug.Log("The video is prepared to run!");
        }
    }
    private void StartVideo()
    {
        BGM.Stop();
        IsVideoPlaying = true;
        CanContinueToNextLine = false;
        //Hide Everything
        dialoguePanel.SetActive(false);
        backgroundAnimator.gameObject.SetActive(false);
        showHideObj.SetActive(false);
        skipDialogueButton.gameObject.SetActive(false);
        skipVideoButton.gameObject.SetActive(true);
        videoPlayer.Play();
        Debug.Log("The video is Playing");
    }
    private void CloseVideo(VideoPlayer vp)
    {
        videoPlayer.Stop();
        videoPlayer.clip = null;
        IsVideoPlaying = false;
        CanContinueToNextLine = true;
        
        //Auto move to the next line or exit the dialogue when it ends after the video
        if (currentStory.canContinue)
            {
                //Show Everything belongs to the dialogue system
                dialoguePanel.SetActive(true);
                backgroundAnimator.gameObject.SetActive(true);
                showHideObj.SetActive(true);
                skipDialogueButton.gameObject.SetActive(true);
                skipVideoButton.gameObject.SetActive(false);
                //Auto continue to next line
                ContinueStory();
            }
        else
            {
                StartCoroutine(ExitDialogueMode());
            }

    }    
    private void HandleSkipVideoButton()
    {
        if (skipButtonCoroutine != null)
        {
            StopCoroutine(skipButtonCoroutine);
        }

        if (skipVideoButtonShowed)
        {
            skipVideoButtonAnimator.SetBool("IsShowed", false);
            StopCoroutine(skipButtonCoroutine);
        }
        else
        {
            skipVideoButtonAnimator.SetBool("IsShowed", true);            
            skipButtonCoroutine = StartCoroutine(AutoHideSkipVideoButton());
        }
        skipVideoButtonShowed = !skipVideoButtonShowed;
    }
    private IEnumerator AutoHideSkipVideoButton()
    {
        yield return new WaitForSeconds(autoHideDelay);
        skipVideoButtonAnimator.SetBool("IsShowed", false);
    }
    private void SkipVideo()
    {
        CloseVideo(videoPlayer);
    }
}