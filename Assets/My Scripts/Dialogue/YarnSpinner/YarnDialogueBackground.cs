using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class YarnDialogueBackground : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    
    [YarnCommand("UseImage")]
    public void UseImage(string imageName)
    {
        if (backgroundImage == null)
        {
            Debug.LogError("Background Image component is not assigned!");
            return;
        }
    
        // Try to load the sprite from Resources/background folder
        Sprite sprite = Resources.Load<Sprite>("background/" + imageName);
        
        // If not found, try without the folder prefix (in case full path was provided)
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>(imageName);
        }
        
        if (sprite == null)
        {
            Debug.LogWarning($"Could not find sprite with name '{imageName}'");
            return;
        }
        
        // Set the new background image
        backgroundImage.sprite = sprite;
    }
}
