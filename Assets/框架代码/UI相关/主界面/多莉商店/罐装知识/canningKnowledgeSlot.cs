using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class canningKnowledgeSlot : MonoBehaviour
{
    public RectTransform selfTrans;
    public Image itemImage;
    public Text titleText;
    public Text descriptionText;
    public Text priceText;
    public Text remainText;
    public Image backImage;
    public Image purchaseButtonImage;
    public Sprite canPurchaseSprite;
    public Sprite cannotPurchaseSprite;
    
    private KnowledgeBuffer buffer;

    private void Awake()
    {
        selfTrans = GetComponent<RectTransform>();
    }


    public void Init(Sprite sprite, string title, string description, 
        KnowledgeBuffer knowledgeBuffer)
    {
        buffer = knowledgeBuffer;
        
        itemImage.sprite = sprite;
        titleText.text = title;
        descriptionText.text = description;
        priceText.text = buffer.price.ToString();

        RefreshRemainNum();
    }

    public void OnPurchase()
    {
        if (gameManager.Mora < buffer.price)
        {
            ShopUIController.ShowText("哎呀，客官好像没有足够的摩拉呢。不过没关系，我会帮客官把这批货留着的，嘿嘿~");
            return;
        }

        if (buffer.maxNum == buffer.total)
        {
            ShopUIController.ShowText("真是抱歉，客官要的那个已经没货了。");
            return;
        }
        
        gameManager.GetMora(-buffer.price);
        ShopUIController.RefreshMora();
        
        ShopUIController.ShowText("客官出手可真是大方~我这里还有好多货，客官不看看再走吗？");
        buffer.Buy();
        RefreshRemainNum();
        
        ShopUIController.PlayBuyAudio();
    }

    private void RefreshRemainNum()
    {
        remainText.text = buffer.total - buffer.maxNum + "/" + buffer.total;
        if (buffer.maxNum == buffer.total)
        {
            backImage.color = new Color32(160, 160, 160, 200);
            purchaseButtonImage.sprite = cannotPurchaseSprite;
        }
        else
        {
            backImage.color = new Color32(255, 255, 255, 200);
            purchaseButtonImage.sprite = canPurchaseSprite;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(selfTrans);
    }
}
