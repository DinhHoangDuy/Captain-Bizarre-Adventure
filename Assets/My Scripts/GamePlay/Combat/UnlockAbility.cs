using System;
using UnityEngine;

// This script is attached to the unlockable skills GameObject in the game, which will be dropped after a certain condition is met (defeating a boss, found on the ground, etc.)
public class UnlockAbility : MonoBehaviour, IDataPersistence
{
    private string skillNameID;
    [SerializeField] private SkillToUnlock skillToUnlock;
    private CaptainUnlockableSkillStatus skillUnlockStatus;
    [SerializeField] private ParticleSystem particleSystem;
    private bool isCollected = false;

    void Awake()
    {
        skillUnlockStatus = FindAnyObjectByType<CaptainUnlockableSkillStatus>();
        // Get the name of the skill to unlock
        skillNameID = Enum.GetName(typeof(SkillToUnlock), skillToUnlock);
        if (skillUnlockStatus == null)
        {
            Debug.LogError("Character Skill Set is null");
        }
    }
    
    // If one of the character skill is already unlocked and applied, self-destroy immediately
    private void Start()
    {
        if (skillToUnlock == SkillToUnlock.DoubleJump && skillUnlockStatus.DoubleJumpActive)
        {
            Destroy(gameObject);
        }
        else if (skillToUnlock == SkillToUnlock.WallJump && skillUnlockStatus.WallJumpActive)
        {
            Destroy(gameObject);
        }
        else if (skillToUnlock == SkillToUnlock.Dash && skillUnlockStatus.DashActive)
        {
            Destroy(gameObject);
        }
        else if (skillToUnlock == SkillToUnlock.Ultimate && skillUnlockStatus.UltimateActive)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UnlockSkill(skillToUnlock);
            Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity);

            // Inactivate the game object
            gameObject.SetActive(false);
            isCollected = true;
        }
    }

    public void UnlockSkill(SkillToUnlock skill)
    {
        switch (skill)
        {
            case SkillToUnlock.Dash:
                skillUnlockStatus.DashActive = true;
                break;
            case SkillToUnlock.WallJump:
                skillUnlockStatus.WallJumpActive = true;
                break;
            case SkillToUnlock.DoubleJump:
                skillUnlockStatus.DoubleJumpActive = true;
                break;
            case SkillToUnlock.Ultimate:
                skillUnlockStatus.UltimateActive = true;
                break;
        }
    }

    public void LoadData(GameData data)
    {
        data.unlockedSkills.TryGetValue(skillNameID, out isCollected);
        if(isCollected)
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.unlockedSkills.ContainsKey(skillNameID))
        {
            data.unlockedSkills.Remove(skillNameID);
        }
        data.unlockedSkills.Add(skillNameID, isCollected);
    }
}

public enum SkillToUnlock
{
    Ultimate,
    DoubleJump,
    WallJump,
    Dash
}
