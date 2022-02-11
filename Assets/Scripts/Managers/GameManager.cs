using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager> 
{
    [Header("Game States")]
    public int score = 0;
    public int coins = 0;
    public float speedScale = 1;
    public float scoreMult = 1.0f;
    //public float scoreMultTapDash = 2.0f;
   
    [Header("Set Up")]
    public RhythmSet rhythmSet;
    public GameObject player;
    public Transform playingPosition;
    public CameraControlScript cameraScript;
    public GameObject slashParticle;

    private bool rhythmWon = false;
    //private float gameTime = 0;
    private float scoreInterval = 1.0f;
    private bool gameOver = false;
    private bool isGameStart = false;
    private Swipeable currentTarget;
    private List<Swipeable> swipeableEntities = new List<Swipeable>();
    private NinjaControl ninja;

    public bool IsRhythmMode { get; set; } = true;
    public delegate void GameManagerEvent();
    public event GameManagerEvent OnGameStart;
    public event GameManagerEvent OnGameOver;
    public event GameManagerEvent OnGameWin;
    public event GameManagerEvent OnAddCoin;
    public event GameManagerEvent OnAddScore;
    public delegate void SpeedScaleEvent(float scale);
    public event SpeedScaleEvent OnUpdateSpeedScale;
    private IEnumerator gameModeCoroutine;

    private void Awake() {      
        ninja = player.GetComponent<NinjaControl>();
    }

    private void Start() {
        LoadProgress();
        OnAddCoin?.Invoke();
        OnAddScore?.Invoke();
    }

    public void StartGame() {
        if (gameOver) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        player.transform.position = playingPosition.position;
        player.transform.rotation = playingPosition.rotation;
        StartCoroutine(PlayerStartSequence());

        cameraScript.Move(new Vector3(-1, 15, -20), 3.0f);

        isGameStart = true;
        OnGameStart?.Invoke();

        StartCoroutine(PassiveScore());
        SpawnerManager.Instance.OnSpawn += OnSpawnEnemy;


        if (IsRhythmMode) {
            ninja.isRhythmMode = true;
            SpawnerManager.Instance.StartSpawnerRhythm(rhythmSet);
            gameModeCoroutine = RhythmCoroutine();
            SoundManager.Instance.PlaySound(rhythmSet.mainSong, 1);

        } 
        else {
            SpawnerManager.Instance.StartSpawner();
            gameModeCoroutine = NormalCoroutine();
        }

        StartCoroutine(gameModeCoroutine);
    }

    private void OnSpawnEnemy(Swipeable swipeable) {
        swipeableEntities.Add(swipeable);
        swipeable.yInRange = ninja.transform.position.y + ninja.attackRange;
        swipeable.InRange += OnRangeReached;
    }

    private void OnRangeReached(Swipeable swipeable) {
        swipeableEntities.Remove(swipeable);
        
        swipeable.InRange -= OnRangeReached;
        

        if (swipeable.isTargeted) {
            // Should be destroyed by the ninja
            Instantiate(slashParticle, swipeable.transform.position, Quaternion.identity);
            AddScore(2);
        } else {
            swipeable.MarkAsFailed();
            Destroy(swipeable.gameObject, 1.0f);
        }

    }

    private IEnumerator NormalCoroutine() {
        //StartCoroutine(PassiveScore());

        while (isGameStart) {

            if (currentTarget == null) currentTarget = swipeableEntities[0];
            if (currentTarget == null) yield return null;

            //Cannot move when attacking
            //if (ninja.IsAttacking()) yield return null;

            //Get swipe direction
            /*
            Direction curDir = SwipeTracker.Instance.GetSwipeDirection();
            switch (curDir) {
                case Direction.Any:
                case Direction.None:
                    yield return null;
                case Direction.Tap:
                    if (isTapDashing == false)
                        StartCoroutine(TapDash());
                    break;
                default:
                    if (currentTarget == null) {
                        //Swiped without a target
                        yield return null;
                    }
                    break;
            }

            if (currentTarget == null) yield return null;

            //Get swipeable target   
            Swipeable curTar = currentTarget.GetComponent<Swipeable>();
            Direction tarDir = curTar.GetTargetDirection();

            //Swiped too early 
            if (tarDir == Direction.None) {continue;}

            //Frenzy allows mistakes //  || isFrenzyActivated
            if (tarDir == Direction.Any || tarDir == curDir) {
                AddCoins(2 * (int)scoreMult);
                score += 10 * (int)scoreMult;
                curTar.MarkAsSuccess();
                ninja.AddTarget(currentTarget);
                swipeableEntities.RemoveAt(0);
                currentTarget = null;
                continue;
            }

            //Mismatch Direction
            Debug.Log("Wrong! You swiped " + curDir + ". Expected " + tarDir);
            isGameStart = false;
            GameOverSequence();
            */
        }

        yield return null;
    }

    private IEnumerator RhythmCoroutine() {
        //StartCoroutine(PassiveScore());

        float secondsToReach = 18 / 8;
        float ratio = 1;
        if (BeatManager.beatMeasureInterval != 0) {
            ratio = secondsToReach / BeatManager.beatMeasureInterval;
        }
        speedScale = ratio;

        SwipeTracker.Instance.OnTap += Tap;

        //while (isGameStart) {
        //
        //}
        
        Debug.Log("Rhythm End");
        yield return null;
    }

    private void Tap() {

        if (swipeableEntities.Count <= 0)  return;
        currentTarget = swipeableEntities[0];
        //if (currentTarget == null) return;

        ninja.AddTarget(currentTarget);

        swipeableEntities.RemoveAt(0);
        currentTarget.MarkAsSuccess();
        currentTarget = null;
    }

    public void AddCoins(int amount) {
        coins += amount;
        OnAddCoin?.Invoke();
    }

    public void AddScore(int amount) {
        score += amount;
        OnAddScore?.Invoke();
    }

    private IEnumerator PlayerStartSequence() {
        ninja.PlayAnimation("RunAnimation");    
        yield return new WaitForSeconds(2.0f);
        SwipeTracker.Instance.StartTracking();
        yield return null;
    }

    private void GameOverSequence() {
        Debug.Log("Game Over!");
        isGameStart = false;
        gameOver = true;
        SaveProgress();
        StopAllCoroutines();

        //Make player fall
        player.GetComponent<Rigidbody>().isKinematic = false;

        foreach (Swipeable spawned in swipeableEntities) {
            Destroy(spawned.gameObject);
        }
        
        swipeableEntities.Clear();

        if (rhythmWon) { OnGameWin?.Invoke(); } 
        else { OnGameOver?.Invoke(); }    
    }

    private IEnumerator PassiveScore() {

        while (isGameStart) {
            yield return new WaitForSeconds(scoreInterval);
            AddScore(1);
        }

        yield return null;
    }

    private void PerformTapDash() {

    }

    private IEnumerator TapDash() {   
        speedScale *= 2.0f;
        //scoreMult *= scoreMultTapDash;

        UIManager.Instance.SetLowerDisplayText("Dash");

        yield return new WaitForSeconds(3.0f);

        speedScale /= 2.0f;
        //scoreMult /= scoreMultTapDash;
        UIManager.Instance.SetLowerDisplayText("");    
    }

    public void SaveProgress() {
        PlayerPrefs.SetInt("coin", coins);
    }

    public void LoadProgress() {
        coins = PlayerPrefs.GetInt("coin");
    }

    public void DeleteProgress() {
        PlayerPrefs.DeleteAll();
    }


    private void OnEnable() {
        Start();
    }

}