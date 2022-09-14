using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public void PlaySound(string sound)
    {
        SoundManager.instance.Play(sound);
    }
    public void GameStart()
    {
        GameManager.instance.GameStart();
    }
    public void SkipStory()
    {

        GameManager.instance.GameStart();
        gameObject.SetActive(false);
    }
}
