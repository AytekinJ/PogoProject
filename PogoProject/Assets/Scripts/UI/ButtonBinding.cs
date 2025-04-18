using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyBinder : MonoBehaviour
{
    private KeyCode currentKey;
    private TextMeshProUGUI buttonText;
    private Button button;
    [SerializeField] private GameSetting settings;
    [SerializeField] private string bindingType; // "Up", "Down", "Left", "Right", etc.

    private bool isWaitingForKeyInput = false;

    private void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
        
        if (buttonText == null)
        {
            Debug.LogError("KeyBinder: TextMeshProUGUI component not found on button!", this);
            return;
        }
        
        if (button == null)
        {
            Debug.LogError("KeyBinder: Button component not found!", this);
            return;
        }
        
        // Add click listener
        button.onClick.AddListener(StartListeningForKey);
    }

    // Called by KeyBinderInitializer
    public void InitializeKey(KeyCode key)
    {
        currentKey = key;
        
        // Update the button text to show current key binding
        if (buttonText != null)
        {
            buttonText.text = key.ToString();
        }
    }

    public void StartListeningForKey()
    {
        if (isWaitingForKeyInput) return;
        
        isWaitingForKeyInput = true;
        buttonText.text = ". . .";
    }

    private void Update()
    {
        if (!isWaitingForKeyInput) return;

        // Check for any key press
        
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
        if (Input.GetKeyDown(key))
        {
            AssignNewKey(key);
            break;
        }
        }
        
        
        // Allow canceling with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelKeyBinding();
        }
    }

    private void AssignNewKey(KeyCode newKey)
    {
        // Skip keys that shouldn't be bindable
        if (newKey == KeyCode.Escape || newKey == KeyCode.Return)
        {
            CancelKeyBinding();
            return;
        }

        currentKey = newKey;
        isWaitingForKeyInput = false;
        
        // Update button text
        if (buttonText != null)
        {
            buttonText.text = newKey.ToString();
        }
        
        // Update the corresponding key in settings based on binding type
        if (settings != null)
        {
            UpdateSettingsKey(newKey);
        }
        
        // Optional: Save settings
        SaveSettings();
    }

    private void CancelKeyBinding()
    {
        isWaitingForKeyInput = false;
        
        // Reset text to current key
        if (buttonText != null)
        {
            buttonText.text = currentKey.ToString();
        }
    }

    private void UpdateSettingsKey(KeyCode newKey)
    {
        if (settings == null) return;

        // Find button index in KeyBinderInitializer
        KeyBinderInitializer initializer = FindFirstObjectByType<KeyBinderInitializer>();
        if (initializer != null)
        {
            int buttonIndex = initializer.buttons.IndexOf(button);
            
            // Update the appropriate setting based on button index
            switch (buttonIndex)
            {
                case 0: settings.JumpButton = newKey; break;
                case 1: settings.up = newKey; break;
                case 2: settings.right = newKey; break;
                case 3: settings.left = newKey; break;
                case 4: settings.down = newKey; break;
                case 5: settings.attack = newKey; break;
                case 6: settings.upAim = newKey; break;
                case 7: settings.rightAim = newKey; break;
                case 8: settings.leftAim = newKey; break;
                case 9: settings.downAim = newKey; break;
                case 10: settings.DpadUp = newKey; break;
                case 11: settings.DpadRight = newKey; break;
                case 12: settings.DpadLeft = newKey; break;
                case 13: settings.DpadDown = newKey; break;
                default: Debug.LogWarning("KeyBinder: Unknown button index: " + buttonIndex); break;
            }
        }
    }

    private void SaveSettings()
    {
        // Implement if you have a settings saving system
        // For example:
        // SettingsManager.Instance.SaveSettings();
    }
}