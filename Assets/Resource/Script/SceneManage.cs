using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public static SceneManage instance;
    private void Awake()
    {
        instance = this;
    }
    public void ChangeScene(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }

    public void LeaderboardBtn()
    {
        PlayFabScript.instance.GetLeaderboard();
    }
}
