using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 5;
    #region Movement
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float dashForce = 10f;
    public float MoveSpeed { get { return moveSpeed; } }
    public float JumpForce { get { return jumpForce; } }
    public int ExtraJumps { get { return extraJumps; } }
    public float WallJumpForce { get { return wallJumpForce; } }
    public float WallSlideSpeed { get { return wallSlideSpeed; } }
    public float GravityScale { get { return gravityScale; } }
    public float DashForce { get { return dashForce; } }

    #endregion
}
