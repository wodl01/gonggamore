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

    private void Awake()
    {
        instance = this;
    }
}
