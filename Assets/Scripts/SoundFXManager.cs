using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;
    // Singleton pattern to ensure that functions can be called using SoundFXManager.Instance._________
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // General function to play sound clips
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign audioclip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLength = audioSource.clip.length;

        //destroy game object
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //assign a random index
        int rand = Random.Range(0, audioClip.Length);
        //spawn in gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign audioclip
        audioSource.clip = audioClip[rand];

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLength = audioSource.clip.length;

        //destroy game object
        Destroy(audioSource.gameObject, clipLength);
    }
}
