using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ExpansionChipSO : ScriptableObject
{
    public string chipName;
    [TextArea]
    public string chipDescription;
    public Sprite chipIcon;
    public int chipLoad;
}
