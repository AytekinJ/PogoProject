using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFadeScript : MonoBehaviour
{
    public GameObject FadePannel;
    public GameObject DamagePanel;

    public static Image FadeUI;
    public static Image DamageUI;

    public SpriteRenderer[] PlayerNormalSkins;

    public CameraFollow cameraFollowScript;

    void Start()
    {
        FadePannel = GameObject.FindGameObjectWithTag("FadePanel");
        DamagePanel = GameObject.FindGameObjectWithTag("DamagePanel");

        FadeUI = FadePannel.GetComponent<Image>();
        DamageUI = DamagePanel.GetComponent<Image>();
        cameraFollowScript = GetComponent<CameraFollow>();

        PlayerNormalSkins = GameObject.FindGameObjectWithTag("Player").GetComponentsInChildren<SpriteRenderer>();

        DamageUI.color = new Color(DamageUI.color.r, DamageUI.color.g, DamageUI.color.b, 0f);
    }

    public void StartFade(float duration, bool fade, bool unfadeAfter)
    {
        StartCoroutine(FadeCoroutine(duration, fade, unfadeAfter));
    }

    public void StartDamageFlash(float duration)
    {
        StartCoroutine(DamageFlashCoroutine(duration));
    }

    private IEnumerator FadeCoroutine(float duration, bool fade, bool unfadeAfter)
    {
        float startAlpha = FadeUI.color.a;
        float targetAlpha = fade ? 1f : 0f;
        float elapsedTime = 0f;

        if (fade)
        {
            cameraFollowScript.enabled = false;
            SetPlayerAlpha(0f);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            FadeUI.color = new Color(FadeUI.color.r, FadeUI.color.g, FadeUI.color.b, newAlpha);
            yield return null;
        }

        FadeUI.color = new Color(FadeUI.color.r, FadeUI.color.g, FadeUI.color.b, targetAlpha);

        if (unfadeAfter)
        {
            cameraFollowScript.enabled = true;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(UnfadePlayer(duration * 2));
            StartCoroutine(FadeCoroutine(duration * 2, false, false));
        }
        else
        {
            //cameraFollowScript.enabled = true;
        }
    }

    private IEnumerator DamageFlashCoroutine(float duration)
    {
        float elapsedTime = 0f;
        float flashAlpha = 0.5f;

        DamageUI.color = new Color(DamageUI.color.r, DamageUI.color.g, DamageUI.color.b, flashAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(flashAlpha, 0f, elapsedTime / duration);
            DamageUI.color = new Color(DamageUI.color.r, DamageUI.color.g, DamageUI.color.b, newAlpha);
            yield return null;
        }

        DamageUI.color = new Color(DamageUI.color.r, DamageUI.color.g, DamageUI.color.b, 0f);
    }

    private IEnumerator UnfadePlayer(float duration)
    {
        float elapsedTime = 0f;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            SetPlayerAlpha(newAlpha);
            yield return null;
        }

        SetPlayerAlpha(1f);
    }

    private void SetPlayerAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in PlayerNormalSkins)
        {
            Color color = sr.color;
            sr.color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}
