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

    public GameObject UI;
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
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        UI.SetActive(false);
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
            if (resurrectionPanelCool < 0 && isResurrentionPanelOn)
            {
                SoundManager.instance.PlayRandomBGM();
                SceneManage.instance.ChangeScene(2);
            }

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
            SoundManager.instance.PlayBGM(0);
        }

    }
    public void ResurrectionMoneyBtn()
    {
        gameManager.AddInGameMoneyValue(-5000000);
        isResurrentionPanelOn = false;
        gameManager.SetHpValue(3);
    }
    public void ResurrectionAdBtn()
    {
        gameManager.adActive = true;
        isResurrentionPanelOn = false;
        gameManager.SetHpValue(3);
    }

    public void InputResurrectionBtn()
    {
        gameManager.Resurrent();

        playerScript.SetInvincibility(true);

        ResurrectionChancePanel(false);

        SoundManager.instance. PlayRandomBGM();
    }
}
