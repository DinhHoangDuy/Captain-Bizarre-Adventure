using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";
    public bool profileHasData = false;

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI lastLocationSavedText;
    [SerializeField] private TextMeshProUGUI percentageCompleteText;

    private Button saveSlotButton;

    private void Awake() 
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data) 
    {
        // there's no data for this profileId
        if (data == null) 
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            profileHasData = false;
        }
        // there is data for this profileId
        else 
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            profileHasData = true;

            lastLocationSavedText.text = data.lastLocationSaved;
            percentageCompleteText.text = "To be continued...";
        }
    }

    public string GetProfileId() 
    {
        return this.profileId;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
}