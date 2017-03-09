﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour {

    public float Speed = 20.0f;
    public int KeySoundFreq = 3;
    public AudioClip[] KeySound;

    private Text m_text;
    private float m_letterDelay;
    private float m_spaceDelay;

    private AudioSource m_audioSource;
    private WidgetFader m_widgetFader;

    private GameObject m_followGameObject;
    private Bounds m_followGameObjectBounds;

    // Use this for initialization
    void Awake()
    {
        m_text = GetComponentInChildren<Text>();
        m_text.text = "";

        m_letterDelay = 1.0f / Speed;
        m_spaceDelay = 3.0f / Speed;

        m_audioSource = GetComponent<AudioSource>();

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();

        GameObject mainUI = GameObject.Find("MainUI");
        transform.SetParent(mainUI.transform);

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.0f);
        rectTransform.localScale = Vector3.one;
    }

    void Update()
    {
        Vector3 objectPos = new Vector3(m_followGameObject.transform.position.x,
                                        m_followGameObject.transform.position.y + 2.0f*m_followGameObjectBounds.extents.y + UIGlobals.PegDistanceToObject,
                                        0.0f);
        transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, objectPos);
    }

    public void FollowGameObject(GameObject obj, Bounds bounds)
    {
        m_followGameObject = obj;
        m_followGameObjectBounds = bounds;
    }

    public void Enable()
    {
        m_widgetFader.Enable();
    }

    public void Disable()
    {
        m_widgetFader.Disable();
    }
    public void DisableImmediate()
    {
        m_widgetFader.DisableImmediate();
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }

    public void PlayText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(CO_PlayText(text));
    }

    IEnumerator CO_PlayText(string text)
    {
        int count = KeySoundFreq - 1;

        foreach (char letter in text)
        {
            m_text.text += letter.ToString();
            count++;

            if (count == KeySoundFreq)
            {
                m_audioSource.PlayOneShot(KeySound[Random.Range(0, KeySound.Length)]);
                count = 0;
            }

            if (letter == ' ')
            {
                yield return new WaitForSeconds(m_spaceDelay);
            }
            else
            {
                yield return new WaitForSeconds(m_letterDelay);
            }
        }
    }
}
