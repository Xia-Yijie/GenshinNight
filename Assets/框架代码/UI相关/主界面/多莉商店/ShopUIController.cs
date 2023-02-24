using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    public static ShopUIController instance;
    
    public gradualChange gl_;
    public Animator rightAnim;
    public Animator DoriAnim;
    public Animator dialogAnim;
    public Text dialogText;
    public Text moraText;
    public Text PrimogemText;
    public GameObject moreTop;
    public GameObject primogemTop;

    public RectTransform selectObj;
    public List<Transform> UpButtons;  // 0:罐装知识，1:知识配置，2:祈愿，3:道具
    public List<GameObject> Pages;
    [HideInInspector] public int showingPageID;

    public AudioClip OpenShopAudio;
    public AudioClip CloseShopAudio;
    public AudioClip SelectUpAudio;
    public AudioClip SelectElementAudio;
    public AudioClip SelectStrengthenAudio;
    public AudioClip CancelStrengthenAudio;
    public AudioClip BuyAudio;
    
    public canningKnowledgeUI ckui;
    public canKnowSettingUI cksui;
    public wishUI wui;

    private void Awake()
    {
        instance = this;
    }

    public void Show_Shop()
    {
        gl_.Show();
        rightAnim.SetTrigger("appear");
        DoriAnim.SetTrigger("appear");
        dialogAnim.SetBool("appear", false);

        
        cksui.InitOpen();
        ckui.InitOpen();
        wui.InitOpen();
        
        
        SelectPage(0);
        HideText();
        
        RefreshMora();
        Invoke(nameof(ShowWelcomeText), 0.3f);
        
        AudioManager.PlayEFF(OpenShopAudio, 0.5f);
    }

    public void Hide_Shop()
    {
        gl_.Hide();
        rightAnim.SetTrigger("disappear");
        DoriAnim.SetTrigger("disappear");
        HideText();
        AudioManager.PlayEFF(CloseShopAudio);
    }

    private void ShowWelcomeText()
    {
        ShowText("欢迎来到多莉商店！");
    }

    public static void ShowText(string text)
    {
        instance.dialogText.text = text;
        instance.dialogAnim.SetBool("appear", true);
        instance.dialogAnim.SetBool("disappear", true);
        
    }

    public static void RefreshMora()
    {
        instance.moraText.text = gameManager.Mora.ToString();
    }
    
    public static void RefreshPrimogem()
    {
        instance.PrimogemText.text = gameManager.Primogem.ToString();
    }

    public void HideText()
    {
        dialogAnim.SetBool("appear", false);
        dialogAnim.SetBool("disappear", true);
    }

    public void TouchDori()
    {
        DoriAnim.SetTrigger("jump");
        ShowText("哈哈哈");
    }

    public void SelectPage(int id)
    {
        if (id < 0 || id > 3) return;
        
        showingPageID = id;
        selectObj.SetParent(UpButtons[id]);
        selectObj.anchoredPosition = Vector2.zero;
        foreach (var i in Pages) i.SetActive(false);
        Pages[id].SetActive(true);

        RefreshMora();
        RefreshPrimogem();

        switch (id)
        {
            case 0:
                ShowText(DoriTalk.Shop);
                moreTop.SetActive(true);
                primogemTop.SetActive(false);
                ckui.isStrengthen = cksui.isStrengthen;
                ckui.ChoosePage(cksui.showingID);
                break;
            case 1:
                ShowText(DoriTalk.Setting);
                moreTop.SetActive(true);
                primogemTop.SetActive(false);
                cksui.isStrengthen = ckui.isStrengthen;
                cksui.ChoosePage(ckui.showingID);
                break;
            case 2:
                ShowText(DoriTalk.Wish);
                moreTop.SetActive(false);
                primogemTop.SetActive(true);
                break;
        }
    }
    
    public void PlaySelectUpAudio(){
        AudioManager.PlayEFF(SelectUpAudio, 0.6f);
    }
    
    public void PlaySelectElementAudio(){
        AudioManager.PlayEFF(SelectElementAudio, 0.6f);
    }

    public static void PlayBuyAudio()
    {
        AudioManager.PlayEFF(instance.BuyAudio, 0.6f);
    }

}

public class DoriTalk
{
    public const string Shop = "全提瓦特最大的罐装知识零售商就在这里啦~只要客官摩拉管够，没有多莉" +
                               "提供不了的商品！";
    public const string Setting = "多莉提供最完美的售后服务！客官可以在这里选择激活或遗忘已购买的" +
                                   "罐装知识。放心，遗忘后再激活是免费的哦~";

    public const string Wish = "多莉提供超级友好的代抽服务！只需要提供一点点保底的原石，就能百分百邀请到一位" +
                               "心仪的伙伴加入队伍，是不是超级划算呐~";
}