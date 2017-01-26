using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.kleberswf.lib.core;

public class MonsterController : Singleton<MonsterController>  {

    public int m_chanceToSpawn;
    public int m_maxEnemies;
    public GameObject m_winText;
    public GameObject m_loseText;

    private GameObject m_vortex;
    private List<Monster> m_spawnEnemies;
    private GameObject m_enemyPrefab;
    

	// Use this for initialization
	void Start () {
        m_vortex = GameObject.FindGameObjectWithTag("Vortex");
        m_enemyPrefab = Resources.Load("Monster0") as GameObject;
        m_spawnEnemies = new List<Monster>();
        m_winText.SetActive(false);
        m_loseText.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_spawnEnemies.Count == m_maxEnemies)
            return;

        bool spawn = (Random.Range(0, m_chanceToSpawn) == 0);

        if (spawn)
            SpawnEnemy();
	}

    void SpawnEnemy()
    {
        GameObject spawnEnemy = GameObject.Instantiate(m_enemyPrefab, m_vortex.transform.position, Quaternion.identity);

        Monster monster = spawnEnemy.GetComponent<Monster>();

        m_spawnEnemies.Add(monster);
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