using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyOfBloodMoonChip : MonoBehaviour, IChip
{
    public bool isBuffActive { get; set; }
    [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}
    ExpansionChipStatus IChip.expansionChipStatus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private ExpansionChipStatus expansionChipStatus;

    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();    }

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
        Debug.Log("Applying Buff: Key of Blood Moon Chip");
        expansionChipStatus.isKeyOfBloodMoonEquipped = true;
        Debug.Log("Key of Blood Moon Chip is active: " + expansionChipStatus.isKeyOfBloodMoonEquipped.ToString() + ")");
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Key of Blood Moon Chip");
        expansionChipStatus.isKeyOfBloodMoonEquipped = false;
        Debug.Log("Key of Blood Moon Chip is active: " + expansionChipStatus.isKeyOfBloodMoonEquipped.ToString() + ")");
        isBuffActive = false;
    }    
}
