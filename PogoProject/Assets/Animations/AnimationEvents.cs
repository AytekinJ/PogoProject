using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void EndAttack()
    {
        Controller.canChangeAnim = true;
    }
}
