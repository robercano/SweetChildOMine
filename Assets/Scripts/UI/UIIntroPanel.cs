using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;

public class UIIntroPanel : MonoBehaviour {

    public Sprite Avatar1;
    public Sprite Avatar2;
    [TextArea]
    public string[] Dialog;

    public delegate void OnExitDelegate();
    public OnExitDelegate OnExit;

    UIIntroPanelLayer[] m_layers;

    Button m_nextButton;
    Text m_nextButtonText;

    int m_currentLayer;
    int m_currentDialog;

    void Awake()
    {
        m_layers = GetComponentsInChildren<UIIntroPanelLayer>();
        Assert.IsTrue(m_layers.Length == 2);

        Transform buttonTransform = transform.FindDeepChild("NextStartButton");
        m_nextButton = buttonTransform.GetComponent<Button>();
        m_nextButtonText = buttonTransform.GetComponentInChildren<Text>();

        m_currentLayer = 1;
        m_currentDialog = -1;
    }

    void Start()
    {
        m_layers[0].Avatar = Avatar1;
        m_layers[1].Avatar = Avatar2;

        NextDialog();
    }

    public void NextDialog()
    {
        m_currentLayer = (1 - m_currentLayer); // Flip the layer
        m_currentDialog++;

        if (m_currentDialog >= Dialog.Length)
        {
            return;
        }

        if (m_currentDialog ==  (Dialog.Length - 1))
        {
            m_nextButtonText.text = "Start!";
            m_nextButton.onClick.RemoveAllListeners();
            m_nextButton.onClick.AddListener(() => OnExitHandler());
        }

        m_layers[m_currentLayer].gameObject.SetActive(true);
        m_layers[1 - m_currentLayer].gameObject.SetActive(false);

        m_layers[m_currentLayer].PlayText(Dialog[m_currentDialog]);
    }

    public void OnExitHandler()
    {
        if (OnExit != null)
        {
            OnExit();
        }

        Destroy(gameObject);
    }
}
