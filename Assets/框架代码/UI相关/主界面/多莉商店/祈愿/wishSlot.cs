using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wishSlot : MonoBehaviour
{
    public Image backImage;
    public Image headImage;
    public Text nameText;
    public Text descriptionText;
    public Text primogemCostText;
    public GameObject primogemObj;
    public Image wishButtonImage;
    public Text wishButtonText;
    public Sprite blueButtonSprite;
    public Sprite grayButtonSprite;

    private operData od_;
    private bool isAble;
    private int primogemCost = 28800;

    public void Init(operData ood, bool isAble_)
    {
        od_ = ood;
        isAble = isAble_;
        Refresh();
    }

    public void Refresh()
    {
        if (od_ == null) return;

        if (isAble)
        {
            backImage.color = new Color32(255, 255, 255, 200);
            primogemObj.SetActive(true);
            wishButtonImage.sprite = blueButtonSprite;
            wishButtonText.text = "祈愿";
        }
        else
        {
            backImage.color = new Color32(160, 160, 160, 200);
            primogemObj.SetActive(false);
            wishButtonImage.sprite = grayButtonSprite;
            wishButtonText.text = "已拥有";
        }
        
        headImage.sprite = od_.shopImage;
        nameText.text = od_.Name;
        descriptionText.text = od_.shopDescription;
        primogemCostText.text = primogemCost.ToString();
    }

    public void ClickWish()
    {
        if (gameManager.Primogem < primogemCost)
        {
            ShopUIController.ShowText("看来客官和这位角色的缘分还没到呢，等客官拥有足够多的原石之后再来试试吧");
            return;
        }
        
        if (!isAble)
        {// 已经拥有了该干员
            ShopUIController.ShowText("客官已经是这位角色的伙伴了哦，多莉这边暂不提供命座抽取服务");
            return;
        }
        
        gameManager.GetPrimogem(-primogemCost);
        ShopUIController.RefreshPrimogem();
        ShopUIController.ShowText("哇哦，客官今天的手气真不错，居然180发就出金了！是一位十分强力的伙伴呢");

        gameManager.AllOperValid[od_.EnName] = true;
        gameManager.UnlockCharacterNum++;

        wishUI.RefreshPage(wishUI.instance.showingType);
    }

}
