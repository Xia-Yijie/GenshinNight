using System;
using UnityEngine.UI;
using UnityEngine;

public class canKnowSettingSlot : MonoBehaviour
{
    public Image itemImage;
    public Text titleText;
    public Text descriptionText;
    public Text numText;
    public Image backImage;
    public Image clickButtonImage;
    public Text statusText;
    public Sprite greenSprite;
    public Sprite redSprite;
    
    private int hadNum;
    private int totalNum;
    private bool isEnable;
    private Action enableAction;

    public void Init(Sprite sprite, string title, string description, 
        Action enableAction_, int hadNum_, int totalNum_, bool isEnable_)
    {
        itemImage.sprite = sprite;
        titleText.text = title;
        descriptionText.text = description;

        hadNum = hadNum_;
        totalNum = totalNum_;
        enableAction = enableAction_;
        isEnable = isEnable_;

        if (totalNum == 0) isEnable = false;
        RefreshSlot();
    }

    public void OnClick()
    {
        isEnable = !isEnable;
        if (totalNum == 0) isEnable = false;
        hadNum = isEnable ? totalNum : 0;
        enableAction?.Invoke();
        RefreshSlot();
    }

    private void RefreshSlot()
    {
        numText.text = hadNum + "/" + totalNum;
        if (!isEnable)
        {
            backImage.color = new Color32(160, 160, 160, 200);
            clickButtonImage.sprite = redSprite;
            statusText.text = totalNum == 0 ? "未获得" : "已关闭";
        }
        else
        {
            backImage.color = new Color32(255, 255, 255, 200);
            clickButtonImage.sprite = greenSprite;
            statusText.text = "已激活";
        }
    }
}
