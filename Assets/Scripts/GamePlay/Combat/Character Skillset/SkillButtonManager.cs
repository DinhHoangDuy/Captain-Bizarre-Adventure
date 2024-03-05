using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private Button attackButton;
    [SerializeField] private Button dashButton;
    [SerializeField] private Button ultButton;
    [SerializeField] private TextMeshProUGUI ultCooldownText;
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
    private void Start()
    {
        attackButton.onClick.AddListener(TriggerAttack);
        ultButton.onClick.AddListener(TriggerUltimate);
    }

    private void Update()
    {
        #region Ultimate Cooldown Text
        if (captainBloodMoonBlade._currentUltimateCooldown > 0)
        {
            ultButton.interactable = false;
            ultCooldownText.text = captainBloodMoonBlade._currentUltimateCooldown.ToString("F1");
        }
        else
        {
            ultButton.interactable = true;
            ultCooldownText.text = "";
        }
        #endregion
    }

    private void TriggerAttack()
    {
        captainBloodMoonBlade.BasicAttack();
    }
    private void TriggerUltimate()
    {
        captainBloodMoonBlade.UltimateAttack();
    }
    private void TriggerDash()
    {
        
    }
}