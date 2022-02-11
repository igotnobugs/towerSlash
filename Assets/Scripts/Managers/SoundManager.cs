using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> 
{
    public int soundSize;
    private List<AudioSource> audioSources;

    private void Start() 
	{
        audioSources = new List<AudioSource>();
        for (int i = 0; i < soundSize; i++) {
            GameObject newGameObject = new GameObject();
            AudioSource newAudioSource = newGameObject.AddComponent<AudioSource>();

            newGameObject.transform.parent = transform;
            audioSources.Add(newAudioSource);
        }
    }


    public void PlaySound(AudioClip clip, float volume) {
        for (int i = 0; i < audioSources.Count; i++) {
            if (!audioSources[i].isPlaying) {
                audioSources[i].clip = clip;
                audioSources[i].volume = volume;
                audioSources[i].Play();
                return;
            }
        }

        GameObject newGameObject = new GameObject();
        AudioSource newAudioSource = newGameObject.AddComponent<AudioSource>();

        newAudioSource.clip = clip;
        newAudioSource.volume = volume;
        newAudioSource.Play();

        newGameObject.transform.parent = transform;
        audioSources.Add(newAudioSource);

    }
}
