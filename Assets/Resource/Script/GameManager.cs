using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using System.Security.Cryptography;
//using System.Numerics;

[System.Serializable]
public class UserDatas
{
    public string deviceId;
    public bool removeAd;
    public int inGameMoney;
    public int curPlayerLv;
    public int curExp;

    public int maxHeartItemAmount;
    public int chickenItemAmount;
    public int noHitItemAmount;
    public int bonusItemAmount;

    public int bestScore;
}



public class GameManager : MonoBehaviour
{
    public string userName;
    public static string playfabid;

    public static GameManager instance;
    private MenuManager menuManager;
    private PlayFabScript playFabScript;
    private PlayerScript playerScript;
    private BossScript bossScript;
    private InGameManager inGameManager;
    public bool testMode;
    private bool gameStart = false;

    [SerializeField] private int hp;
    [SerializeField] private int chicken;
    [SerializeField] private int money;
    [SerializeField] private int score = 0;

    [SerializeField] private int[] minScoreToNextStage;

    [SerializeField]
    private GameObject poopPrefap;
    [SerializeField]
    private GameObject moneyPrefap;
    [SerializeField]
    private GameObject money2Prefap;
    [SerializeField]
    private GameObject money3Prefap;
    [SerializeField]
    private GameObject cardPrefap;
    [SerializeField]
    private GameObject coffeePrefap;
    [SerializeField]
    private GameObject virusPrefap;
    [SerializeField]
    private GameObject chickenPrefap;
    [SerializeField]
    private List<GameObject> prefapList = new List<GameObject>();

    [Header("CoolTimes")]
    [SerializeField] private float maxfeverTime;
    [SerializeField] private float curFeverTime;

    [SerializeField] private float maxChickenTime;
    private float curChickenTime;

    public bool isCoffeeTime;
    [SerializeField] private float maxCoffeeTime;
    private float curCoffeeTime;

    public bool isFastTime;
    [SerializeField] private float maxFastTime;
    private float curFastTime;

    public bool isReverse;
    [SerializeField] private float maxReverseTime;
    private float curReverseTime;

    [SerializeField] private float maxRepeatBossCoolTime;
    [SerializeField] float curRepeatBossCoolTime;

    [Header("CurStatues")]
    [SerializeField]
    public int summonType;
    [SerializeField]
    public bool bossTime;
    [SerializeField]
    private bool resurrectionChance;
    [SerializeField]
    private bool isGameOver;
    public bool adActive;
    
    [Header("Ui")]
    [SerializeField] Text scoreText;
    [SerializeField] Text moneyText;

    [Header("Stage")]
    public int curStage;
    public bool isPauseGame;
    public bool isRepeatMode;
    [SerializeField] private List<UnityEngine.Vector3> gravityPerStage;

    [Header("PlayerLvBalence")]
    [SerializeField] int[] maxExp;
    [SerializeField] Sprite[] profileThumbnails;
    [SerializeField] string[] gradeName;
    [SerializeField] string[] abilityInfo;
    [SerializeField] float[] bonusValue;

