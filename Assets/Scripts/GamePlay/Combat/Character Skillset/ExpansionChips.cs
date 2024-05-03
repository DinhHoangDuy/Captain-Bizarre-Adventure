using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpansionChips : MonoBehaviour
{
    // This list contains some ideas about the equipments which will affect how the Captain will action in the game.
    // This function will kinda replicate how it works in Hollow Knight or Honkai Impact APHO 2 Expansion Chips system.
    // An equipment will have some attributes like:
    // - Name
    // - Description
    // - Weight: Captain only support a certain amount of weight. Captain will not allow to equip an equipment if its weight is over the current weight left of the Captain.
    // - Effect: The effect of the equipment when it is equiped.
    // Max weight of the Captain: 10

    // List of equipments:
    // 1. Sweet Snacks:
    //     - When equiped: Captain will gain SP after a certain amount of time.
    //     - Weight: 1
    // 2. Sharp Blade:
    //     - When equiped: Captain gain 5% Total DMG boost
    //     - Weight: 1
    // 3. Bloodlust:
    //     - When equiped: Captain's basic attack have a 10% chance to heal 1 health point.
    //     - Weight: 2
    // 4. Blink:
    //     - When equiped: Captain's Dash will regenerat 5SP. This effect has 10 seconds cooldown.
    //     - Weight: 2
    //==============Negative Effect (This is for End game experience, where the player can choose to make the game harder by equiping this equipment)================
    // 1. Weak Body: 
    //     - When equiped: Captain's Total DMG boost reduces by 50%.
    //     - Weight: 0

    [SerializeField] private bool SweetSnacks = false;
    [SerializeField] private int SweetSnacksSPRegenRate = 4;
    [SerializeField] private int SweetSnacksSPRegenAmount = 1;
    

    [SerializeField] private bool SharpBlade = false;
    [SerializeField] private int SharpBladeDMGBoost = 5;
    [SerializeField] private bool Bloodlust = false;
    [SerializeField] private int BloodlustTriggerChance = 10;

    [SerializeField] private bool Blink = false;

    // Negative effect
    [SerializeField] private bool WeakBody = false;

    // Public Variables
    public bool _SweetSnacks { get { return SweetSnacks; } }
    public int _SweetSnacksSPRegenAmount { get { return SweetSnacksSPRegenAmount; } }
    public int _SweetSnacksSPRegenRate { get { return SweetSnacksSPRegenRate; } }

    public bool _SharpBlade { get { return SharpBlade; } }
    public int _SharpBladeDMGBoost { get { return SharpBladeDMGBoost; } }

    public bool _Bloodlust { get { return Bloodlust; } }
    public int _BloodlustTriggerChance { get { return BloodlustTriggerChance; } }

    public bool _Blink { get { return Blink; } }

    // Negative effect
    public bool _WeakBody { get { return WeakBody; } }
}
