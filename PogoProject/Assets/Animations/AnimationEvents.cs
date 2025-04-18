using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void EndAttack()
    {
        Controller.Instance.canChangeAnim = true;
    }
}
