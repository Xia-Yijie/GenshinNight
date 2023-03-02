using System;
using UnityEngine.UI;
using UnityEngine;

public class canKnowSettingSlot : MonoBehaviour
{
    public RectTransform selfTrans;
    public Image itemImage;
    public Text titleText;
    public Text descriptionText;
    public Text numText;
    public Image backImage;
    public Image clickButtonImage;
    public Text statusText;
    public Sprite greenSprite;
    public Sprite redSprite;
    public AudioClip enableAudio;
    public AudioClip disableAudio;
    
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
        
        RefreshSlot();
    }

    public void OnClick()
    {
        if(buffer.isEnabled()) AudioManager.PlayEFF(disableAudio);
        else AudioManager.PlayEFF(enableAudio);
        buffer.SwitchEnable();
        RefreshSlot();
    }

    private void RefreshSlot()
    {
        numText.text = buffer.num + "/" + buffer.maxNum;
        if (!buffer.isEnabled())
        {
            backImage.color = new Color32(160, 160, 160, 200);
            clickButtonImage.sprite = redSprite;
            statusText.text = buffer.maxNum == 0 ? "未获得" : "已关闭";
        }
        else
        {
            backImage.color = new Color32(255, 255, 255, 200);
            clickButtonImage.sprite = greenSprite;
            statusText.text = "已激活";
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(selfTrans);
    }
}
