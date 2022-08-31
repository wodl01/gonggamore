using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [SerializeField]
    private GameObject pausePanel;

    private GameManager gameManager;

    public GameObject itemBtn;

    public GameObject[] heartObs;
    public GameObject[] chickenObs;

    public bool isResurrentionPanelOn;
    public GameObject resurrectionPanel;
    public Button resurrectionAdBtn;
    public Button resurrectionMoneyBtn;

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
        if (isResurrentionPanelOn)
        {
            resurrectionPanelCool -= Time.deltaTime;
            if(resurrectionPanelCool < 0)
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
        if (active)
        {
            PauseGame(true);
            resurrectionMoneyBtn.interactable = gameManager.GetInGameMoneyValue() >= 5000000;
            resurrectionAdBtn.interactable = Application.internetReachability != NetworkReachability.NotReachable;

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

        PauseGame(false);
    }
    public void ResurrectionAdBtn()
    {
        gameManager.SetHpValue(4);

        PauseGame(false);

        gameManager.adActive = true;
    }
}
