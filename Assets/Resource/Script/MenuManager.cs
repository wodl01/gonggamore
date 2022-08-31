using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;


    private GameManager gameManager;
    private PlayFabScript playfabScript;

    [Header("Ui")]
    public GameObject loadingPanel;
    public Text debugText;
    [SerializeField] Text bestScoreText;
    public Text inGameMoneyText;
    public Text playerNickNameText;
    [SerializeField] Transform rowsParent;
    public GameObject leaderboardPanel;
    public GameObject nickNamePanel;
    public InputField nameInputField;
    public Text nameErrorText;

    public Text lvText;
    public Text gradeText;
    public Image profileThumbnailImage;
    public Text expPersentText;
    public Slider expSlider;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        playfabScript = PlayFabScript.instance;


        playfabScript.rowsParent = rowsParent;


        gameManager.PlayerProfileUpdate();
    }

    public void LeaderboardBtn()
    {
        gameManager.ActiveOb(leaderboardPanel);
        if (leaderboardPanel.activeSelf)
            playfabScript.GetLeaderboard();
    }

    public void NickNameConfirmBtn()
    {
        playfabScript.SubmitNameButton();
    }

    public void GetMoneyTest(int index)
    {
        gameManager.AddInGameMoneyValue(index);
    }
}
