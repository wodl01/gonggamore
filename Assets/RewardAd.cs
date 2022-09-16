using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardAd : MonoBehaviour
{
   // public InterstitialAd interstitialAd;
    public bool isTestMode;
    //public Text LogText;
    public Button RewardAdsBtn;
    void Start()
    {

        LoadRewardAd();
    }

    AdRequest GetAdRequest()
    {
        return new AdRequest.Builder().Build();
    }



    #region ¸®¿öµå ±¤°í
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "";
    public RewardedAd rewardAd;


    public void LoadRewardAd()
    {
        Debug.Log("ÁØºñ");
        var requestConfiguration = new RequestConfiguration
   .Builder().build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);

        rewardAd.LoadAd(GetAdRequest());
    }


    public void ShowRewardAd()
    {
        if (!GameManager.instance.userData.removeAd)
            rewardAd.Show();
        Debug.Log("º¸¿©ÁÜ");
        SceneManage.instance.ChangeScene(2);
    }
    public void ShowRewardAd2()
    {
        if (!GameManager.instance.userData.removeAd)
            rewardAd.Show();
        Debug.Log("º¸¿©ÁÜ");
    }
    #endregion
}
