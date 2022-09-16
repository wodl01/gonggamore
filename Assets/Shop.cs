using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItemGroup
{
    public Button buyButton;
    public Text itemAmountText;
    public int itemPrice;
    public int maxItemAmount;
}

public class Shop : MonoBehaviour
{
    private GameManager gameManager;


    [SerializeField]
    private GameObject shopPanel;
    [SerializeField]
    private Text inGameMoneyText;
    [SerializeField]
    private Button removeAdBtn;


    [SerializeField]
    private ShopItemGroup[] itemGroup;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void OpenShop(bool active)
    {
        shopPanel.SetActive(active);

        ShopPanelUpdate();

        if (!active)
            gameManager.SaveData();
    }

    public void ShopPanelUpdate()
    {
        inGameMoneyText.text = "통장잔고:" + GameManager.TextChanger(gameManager.GetInGameMoneyValue()) + "원";

        for (int i = 0; i < itemGroup.Length; i++)
        {
            ShopItemGroup curItem = itemGroup[i];

            curItem.itemAmountText.text = gameManager.GetItemAmount(i).ToString();

            if(gameManager.GetInGameMoneyValue() >= curItem.itemPrice && gameManager.GetItemAmount(i) != curItem.maxItemAmount)
                curItem.buyButton.interactable = true;
            else
                curItem.buyButton.interactable = false;
        }

        removeAdBtn.interactable = !gameManager.userData.removeAd;
        removeAdBtn.transform.GetChild(1).GetComponent<Text>().text = gameManager.userData.removeAd ? "구매완료" : "구매하기";
    }

    public void BuyShopItem(int itemCode)
    {
        gameManager.AddInGameMoneyValue(-itemGroup[itemCode].itemPrice);

        gameManager.AddItemAmount(itemCode, 1);

        ShopPanelUpdate();


    }
}
