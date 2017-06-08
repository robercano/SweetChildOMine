using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BigotitosTimer : MonoBehaviour {

    public float TotalTime;
    public float BigotitosLickTime;
    public GameObject[] RopeSections;
    public GameObject Bigotitos;
    public AudioClip CatMeow;

    Animator m_crankAnimator;
    float m_lastCrankRotationTime;
    float m_timePerStep;

    AudioSource m_audioSource;

    Animator m_bigotitosAnimator;
    float m_lastLickTime;
    float m_nextLickTime;

    bool m_activated;
    int m_currentSection;

    RectTransform m_bigotitosRectTransform;

    void Awake () {
        m_audioSource = GetComponent<AudioSource>();

        m_crankAnimator = transform.FindDeepChild("Crank").GetComponent<Animator>();
        m_bigotitosAnimator = transform.FindDeepChild("Bigotitos").GetComponent<Animator>();

        m_timePerStep = TotalTime / RopeSections.Length;
        m_activated = false;
        m_currentSection = -1;

        m_bigotitosRectTransform = Bigotitos.GetComponent<RectTransform>();

        foreach (var section in RopeSections)
        {
            section.SetActive(false);
        }

        AddRope();
        BigotitosLick();
    }

    void Update()
    {
        if (m_activated == false)
        {
            return;
        }

        if ((Time.time - m_lastCrankRotationTime) >= m_timePerStep)
        {
            RotateCrank();
        }
        if (Time.time - m_lastLickTime >= m_nextLickTime)
        {
            BigotitosLick();
        }
    }

    public void ActivateTimer()
    {
        m_activated = true;
        RotateCrank();
    }

    public void BigotitosLick()
    {
        AudioSource.PlayClipAtPoint(CatMeow, Camera.main.transform.position);
        m_bigotitosAnimator.SetTrigger("Lick");
        m_lastLickTime = Time.time;
        m_nextLickTime = UnityEngine.Random.Range(1.0f, BigotitosLickTime);
    }

	public void RotateCrank()
    {
        if (m_currentSection >= (RopeSections.Length - 1))
        {
            return;
        }

        m_audioSource.Play();
        m_crankAnimator.SetTrigger("Rotate");
        ExecuteAfterSeconds(AddRope, 1.33f);
        m_lastCrankRotationTime = Time.time;
    }

    void HangBigotitos()
    {
        Vector2 ropePos = RopeSections[m_currentSection].GetComponent<RectTransform>().localPosition;

        Vector2 bigotitosPos = new Vector2(ropePos.x + 0.5f, ropePos.y + 0.5f + m_bigotitosRectTransform.sizeDelta.y / 2.0f);

        m_bigotitosRectTransform.localPosition = bigotitosPos;
    }

    public void AddRope()
    {
        m_currentSection++;
        RopeSections[m_currentSection].SetActive(true);

        HangBigotitos();
    }

    public void ExecuteAfterSeconds(Action action, float seconds)
    {
        StartCoroutine(ExecuteAfterSeconds_CO(action, seconds));
    }

    public IEnumerator ExecuteAfterSeconds_CO(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}
