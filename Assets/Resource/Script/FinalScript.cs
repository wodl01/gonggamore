using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScript : MonoBehaviour
{
    public static FinalScript instance;

    public GameObject newBestText;

    public Text ScoreText;
    public Text finalMoneyText;
    public Text finalScoreText;
    public Text bonusScoreText;
    public Text bonusScoreText2;

    [SerializeField]
    public Animator animator;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        /*GetComponent<RewardAd>().ShowRewardAd();
        if (GameManager.instance.adActive)
        {
            GetComponent<RewardAd>().LoadRewardAd();
        }
        else
        {
            animator.SetBool("Start",true);
        }*/

        GameManager.instance.SettingFinalPanel();
    }
}
