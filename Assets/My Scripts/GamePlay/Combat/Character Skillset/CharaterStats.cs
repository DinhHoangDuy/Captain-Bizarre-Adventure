using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStats : MonoBehaviour, IDataPersistence
{
    public int maxHealth;
    #region Movement
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float dashForce = 10f;
    [Tooltip("Character moving acceleration")] public float acceleration;
    [Tooltip("Character moving deceleration")] public float deceleration;
    public float velocityPower;

    public float MoveSpeed { get { return moveSpeed; } }
    public float JumpForce { get { return jumpForce; } }
    public float WallJumpForce { get { return wallJumpForce; } }
    public float WallSlideSpeed { get { return wallSlideSpeed; } }
    public float GravityScale { get { return gravityScale; } }
    public float DashForce { get { return dashForce; } }
    #endregion

    #region Healing
    [Header("Healing")]
    [SerializeField] private int potionHealAmount = 3;
    [SerializeField] private int requiredSPForHeal = 30;
    public int _potionHealAmount { get { return potionHealAmount; } }
    public int _requiredSPForHeal { get { return requiredSPForHeal; } }

    #endregion


    #region Save and Load System
    public void LoadData(GameData data)
    {
        this.maxHealth = data.currentMaxHealth;
    }

    public void SaveData(ref GameData data)
    {
        data.currentMaxHealth = this.maxHealth;
    }
    #endregion
}
