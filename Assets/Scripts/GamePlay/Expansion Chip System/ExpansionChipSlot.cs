using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpansionChipSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Chip Variables on the inspector")]
    public TextMeshProUGUI chipNameUIText;
    public Image chipIconUIImage;
    public Image selectedShader;
    public Image equippedShader;
    public Image lockedShader;

    [Header("Add Chip Name, Chip Icon Image and Chip Description from the Description Panel here")]
    [SerializeField] private Image chipIconDescription;
    [SerializeField] private TextMeshProUGUI  chipNameDescriptionPanel;
    [SerializeField] private TextMeshProUGUI chipDescriptionText;

    // Chip Data
    [HideInInspector] public ExpansionChipSO chipData;
    [HideInInspector] public string chipNameData;
    [HideInInspector] public string chipDescriptionData;
    private Sprite chipIconData;
    [HideInInspector] public int chipLoadData;
    public bool isLocked;
    [HideInInspector] public bool isEquipped = false;
    [HideInInspector] public bool isSelected = false;

    // Dependencies
    private ExpansionChipManager expansionChipManager;

    void Awake()
    {
        expansionChipManager = GameObject.Find("/Player UI").GetComponent<ExpansionChipManager>();
    }

    void Start()
    {
        if (chipData == null)
        {
            isLocked = true;
            chipIconData = expansionChipManager.blankChipIcon;
            return;
        }

        // Obtain Chip Data, ready to be used in the UI
        chipNameData = chipData.chipName;
        chipDescriptionData = chipData.chipDescription;
        chipIconData = chipData.chipIcon;
        chipLoadData = chipData.chipLoad;

        // Set Chip Name in the UI
        chipNameUIText.text = chipNameData;

        // Set the chip icon in the UI
        chipIconUIImage.sprite = chipIconData;

        // Lock the chip
        isLocked = true;
    }
    void Update()
    {
        // Check if the chip is locked
        if (isLocked)
        {
            // Set the chip icon to gray if it is locked
            chipIconUIImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            lockedShader.gameObject.SetActive(true);
        }
        else
        {
            chipIconUIImage.color = new Color(1f, 1f, 1f, 1f);
            lockedShader.gameObject.SetActive(false);
        }
        // Check if the chip is selected
        if (isSelected)
        {
            selectedShader.gameObject.SetActive(true);
        }
        else
        {
            selectedShader.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }
    void OnLeftClick()
    {
        Debug.Log("Left Clicked on " + gameObject.name);
        if(!isSelected)
        {
            expansionChipManager.DeselectAllSlots();
            isSelected = true;

            // Sent data to the Description Panel
            chipNameDescriptionPanel.text = chipNameData;
            chipIconDescription.sprite = chipIconData;
            chipDescriptionText.text = chipDescriptionData;
        }
        else
        {
            isSelected = false;
            expansionChipManager.DeleteDescription();
        }
    }
}
