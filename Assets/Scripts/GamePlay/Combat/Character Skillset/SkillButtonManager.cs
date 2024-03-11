using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private Button ultButton;
    [SerializeField] private Image ultCooldownImage;

    #endregion

    #region Script Dependencies
    private CaptainBloodMoonBlade captainBloodMoonBlade;
    private PlatformerMovement2D platformerMovement2D;
    #endregion
    private void Awake()
    {
        captainBloodMoonBlade = GetComponent<CaptainBloodMoonBlade>();
        platformerMovement2D = GetComponent<PlatformerMovement2D>();
    }

    private void Update()
    {
        #region Ultimate Cooldown Text
        if (captainBloodMoonBlade._currentUltimateCooldown > 0)
        {
            ultButton.interactable = false;
            ultCooldownImage.fillAmount += Time.deltaTime / captainBloodMoonBlade._ultimateCooldown;
        }
        else
        {
            ultButton.interactable = true;
            ultCooldownImage.fillAmount = 0;
        }
        #endregion
    }
}