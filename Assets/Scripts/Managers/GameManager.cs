using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager> 
{
    private bool isGameStart = false;
    private bool isSpawningStart = false;
    private float yThreshold = -4.5f;
    private GameObject currentTarget;
    private SwipeTracker swipeControl;
    private List<GameObject> swipeableEntities = new List<GameObject>();
    private Spawner spawner;
    private NinjaControl ninja;
    private UIManager ui;
    private SoundManager soundManager;

    private bool gameOver = false;
    private bool isTapDashing = false;
    private float defSpeedScale;
    private int score = 0;
    private float gameTime = 0;
    private float scoreInterval = 1;

    public GameObject player;
    public Transform startingPosition;
    public Transform playingPosition;  
    public CameraControlScript cameraScript;
    public int coins = 0;
    public PopUpScript GameOverPanel;
    public PopUpScript YouWinPanel;
    public GameObject background; 
    public float speedScale = 1;
    public GameObject[] enemiesToSpawn;

    //Upgrade Variables
    private bool isUpgradeUpdated = false;
    private List<GameObject> powerUpsToSpawn = new List<GameObject>();
    public GameObject frenzyPowerUp = null;
    public bool IsUnlockFrenzy { get; set; } = false;
    public GameObject shieldPowerUp = null;
    public bool IsUnlockShield { get; set; } = false;
    public GameObject dashPowerUp = null;
    public bool IsUnlockDash { get; set; } = false;
    public bool IsFrenzyTime { get; set; } = false;
    public bool IsShieldTime { get; set; } = false;
    public bool IsDashTime { get; set; } = false;
    public bool IsSpeedUp { get; set; } = false;
    public bool IsVisionOn { get; set; } = false;
    public bool IsLucky { get; set; } = false;

    //PowerUp Variables
    private bool isFrenzyActivated = false;
    private bool isShieldActivated = false;
    private bool isDashActivated = false;
    private float scaleDuration = 2.0f;
    public float frenzyDuration = 5.0f;
    public float shieldDuration = 5.0f;
    public float dashDuration = 5.0f;
    public float scoreMult = 1.0f;
    public float scoreMultTapDash = 2.0f;

    //Rhythm vars
    private bool mainSoundPlaying = false;
    private bool rhythmWon = false;
    public GameObject[] enemiesToSpawnInRhythm;
    public bool IsRhythmMode { get; set; } = false;
    public RhythmSet rhythmSet;
    

    private void Awake() {      
        ninja = player.GetComponent<NinjaControl>();
        spawner = GetComponent<Spawner>();
        swipeControl = GetComponent<SwipeTracker>();
        soundManager = GetComponent<SoundManager>();
        ui = GetComponent<UIManager>();        
    }

    private void Start() {
        LoadProgress();
        yThreshold = playingPosition.transform.position.y - 2.0f;
        ui.SetCoinText(coins);
        ui.SetScoreText(score);
        defSpeedScale = speedScale;
    }

    private void Update() 
	{
        if (!isGameStart) return;

        ui.SetScoreText(score);

        if (IsRhythmMode) {
            RhythmMode();
        } else {
            NormalMode();
        }
    }

    private void NormalMode() {      
        if (!isUpgradeUpdated) UpdateUpgrades();

        //Move background down, instead of player moving
        background.transform.Translate(0, -1.0f * Time.deltaTime, 0);

        if (!isSpawningStart) {
            spawner.SetSpawns(enemiesToSpawn, powerUpsToSpawn);
            spawner.StartSpawner(swipeableEntities);
            StartCoroutine(UpdateEnemySpeedScale());
            isSpawningStart = true;
        }
        else {
            //Score increase
            gameTime += Time.deltaTime;
            if (gameTime > scoreInterval) {
                gameTime -= scoreInterval;
                score++;
            }
            if (currentTarget == null) currentTarget = GetTarget();

            //Shield stops passing threshold
            if (TrackEnemyPassingThreshold() && !isShieldActivated && !isDashActivated) {
                Debug.Log("A target just passed you!");
                GameOverSequence();
                return;
            }

            //Cannot move when attacking
            if (ninja.IsAttacking()) return;

            //Get swipe direction
            Direction curDir = swipeControl.GetSwipeDirection();
            switch (curDir) {
                case Direction.Any:
                case Direction.None:
                    return;
                case Direction.Tap:
                    if (isTapDashing == false)
                        StartCoroutine(TapDash());
                    break;
                default:
                    if (currentTarget == null) {
                        //Swiped without a target
                        return;
                    }
                    break;
            }

            if (currentTarget == null) return;

            //Get swipeable target   
            Swipeable curTar = currentTarget.GetComponent<Swipeable>();
            Direction tarDir = curTar.GetTargetDirection();

            //Swiped too early 
            if (tarDir == Direction.None) {
                return;
            }

            //Frenzy allows mistakes
            if (tarDir == Direction.Any || tarDir == curDir || isFrenzyActivated) {
                AddCoins(2 * (int)scoreMult);
                score += 10 * (int)scoreMult;
                curTar.MarkAsSuccess();
                ninja.AddTarget(currentTarget);
                swipeableEntities.RemoveAt(0);
                currentTarget = null;
                return;
            }

            //Mismatch Direction
            Debug.Log("Wrong! You swiped " + curDir + ". Expected " + tarDir);
            GameOverSequence();
        }
    }

    private void RhythmMode() {

        if (!mainSoundPlaying && BeatManager.beatStarts) {
            soundManager.PlaySound(rhythmSet.mainSong, 1);
            mainSoundPlaying = true;
        }

        background.transform.Translate(0, -1.0f * Time.deltaTime, 0);
        BeatManager.startBeating = true;

        float secondsToReach = 18 / 8;
        float ratio = 1;
        if (BeatManager.beatMeasureInterval != 0) {
            ratio = secondsToReach / BeatManager.beatMeasureInterval;
        }      
        speedScale = ratio;

        //Score increase
        gameTime += Time.deltaTime;
        if (gameTime > scoreInterval) {
            gameTime -= scoreInterval;
            score++;
        }

        //Just end it
        if (score > 1300) {
            rhythmWon = true;
            GameOverSequence();
            return;
        }

        if (!isSpawningStart) {
            ninja.isRhythmMode = true;
            spawner.SetSpawns(enemiesToSpawnInRhythm);
            spawner.StartSpawnerRhythm(swipeableEntities, rhythmSet);
            StartCoroutine(UpdateEnemySpeedScale());
            isSpawningStart = true;
        }
        else {

            if (currentTarget == null) currentTarget = GetTarget();

            if (TrackEnemyPassingThreshold()) {
                Debug.Log("A target just passed you!");
                GameOverSequence();
                return;
            }
            if (currentTarget == null) return;
            Direction curDir = swipeControl.GetSwipeDirection();
            BeatStatus status = BeatStatus.None;
            switch (curDir) {
                case Direction.Any:
                case Direction.None:
                    return;              
                case Direction.Left:
                case Direction.Right:
                case Direction.Up:
                case Direction.Down:
                case Direction.Tap:
                    switch (BeatManager.beatStatus) {
                        case (BeatStatus.Early):
                            status = BeatStatus.Early;
                            Debug.Log("Early!");
                            break;
                        case (BeatStatus.Perfect):
                            status = BeatStatus.Perfect;
                            Debug.Log("Perfect!");
                            break;
                        case (BeatStatus.Late):
                            status = BeatStatus.Late;
                            Debug.Log("Late!");
                            break;
                        default:
                            Debug.Log("Oops!");
                            break;
                    }
                    break;
            }

            //Get swipeable target   
            Swipeable curTar = currentTarget.GetComponent<Swipeable>();
            Direction tarDir = curTar.GetTargetDirection();

            //Swiped too early 
            if (tarDir == Direction.None) {
                return;
            }

            if (tarDir == Direction.Any || tarDir == curDir && 
                status != BeatStatus.None) {

                switch (status) {
                    case (BeatStatus.Early):
                        score += 5;
                        break;
                    case (BeatStatus.Perfect):
                        score += 20;
                        break;
                    case (BeatStatus.Late):
                        score += 5;
                        break;
                }

                curTar.MarkAsSuccess();

                ninja.AddTarget(currentTarget);

                swipeableEntities.RemoveAt(0);

                currentTarget = null;
                return;
            }
        }
    }

    public void AddCoins(int amount) {
        coins += amount;
        ui.SetCoinText(coins);
    }

    public GameObject GetTarget() { 
        if (swipeableEntities.Count <= 0) return null;
        if (swipeableEntities[0] == null)
            swipeableEntities.RemoveAll(item => item == null);
        return swipeableEntities[0];
    }

    private bool TrackEnemyPassingThreshold() {
        if (swipeableEntities.Count <= 0) return false;
        if (swipeableEntities[0].transform.position.y < yThreshold) 
            return true;     
        return false;
    }

    public void StartGame() {    
        if (gameOver) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        player.transform.position = playingPosition.position;
        player.transform.rotation = playingPosition.rotation;
        StartCoroutine(PlayerStartSequence());
        cameraScript.Move(new Vector3(-1, 15, -20), 3.0f);
        isGameStart = true;
    }

    private IEnumerator PlayerStartSequence() {
        ninja.PlayAnimation("RunAnimation");    
        yield return new WaitForSeconds(2.0f);
        swipeControl.StartTracking();
        yield return null;
    }

    private void GameOverSequence() {
        isGameStart = false;
        gameOver = true;
        SaveProgress();
        spawner.StopSpawner();
        StopAllCoroutines();

        //Make player fall
        player.GetComponent<Rigidbody>().isKinematic = false;

        foreach (GameObject spawned in swipeableEntities) {
            Destroy(spawned);
        }
        
        swipeableEntities.Clear();
        powerUpsToSpawn.Clear();

        if (rhythmWon) {
            YouWinPanel.Show();
        } else {
            GameOverPanel.Show();
        }    
    }

    private IEnumerator UpdateEnemySpeedScale() {
        for (; ;)  {
            SetAllEnemySpeedScale(speedScale);
            yield return new WaitForSeconds(0.0001f);
        }
    }


    private void SetAllEnemySpeedScale(float speed) {
        if (swipeableEntities.Count <= 0) return;
        if (swipeableEntities[0] == null)
            swipeableEntities.RemoveAll(item => item == null);
        foreach (GameObject spawned in swipeableEntities) {
            Enemy enemy = spawned.GetComponent<Enemy>();
            enemy.ScaleDescentSpeed(speed);
        }
    }

    private void UpdateUpgrades() {
        isUpgradeUpdated = true;

        if (IsUnlockFrenzy) powerUpsToSpawn.Add(frenzyPowerUp);
        if (IsUnlockShield) powerUpsToSpawn.Add(shieldPowerUp);
        if (IsUnlockDash) powerUpsToSpawn.Add(dashPowerUp);

        if (IsLucky) {
            if (IsUnlockFrenzy) powerUpsToSpawn.Add(frenzyPowerUp);
            if (IsUnlockShield) powerUpsToSpawn.Add(shieldPowerUp);
            if (IsUnlockDash) powerUpsToSpawn.Add(dashPowerUp);
        }

        if (IsSpeedUp) speedScale *= 2.0f;
    }

    public bool BuyUpgrade(int cost, UpgradeType upgrade) {
        if (CheckRemoveCoins(cost)) {
            ToggleUpgrade(upgrade);
            ui.SetCoinText(coins);
            return true;
        }
        return false;
    }

    private bool CheckRemoveCoins(int cost) {
        if (coins < cost) return false;
        coins -= cost;
        return true;
    }

    public void ToggleUpgrade(UpgradeType upgrade) {
        switch (upgrade) {
            case (UpgradeType.UnlockFrenzy):
                IsUnlockFrenzy = !IsUnlockFrenzy;
                break;
            case (UpgradeType.UnlockShield):
                IsUnlockShield = !IsUnlockShield;
                break;
            case (UpgradeType.UnlockDash):
                IsUnlockDash = !IsUnlockDash;
                break;
            case (UpgradeType.FrenzyTime):
                IsFrenzyTime = !IsFrenzyTime;
                break;
            case (UpgradeType.ShieldTime):
                IsShieldTime = !IsShieldTime;
                break;
            case (UpgradeType.DashTime):
                IsDashTime = !IsDashTime;
                break;
            case (UpgradeType.SpeedUp):
                IsSpeedUp = !IsSpeedUp;
                break;
            case (UpgradeType.Vision):
                IsVisionOn = !IsVisionOn;
                break;
            case (UpgradeType.Lucky):
                IsLucky = !IsLucky;
                break;
            default:
                Debug.Log("Upgrade not handled");
                break;
        }
    }

    public void SetUpgrade(UpgradeType upgrade, bool boolean) {
        switch (upgrade) {
            case (UpgradeType.UnlockFrenzy):
                IsUnlockFrenzy = boolean;
                break;
            case (UpgradeType.UnlockShield):
                IsUnlockShield = boolean;
                break;
            case (UpgradeType.UnlockDash):
                IsUnlockDash = boolean;
                break;
            case (UpgradeType.FrenzyTime):
                IsFrenzyTime = boolean;
                break;
            case (UpgradeType.ShieldTime):
                IsShieldTime = boolean;
                break;
            case (UpgradeType.DashTime):
                IsDashTime = boolean;
                break;
            case (UpgradeType.SpeedUp):
                IsSpeedUp = boolean;
                break;
            case (UpgradeType.Vision):
                IsVisionOn = boolean;
                break;
            case (UpgradeType.Lucky):
                IsLucky = boolean;
                break;
            case (UpgradeType.RhythmMode):
                IsRhythmMode = boolean;
                break;
            default:
                Debug.Log("Upgrade not handled");
                break;
        }
    }

    public void ActivatePowerUp(PowerType powerUp) {
        switch (powerUp) {
            case (PowerType.Frenzy):
                StartCoroutine(ActivateFrenzy());
                break;
            case (PowerType.Shield):
                StartCoroutine(ActivateShield());
                break;
            case (PowerType.Dash):
                StartCoroutine(ActivateDash());
                break;
            default:
                break;
        }
    }

    private IEnumerator ActivateFrenzy() {     
        isFrenzyActivated = true;

        float newDuration = frenzyDuration;
        if (IsFrenzyTime) newDuration *= scaleDuration;

        ui.SetLowerDisplayText("Frenzy Activated");

        yield return new WaitForSeconds(newDuration);
        
        isFrenzyActivated = false;

        ui.SetLowerDisplayText("");

        yield return null;
    }

    private IEnumerator ActivateShield() {
        isShieldActivated = true;

        float newDuration = shieldDuration;
        if (IsFrenzyTime) newDuration *= scaleDuration;

        ui.SetLowerDisplayText("Shield Activated");

        yield return new WaitForSeconds(newDuration);

        isShieldActivated = false;

        ui.SetLowerDisplayText("");

        yield return null;
    }

    private IEnumerator ActivateDash() {
        isDashActivated = true;

        spawner.StopSpawner();

        float newDuration = dashDuration;
        if (IsFrenzyTime) newDuration *= scaleDuration;

        ui.SetLowerDisplayText("Dash Activated");

        yield return new WaitForSeconds(newDuration);

        isDashActivated = false;

        ui.SetLowerDisplayText("");

        spawner.ResumeSpawner();

        yield return null;
    }

    private IEnumerator TapDash() {   
        isTapDashing = true;
        speedScale *= 2.0f;
        scoreMult *= scoreMultTapDash;
 
        ui.SetLowerDisplayText("Dash");

        yield return new WaitForSeconds(3.0f);

        speedScale /= 2.0f;
        scoreMult /= scoreMultTapDash;
        ui.SetLowerDisplayText("");    
        isTapDashing = false;
    }

    public void SaveProgress() {
        PlayerPrefs.SetInt("coin", coins);

        PlayerPrefs.SetInt("IsFrenzyUnlock", BoolToInt(IsUnlockFrenzy));
        PlayerPrefs.SetInt("IsShieldUnlock", BoolToInt(IsUnlockShield));
        PlayerPrefs.SetInt("IsDashUnlock", BoolToInt(IsUnlockDash));

        PlayerPrefs.SetInt("IsFrenzyTime", BoolToInt(IsFrenzyTime));
        PlayerPrefs.SetInt("IsShieldTeim", BoolToInt(IsShieldTime));
        PlayerPrefs.SetInt("IsDashTime", BoolToInt(IsDashTime));

        PlayerPrefs.SetInt("IsSpeedUp", BoolToInt(IsSpeedUp));
        PlayerPrefs.SetInt("IsVisionOn", BoolToInt(IsVisionOn));
        PlayerPrefs.SetInt("IsLucky", BoolToInt(IsLucky));
    }

    public void LoadProgress() {
        coins = PlayerPrefs.GetInt("coin");

        IsUnlockFrenzy = IntToBool(PlayerPrefs.GetInt("IsFrenzyUnlock"));
        IsUnlockShield = IntToBool(PlayerPrefs.GetInt("IsShieldUnlock"));
        IsUnlockDash = IntToBool(PlayerPrefs.GetInt("IsDashUnlock"));

        IsFrenzyTime = IntToBool(PlayerPrefs.GetInt("IsFrenzyTime"));
        IsShieldTime = IntToBool(PlayerPrefs.GetInt("IsShieldTime"));
        IsDashTime = IntToBool(PlayerPrefs.GetInt("IsDashTime"));

        IsSpeedUp = IntToBool(PlayerPrefs.GetInt("IsSpeedUp"));
        IsVisionOn = IntToBool(PlayerPrefs.GetInt("IsVisionOn"));
        IsLucky = IntToBool(PlayerPrefs.GetInt("IsLucky"));
    }

    public void DeleteProgress() {
        PlayerPrefs.DeleteAll();
    }

    private int BoolToInt(bool boolean) {
        if (boolean) return 1;
        return 0;
    }

    private bool IntToBool(int integer) {
        if (integer > 0) return true;
        return false;
    }

    private void OnEnable() {
        Start();
    }

}