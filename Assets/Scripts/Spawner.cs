﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public Wave[] waves;
	public Enemy enemy;

	private LivingEntity playerEntity;
	private Transform playerT;

	private Wave currentWave;
	private int currentWaveNumber = 0;

	private int enemiesRemainingToSpawn;
	private int enemiesRemainingAlive;
	private float nextSpawnTime;

	private MapGenerator map;

	private float timeBetweenCampingChecks = 2;
	private float campThresholdDistance = 1.5f;
	private float nextCampCheckTime;
	private Vector3 campPositionOld;
	private bool isCamping;

	private bool isDisabled;

	void Start()
	{
		playerEntity = FindObjectOfType<Player>();
		playerT = playerEntity.transform;

		nextCampCheckTime = timeBetweenCampingChecks + Time.time;
		campPositionOld = playerT.position;
		playerEntity.OnDeath += OnPlayerDeath;

		map = FindObjectOfType<MapGenerator>();
		NextWave();
	}

	void Update()
	{
		if (!isDisabled)
		{
			if (Time.time > nextCampCheckTime)
			{
				nextCampCheckTime = Time.time + timeBetweenCampingChecks;

				isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
				campPositionOld = playerT.position;
			}

			if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
			{
				enemiesRemainingToSpawn--;
				nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

				StartCoroutine(SpawnEnemy());
			}
		}
	}

	IEnumerator SpawnEnemy()
	{
		float spawnDelay = 1;
		float tileFlashSpeed = 4;

		Transform spawntile = map.GetRandomTile();
		if (isCamping)
		{
			spawntile = map.GetTileFromPosition(playerT.position);
		}
		Material tileMat = spawntile.GetComponent<Renderer>().material;
		Color initialColor = tileMat.color;
		Color flashColor = Color.red;
		float spawnTimer = 0;

		while (spawnTimer < spawnDelay)
		{
			tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, spawntile.position + Vector3.up, Quaternion.identity);
		spawnedEnemy.OnDeath += OnEnemyDeath;
	}

	void OnPlayerDeath()
	{
		isDisabled = true;
	}

	void OnEnemyDeath()
	{
		enemiesRemainingAlive--;

		if (enemiesRemainingAlive == 0)
		{
			NextWave();
		}
	}

	void NextWave()
	{
		currentWaveNumber++;
		if (currentWaveNumber - 1 < waves.Length)
		{
			//print("Wave: " + currentWaveNumber);
			currentWave = waves[currentWaveNumber - 1];

			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;
		}
	}

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
