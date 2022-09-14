using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] GameObject bgmGroupOb;
    public AudioSource[] audioSource;
    public AudioSource[] bgmSource;
    public Dictionary<string, AudioSource> sounds;
    public Dictionary<int, AudioSource> bgms;

    [SerializeField]
    public static AudioSource curBgm;

    public static bool randomBgm;


    private void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        audioSource = GetComponents<AudioSource>();
        bgmSource = bgmGroupOb.GetComponents<AudioSource>();
        sounds = new Dictionary<string, AudioSource>();
        bgms = new Dictionary<int, AudioSource>();

        bgms.Add(0, bgmSource[0]);
        bgms.Add(1, bgmSource[1]);
        bgms.Add(2, bgmSource[2]);
        bgms.Add(3, bgmSource[3]);
        bgms.Add(4, bgmSource[4]);
        bgms.Add(5, bgmSource[5]);
        bgms.Add(6, bgmSource[6]);
        bgms.Add(7, bgmSource[7]);
        bgms.Add(8, bgmSource[8]);


        sounds.Add("Poop", audioSource[0]);
        sounds.Add("CoinGet", audioSource[1]);
        sounds.Add("Hit", audioSource[2]);
        sounds.Add("CardGet", audioSource[3]);
        sounds.Add("CoffeeGet", audioSource[4]);
        sounds.Add("NoHit", audioSource[5]);
        sounds.Add("BossAppear", audioSource[6]);
        sounds.Add("GameOver", audioSource[7]);

    }
    //  SoundManager.Play("Effect_Getcombine");

    public void PlayBGM(int BGMName)
    {
        randomBgm = false;
        for (int i = 0; i < bgms.Count; i++)
        {
            bgms[i].Stop();
        }

        curBgm = bgms[BGMName];
        curBgm.Play();

    }

    public void PlayRandomBGM()
    {
        randomBgm = true;
        for (int i = 0; i < bgms.Count; i++)
        {
            bgms[i].Stop();
        }
        curBgm = bgms[Random.Range(1, 9)];
        curBgm.Play();
    }

    public void Play(string soundName)
    {
        if (sounds.ContainsKey(soundName))
        {
            sounds[soundName].Play();
        }
    }

    private void FixedUpdate()
    {
        if (randomBgm && !curBgm.isPlaying)
        {
            PlayRandomBGM();
        }
    }
}
