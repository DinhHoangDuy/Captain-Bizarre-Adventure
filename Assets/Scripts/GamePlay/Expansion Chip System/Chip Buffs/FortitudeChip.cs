using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class FortitudeChip : MonoBehaviour, IChip
{
    [Header("Buff Value")]
    [Tooltip("This value will be added to character's max health")]
    public int buffValue = 1;

    public bool isBuffActive { get; set; }
        [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}
    public ExpansionChipStatus expansionChipStatus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
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
        Debug.Log("Applying Buff: Fortitude Chip");
        playerHealth.maxHealth += buffValue;
        Debug.Log("Current Max Health: " + playerHealth.maxHealth);
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Fortitude Chip");
        playerHealth.maxHealth -= buffValue;
        Debug.Log("Current Max Health: " + playerHealth.maxHealth);
        isBuffActive = false;
    }
}
