using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIIntroPanelLayer : MonoBehaviour, IPointerClickHandler {

    public float Speed = 20.0f;
    public int KeySoundFreq = 3;
    public AudioClip[] KeySound;

    public Sprite Avatar
    {
        get
        {
            return m_avatar.sprite;
        }
        set
        {
            m_avatar.sprite = value;
        }
    }
    public string Text
    {
        get
        {
            return m_dialog.text;
        }
        set
        {
            StopAllCoroutines();
            m_dialog.text = value;
        }
    }
    Image m_avatar;
    Text m_dialog;
    AudioSource m_audioSource;
    float m_letterDelay;
    float m_spaceDelay;
    string m_textToPlay;

    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();

        m_avatar = GetComponentInChildren<Image>();
        m_avatar.sprite = null;

        m_dialog = GetComponentInChildren<Text>();
        m_dialog.text = "";

        m_letterDelay = 1.0f / Speed;
        m_spaceDelay = 3.0f / Speed;
    }

    public void FinishPlayText()
    {
        Text = m_textToPlay;
    }

    public void PlayText(string text)
    {
        StopAllCoroutines();
        m_textToPlay = text;
        StartCoroutine(CO_PlayText());
    }

    IEnumerator CO_PlayText()
    {
        int count = KeySoundFreq - 1;

        m_dialog.text = "";

        foreach (char letter in m_textToPlay)
        {
            m_dialog.text += letter.ToString();
            count++;

            if (count == KeySoundFreq)
            {
                m_audioSource.PlayOneShot(KeySound[UnityEngine.Random.Range(0, KeySound.Length)]);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        FinishPlayText();
    }
}
