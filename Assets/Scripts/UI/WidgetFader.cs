using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WidgetFader : MonoBehaviour {

    public float FadeTime;
    public int FadeSteps;

    private CanvasGroup m_canvasGroup;

    void Awake()
    {
        m_canvasGroup = GetComponent< CanvasGroup > ();
        Assert.IsNotNull(m_canvasGroup);
    }

    IEnumerator FadeIn()
    {
        for (int i = 0; i < FadeSteps; i++)
        {
            m_canvasGroup.alpha = i / (float)FadeSteps;
            yield return new WaitForSeconds(FadeTime / FadeSteps);
        }
        m_canvasGroup.alpha = 1.0f;
    }
    IEnumerator FadeOut()
    {
        for (int i = 0; i < FadeSteps; i++)
        {
            m_canvasGroup.alpha = 1.0f - (i / (float)FadeSteps);
            yield return new WaitForSeconds(FadeTime / FadeSteps);
        }
        m_canvasGroup.alpha = 0.0f;
        gameObject.SetActive(false);
    }

    /* Public interface */
    public void Enable()
    {
        gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }
    public void EnableImmediate()
    {
        m_canvasGroup.alpha = 1.0f;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        if (gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }
    public void DisableImmediate()
    {
        gameObject.SetActive(false);
    }
}
