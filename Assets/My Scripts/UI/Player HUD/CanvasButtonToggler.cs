using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasButtonToggler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button buttonToToggle;
    
    private void Start()
{
        // Ensure button starts deactivated
        if (buttonToToggle != null)
            buttonToToggle.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Toggle button visibility when canvas is clicked
        if (buttonToToggle != null)
            buttonToToggle.gameObject.SetActive(!buttonToToggle.gameObject.activeSelf);
    }
}