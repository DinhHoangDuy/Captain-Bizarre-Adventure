using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SPBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI SPText;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxSP(int sp)
    {
        slider.maxValue = sp;
    }
    public void SetStartSP(int sp)
    {
        slider.value = sp;
    }
    public void SetSP(float sp)
    {
        slider.value = sp;
    }
    public void SetSPText(float sp)
    {
        SPText.text = sp + " / " + slider.maxValue;
    }
}
