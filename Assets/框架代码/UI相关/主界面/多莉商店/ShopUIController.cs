using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        ShowText(DoriTalk.Interact[Random.Range(0, DoriTalk.Interact.Count)]);
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



    public static List<string> Interact = new List<string>
    {
        "我爱摩拉，摩拉爱我，啦啦啦~",
        "先把商品涨价百分之三十，然后再打八折卖出，不仅我赚得多了，顾客也会觉得划算，这就是双赢！",
        "摩拉在阳光下闪闪发光，真好看！嘿嘿",
        "我的摩拉是我的，你的摩拉还是我的，嘿嘿",
        "想见见驮兽吗？没问题！你想见多少我就有多少。只不过这个门票钱嘛，欸嘿嘿…",
        "想听故事？那就…给我一百万摩拉。嗯…看在我们交情的份上，给你打个九点九折好了，九十九万摩拉。怎么样？很划算吧！",
        "万能的多莉商店，应有尽有，童叟无欺。只要摩拉够，只有想不到，没有买不到。需要任何东西，或者遇到任何困难，都可以来找我喔！",
        "只要摩拉管够，和教令院对着干也没什么大不了的",
        "以前也有些蠢蘑菇试图欠账不还，嘿嘿，也不看看我是谁。不如，你来猜猜看他们的下场如何吧？",
    };
}