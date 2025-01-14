using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathChip : MonoBehaviour, IChip
{   
    /*
        If passive "Unbreakable Will" is active, Captain has +20% Crit DMG.
    */


    public ExpansionChipSlot expansionChipSlot { get; set; }
    public bool isBuffActive { get; set; }
    private CaptainSkillSet skillset;
    // Start is called before the first frame update
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        skillset = GameObject.FindGameObjectWithTag("Player").GetComponent<CaptainSkillSet>();
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
        ExpansionChipStatus.instance.isWrathChipEquipped = true;
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        ExpansionChipStatus.instance.isWrathChipEquipped = false;
        isBuffActive = false;
    }    
}