    public UserDatas userData;
    [SerializeField]
    string filePath;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        playFabScript = GetComponent<PlayFabScript>();
    }
    private void Start()
    {
        filePath = Application.persistentDataPath + "/WM3ko61Q0t83Uimdwp9ArhkmVE32TvV2.txt";
        SoundManager.instance.PlayRandomBGM();
#if UNITY_EDITOR
        testMode = true;
#endif

        LoadData();

        PlayerProfileUpdate();
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(userData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        string code = System.Convert.ToBase64String(bytes);

        File.WriteAllText(filePath, code);
    }
    public void LoadData()
    {
        if (!File.Exists(filePath)) { ResetDatas(); return; };

        string code = File.ReadAllText(filePath);

        byte[] bytes = System.Convert.FromBase64String(code);
        string jdata = System.Text.Encoding.UTF8.GetString(bytes);

        userData = JsonUtility.FromJson<UserDatas>(jdata);

        menuManager.startBtn.interactable = userData.deviceId == SystemInfo.deviceUniqueIdentifier;
    }
    public void ResetDatas()
    {
        userData = new UserDatas();
        userData.deviceId = SystemInfo.deviceUniqueIdentifier;

        menuManager.startBtn.interactable = userData.deviceId == SystemInfo.deviceUniqueIdentifier;
        SaveData();
        LoadData();
    }
    public void PlayerProfileUpdate()
    {
        menuManager = MenuManager.instance;

        menuManager.inGameMoneyText.text = "통장잔고:" + TextChanger(userData.inGameMoney) + "원";
        menuManager.profileThumbnailImage.sprite = profileThumbnails[userData.curPlayerLv];
        menuManager.gradeText.text = gradeName[userData.curPlayerLv];
        menuManager.lvText.text = "Lv." + userData.curPlayerLv.ToString();
        menuManager.playerAbilityInfoText.text = abilityInfo[userData.curPlayerLv];
        menuManager.playerAbilityTitleText.text = "최종점수 +" + bonusValue[userData.curPlayerLv] + "%";
        menuManager.profileThumbnailImage.sprite = profileThumbnails[userData.curPlayerLv];
        menuManager.expPersentText.text = string.Format("{0:0.#}", (((float)userData.curExp / (float)maxExp[userData.curPlayerLv]) * 100)) + "%";
        menuManager.expSlider.value = ((float)userData.curExp / (float)maxExp[userData.curPlayerLv]);

        if (PlayFabClientAPI.IsClientLoggedIn())
            menuManager.playerNickNameText.text = userName;
    }

    public void GameStart()
    {
        bossScript = BossScript.instance;
        inGameManager = InGameManager.instance;
        inGameManager.UI.SetActive(true);
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        Time.timeScale = 1;
        isPauseGame = false;
        gameStart = true;
        isGameOver = false;
        isRepeatMode = false;
        
        bossTime = false;
        adActive = false;
        resurrectionChance = true;
        summonType = 0;
        curStage = 0;

        inGameManager.scoreXText.SetActive(!PlayFabClientAPI.IsClientLoggedIn());

        curRepeatBossCoolTime = maxRepeatBossCoolTime;
        curPoopCool = Random.Range(0.2f, 1f);
        curMoneyCool = Random.Range(0.5f, 2f);
        curCardCool = Random.Range(15f, 50f);
        curCoffeeCool = Random.Range(15f, 50f);
        curVirusCool = Random.Range(15f, 50f);

        score = 0;
        money = 0;

        SetHpValue(3 + userData.maxHeartItemAmount);
        AddScoreValue(0);
        AddMoneyValue(0);

        if (userData.chickenItemAmount > 0)
        {
            userData.chickenItemAmount--;
            summonType = 2;

            curChickenTime = maxChickenTime;
            restTime = 4;

            Message("오늘은 치맥이다!!");
        }

        userData.maxHeartItemAmount = 0;



        SaveData();
    }

    public void FeverTimeStart()
    {
        DestroyAllPrefaps();
        summonType = 1;
        restTime = 4;
        curFeverTime = maxfeverTime;

        Message("법인카드를 얻었다!!");
    }
    public void CoffeeTimeStart()
    {
        isCoffeeTime = true;
        curCoffeeTime = maxCoffeeTime;
        playerScript.coffeeParticle.Play();
    }
    public void ReverceTimeStart()
    {
        isReverse = true;
        curReverseTime = maxReverseTime;
        playerScript.revertParticle.Play();
    }
    public void TimeFasterStart()
    {
        isFastTime = true;
        curFastTime = maxFastTime;
        playerScript.timeParticle.Play();
    }
    public void HitVirus()
    {
        Application.Quit();
    }
    #region Prefaps

    private void SummonPoop()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(poopPrefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().fallRandomRange = gravityPerStage[curStage];
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonMoney()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(moneyPrefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonFeverMoney()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(money2Prefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonFeverMoney2()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(money3Prefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonChicken()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(chickenPrefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonCard()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(cardPrefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonCoffee()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(coffeePrefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }
    public void SummonVirus()
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject prefap = Instantiate(virusPrefap, pos, UnityEngine.Quaternion.identity);
        prefapList.Add(prefap);
        prefap.GetComponent<PoopScript>().gameManager = this;
        prefap.GetComponent<PoopScript>().SummonEvent();
    }

    public void RemovePrefapList(GameObject target)
    {
        prefapList.Remove(target);
    }
    public void DestroyAllPrefaps()
    {
        for (int i = 0; i < prefapList.Count; i++)
        {
            Destroy(prefapList[i]);
        }
        prefapList = new List<GameObject>();
    }
    #endregion

    #region ValueChange

    public void GetDamaged()
    {
        if (chicken > 0)
            chicken--;
        else
            hp--;

        HeartIconUpdate();

        CheckGameOver();
    }
    public void AddHpValue(int index, bool isChicken)
    {
        if (isChicken)
            chicken += index;
        else
            hp += index;

        HeartIconUpdate();

    }
    public void HeartIconUpdate()
    {
        for (int i = 0; i < inGameManager.chickenObs.Length; i++)
        {
            inGameManager.chickenObs[i].SetActive(false);
        }
        if (chicken >= 0)
            for (int i = 0; i < chicken; i++)
            {
                inGameManager.chickenObs[i].SetActive(true);
            }
        for (int i = 0; i < inGameManager.heartObs.Length; i++)
        {
            inGameManager.heartObs[i].SetActive(false);
        }
        if (hp >= 0)
            for (int i = 0; i < hp; i++)
            {
                inGameManager.heartObs[i].SetActive(true);
            }
    }
    public void SetHpValue(int index)
    {
        hp = index;
        CheckGameOver();

        HeartIconUpdate();
    }
    public void AddScoreValue(int index)
    {
        score += index;

        scoreText.text = score.ToString();

        if (!isRepeatMode)
            CheckNextStage();
    }
    public void AddMoneyValue(int index)
    {
        money += index;

        if (money <= 0)
            money = 0;

        if (money == 0)
            moneyText.text = "0원";
        else
            moneyText.text = TextChanger(money) + "원";
    }
    public void AddExp(int index)
    {
        userData.curExp += index;

        if (userData.curExp > maxExp[userData.curPlayerLv])
        {
            userData.curExp = 0;
            userData.curPlayerLv++;
        }
        SaveData();
    }
    public void AddInGameMoneyValue(int index)
    {
        userData.inGameMoney += index;

        if (money < 0)
            userData.inGameMoney = 0;

        if(SceneManager.GetActiveScene().buildIndex == 0)
        menuManager.inGameMoneyText.text = "통장잔고:" + TextChanger(userData.inGameMoney) + "원";
    }
    public int GetInGameMoneyValue()
    {
        return userData.inGameMoney;
    }
    public void AddItemAmount(int itemCode, int itemAmount)
    {
        switch (itemCode)
        {
            case 0:
                userData.maxHeartItemAmount += itemAmount;
                break;
            case 1:
                userData.chickenItemAmount += itemAmount;
                break;
            case 2:
                userData.noHitItemAmount += itemAmount;
                break;
            case 3:
                userData.bonusItemAmount += itemAmount;
                break;
        }

    }
    public int GetItemAmount(int itemCode)
    {
        int value = 0;
        switch (itemCode)
        {
            case 0:
                value = userData.maxHeartItemAmount;
                break;
            case 1:
                value = userData.chickenItemAmount;
                break;
            case 2:
                value = userData.noHitItemAmount;
                break;
            case 3:
                value = userData.bonusItemAmount;
                break;
        }
        return value;
    }
    #endregion;

    private static string[] goldUnitArr = new string[] { "", "만", "억" };
    public static string TextChanger(int num)
    {
        if (num == 0) return "0";
        int placeN = 4;
        int value = num;
        List<int> numList = new List<int>();
        int p = (int)Mathf.Pow(10, placeN);

        do
        {
            numList.Add((int)(value % p));
            value /= p;
        }
        while (value >= 1);
        string retStr = "";
        for (int i = 0; i < numList.Count; i++)
        {
            if(numList[i] != 0)
            retStr = numList[i] + goldUnitArr[i] + retStr;
        }

        return retStr;
    }

    void CheckNextStage()
    {
        if (score >= minScoreToNextStage[curStage] && summonType == 0 && !bossTime)
        {
            int bossCode = curStage;
            DestroyAllPrefaps();
            bossScript.BossAppearEvent(bossCode);
            bossTime = true;
        }
    }
    public void BossClear()
    {
        summonType = 0;
        bossTime = false;
        


        if (!isRepeatMode && curStage == minScoreToNextStage.Length - 1)
        {
            Message("그것은 모두 꿈이였다(하드모드)");
            isRepeatMode = true;
        }


        if (!isRepeatMode)
            curStage++;
    }
    bool isAdActing;
    public void CheckGameOver()
    {
        if (hp < 1)
        {
            if (resurrectionChance)
            {
                isPauseGame = true;
                gameStart = false;
                resurrectionChance = false;
                if(inGameManager.GetComponent<RewardAd>().rewardAd.IsLoaded() || userData.inGameMoney >= 5000000)
                {
                    inGameManager.ResurrectionChancePanel(true);
                    return;
                }
                else
                {
                    SceneManage.instance.ChangeScene(2);
                }
            }
            else
            {
                if (adActive)
                {
                    adActive = false;
                    inGameManager.GetComponent<RewardAd>().ShowRewardAd();
                }
                else
                {
                    SceneManage.instance.ChangeScene(2);
                }
            }
        }
    }
    public void Resurrent()
    {
        isPauseGame = false;
        gameStart = true;
    }

    public void GameOver()
    {
        SettingFinalPanel();
 
    }
    public void SettingFinalPanel()
    {
        gameStart = false;
        isGameOver = true;
        FinalScript finalScript = FinalScript.instance;

        if (userData.bestScore < score + money)
            finalScript.newBestText.SetActive(true);



        userData.inGameMoney += money;

        int finalScore = 0;

        finalScript.ScoreText.text = "점수:" + TextChanger(score).ToString();
        finalScript.finalMoneyText.text = "+" + TextChanger(money) + "원";
        finalScore = score + money;

        int bonusUserValue = Mathf.RoundToInt((bonusValue[userData.curPlayerLv] / 100) * finalScore);
        finalScript.bonusScoreText2.text = "캐릭터 보너스 +" + bonusUserValue + "점";
        finalScore += bonusUserValue;
        if (userData.bonusItemAmount > 0)
        {
            userData.bonusItemAmount--;
            finalScript.bonusScoreText.gameObject.SetActive(true);
            finalScript.bonusScoreText.text = "보너스 아이템!" + TextChanger(finalScore) + "X2";
            finalScore += finalScore;
        }
        else finalScript.bonusScoreText.gameObject.SetActive(false);

        finalScript.finalScoreText.text = "최종점수:" + TextChanger(finalScore).ToString() + "점!";

        AddExp(Mathf.RoundToInt((float)score / 100));

        SaveData();

        playFabScript.SendLeaderboard(finalScore);


    }
    public void UseItem(int itemCode)
    {
        switch (itemCode)
        {
            case 0:
                if(userData.noHitItemAmount > 0 && !playerScript.isInvincibility)
                {
                    playerScript.SetInvincibility(true);
                    playerScript.barrier.SetActive(true);
                    userData.noHitItemAmount--;

                    SoundManager.instance.Play("NoHit");
                }
                break;
        }
        SaveData();
    }

    string messageInfo;
    public void Message(string message)
    {
        messageInfo = message;
        StartCoroutine(MessageActive());

    }
    IEnumerator MessageActive()
    {
        inGameManager.messageOb.GetComponent<Text>().text = messageInfo;
        inGameManager.messageOb.SetActive(true);
        yield return new WaitForSeconds(3);
        inGameManager.messageOb.SetActive(false);
    }

    private float curPoopCool;
    private float curMoneyCool;
    private float curMoney1Cool;
    private float curCardCool;
    private float curCoffeeCool;
    private float curVirusCool;

    [SerializeField]private float restTime;

    private void FixedUpdate()
    {
        if (isRepeatMode)
        {
            if(!bossTime)
            curRepeatBossCoolTime -= Time.deltaTime;
            if(curRepeatBossCoolTime < 0 && summonType != 1)
            {
                DestroyAllPrefaps();
                curRepeatBossCoolTime = maxRepeatBossCoolTime;
                summonType = -1;
                bossScript.BossAppearEvent(Random.Range(0, 10));
                bossTime = true;
            }
        }
        if (gameStart)
        {
            if (isCoffeeTime)
            {
                curCoffeeTime -= Time.deltaTime;
                if (curCoffeeTime < 0)
                {
                    isCoffeeTime = false;
                    playerScript.coffeeParticle.Stop();
                }
            }
            if (isFastTime)
            {
                curFastTime -= Time.deltaTime;
                if (curFastTime < 0)
                {
                    isFastTime = false;
                    playerScript.timeParticle.Stop();
                }
            }
            if (isReverse)
            {
                curReverseTime -= Time.deltaTime;
                if (curReverseTime < 0)
                {
                    isReverse = false;
                    playerScript.revertParticle.Stop();
                }
            }
            switch (summonType)
            {
                case 0:
                    curPoopCool -= Time.deltaTime;
                    curMoneyCool -= Time.deltaTime;
                    curCardCool -= Time.deltaTime;
                    curCoffeeCool -= Time.deltaTime;
                    curVirusCool -= Time.deltaTime;
                    if (curPoopCool < 0)
                    {
                        float randomNum = Random.Range(gravityPerStage[curStage].z - 0.2f, gravityPerStage[curStage].z + 0.2f);
                        int index = isCoffeeTime ? 2 : 1;
                        SummonPoop();
                        curPoopCool = randomNum * index;
                    }
                    if (curMoneyCool < 0)
                    {
                        float randomNum = Random.Range(0.5f, 2f);
                        SummonMoney();
                        curMoneyCool = randomNum;
                    }
                    if (curCardCool < 0)
                    {
                        float randomNum = Random.Range(25f, 50f);
                        if (!bossTime)
                            SummonCard();
                        curCardCool = randomNum;
                    }
                    if (curCoffeeCool < 0)
                    {
                        float randomNum = Random.Range(25f, 50f);
                        SummonCoffee();
                        curCoffeeCool = randomNum;
                    }
                    if (curVirusCool < 0)
                    {
                        float randomNum = Random.Range(25f, 50f);
                        SummonVirus();
                        curVirusCool = randomNum;
                    }
                    break;
                case 1:
                    curFeverTime -= Time.deltaTime;
                    if (curFeverTime < 0)
                    {
                        restTime -= Time.deltaTime;
                        if (restTime < 0)
                            summonType = 0;
                    }
                    else
                    {
                        curMoneyCool -= Time.deltaTime;
                        curMoney1Cool -= Time.deltaTime;
                        if (curMoneyCool < 0)
                        {
                            SummonFeverMoney();
                            curMoneyCool = 0.08f;
                        }
                        if (curMoney1Cool < 0)
                        {
                            SummonFeverMoney2();
                            curMoney1Cool = 0.1f;
                        }
                    }
                    break;
                case 2:
                    curChickenTime -= Time.deltaTime;
                    if (curChickenTime < 0)
                    {
                        restTime -= Time.deltaTime;
                        if (restTime < 0)
                            summonType = 0;
                    }
                    else
                    {
                        curMoneyCool -= Time.deltaTime;
                        curMoney1Cool -= Time.deltaTime;
                        if (curMoneyCool < 0)
                        {
                            SummonChicken();
                            curMoneyCool = 0.05f;
                        }
                    }
                    break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7)) PlayerPrefs.DeleteAll();
    }

    public void ActiveOb(GameObject target)
    {
        target.SetActive(!target.activeSelf);
    }
}
