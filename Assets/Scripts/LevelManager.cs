using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.kleberswf.lib.core;

public class LevelManager : Singleton<LevelManager> {

	public GameObject m_winText;
	public GameObject m_loseText;

    private UIModalManager m_modalManager;


    override protected void Awake()
    {
        m_modalManager = UIModalManager.Instance;
    }

	void Start () {
		m_winText.SetActive(false);
		m_loseText.SetActive(false);

        // Show the intro panel
        m_modalManager.EnableIntroPanel();
    }
	
	public void EnemyDestroyed()
	{
		if (Monster.NumSpawnMonsters <= 0)
		{
			StartCoroutine(ShowWin());
		}

	}

	public void PlayerDestroyed()
	{
		StartCoroutine(ShowLose());
	}

	IEnumerator ShowWin()
	{
		m_winText.SetActive(true);
		Transform child = m_winText.transform.FindChild("Text");

		Text text = child.GetComponent<Text>();

		while (true)
		{
			text.color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
			yield return new WaitForSeconds(0.1f);
		}  
	}

	IEnumerator ShowLose()
	{
		m_loseText.SetActive(true);
		Transform child = m_loseText.transform.FindChild("Text");

		Text text = child.GetComponent<Text>();

		while (true)
		{
			text.color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
			yield return new WaitForSeconds(0.1f);
		}
	}
}
