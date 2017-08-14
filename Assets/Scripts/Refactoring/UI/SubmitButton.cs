using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SubmitButton : MonoBehaviour
{

    public string submitButton = "Submit";

    private Button uiButton;

    private void Awake()
    {
        uiButton = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetButton(submitButton))
        {
            if (uiButton.onClick != null)
            {
                uiButton.onClick.Invoke();
            }
        }
    }
}