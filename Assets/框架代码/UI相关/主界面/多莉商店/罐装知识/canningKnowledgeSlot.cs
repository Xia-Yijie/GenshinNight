using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class canningKnowledgeSlot : MonoBehaviour
{
    public Image itemImage;
    public Text titleText;
    public Text descriptionText;
    public Text priceText;
    public Text remainText;
    public Image backImage;
    public Image purchaseButtonImage;
    public Sprite canPurchaseSprite;
    public Sprite cannotPurchaseSprite;
    
    private int price;
    private int remainNum;
    private int totalNum;
    private Action purchaseAction;

    public void Init(Sprite sprite, string title, string description, int price_,
        Action buyAction, int remainNum_, int totalNum_)
    {
        itemImage.sprite = sprite;
        titleText.text = title;
        descriptionText.text = description;
        priceText.text = price_.ToString();
        
        price = price_;
        remainNum = remainNum_;
        totalNum = totalNum_;
        purchaseAction = buyAction;

        RefreshRemainNum();
    }

    public void OnPurchase()
    {
        if (gameManager.Mora < price)
        {
            ShopUIController.ShowText("哎呀，客官好像没有足够的摩拉呢。不过没关系，我会帮客官把这批货留着的，嘿嘿~");
            return;
        }

        if (remainNum == 0)
        {
            ShopUIController.ShowText("真是抱歉，客官要的那个已经没货了。");
            return;
        }

        gameManager.Mora -= price;
        ShopUIController.RefreshMora();
        
        ShopUIController.ShowText("客官出手可真是大方~我这里还有好多货，客官不看看再走吗？");
        purchaseAction?.Invoke();
        remainNum--;
        RefreshRemainNum();
    }

    private void RefreshRemainNum()
    {
        remainText.text = remainNum + "/" + totalNum;
        if (remainNum == 0)
        {
            backImage.color = new Color32(160, 160, 160, 200);
            purchaseButtonImage.sprite = cannotPurchaseSprite;
        }
        else
        {
            backImage.color = new Color32(255, 255, 255, 200);
            purchaseButtonImage.sprite = canPurchaseSprite;
        }
    }
    

}
