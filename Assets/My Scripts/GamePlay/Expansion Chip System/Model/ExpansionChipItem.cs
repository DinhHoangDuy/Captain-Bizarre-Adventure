using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ExpansionChipItem : MonoBehaviour, IDataPersistence
{
    [SerializeField] public ExpansionChipSO chipSO;
    [SerializeField] private ParticleSystem particleSystem;
    public bool isUnlocked = false;
    private void Start()
    {
        // Defensively check if the chipSO is set
        if (chipSO == null)
        {
            Debug.LogError("chipSO is not set on " + gameObject.name);
            return;
        }
        // Set the chip icon in the Object
        // spriteRenderer.sprite = chipSO.chipIcon;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is collecting the chip: " + chipSO.chipName);
            Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity);
            ExpansionChipManager.instance.UnlockChip(chipSO);
            isUnlocked = true;
            DeavtivateItem();
        }
    }
    private void DeavtivateItem()
    {
        // gameObject.SetActive(false);
        // deactivate the sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        // deactivate the all colliders attached to the object
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }       

    }

    #region Save and Load Data
    public void LoadData(GameData data)
    {
        // throw new System.NotImplementedException();
        data.unlockedChips.TryGetValue(chipSO.chipName, out isUnlocked);
        if (isUnlocked)
        {
            DeavtivateItem();
        }
    }

    public void SaveData(ref GameData data)
    {
        // throw new System.NotImplementedException();
        if (data.unlockedChips.ContainsKey(chipSO.chipName))
        {
            data.unlockedChips.Remove(chipSO.chipName);
        }
        data.unlockedChips.Add(chipSO.chipName, isUnlocked);
    }
    #endregion
}
