using UnityEngine;

public class UIPopUpArea : MonoBehaviour
{
    public GameObject UIPrefab;

    public Transform TranformPos;

    GameObject CreatedUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CreatedUI = null;
        CreatedUI = Instantiate(UIPrefab, TranformPos.position, Quaternion.identity, null);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CreatedUI.GetComponent<UIPopUp>().StartFade();
        CreatedUI = null;
    }

}
