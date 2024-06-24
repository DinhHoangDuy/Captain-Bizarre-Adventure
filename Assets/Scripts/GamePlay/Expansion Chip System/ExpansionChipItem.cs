using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class ExpansionChipItem : MonoBehaviour
{
    [SerializeField] private ExpansionChipSO chipData;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        spriteRenderer.sprite = chipData.chipIcon;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is collecting the chip: " + chipData.chipName);
            ExpansionChipManager.instance.UnlockChip(chipData);
            Destroy(gameObject);
        }
    }  
}
