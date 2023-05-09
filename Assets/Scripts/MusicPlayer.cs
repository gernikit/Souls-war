using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    static AudioSource audioSource;
    [SerializeField]
    static AudioClip nextClip;

    static float defaultVolume;

    [SerializeField]
    private AudioClip[] levelMusicsTemp;
    static AudioClip[] levelMusics;
    [SerializeField]
    private AudioClip[] levelWinTemp;
    static AudioClip[] levelWin;
    [SerializeField]
    private AudioClip[] levelLoseTemp;
    static AudioClip[] levelLose;

    static bool fadeTransition = false;

    [SerializeField]
    static float fadeSpeed = 5f;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        levelLose = levelLoseTemp;
        levelWin = levelWinTemp;
        levelMusics = levelMusicsTemp;

        System.Random rnd = new System.Random();
        PlayLevelMusic(rnd.Next(0, levelMusics.Length));
        defaultVolume = audioSource.volume;
    }
    private void Update()
    {
        if (fadeTransition)
            OnFadeTransition();
    }

    public static void PlayWinMusic()
    {
        System.Random rnd = new System.Random();
        nextClip = levelWin[rnd.Next(0, levelWin.Length)];
        fadeTransition = true;
    }

    public static void PlayLoseMusic()
    {
        System.Random rnd = new System.Random();
        nextClip = levelLose[rnd.Next(0, levelLose.Length)];
        fadeTransition = true;
    }

    private void PlayLevelMusic(int index)
    {
        audioSource.clip = levelMusics[index];
        audioSource.Play();
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
    static void FadeIn()
    {
        audioSource.volume = Mathf.Clamp(audioSource.volume + fadeSpeed, 0.1f, defaultVolume);
    }

    static void FadeOut()
    {
        audioSource.volume = Mathf.Clamp(audioSource.volume - fadeSpeed, 0.1f, defaultVolume);
    }
}
