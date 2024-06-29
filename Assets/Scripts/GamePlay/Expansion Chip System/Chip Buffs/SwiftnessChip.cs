using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiftnessChip : MonoBehaviour, IChip
{
    /*
        Decrease 10% of required SP, reduce 40% Ultimate Cooldown
    */
    public ExpansionChipSlot expansionChipSlot { get; set; }
    public ExpansionChipStatus expansionChipStatus { get; set; }
    public bool isBuffActive { get; set; }
    [SerializeField] private int reduceCooldownValue = 40;
    public static int SwitfChipReduceCooldownValue;

    // Start is called before the first frame update
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();

        SwitfChipReduceCooldownValue = reduceCooldownValue;
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
        expansionChipStatus.isSwiftnessChipEquipped = true;
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        expansionChipStatus.isSwiftnessChipEquipped = false;
        isBuffActive = false;
    }
}
