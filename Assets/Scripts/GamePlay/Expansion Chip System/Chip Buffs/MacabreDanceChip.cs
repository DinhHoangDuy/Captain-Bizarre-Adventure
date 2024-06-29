using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacabreDanceChip : MonoBehaviour, IChip
{
    /*
        Killing enemies resets Ultimate CD. The next Ultimate will have 30% Total DMG Boost
    */
    [Header("Macabre Dance Chip Buff")]
    public float totalDMGBoost = 30f;
    public ExpansionChipSlot expansionChipSlot { get; set; }
    public ExpansionChipStatus expansionChipStatus { get; set; }
    public bool isBuffActive { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();
        ExpansionChipStatus.instance.MacabreDanceTotalDMGBoost = totalDMGBoost;
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
        expansionChipStatus.isMacabreDanceChipEquipped = true;
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        expansionChipStatus.isMacabreDanceChipEquipped = false;
        isBuffActive = false;
    }
    
}
