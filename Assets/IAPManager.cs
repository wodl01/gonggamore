using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPManager : MonoBehaviour
{   
    public void BuyItem(UnityEngine.Purchasing.Product product)
    {
        GameManager.instance.userData.removeAd = true;
        GameManager.instance.SaveData();
    }

    public void test()
    {
        GameManager.instance.userData.removeAd = true;
        GameManager.instance.SaveData();
    }
}
