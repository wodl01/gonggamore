using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void BuyItem(UnityEngine.Purchasing.Product product)
    {
        GameManager.instance.userData.removeAd = true;
        GameManager.instance.SaveData();
    }
}
