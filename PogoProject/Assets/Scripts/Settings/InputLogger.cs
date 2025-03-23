using UnityEngine;

public class InputLogger : MonoBehaviour
{
       void Update()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Debug.Log("Key Pressed: " + key);
            }
        }

        string[] buttonNames =
        {
            "Fire1", "Fire2", "Fire3", "Jump", "Submit", "Cancel",
            "JoystickButton0", "JoystickButton1", "JoystickButton2", "JoystickButton3",
            "JoystickButton4", "JoystickButton5", "JoystickButton6", "JoystickButton7",
            "JoystickButton8", "JoystickButton9", "JoystickButton10", "JoystickButton11",
            "JoystickButton12", "JoystickButton13", "JoystickButton14", "JoystickButton15"
        };

        //foreach (string button in buttonNames)
        //{
        //    if (Input.GetButtonDown(button))
        //    {
        //        Debug.Log("Controller Button Pressed: " + button);
        //    }
        //}
    }
}
