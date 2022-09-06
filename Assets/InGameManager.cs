using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    [SerializeField]
    private BossScript bossScript;

    [SerializeField]
    private GameObject pausePanel;

    private GameManager gameManager;
    [SerializeField]
    private PlayerScript playerScript;

    public GameObject itemBtn;

    public GameObject[] heartObs;
    public GameObject[] chickenObs;

    public bool isResurrentionPanelOn;
    public GameObject resurrectionPanel;
    public Button resurrectionAdBtn;
    public Button resurrectionMoneyBtn;
    public Slider timsSlider;
    public GameObject messageOb;
    public GameObject scoreXText;

    [SerializeField]
    private float resurrectionPanelCool;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.instance;

        ItemBtnUpdate();
    }

    public void PauseGame(bool active)
    {
        Time.timeScale = active ? 0 : 1;
        gameManager.isPauseGame = active;
    }

    public void Update()
    {
#if UNITY_ANDROID
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    PausePanelOn(!gameManager.isPauseGame);
#endif
        if (resurrectionPanelCool > 0)
        {
            resurrectionPanelCool -= Time.deltaTime;
            timsSlider.value = resurrectionPanelCool / 10;
            if (resurrectionPanelCool < 0)
                gameManager.CheckGameOver();
        }
    }



    public void InputItemBtn()
    {
        gameManager.UseItem(0);

        ItemBtnUpdate();
    }

    void ItemBtnUpdate()
    {
        itemBtn.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = gameManager.GetItemAmount(2).ToString();

        itemBtn.SetActive(gameManager.GetItemAmount(2) > 0);
    }

    public void ResurrectionChancePanel(bool active)
    {
        resurrectionPanel.SetActive(active);
        if (gameManager.bossTime)
            bossScript.StopBoss(active);
        if (active)
        {

            gameManager.DestroyAllPrefaps();
            resurrectionPanel.transform.GetChild(3).GetComponent<Text>().text = "ÅëÀåÀÜ°í:" + GameManager.TextChanger(gameManager.GetInGameMoneyValue()) + "¿ø";

            resurrectionMoneyBtn.interactable = gameManager.GetInGameMoneyValue() >= 5000000;
            resurrectionAdBtn.interactable = GetComponent<RewardAd>().rewardAd.IsLoaded();

            if(resurrectionMoneyBtn.interactable || resurrectionAdBtn.interactable)
            {
                resurrectionPanelCool = 10;
                isResurrentionPanelOn = true;
            }
            else
            {
                gameManager.CheckGameOver();
            }
        }
    }
    public void ResurrectionMoneyBtn()
    {
        gameManager.AddInGameMoneyValue(-5000000);

        gameManager.SetHpValue(3);
    }
    public void ResurrectionAdBtn()
    {
        gameManager.adActive = true;

        gameManager.SetHpValue(4);
    }

    public void InputResurrectionBtn()
    {
        gameManager.Resurrent();

        playerScript.SetInvincibility(true);

        ResurrectionChancePanel(false);
    }
}
