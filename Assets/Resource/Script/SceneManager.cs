using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void ChangeScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
    }

    public void LeaderboardBtn()
    {
        PlayFabScript.instance.GetLeaderboard();
    }
}
