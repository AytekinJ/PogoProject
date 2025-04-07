using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyBinder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    private bool isListening = false;
    private KeyCode assignedKeyCode;

    void Start()
    {
        if (PlayerPrefs.HasKey(gameObject.name + "_KeyCode"))
        {
            assignedKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(gameObject.name + "_KeyCode"));
        }
        else
        {
            assignedKeyCode = KeyCode.Space;
        }

        UpdateButtonText();

        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError("Butonun TextMeshProUGUI komponenti bulunamadı!", this);
            }
        }
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = assignedKeyCode.ToString();
        }
    }

    public void StartListening()
    {
        if (!isListening)
        {
            isListening = true;
            if (buttonText != null)
            {
                buttonText.text = "...";
            }
        }
    }

    void OnGUI()
    {
        if (isListening)
        {
            Event e = Event.current;
            if (e.isKey || e.isMouse)
            {
                KeyCode pressedKeyCode;

                if (e.isKey)
                {
                    pressedKeyCode = e.keyCode;
                }
                else if (e.isMouse)
                {
                    switch (e.button)
                    {
                        case 0: pressedKeyCode = KeyCode.Mouse0; break;
                        case 1: pressedKeyCode = KeyCode.Mouse1; break;
                        case 2: pressedKeyCode = KeyCode.Mouse2; break;
                        default: pressedKeyCode = KeyCode.None; break;
                    }
                }
                else
                {
                    pressedKeyCode = KeyCode.None;
                }

                if (pressedKeyCode != KeyCode.None)
                {
                    if (pressedKeyCode == KeyCode.Escape)
                    {
                        Debug.Log("Tuş atama iptal edildi.");
                    }
                    else
                    {
                        assignedKeyCode = pressedKeyCode;
                        PlayerPrefs.SetString(gameObject.name + "_KeyCode", assignedKeyCode.ToString());
                        PlayerPrefs.Save();
                    }

                    isListening = false;
                    UpdateButtonText();
                }
            }
        }
    }

    public KeyCode GetAssignedKey()
    {
        return assignedKeyCode;
    }
    // KeyBinder.cs içine eklenecek public metot:
    public void InitializeKey(KeyCode key)
    {
        assignedKeyCode = key; 
        UpdateButtonText();   
    }
}