using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static Equipment;
using static UnityEditor.Progress;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/07
//  創建用途:  背包管理
//==========================================
public class ItemManager : MonoBehaviour
{
    #region 靜態變數
    private static ItemManager instance;
    public static ItemManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ItemManager>();
            return instance;
        }
    }
    #endregion 

    public List<Equipment> EquipDataList = new List<Equipment>();       //身上裝備資料
    [HideInInspector] public List<Equipment> BagItems = new List<Equipment>();     //背包格數
    public TextMeshProUGUI CoinText;       //金幣Text

    [Header("生成背包格數的Content"), SerializeField] private Transform bagContent;
    [Header("背包格數生成量"), SerializeField] private int bagCount;
    [Header("背包物品拖曳腳本"), SerializeField] private BagItemEquip bagItemEquip;
    public int BagCount => bagCount;
    [Header("背包格預置物"), SerializeField] private GameObject bagObject;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="itemBagList"></param>
    public void Init(List<EquipmentDataToJson> itemBagList = null)
    {
        //從本地獲取背包資料
        if (itemBagList.CheckAnyData())
        {
            //BagItems = itemBagList;
            //依照設定多少數量 生成背包格數
            for (int i = 0; i < itemBagList.Count; i++)
            {
                //生成背包格並顯示出來
                GameObject bag = Instantiate(bagObject, bagContent);
                //紀錄背包格的物品資料
                BagItems.Add(bag.GetComponent<Equipment>());
                bag.SetActive(true);
                switch (itemBagList[i].Type)
                {
                    case "Weapon":
                        //設定物品資料
                        bag.GetComponent<Equipment>().EquipmentDatas = new EquipmentData()
                        {
                            Weapon = GameData.WeaponsDic[itemBagList[i].CodedID],
                            Qty = itemBagList[i].Qty
                        };
                        break;

                    case "Armor":
                        //設定物品資料
                        bag.GetComponent<Equipment>().EquipmentDatas = new EquipmentData()
                        {
                            Armor = GameData.ArmorsDic[itemBagList[i].CodedID],
                            Qty = itemBagList[i].Qty
                        };
                        break;

                    case "Item":
                        //設定物品資料
                        bag.GetComponent<Equipment>().EquipmentDatas = new EquipmentData()
                        {
                            Item = GameData.ItemsDic[itemBagList[i].CodedID],
                            Qty = itemBagList[i].Qty
                        };
                        break;

                    default:
                        break;
                }
                bag.GetComponent<Equipment>().Qty = itemBagList[i].Qty;
                //設定物品圖片
                bag.GetComponent<Equipment>().EquipImage.sprite = CommonFunction.GetItemSprite(bag.GetComponent<Equipment>().EquipmentDatas) == null ?
                    bagItemEquip.BagItemOriginImage : CommonFunction.GetItemSprite(bag.GetComponent<Equipment>().EquipmentDatas);
            }
        }
        //無本地資料
        else
        {
            //檢查背包格數是否設定正確
            if (!bagCount.Equals(0))
            {
                //依照設定多少數量 生成背包格數
                for (int i = 0; i < bagCount; i++)
                {
                    //生成背包格並顯示出來
                    GameObject bag = Instantiate(bagObject, bagContent);
                    //紀錄背包格的物品資料
                    BagItems.Add(bag.GetComponent<Equipment>());
                    bag.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 獲取防具
    /// </summary>
    /// <param name="ItemImage">物品圖片</param>
    /// <param name="armorvaule">防具資料</param>
    public void PickUp(Sprite ItemImage, ArmorDataModel armorvaule)
    {
        foreach (var item in BagItems)
        {
            //檢查該格是否有空位
            if (item.EquipmentDatas.Armor == null
                && item.EquipmentDatas.Weapon == null
                    && item.EquipmentDatas.Item == null)
            {
                //設定圖片
                item.EquipImage.sprite = ItemImage;
                //設定資料
                item.GetComponent<Equipment>().EquipmentDatas.Armor = armorvaule;
                //設定堆疊數量資料
                item.Qty = 1;
                CommonFunction.MessageHint(("獲得" + armorvaule.Name.GetText()), HintType.NormalItem);
                return;
            }
            else
                continue;
        }
    }

    /// <summary>
    /// 獲取武器
    /// </summary>
    /// <param name="ItemImage">物品圖片</param>
    /// <param name="weaponvaule">防具資料</param>
    public void PickUp(Sprite ItemImage, WeaponDataModel weaponvaule)
    {
        foreach (var item in BagItems)
        {
            //檢查該格是否有空位
            if (item.EquipmentDatas.Armor == null
                && item.EquipmentDatas.Weapon == null
                    && item.EquipmentDatas.Item == null)
            {
                //設定圖片
                item.EquipImage.sprite = ItemImage;
                //設定資料
                item.GetComponent<Equipment>().EquipmentDatas.Weapon = weaponvaule;
                //設定物品堆疊資料
                item.Qty = 1;
                CommonFunction.MessageHint(("獲得" + weaponvaule.Name.GetText()), HintType.NormalItem);
                return;
            }
            else
                continue;
        }
    }

    /// <summary>
    /// 獲取道具
    /// </summary>
    /// <param name="ItemImage">物品圖片</param>
    /// <param name="itemvaule">道具資料</param>
    public void PickUp(Sprite ItemImage, ItemDataModel itemvaule, int qty = 1)
    {
        //檢查包包是否有物品與撿起物品一樣並且可以堆疊
        if (itemvaule.Stackability)
        {
            var getSameData = BagItems.Where(x => x.EquipmentDatas.Item?.CodeID == itemvaule.CodeID).FirstOrDefault();
            if (getSameData != null)
            {
                getSameData.Qty += qty;
                return;
            }
        }

        foreach (var item in BagItems)
        {
            //檢查該格是否有空位
            if (item.EquipmentDatas.Armor == null
                && item.EquipmentDatas.Weapon == null
                    && item.EquipmentDatas.Item == null)
            {
                //設定圖片
                item.EquipImage.sprite = ItemImage;
                //設定資料
                item.GetComponent<Equipment>().EquipmentDatas.Item = itemvaule;
                item.Qty = qty;
                CommonFunction.MessageHint(("獲得" + itemvaule.Name.GetText()), HintType.NormalItem);
                return;
            }
            else if (item.EquipmentDatas.Item?.CodeID == itemvaule.CodeID)
            {
                item.Qty += qty;
            }
            else
                continue;
        }
    }

    /// <summary>
    /// 移除指定物品(指定數量移除數量)
    /// </summary>
    /// <param name="codeID"></param>
    /// <param name="Qty">可堆疊的物品需要額外設定數量</param>
    public void RemoveItem(string codeID, int Qty = 0)
    {
        var queryResult = BagItems.Where(x => x.EquipmentDatas.Weapon?.CodeID == codeID || x.EquipmentDatas.Armor?.CodeID == codeID || x.EquipmentDatas.Item?.CodeID == codeID).FirstOrDefault();
        if (queryResult != null)
        {
            //若物品數量 大於 移除量 刪除指定數量即可
            if (queryResult.Qty < Qty && !Qty.Equals(0))
                queryResult.Qty -= Qty;
            else
            {
                //清空物品的資料
                queryResult.EquipmentDatas.Weapon = null;
                queryResult.EquipmentDatas.Armor = null;
                queryResult.EquipmentDatas.Item = null;
                //清空數量資料
                queryResult.Qty = 0;
                //設定圖片為原始圖片
                queryResult.EquipImage.sprite = bagItemEquip.BagItemOriginImage;
            }
        }

        BagItemSort();
    }

    /// <summary>
    /// 獲取金幣
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="isInit">是否為初始化帶入資料使用</param>
    public void PickUp(int coin, bool isInit = false)
    {
        if (coin.Equals(0)) return;
        PlayerDataOverView.Instance.PlayerData_.Coin += int.Parse(coin.ToString());
        CoinText.text = PlayerDataOverView.Instance.PlayerData_.Coin.ToString();
        //非初始化的方式更新金幣資料 呼叫系統訊息
        if (!isInit)
            CommonFunction.MessageHint(("獲得" + coin.ToString() + "金幣"), HintType.NormalItem);
    }

    /// <summary>
    /// 包包排序
    /// </summary>
    public void BagItemSort()
    {        //背包物品重新排列 有資料的格數優先排列 物品=>武器=>防具
        BagItems = BagItems.OrderByDescending(x => x.EquipmentDatas.Item != null)
            .ThenByDescending(x => x.EquipmentDatas.Weapon != null)
            .ThenByDescending(x => x.EquipmentDatas.Armor != null).ToList();
    }
}
