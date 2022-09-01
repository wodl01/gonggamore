using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayFabScript : MonoBehaviour
{
    public static PlayFabScript instance;
    private MenuManager menuManager;

    public GameObject rowPrefab;
    public Transform rowsParent;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Login();
    }

    void Login()
    {
        MenuManager.instance.loadingPanel.SetActive(true);
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        MenuManager.instance.loadingPanel.SetActive(false);
        Debug.Log("Successful Login Create!");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;

        menuManager = MenuManager.instance;
        if (name == null)
            menuManager.nickNamePanel.SetActive(true);
        else
        {
            menuManager.playerNickNameText.text = name;
            GameManager.instance.PlayerProfileUpdate();
        }
    }
    public void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = menuManager.nameInputField.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, NickNameError);
    }
    void NickNameError(PlayFabError error)
    {
        menuManager.nameErrorText.text = error.ErrorMessage;
    }


    void GetAppearance()
    {

    }
    void SaveAppearance(int lv, int exp)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Lv", lv.ToString() },
                { "Exp", exp.ToString() }
            }
        };
    }


    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result) 
    {
        Debug.Log("Update display name!");
        MenuManager.instance.debugText.text = "Update display name!";
        menuManager.nickNamePanel.SetActive(false);
    }

    void OnError(PlayFabError error)
    {
        MenuManager.instance.loadingPanel.SetActive(false);
        Debug.Log("Error whild loging in account!");
        Debug.Log(error.GenerateErrorReport());
        MenuManager.instance.debugText.text = error.GenerateErrorReport();
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard sent");
        //MenuManager.instance.debugText.text = "Successfull leaderboard sent";
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = "#" + (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = GameManager.TextChanger(item.StatValue) + "Á¡";

            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }
}
