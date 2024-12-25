using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamBuilderChip : MonoBehaviour, IChip
{
    public ExpansionChipSlot expansionChipSlot { get; set; }
    public ExpansionChipStatus expansionChipStatus { get; set; }
    public bool isBuffActive { get; set; }


    [Header("Dream Builder Chip Buff")]
    public float dreamBuilderPlatformDuration = 5f;
    public float dreamBuilderPlatformCooldown = 8f;
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();

        // Sent Buff Data to Expansion Chip Status script
        ExpansionChipStatus.instance.dreamBuilderPlatformDuration = dreamBuilderPlatformDuration;
        expansionChipStatus.dreamBuilderPlatformCooldown = dreamBuilderPlatformCooldown;
        expansionChipStatus.dreamBuilderPlatformCurrentCooldown = 0;
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
        expansionChipStatus.isDreamBuilderChipEquipped = true;
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        expansionChipStatus.isDreamBuilderChipEquipped = false;
        isBuffActive = false;
    }
}
