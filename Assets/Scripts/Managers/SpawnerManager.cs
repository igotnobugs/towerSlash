using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : Singleton<SpawnerManager>
{
    public float spawnBaseSpeed = 4.0f;

    [Header("Set up")]
    public Transform spawnLocationWalk;
    public Transform spawnLocationFly;
    public Swipeable[] enemiesToSpawn; // Normal
    public Swipeable[] enemiesToSpawnInRhythm;

    private bool isSpawning = false;
    private RhythmSet _rhythmSet;
    private IEnumerator coroutine_spawner;

    public delegate void SpawnEvent(Swipeable swipeable);
    public event SpawnEvent OnSpawn;


    private void Awake() {
        GameManager.Instance.OnGameOver += StopSpawner;
        GameManager.Instance.OnGameWin += StopSpawner;
    }

    public void StartSpawner() {
        isSpawning = true;
        coroutine_spawner = SpawnEnemy();
        StartCoroutine(coroutine_spawner);
    }

    public void StartSpawnerRhythm(RhythmSet set) {
        Debug.Log("Spawner Started");
        isSpawning = true;
        _rhythmSet = set;
        coroutine_spawner = SpawnRhythm();     
        StartCoroutine(coroutine_spawner);
    }

    public IEnumerator SpawnEnemy() {
        int numberOfSpawns = 0;
        while (isSpawning) {

            int length = enemiesToSpawn.Length;
            int ranEnem = Random.Range(0, length);
            Swipeable spawned = Instantiate(enemiesToSpawn[ranEnem]);
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
            OnSpawn?.Invoke(spawned);

            float randTime = Random.Range(-3.0f, 1.0f);
            yield return new WaitForSeconds(spawnBaseSpeed + randTime);
        }
    }

    public IEnumerator SpawnRhythm() {
        int numberOfSpawns = 0;
        bool hasSpawned = false;
        BeatManager.startBeating = true;

        while (isSpawning) {

            int beat = ((BeatManager.beatCount - 1) % 8);
            int pat = Random.Range(0, _rhythmSet.patterns.Length);

            if (BeatManager.beat && _rhythmSet.patterns[pat].beats[beat] > 0 &&
                !hasSpawned && BeatManager.beatCount > 0 &&
                BeatManager.beatMeasureCount % 2 == 0) {

                hasSpawned = true;
                int length = enemiesToSpawn.Length;
                int ranEnem = Random.Range(0, length);
                Swipeable spawned = Instantiate(enemiesToSpawn[ranEnem]);
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

                SoundManager.Instance.PlaySound(_rhythmSet.tapBeat, 1);

                numberOfSpawns++;
                OnSpawn?.Invoke(spawned);

            } else {
                hasSpawned = false;
            }

            if (BeatManager.beat && BeatManager.beatCount > 0 &&
                BeatManager.beatCount % 8 == 0 &&
                BeatManager.beatMeasureCount > 0) {
                SoundManager.Instance.PlaySound(_rhythmSet.tabEndMeasure, 1);
            }

            yield return new WaitForSeconds(0.00001f);
        }
        Debug.Log("Spawner Stopped");
        yield return null;
    }

    private void Spawn() {
        Debug.Log("Spawned");

        int length = enemiesToSpawn.Length;
        int ranEnem = Random.Range(0, length);
        Swipeable spawned = Instantiate(enemiesToSpawn[ranEnem]);
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
    }

    public void SetSpawnerSpeed(float speed) {
        spawnBaseSpeed = speed;
    }

    private void StopSpawner() {
        Debug.Log("Spawner Stopped");
        isSpawning = false;
        StopCoroutine(coroutine_spawner);
    }

    public void ResumeSpawner() {
        isSpawning = true;
        StartCoroutine(coroutine_spawner);
    }
}
