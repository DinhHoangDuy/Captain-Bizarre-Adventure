using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyOfBloodMoonChip : MonoBehaviour, IChip
{
    public bool isBuffActive { get; set; }
    [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}


    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
    }
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
        ExpansionChipStatus.instance.isKeyOfBloodMoonEquipped = true;
        Debug.Log("Key of Blood Moon Chip is active: " + ExpansionChipStatus.instance.isKeyOfBloodMoonEquipped.ToString() + ")");
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Key of Blood Moon Chip");
        ExpansionChipStatus.instance.isKeyOfBloodMoonEquipped = false;
        Debug.Log("Key of Blood Moon Chip is active: " + ExpansionChipStatus.instance.isKeyOfBloodMoonEquipped.ToString() + ")");
        isBuffActive = false;
    }    
}
