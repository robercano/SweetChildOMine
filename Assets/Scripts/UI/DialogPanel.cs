using System.Collections;
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
    }

    void Update()
    {
        // Compensate mirroring when character is facing left
        transform.localScale = transform.parent.localScale;
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
