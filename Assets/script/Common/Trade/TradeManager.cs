using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//==========================================
//  創建者:家豪
//  創建日期:2024/06/24
//  創建用途: 交易管理器
//==========================================
public class TradeManager : MonoBehaviour
{
    #region 靜態變數

    private static TradeManager instance;
    public static TradeManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TradeManager>(true);
            return instance;
        }
    }

    #endregion

    #region 遊戲物件

    [Header("NPC交易區")]
    [SerializeField] private EquipTradeData npcEquipTradeItem;       //生成NPC交易物品資料的物件
    [SerializeField] private Transform npcEquipTradeItemTrans;       //生成NPC交易物品資料的參考
    private List<EquipTradeData> tempNpcEquipTradeItemList = new List<EquipTradeData>();
    public EventHandler<EquipTradeData> NpcTradeItemEvent;      //NPC物品點擊事件
    [Header("玩家交易區")]
    [SerializeField] private EquipTradeData playerEquipTradeItem;       //生成玩家交易物品資料的物件
    [SerializeField] private Transform playerEquipTradeItemTrans;       //生成玩家交易物品資料的參考
    [SerializeField] private TextMeshProUGUI playerCoinText;       //玩家的金幣文字
    private List<EquipTradeData> tempPlayerEquipTradeItemList = new List<EquipTradeData>();
    public EventHandler<EquipTradeData> PlayerTradeItemEvent;       //玩家物品點擊事件


    [Header("按鈕區")]
    [SerializeField] private Button buyBtn;       //購買按鈕
    [SerializeField] private Button sellBtn;       //販售按鈕

    [Header("最後交易區塊")]
    [SerializeField] private GameObject finalTradePanel;       //面板物件
    [SerializeField] private TextMeshProUGUI fincalTradePriceText;      //購買價格文字
    [SerializeField] private Image fianlTradeItemImage;      //購買道具圖片
    [SerializeField] private TextMeshProUGUI finalTradeItemNameText;      //購買道具名稱文字
    [SerializeField] private GameObject finalTradeQtyArea;      //購買數量區塊(物件)
    [SerializeField] private TextMeshProUGUI finalTradeQtyText;      //購買數量區塊文字
    [SerializeField] private Button tradeQtyAddCountBtn;      //交易區塊按鈕(增加數量)
    [SerializeField] private Button tradeQtySubtractCountBtn;      //交易區塊按鈕(減少數量)
    [SerializeField] private Button checkTradeBtn;      //確認交易按鈕
    [SerializeField] private TextMeshProUGUI checkTradeBtnText;      //確認交易按鈕文字

    //[Header("販售面板")]
    //[SerializeField] private GameObject redeemItemPanel;       //面板物件
    //[SerializeField] private TextMeshProUGUI redeemPriceText;      //販售價格文字
    //[SerializeField] private Image redeemItemImage;      //販售道具圖片
    //[SerializeField] private TextMeshProUGUI redeemItemNameText;      //販售道具名稱文字
    //[SerializeField] private GameObject redeemQtyArea;      //販售數量區塊(物件)
    //[SerializeField] private TextMeshProUGUI redeemQtyText;      //販售數量區塊文字

    #endregion

    #region 遊戲資料

    [Header("遊戲資料")]
    [SerializeField] private int buyItemQtyMaxCount = 99;       //購買物品的上限數量                                         
    [SerializeField] private Sprite bagItemOriginImage; //空背包圖
    private EquipTradeData tempSelectItem;       //當前選擇的物品
    public EquipTradeData TempSelectItem
    {
        get
        {
            return tempSelectItem;
        }
        set
        {
            tempSelectItem = value;
        }
    }
    //紀錄物品基本訊息
    private IItemBasal tempSelectItemBasalData => tempSelectItem.EquipmentDatas.ItemCommonData;
    //暫存NPC物品清單
    List<string> tempNpcItemList = new List<string>();
    //背包管理腳本
    private BagManager itemManager;
    #endregion

    #region 事件區

    public EventHandler<EquipTradeData> SelectItemEvent;        //選擇物品事件

    #endregion

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(List<string> npcItemList, List<Equipment> playerItemList)
    {
        ClearData();
        //獲取背包管理腳本
        itemManager = BagManager.Instance;
        //紀錄NPC商品
        tempNpcItemList = npcItemList;
        //獲取最大生成數量
        int bagMaxCount = itemManager.BagCount;
        //NPC物件生成區
        for (int i = 0; i < bagMaxCount; i++)
        {
            EquipTradeData tempData = Instantiate(npcEquipTradeItem, npcEquipTradeItemTrans);
            tempData.gameObject.SetActive(true);
            if (npcItemList.Count > i)
            {
                tempData.EquipmentDatas.Weapon = GameData.WeaponsDic.Where(x => x.Key == npcItemList[i]).Select(x => x.Value).FirstOrDefault();
                tempData.EquipmentDatas.Armor = GameData.ArmorsDic.Where(x => x.Key == npcItemList[i]).Select(x => x.Value).FirstOrDefault();
                tempData.EquipmentDatas.Item = GameData.ItemsDic.Where(x => x.Key == npcItemList[i]).Select(x => x.Value).FirstOrDefault();
                tempData.EquipImage.sprite = CommonFunction.GetItemSprite(tempData.EquipmentDatas) == null ?
                    bagItemOriginImage : CommonFunction.GetItemSprite(tempData.EquipmentDatas);
                tempData.Qty = buyItemQtyMaxCount;
            }
            tempNpcEquipTradeItemList.Add(tempData);
        }
        //玩家物件生成區
        for (int i = 0; i < bagMaxCount; i++)
        {
            EquipTradeData tempData = Instantiate(playerEquipTradeItem, playerEquipTradeItemTrans);
            tempData.gameObject.SetActive(true);
            if (playerItemList.Count >= i)
            {
                tempData.EquipmentDatas = playerItemList[i].EquipmentDatas;
                tempData.EquipImage.sprite = CommonFunction.GetItemSprite(tempData.EquipmentDatas) == null ?
                    bagItemOriginImage : CommonFunction.GetItemSprite(tempData.EquipmentDatas);
                tempData.Qty = playerItemList[i].Qty;
            }
            tempPlayerEquipTradeItemList.Add(tempData);
        }
        //玩家金幣
        playerCoinText.text = PlayerDataOverView.Instance.PlayerData_.Coin.ToString();
        PanelManager.Instance.SetPanelOpen("TradePanel");
    }

    /// <summary>
    /// 清除生成的資料
    /// </summary>
    public void ClearData()
    {
        if (tempNpcEquipTradeItemList.Count > 0)
        {
            tempNpcEquipTradeItemList.ForEach(x => Destroy(x.gameObject));
            tempNpcEquipTradeItemList.Clear();
        }
        if (tempPlayerEquipTradeItemList.Count > 0)
        {
            tempPlayerEquipTradeItemList.ForEach(x => Destroy(x.gameObject));
            tempPlayerEquipTradeItemList.Clear();
        }
        //if (tempNpcItemList.Count > 0)
        //{
        //    tempNpcItemList.Clear();
        //}
    }

    /// <summary>
    /// 交易區塊按鈕設定
    /// </summary>
    /// <param name="equipmentType"></param>
    public void ButtonSetting(string equipmentType)
    {
        buyBtn.gameObject.SetActive(equipmentType == "NPC");
        sellBtn.gameObject.SetActive(equipmentType == "Player");
    }

    #region 觸發事件

    /// <summary>
    /// 點擊事件
    /// </summary>
    /// <param name="data"></param>
    public void OnClick(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;
        EquipTradeData tempData = data.pointerClick.gameObject.GetComponent<EquipTradeData>();
        SelectItemEvent?.Invoke(this, tempData);
        //ButtonSetting(data.pointerCurrentRaycast.gameObject.GetComponent<Equipment>().EquipmentType);
    }

    #endregion

    #region 按鈕功能

    /// <summary>
    /// 設定最後交易面板的物品相關資訊
    /// </summary>
    /// <param name="tradeType">交易類型 1:購買、2:販賣</param>
    public void FinalTradePanelSetting(int tradeType)
    {
        if (tempSelectItem == null) return;
        //設定圖片
        fianlTradeItemImage.sprite = CommonFunction.GetItemSprite(tempSelectItem.EquipmentDatas.Clone());

        finalTradePanel.SetActive(true);
        fincalTradePriceText.text = tempSelectItemBasalData.Price.ToString();
        finalTradeItemNameText.text = tempSelectItemBasalData.Name.GetText();
        finalTradeQtyArea.SetActive(tempSelectItemBasalData.Stackability);

        checkTradeBtn.onClick.RemoveAllListeners();
        checkTradeBtn.onClick.AddListener(() =>
        {
            if (tradeType.Equals(1))
                BuyItemProcessor();
            else
                SellItemProcessor();

            //設定交易文字
            checkTradeBtnText.text = tradeType.Equals(1) ? "確認購買" : "確認販賣";
        });

        tradeQtyAddCountBtn.onClick.RemoveAllListeners();
        tradeQtyAddCountBtn.onClick.AddListener(() =>
        {
            if (tradeType.Equals(1))
                BuyItemPanelQtySetting(1);
            else
                SellItemPanelQtySetting(1);
        });
        tradeQtySubtractCountBtn.onClick.RemoveAllListeners();
        tradeQtySubtractCountBtn.onClick.AddListener(() =>
        {
            if (tradeType.Equals(1))

                BuyItemPanelQtySetting(-1);
            else
                SellItemPanelQtySetting(-1);


        });
        finalTradeQtyText.text = "1";
    }

    /// <summary>
    /// 販售背包物品
    /// </summary>
    public void SellItemPanelSetting()
    {
        //var weaponData = tempSelectItem.EquipmentDatas.Clone().Weapon;
        //var armorData = tempSelectItem.EquipmentDatas.Clone().Armor;
        //var itemData = tempSelectItem.EquipmentDatas.Clone().Item;
        ////設定圖片
        //redeemItemImage.sprite = CommonFunction.GetItemSprite(tempSelectItem.EquipmentDatas.Clone());
        //if (weaponData != null)
        //{
        //    redeemItemPanel.SetActive(true);
        //    redeemPriceText.text = weaponData.Price.ToString();
        //    redeemItemNameText.text = weaponData.Name;
        //    redeemQtyArea.SetActive(weaponData.Stackability);
        //}
        //if (armorData != null)
        //{
        //    redeemItemPanel.SetActive(true);
        //    redeemPriceText.text = armorData.Price.ToString();
        //    redeemItemNameText.text = armorData.Name;
        //    redeemQtyArea.SetActive(armorData.Stackability);
        //}
        //if (itemData != null)
        //{
        //    redeemItemPanel.SetActive(true);
        //    redeemPriceText.text = itemData.Price.ToString();
        //    redeemItemNameText.text = itemData.Name;
        //    redeemQtyArea.SetActive(itemData.Stackability);
        //}
    }

    /// <summary>
    /// 購買物品的數量設定
    /// </summary>
    /// <param name="count"></param>
    public void BuyItemPanelQtySetting(int count)
    {
        //取得當前面板上設定數量
        int tempCount = int.Parse(finalTradeQtyText.text);
        if ((tempCount + count) > 0 && (tempCount + count) <= buyItemQtyMaxCount)
            finalTradeQtyText.text = (tempCount + count).ToString();
        else if ((tempCount + count) < 0)
            finalTradeQtyText.text = "1";
        else if ((tempCount + count) > buyItemQtyMaxCount)
            finalTradeQtyText.text = buyItemQtyMaxCount.ToString();

        //設定交易總金額
        fincalTradePriceText.text = (int.Parse(finalTradeQtyText.text) * tempSelectItemBasalData.Price).ToString();

    }

    /// <summary>
    /// 販售物品的上限設定
    /// </summary>
    /// <param name="count"></param>
    public void SellItemPanelQtySetting(int count)
    {
        //取得當前面板上設定數量
        int tempCount = int.Parse(finalTradeQtyText.text);
        //取得玩家擁有的道具數量資料
        int maxCount = itemManager.BagItems.Where(x => x.EquipmentDatas == tempSelectItem.EquipmentDatas).FirstOrDefault().Qty;
        if ((tempCount + count) > 00 && (tempCount + count) <= maxCount)
            finalTradeQtyText.text = (tempCount + count).ToString();
        else if ((tempCount + count) < 0)
            finalTradeQtyText.text = "1";
        else if ((tempCount + count) > maxCount)
            finalTradeQtyText.text = maxCount.ToString();

        //設定交易總金額
        fincalTradePriceText.text = (int.Parse(finalTradeQtyText.text) * tempSelectItemBasalData.Redeem).ToString();

    }

    /// <summary>
    /// 購買處理
    /// </summary>
    public void BuyItemProcessor()
    {
        //將物品設定入包包
        if (tempSelectItem.EquipmentDatas.Weapon != null)
            itemManager.PickUp(fianlTradeItemImage.sprite, tempSelectItem.EquipmentDatas.Weapon);
        if (tempSelectItem.EquipmentDatas.Armor != null)
            itemManager.PickUp(fianlTradeItemImage.sprite, tempSelectItem.EquipmentDatas.Armor);
        if (tempSelectItem.EquipmentDatas.Item != null)
            BagManager.Instance.PickUp(fianlTradeItemImage.sprite, tempSelectItem.EquipmentDatas.Item, int.Parse(finalTradeQtyText.text));

        //扣除相對應金幣數量
        if (PlayerDataOverView.Instance.PlayerData_.Coin - int.Parse(fincalTradePriceText.text) > 0)
            itemManager.PickUp((-1 * int.Parse(fincalTradePriceText.text)), true);
        else
            CommonFunction.MessageHint("金幣不足!!!", HintType.Warning);
        finalTradePanel.SetActive(false);

        //以當前NPC資料刷新交易面板
        Init(tempNpcItemList, itemManager.BagItems);
        //玩家金幣
        playerCoinText.text = PlayerDataOverView.Instance.PlayerData_.Coin.ToString();
        //清空暫存資料
        tempSelectItem = null;
    }

    /// <summary>
    /// 販售處理
    /// </summary>
    public void SellItemProcessor()
    {
        //防具、武器處理
        if (tempSelectItem.EquipmentDatas.Weapon != null|| tempSelectItem.EquipmentDatas.Armor != null)
            itemManager.RemoveItem(tempSelectItem.EquipmentDatas);
        //道具處理
        if (tempSelectItem.EquipmentDatas.Item != null)
            itemManager.RemoveItem(tempSelectItemBasalData.CodeID, tempSelectItemBasalData.Stackability ? int.Parse(finalTradeQtyText.text) : 1);

        itemManager.PickUp((int.Parse(fincalTradePriceText.text)), true);

        CommonFunction.MessageHint("成功販賣物品!!!", HintType.NormalItem);
        finalTradePanel.SetActive(false);

        //以當前NPC資料刷新交易面板
        Init(tempNpcItemList, itemManager.BagItems);
        //玩家金幣
        playerCoinText.text = PlayerDataOverView.Instance.PlayerData_.Coin.ToString();
        //清空暫存資料
        tempSelectItem = null;
    }

    #endregion


}