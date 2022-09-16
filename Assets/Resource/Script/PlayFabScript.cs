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

        Login();
    }

    public void Login()
    {
        if (PlayFabClientAPI.IsClientLoggedIn()) return;

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

        GameManager.playfabid = result.PlayFabId;


        Debug.Log("Successful Login Create!");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;

        menuManager = MenuManager.instance;
        if (name == null)
        {
            menuManager.nickNameCode.text = "(" + SystemInfo.deviceUniqueIdentifier.Substring(0, 2) + ")";
            menuManager.nickNamePanel.SetActive(true);

        }

        else
        {
            menuManager.playerNickNameText.text = name;
            GameManager.instance.userName = name;
            GameManager.instance.PlayerProfileUpdate();

        }
    }
    public void SubmitNameButton()
    {
        if (menuManager.nameInputField.text.Length < 3) return;
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = menuManager.nameInputField.text + "(" + SystemInfo.deviceUniqueIdentifier.Substring(0,2) + ")",
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, NickNameError);
    }
    void NickNameError(PlayFabError error)
    {
        menuManager.nameErrorText.text = error.ErrorMessage;
    }



    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result) 
    {
        Debug.Log("Update display name!");
        menuManager.nickNamePanel.SetActive(false);
        menuManager.playerNickNameText.text = menuManager.nameInputField.text + "(" + SystemInfo.deviceUniqueIdentifier.Substring(0, 2) + ")";
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

    public void GetLeaderboardAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Score",
            MaxResultsCount = 9
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, OnError);
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
            texts[2].text = GameManager.TextChanger(item.StatValue);

            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }

    void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult result)
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
            texts[2].text = GameManager.TextChanger(item.StatValue);
            if (item.PlayFabId == GameManager.playfabid)
            {
                texts[0].color = new Color(1, 0, 0.56f);
                texts[1].color = new Color(1, 0, 0.56f);
                texts[2].color = new Color(1, 0, 0.56f);
            }
        }
    }
}
