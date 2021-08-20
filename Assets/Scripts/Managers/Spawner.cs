using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner> 
{
    private bool isSpawning = false;
    private GameObject[] enemiesToSpawn;
    private GameObject[] powerUpsToSpawn;
    private SoundManager soundManager;

    private RhythmSet _rhythmSet;
    private IEnumerator coroutine_spawner;

    public Transform spawnLocationWalk;
    public Transform spawnLocationFly;
    public float spawnBaseSpeed = 4.0f;
    public int enemiesBeforePowerUp = 6;
    public float powerUpChance = 20.0f;

    private void Start() {
        soundManager = GetComponent<SoundManager>();
    }

    public void SetSpawns(GameObject[] eneSpawns, List<GameObject> powSpawns = null) {
        enemiesToSpawn = eneSpawns;
        if (powSpawns != null) {
            powerUpsToSpawn = powSpawns.ToArray();
        }
    }

    public void StartSpawner(List<GameObject> swipeableEntities) {
        isSpawning = true;
        coroutine_spawner = SpawnEnemy(swipeableEntities);
        StartCoroutine(coroutine_spawner);
    }

    public void StartSpawnerRhythm(List<GameObject> swipeableEntities, RhythmSet set) {
        isSpawning = true;
        _rhythmSet = set;
        coroutine_spawner = SpawnRhythm(swipeableEntities);
        StartCoroutine(coroutine_spawner);
    }

    public IEnumerator SpawnEnemy(List<GameObject> enemylistToAdd) {
        int numberOfSpawns = 0;
        while (isSpawning) {
            
            int length = enemiesToSpawn.Length;
            int ranEnem = Random.Range(0, length);
            GameObject spawned = Instantiate(enemiesToSpawn[ranEnem]);
            Enemy enemy = spawned.GetComponent<Enemy>();

            switch (enemy.enemyType) {
                case (EnemyType.TowerObstacle):
                case (EnemyType.Walking):
                    spawned.transform.position = spawnLocationWalk.position;
                    break;
                case (EnemyType.Flying):
                    spawned.transform.position = spawnLocationFly.position;
                    break;
            }
            numberOfSpawns++;


            if (powerUpsToSpawn.Length > 0 && numberOfSpawns > enemiesBeforePowerUp
                && enemy.enemyType != EnemyType.TowerObstacle) {
                float ranPowChance = Random.Range(0, 100.0f);
                if (powerUpChance > ranPowChance) {
                    int ranPow = Random.Range(0, powerUpsToSpawn.Length);
                    GameObject spawnedPow = Instantiate(powerUpsToSpawn[ranPow]);
                    spawnedPow.transform.parent = spawned.transform;
                    enemy.powerUp = spawnedPow.GetComponent<PowerUp>().powerType;
                    numberOfSpawns = 0;
                }
            }
      
            enemylistToAdd.Add(spawned);
            float randTime = Random.Range(-3.0f, 1.0f);
            yield return new WaitForSeconds(spawnBaseSpeed + randTime);
        }
    }

    public IEnumerator SpawnRhythm(List<GameObject> enemylistToAdd) {
        int numberOfSpawns = 0;
        bool hasSpawned = false;
      
        while (isSpawning) {

            int beat = ((BeatManager.beatCount - 1) % 8);
            int pat = Random.Range(0, _rhythmSet.patterns.Length);

            if (BeatManager.beat && _rhythmSet.patterns[pat].beats[beat] > 0 && 
                !hasSpawned && BeatManager.beatCount > 0 && 
                BeatManager.beatMeasureCount % 2 == 0) {

                hasSpawned = true;
                int length = enemiesToSpawn.Length;
                int ranEnem = Random.Range(0, length);
                GameObject spawned = Instantiate(enemiesToSpawn[ranEnem]);
                Enemy enemy = spawned.GetComponent<Enemy>();

                switch (enemy.enemyType) {
                    case (EnemyType.TowerObstacle):
                    case (EnemyType.Walking):
                        spawned.transform.position = spawnLocationWalk.position;
                        break;
                    case (EnemyType.Flying):
                        spawned.transform.position = spawnLocationFly.position;
                        break;
                }

                soundManager.PlaySound(_rhythmSet.tapBeat, 1);

                numberOfSpawns++;
                enemylistToAdd.Add(spawned);             
            } else {
                hasSpawned = false;
            }

            if (BeatManager.beat && BeatManager.beatCount > 0 &&
                BeatManager.beatCount % 8 == 0 &&
                BeatManager.beatMeasureCount > 0) {
                soundManager.PlaySound(_rhythmSet.tabEndMeasure, 1);
            }

            yield return new WaitForSeconds(0.00001f);
        }

        yield return null;
    }


    public void SetSpawnerSpeed(float speed) {
        spawnBaseSpeed = speed;
    }

    public void StopSpawner() {
        isSpawning = false;
        StopCoroutine(coroutine_spawner);
    }

    public void ResumeSpawner() {
        isSpawning = true;
        StartCoroutine(coroutine_spawner);
    }
}
