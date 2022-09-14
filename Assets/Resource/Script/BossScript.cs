using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BossInfo
{
    public Sprite bossImage;
    public int bossHp;
    public string bossLine;
    public string information;
}
public class BossScript : MonoBehaviour
{
    public static BossScript instance;
    private GameManager gameManager;
    [SerializeField] PlayerScript playerScript;

    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;

    [Header("BossUi")]
    [SerializeField] Image bossHpBar;
    [SerializeField] Text bossLineText;
    [SerializeField] Image bossImage;
    [SerializeField] Text infomationText;

    [SerializeField] Animator bossAnimator;


    [Header("BossState")]
    private bool isBossCleared;
    private bool takeHp;
    [SerializeField]
    private bool stop;

    [SerializeField] GameObject[] bossObstacles;

    [Header("BossBalence")]
    [SerializeField] List<BossInfo> bossInfos = new List<BossInfo>();
    private BossInfo curBossInfo;

    [Header("BossObject")]
    [SerializeField]
    private GameObject bossSightOb;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void BossAppearEvent(int curStage)
    {
        curBossInfo = bossInfos[curStage];

        bossLineText.text = curBossInfo.bossLine;
        bossImage.sprite = curBossInfo.bossImage;
        infomationText.text = curBossInfo.information;

        maxHp = curBossInfo.bossHp;
        curHp = maxHp;

        isBossCleared = false;
        SetDeCreasseHp(0);
        UpdateBossHpBar();
        bossAnimator.SetInteger("BossCode", curStage);
        bossAnimator.SetBool("Clear", false);

        gameManager.summonType = -1;
    }

    void UpdateBossHpBar()
    {
        bossHpBar.fillAmount = ((float)curHp / (float)maxHp);
    }
    public void AddBossHpValue(float value)
    {
        curHp += value;

        UpdateBossHpBar();
        if (curHp <= 0 && !isBossCleared)
        {
            bossAnimator.SetInteger("BossCode", -1);
            bossAnimator.SetBool("Clear", true);
            isBossCleared = true;
            gameManager.BossClear();
        }
    }

    public void SetSummonType(int a)
    {
        gameManager.summonType = a;
    }
    public void SetDeCreasseHp(int active)
    {
        takeHp = active == 1 ? true : false;
    }

    public void SummonObjectToRandomPos(int obstacleCode)
    {
        if (stop) return;
        Vector3 pos = new Vector3(Random.Range(-3.2f, 3.2f), 7, 0);
        GameObject curObject = Instantiate(bossObstacles[obstacleCode], pos, Quaternion.identity);
        if(curObject.GetComponent<PoopScript>() != null)
        {
            curObject.GetComponent<PoopScript>().SummonEvent();
            curObject.GetComponent<PoopScript>().gameManager = gameManager;
        }
        if(curObject.GetComponent<ObjectFollowScript>() != null)
        {
            curObject.GetComponent<ObjectFollowScript>().target = playerScript.gameObject.transform;
        }
    }

    bool sightGoRight;
    bool sightOn;
    float sightSpeed;

    public void SightActive(int index)
    {
        sightOn = index == 1 ? true : false;
        if (index == 1)
            bossSightOb.transform.position = new Vector3(Random.Range(-0.1f, 0.1f), 6, 0);
        sightSpeed = 0;
    }
    public void SightObUpdate()
    {
        sightGoRight = bossSightOb.transform.position.x < 0;
        sightSpeed = Random.Range(0.02f, 0.04f);
    }

    public void StopBoss(bool active)
    {
        stop = active;
        bossAnimator.speed = active ? 0 : 1;
    }

    public void PlaySound(string sound)
    {
        SoundManager.instance.Play(sound);
    }

    [SerializeField] float rightBorder;
    [SerializeField] float leftBorder;
    private void FixedUpdate()
    {
        if (takeHp && !stop)
            AddBossHpValue(-Time.deltaTime);

        if (sightOn && !stop)
        {
            bossSightOb.transform.Translate(new Vector3(sightGoRight ? sightSpeed : -sightSpeed, 0, 0));
            Vector3 worldPos = Camera.main.WorldToViewportPoint(bossSightOb.transform.position);
            if (worldPos.x < leftBorder) worldPos.x = leftBorder;
            if (worldPos.x > rightBorder) worldPos.x = rightBorder;
            bossSightOb.transform.position = Camera.main.ViewportToWorldPoint(worldPos);
        }
    }
}
