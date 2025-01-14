using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortitudeChip : MonoBehaviour, IChip
{

    public bool isBuffActive { get; set; }
    [HideInInspector] public ExpansionChipSlot expansionChipSlot {get; set;}
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
        ExpansionChipStatus.instance.isFortitudeChipEquipped = true;
        isBuffActive = true;
    }

    public void RemoveBuff()
    {
        Debug.Log("Removing Buff: Fortitude Chip");
        ExpansionChipStatus.instance.isFortitudeChipEquipped = false;
        isBuffActive = false;
    }
}
