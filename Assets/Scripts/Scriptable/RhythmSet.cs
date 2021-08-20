using UnityEngine;

[CreateAssetMenu(menuName = "Rhythm Set")]
public class RhythmSet : ScriptableObject 
{
    public AudioClip mainSong;
    public RhythmPattern[] patterns;

    public AudioClip tapBeat;
    public AudioClip tabEndMeasure;
}
