using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    static AudioSource audioSource;
    [SerializeField]
    static AudioClip nextClip;

    static float defaultVolume;

    [SerializeField]
    AudioClip[] levelMusicsTemp;
    static AudioClip[] levelMusics;
    [SerializeField]
    AudioClip[] levelWinTemp;
    static AudioClip[] levelWin;
    [SerializeField]
    AudioClip[] levelLoseTemp;
    static AudioClip[] levelLose;

    static bool fadeTransition = false;

    [SerializeField]
    static float fadeSpeed = 5f;//0.009f


    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        levelLose = levelLoseTemp;
        levelWin = levelWinTemp;
        levelMusics = levelMusicsTemp;

        System.Random rnd = new System.Random();
        PlayLevelMusic(rnd.Next(0, levelMusics.Length));
        defaultVolume = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeTransition)
            OnFadeTransition();
    }

    void PlayLevelMusic(int index)
    {
        audioSource.clip = levelMusics[index];
        audioSource.Play();
    }

    public static void PlayWinMusic()
    {
        System.Random rnd = new System.Random();
        nextClip = levelWin[rnd.Next(0, levelLose.Length)];
        fadeTransition = true;
    }

    public static void PlayLoseMusic()
    {
        System.Random rnd = new System.Random();
        nextClip = levelLose[rnd.Next(0, levelLose.Length)];
        fadeTransition = true;
    }

    static void FadeIn()
    {
        audioSource.volume = Mathf.Clamp(audioSource.volume + fadeSpeed, 0.1f, defaultVolume);
    }

    static void FadeOut()
    {
        audioSource.volume = Mathf.Clamp(audioSource.volume - fadeSpeed, 0.1f, defaultVolume);
    }

    static void OnFadeTransition()
    {
        if (audioSource.volume > 0.1f && audioSource.clip != nextClip)
            FadeOut();
        else
        {
            if (audioSource.clip != nextClip)
            {
                audioSource.clip = nextClip;
                audioSource.Play();
            }

            FadeIn();

            if (audioSource.volume >= defaultVolume)
                fadeTransition = false;
        }

    }
}
