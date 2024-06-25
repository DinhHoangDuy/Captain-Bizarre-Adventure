using UnityEngine;

public class PlayerSP : MonoBehaviour
{
    [SerializeField] private SPBar spBar;
    private CaptainMoonBlade skillset;
    private float currentSP;
    public float _currentSP => currentSP;
    private int maxSP;
    public int _maxSP => maxSP;

    private void Start()
    {
        skillset = GetComponent<CaptainMoonBlade>();
        
        currentSP = (int)skillset.currentSP;
        maxSP = skillset._maxSP;
    }
    private void Update()
    {
        currentSP = skillset.currentSP;

        if(currentSP >= maxSP)
            currentSP = maxSP;

        spBar.SetSP(currentSP, maxSP);
    }

    public void IncreaseSPByPersent(int percent)
    {
        skillset.currentSP += maxSP * (percent / 100.0f);
        Debug.Log("SP increased by " + maxSP * (percent / 100.0f));
    }
    public void IncreaseSPByValue(int value)
    {
        skillset.currentSP += value;
        Debug.Log("SP increased by " + value);
    }
}