using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerChip : MonoBehaviour, IChip
{
    public ExpansionChipSlot expansionChipSlot { get; set; }
    public ExpansionChipStatus expansionChipStatus { get; set; }
    public bool isBuffActive { get; set; }
    [SerializeField] private float buffValue = 40f;
    public static float hammerChipBuffValue;

    // Start is called before the first frame update
    void Start()
    {
        expansionChipSlot = GetComponent<ExpansionChipSlot>();
        expansionChipStatus = GameObject.Find("/Player UI").GetComponent<ExpansionChipStatus>();

        hammerChipBuffValue = buffValue;
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
        expansionChipStatus.isHammerChipEquipped = true;
        isBuffActive = true;
    }
    public void RemoveBuff()
    {
        expansionChipStatus.isHammerChipEquipped = false;
        isBuffActive = false;
    }
}
