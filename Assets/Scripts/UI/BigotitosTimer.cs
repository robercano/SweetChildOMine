using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigotitosTimer : MonoBehaviour {


    public float TotalTime;
    public GameObject[] RopeSections;

    Animator m_crankAnimator;
    float m_lastCrankRotationTime;
    float m_timePerStep;
    bool m_activated;
    int m_currentSection;

    void Awake () {
        m_crankAnimator = transform.FindDeepChild("Crank").GetComponent<Animator>();
        m_timePerStep = TotalTime / RopeSections.Length;
        m_activated = false;
        m_currentSection = -1;

        foreach (var section in RopeSections)
        {
            section.SetActive(false);
        }
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
    }

    public void ActivateTimer()
    {
        m_activated = true;
        RotateCrank();
    }

	public void RotateCrank()
    {
        if (m_currentSection >= (RopeSections.Length - 1))
        {
            return;
        }

        m_crankAnimator.SetTrigger("Rotate");
        StartCoroutine(AddRopeAfterSeconds(1.33f));
        m_lastCrankRotationTime = Time.time;
    }

    public IEnumerator AddRopeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        m_currentSection++;
        RopeSections[m_currentSection].SetActive(true);
    }
}
