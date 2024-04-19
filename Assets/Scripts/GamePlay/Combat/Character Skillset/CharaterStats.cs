using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    public int _maxHealth { get { return maxHealth; } }
    #region Movement
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 10f;
    public float Speed { get { return speed; } }
    public float JumpForce { get { return jumpForce; } }
    public float DashForce { get { return dashForce; } }

    #endregion
}
