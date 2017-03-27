using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using com.kleberswf.lib.core;

public class LevelManager : Singleton<LevelManager> {

    BigotitosTimer m_timer;

    override protected void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        m_timer = GameObject.FindObjectOfType<BigotitosTimer>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            UIModalManager.Instance.OnModalFinished += OnEnableTimer;
            UIModalManager.Instance.EnableIntroPanel();
        }
    }

    public void OnEnableTimer()
    {
        m_timer.ActivateTimer();
    }

    public void LoadLevel(string level)
    {
        UIManager.Instance.DestroyAllChildren();
        SceneManager.LoadScene(level);
    }
}
