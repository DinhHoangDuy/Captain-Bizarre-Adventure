using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateIcon : MonoBehaviour
{
    [SerializeField] private Image UltimateIconBackground;
    [SerializeField] private Image UltimateFill;
    [SerializeField] private TextMeshProUGUI RequiredSPText;
    private CaptainMoonBlade skillset;


    private void Start()
    {
        skillset = GameObject.FindGameObjectWithTag("Player").GetComponent<CaptainMoonBlade>();
    }
    private void Update()
    {
        UpdateRequiredSP(skillset._requiredSP);
    }

    public void UpdateCooldownTime(float currentTime, float maxTime)
    {
        if(currentTime <= 0)
        {
            UltimateFill.enabled = false;
            return;
        }
        else
        {
            UltimateFill.enabled = true;
            UltimateFill.fillAmount = 1 - currentTime / maxTime;
        }
    }

    private Color32 UltimateReadyColor = new Color32(255, 255, 255, 255);
    private Color32 UltimateNotReadyColor = new Color32(150, 150, 150, 255);
    public void DisplayUltimateReady()
    {
        UltimateIconBackground.color = UltimateReadyColor;
    }
    public void DisplayUltimateNotReady()
    {
        UltimateIconBackground.color = UltimateNotReadyColor;
    }
    public void UpdateRequiredSP(int requiredSP)
    {
        RequiredSPText.text = requiredSP.ToString();
    }
}
