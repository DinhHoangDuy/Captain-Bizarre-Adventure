using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SPBar : MonoBehaviour
{
    // Use a Slider instead of an Image to represent the SP bar
    private Slider spBar;
    [SerializeField] private TextMeshProUGUI spText; // Reference to the text displaying the SP value
    private void Start()
    {
        // This script is attached to the SP bar Game Object containing a Slider and a TextMeshProUGUI
        // Later on, the game object holding this script will be used and controlled by the PlayerSP script

        spBar = GetComponent<Slider>();
        // Defensive programming to make sure the SP bar is not null
        if (spBar == null)
        {
            Debug.LogError("SP Bar is null!!");
            return;
        }
        if(spText == null)
        {
            Debug.LogError("SP Text is null!!");
            return;
        }
    }

    public void SetSP(float currentSP, int maxSP)
    {
        // Set the SP value and the max SP value of the SP bar
        spBar.value = currentSP;
        spBar.maxValue = maxSP;
        spText.text = currentSP + "/" + maxSP;
    }
}
