using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ExpansionChipItem : MonoBehaviour, IDataPersistence
{
    [SerializeField] private ExpansionChipSO chipData;
    [SerializeField] private ParticleSystem particleSystem;
    // private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        // spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        // Defensively check if the chipData is set
        if (chipData == null)
        {
            Debug.LogError("chipData is not set on " + gameObject.name);
            return;
        }
        // Set the chip icon in the Object
        // spriteRenderer.sprite = chipData.chipIcon;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is collecting the chip: " + chipData.chipName);
            Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity);
            ExpansionChipManager.instance.UnlockChip(chipData);
            Destroy(gameObject);
        }
    }

    #region Save and Load Data
    public void LoadData(GameData data)
    {
        // throw new System.NotImplementedException();
        data.unlockedChips.TryGetValue(chipData.chipName, out bool isUnlocked);
        if (isUnlocked)
        {
            Destroy(gameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        // throw new System.NotImplementedException();
    }
    #endregion
}
