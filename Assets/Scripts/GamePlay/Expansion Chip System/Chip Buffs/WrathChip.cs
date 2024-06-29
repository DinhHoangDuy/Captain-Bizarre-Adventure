using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathChip : MonoBehaviour, IChip
{   
    /*
        If passive "Unbreakable Will" is active, Captain deals 20% bonus Crit DMG.
    */
    [Header("Wrath Chip Buff")]
    public float critDMGBuffValue = 20f;

    public ExpansionChipSlot expansionChipSlot { get; set; }
    public ExpansionChipStatus expansionChipStatus { get; set; }
    public bool isBuffActive { get; set; }
    private CaptainMoonBlade skillset;
    // Start is called before the first frame update
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();
        skillset = GameObject.FindGameObjectWithTag("Player").GetComponent<CaptainMoonBlade>();
        skillset.WarthCritDMGBuffValue = critDMGBuffValue;
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
        expansionChipStatus.isWarthChipEquipped = true;
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        expansionChipStatus.isWarthChipEquipped = false;
        isBuffActive = false;
    }    
}
