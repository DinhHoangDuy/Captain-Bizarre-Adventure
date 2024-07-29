using System;
using UnityEngine;

public class UnlockAbility : MonoBehaviour, IDataPersistence
{
    private string skillNameID;
    [SerializeField] private SkillToUnlock skillToUnlock;
    private CharacterSkillSet characterSkillSet;
    [SerializeField] private ParticleSystem particleSystem;
    private bool isCollected = false;

    void Awake()
    {
        characterSkillSet = FindAnyObjectByType<CharacterSkillSet>();
    }
    
    void Start()
    {
        // Get the name of the skill to unlock
        skillNameID = Enum.GetName(typeof(SkillToUnlock), skillToUnlock);

        if (characterSkillSet == null)
        {
            Debug.LogError("Character Skill Set is null");
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
                characterSkillSet.DashActive = true;
                break;
            case SkillToUnlock.WallJump:
                characterSkillSet.WallJumpActive = true;
                break;
            case SkillToUnlock.DoubleJump:
                characterSkillSet.DoubleJumpActive = true;
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
    DoubleJump,
    WallJump,
    Dash
}
