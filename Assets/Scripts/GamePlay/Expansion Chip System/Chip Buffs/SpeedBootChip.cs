using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBootChip : MonoBehaviour, IChip
{
    /*
        Increase 15% Speed
    */
    [Header("Speed Buff Value")]
    [SerializeField] private float speedBuffValue = 15.0f;
    private float originalSpeed;
    private float speedDifference;

    public ExpansionChipSlot expansionChipSlot { get; set; }
    public ExpansionChipStatus expansionChipStatus { get; set; }
    public bool isBuffActive { get; set; }

    

    // Start is called before the first frame update
    void Awake()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
    }
    void Start()
    {
        originalSpeed = PlatformerMovement2D.instance.movespeed;
        speedDifference = originalSpeed * (speedBuffValue / 100);
        Debug.Log("Original Moving Speed is set: " + originalSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if(expansionChipSlot.isEquipped && !isBuffActive)
        {
            ApplyBuff();
        }
        else if(!expansionChipSlot.isEquipped && isBuffActive)
        {
            RemoveBuff();
        }
    }
    public void ApplyBuff()
    {
        PlatformerMovement2D.instance.movespeed += speedDifference;
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        PlatformerMovement2D.instance.movespeed = originalSpeed;
        isBuffActive = false;
    }
}
