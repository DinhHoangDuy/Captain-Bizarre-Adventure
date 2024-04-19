using UnityEngine;

public class PlayerSP : MonoBehaviour
{
    [SerializeField] private SPBar spBar;
    private CaptainMoonBlade skillset;
    private int currentSP;
    private int maxSP;

    private void Start()
    {
        skillset = GetComponent<CaptainMoonBlade>();
        
        currentSP = skillset._currentSP;
        maxSP = skillset._maxSP;
    }
    private void Update()
    {
        currentSP = skillset._currentSP;
        spBar.SetSP(currentSP, maxSP);
    }
}