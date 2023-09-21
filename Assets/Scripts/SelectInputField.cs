using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectInputField : MonoBehaviour, ISelectHandler
{
    [SerializeField] private InputField inputField;
    
    public void OnSelect(BaseEventData eventData)
    {
        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }
}
