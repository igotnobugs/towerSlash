using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeatStatus { None, Early, Perfect, Late }

public class BeatManager : Singleton<BeatManager>
{
    public float bpm;
    public int beatPerMeasure;

    //Divide beatInterval by ratio
    //Start earlybeat, beatInterval * ratio early.
    //start 
    public float earlyBeatRatio = 0.3f;
    //ratio from earlyBeat to treat it as a full beat
    public float beatGracePeriod = 0.5f;

    public static bool startBeating = false;
    public static bool beat;
    public static bool beatD8;
    public static int beatCount;
    public static int beatCountD8;
    public static bool beatMeasure;
    public static int beatMeasureCount;
    public static BeatStatus beatStatus = BeatStatus.None;

    public static float beatMeasureInterval;
    public static float beatInterval;
    private float beatTimer;
    private float beatIntervalD8;
    private float beatTimerD8;

    public int delayBeat;
    public bool startWithABeat;

    public static bool beatStarts = false;
    public static bool earlyBeat = false;
    public static bool lateBeat = false;

    private IEnumerator coroutine_beatChecker;
    private bool routineStarted = false;

    float timeStartCheckforEarly;// = beatInterval - b; // 0.2319 //start checking for early at this time
    float timeStartCheckForPerfect;// = timeStartCheckforEarly + a; // 0.25095 //startChecking for perfect
    float timeStartCheckForLate;// = timeStartCheckForPerfect + a + a; // 0.28905 //start checking for late
    float timeToStopChecking;// = beatInterval + b; // 0.308 //stop checking 


    public delegate void BeatEvent();
    public event BeatEvent OnBeat;

    private void Start() 
	{
        beatCount = 0;
        Mathf.Clamp(earlyBeatRatio, 0, 1);
    }

    private void Update() 
	{
        if (!startBeating) return;
        DetectBeat();
    }

    private void DetectBeat() {
        beat = false;
        beatMeasure = false;

        beatInterval = 60 / bpm;
        beatTimer += Time.deltaTime;

        beatMeasureInterval = beatInterval * beatPerMeasure;

        //Delay to avoid stutter when loading the music I guess
        if (delayBeat > 0) {       
            if (beatTimer >= beatInterval) {
                beatTimer -= beatInterval;
                delayBeat--;
            }
            return;
        }

        beatStarts = true;

        if (startWithABeat && beatCount == 0) { 
            beatTimer = beatInterval;
        }

        if (beatTimer >= beatInterval) {

            float b = beatInterval * earlyBeatRatio; 
            float a = b * beatGracePeriod; 

            timeStartCheckforEarly = beatInterval - b; 
            timeStartCheckForPerfect = a; 
            timeStartCheckForLate = a + a; 
            timeToStopChecking = b; 


            if (!routineStarted) {
                routineStarted = true;
                coroutine_beatChecker = BeatCycles();
                StartCoroutine(coroutine_beatChecker);
            }

            beatTimer -= beatInterval;
            beat = true;
            OnBeat?.Invoke();
            beatCount++;

            if (beatCount % beatPerMeasure == 0) {
                beatMeasure = true;           
                beatMeasureCount++;
            }

        }

        /*
        beatD8 = false;
        beatIntervalD8 = beatInterval / 8;
        beatTimerD8 += Time.deltaTime;

        if (startWithABeat) {
            beatTimerD8 = beatIntervalD8;       
        }

        if (beatTimerD8 >= beatIntervalD8) {
            beatTimerD8 -= beatIntervalD8;
            beatD8 = true;
            beatCountD8++;
        }
        */
    }

    private IEnumerator BeatCycles() {

        float b = beatInterval * earlyBeatRatio; //0.27 * 0.3 = 0.0381
        float a = b * beatGracePeriod; // 0.0381 * 0.5 = 0.01905

        timeStartCheckforEarly = beatInterval - b; // 0.2319 //start checking for early at this time
        timeStartCheckForPerfect = a; // 0.25095 //startChecking for perfect
        timeStartCheckForLate = a + a; // 0.28905 //start checking for late
        timeToStopChecking = b; // 0.308 //stop checking 

        for (; ; ) {


            beatStatus = BeatStatus.None;
            //Debug.Log(beatStatus);
            yield return new WaitForSeconds(timeStartCheckforEarly);// 0.0381
            beatStatus = BeatStatus.Early;
            //Debug.Log(beatStatus);
            yield return new WaitForSeconds(timeStartCheckForPerfect);
            beatStatus = BeatStatus.Perfect;
            //Debug.Log(beatStatus);
            yield return new WaitForSeconds(timeStartCheckForLate);
            beatStatus = BeatStatus.Late;
            //Debug.Log(beatStatus);
            yield return new WaitForSeconds(timeToStopChecking);
            beatStatus = BeatStatus.None;
            //Debug.Log(beatStatus);
        }
    }

}
