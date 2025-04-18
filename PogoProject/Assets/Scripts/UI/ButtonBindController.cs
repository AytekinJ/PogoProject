using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KeyBinderInitializer : MonoBehaviour
{
    // Buttons in sequence: Up, Right, Left, Down, Attack, AimUp, AimRight, AimLeft, AimDown, DpadUp, DpadRight, DpadLeft, DpadDown
    [SerializeField] public List<Button> buttons;
    [SerializeField] private GameSetting settings;
    
    // List to store the keycodes from settings
    private List<KeyCode> targetKeyCodes = new List<KeyCode>();

    private void Start()
    {
        settings = GameSetting.Instance;
        buttons.Clear();
        foreach (Transform childTransform in transform)
        {
      
            Button button = childTransform.GetComponent<Button>();
            if (button != null)
            {
                buttons.Add(button);
            }
            else
            {
                if (childTransform.childCount > 0)
                {
                    Button childButton = childTransform.GetChild(0).GetComponent<Button>();
                    if (childButton != null)
                    {
                        buttons.Add(childButton);
                    }
                    else
                    {
                         Debug.LogWarning($"'{childTransform.name}' nesnesinde veya ilk çocuğunda Button bulunamadı.", childTransform);
                    }
                } else {
                     Debug.LogWarning($"'{childTransform.name}' nesnesinde Button bulunamadı ve çocuğu yok.", childTransform);
                }
            }
        }
        InitializeKeyBindings();
    }
    
    public void InitializeKeyBindings()
    {
        // Check if settings are assigned
        if (settings == null)
        {
            Debug.LogError("KeyBinderInitializer: GameSetting reference is missing!", this);
            return;
        }

        // Check if we have the correct number of buttons
        const int expectedButtonCount = 14;
        if (buttons == null || buttons.Count != expectedButtonCount)
        {
            Debug.LogError($"KeyBinderInitializer: Please assign exactly {expectedButtonCount} buttons in the Inspector!", this);
            return;
        }

        // Populate the key codes from settings
        PopulateTargetKeyCodes();

        // Loop through buttons and set their text to display the corresponding key
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i >= targetKeyCodes.Count)
            {
                Debug.LogError($"KeyBinderInitializer: Not enough key codes for button at index {i}!");
                continue;
            }

            Button currentButton = buttons[i];
            KeyCode targetKey = targetKeyCodes[i];

            if (currentButton == null)
            {
                Debug.LogWarning($"KeyBinderInitializer: Button at index {i} is not assigned.");
                continue;
            }

            // Find the text component on the button
            TextMeshProUGUI buttonText = currentButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogWarning($"KeyBinderInitializer: Button '{currentButton.name}' does not have a TextMeshProUGUI component!", currentButton);
                continue;
            }

            // Set the text to display the key
            buttonText.text = targetKey.ToString();

            // If the button has a KeyBinder component, initialize it
            KeyBinder keyBinder = currentButton.GetComponent<KeyBinder>();
            if (keyBinder != null)
            {
                keyBinder.InitializeKey(targetKey);
            }
            else
            {
                Debug.LogWarning($"KeyBinderInitializer: Button '{currentButton.name}' does not have a KeyBinder component!", currentButton);
                // Add KeyBinder component if not found
                keyBinder = currentButton.gameObject.AddComponent<KeyBinder>();
                keyBinder.InitializeKey(targetKey);
            }
        }

        Debug.Log("KeyBinderInitializer: All buttons initialized successfully.");
    }

    // Populate the target key codes list from settings
    private void PopulateTargetKeyCodes()
    {
        targetKeyCodes.Clear();

        // Add keys in the expected order
        targetKeyCodes.Add(settings.JumpButton);
        targetKeyCodes.Add(settings.up);
        targetKeyCodes.Add(settings.right);
        targetKeyCodes.Add(settings.left);
        targetKeyCodes.Add(settings.down);
        targetKeyCodes.Add(settings.attack);
        targetKeyCodes.Add(settings.upAim);
        targetKeyCodes.Add(settings.rightAim);
        targetKeyCodes.Add(settings.leftAim);
        targetKeyCodes.Add(settings.downAim);
        targetKeyCodes.Add(settings.DpadUp);
        targetKeyCodes.Add(settings.DpadRight);
        targetKeyCodes.Add(settings.DpadLeft);
        targetKeyCodes.Add(settings.DpadDown);
    }
}