using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kleberswf.lib.core;

public class MonsterController : Singleton<MonsterController>  {

    public int m_chanceToSpawn;
    public int m_maxEnemies;

    private GameObject m_vortex;
    private List<Monster> m_spawnEnemies;
    private GameObject m_enemyPrefab;

	// Use this for initialization
	void Start () {
        m_vortex = GameObject.FindGameObjectWithTag("Vortex");
        m_enemyPrefab = Resources.Load("Monster0") as GameObject;
        m_spawnEnemies = new List<Monster>();
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
}