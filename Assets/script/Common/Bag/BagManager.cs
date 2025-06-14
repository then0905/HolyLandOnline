using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/07
//  創建用途:  背包管理
//==========================================
public class BagManager : MonoBehaviour
{
    #region 靜態變數
    private static BagManager instance;
    public static BagManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<BagManager>();
            return instance;
        }
    }
    #endregion 

    public List<Equipment> EquipDataList = new List<Equipment>();       //身上裝備資料
    [HideInInspector] public List<Equipment> BagItems = new List<Equipment>();     //背包格數
    public TextMeshProUGUI CoinText;       //金幣Text

    [Header("生成背包格數的Content"), SerializeField] private Transform bagContent;

    [Header("背包格數生成量"), SerializeField] private int bagCount;
    public int BagCount => bagCount;
    [Header("背包物品拖曳腳本"), SerializeField] private BagItem bagItemEquip;
    public BagItem BagItemEquip => bagItemEquip;

    [Header("背包格預置物"), SerializeField] private GameObject bagObject;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="itemBagList">背包物品資料</param>
    /// <param name="equipDataList">穿戴裝備資料</param>
    public void Init(List<EquipmentDataToJson> itemBagList = null, List<EquipmentDataToJson> equipDataList = null)
    {
        //從本地獲取背包資料
        if (itemBagList.CheckAnyData())
        {
            //BagItems = itemBagList;
            //依照設定多少數量 生成背包格數
            for (int i = 0; i < itemBagList.Count; i++)
            {
                //生成背包格並顯示出來
                Equipment bag = Instantiate(bagObject, bagContent).GetComponent<Equipment>();
                //紀錄背包格的物品資料
                BagItems.Add(bag);
                bag.name = $"BagItem_{i + 1}";
                bag.gameObject.SetActive(true);
                switch (itemBagList[i].Type)
                {
                    case "Weapon":
                        //設定物品資料
                        bag.EquipmentDatas = new EquipmentData()
                        {
                            Weapon = GameData.WeaponsDic[itemBagList[i].CodedID],
                            Qty = itemBagList[i].Qty,
                            ForceLv = itemBagList[i].ForceLv
                        };
                        break;

                    case "Armor":
                        //設定物品資料
                        bag.EquipmentDatas = new EquipmentData()
                        {
                            Armor = GameData.ArmorsDic[itemBagList[i].CodedID],
                            Qty = itemBagList[i].Qty,
                            ForceLv = itemBagList[i].ForceLv
                        };
                        break;

                    case "Item":
                    case "Potion":
                    case "Utility":
                    case "Buff":
                    case "Unavailable":
                        //設定物品資料
                        bag.EquipmentDatas = new EquipmentData()
                        {
                            Item = GameData.ItemsDic[itemBagList[i].CodedID],
                            Qty = itemBagList[i].Qty
                        };
                        break;

                    default:
                        break;
                }
                bag.Qty = itemBagList[i].Qty;
                //設定物品圖片
                bag.EquipImage.sprite = CommonFunction.GetItemSprite(bag.EquipmentDatas) == null ?
                    bagItemEquip.BagItemOriginImage : CommonFunction.GetItemSprite(bag.EquipmentDatas);
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
                    bag.name = $"BagItem_{i + 1}";
                    bag.SetActive(true);
                }
            }
        }

        //玩家裝備資料設定
        if (equipDataList.CheckAnyData())
        {
            foreach (var data in equipDataList)
            {
                if (GameData.WeaponsDic.TryGetValue(data.CodedID, out WeaponDataModel weaponData))
                {
                    var weaponItem = EquipDataList.Where(x => x.GetComponent<EquipData>().PartID.Any(y => y == weaponData.TackHandID)).FirstOrDefault();
                    if (weaponItem != null)
                    {
                        weaponItem.EquipmentDatas.Weapon = weaponData;
                        weaponItem.EquipmentDatas.ForceLv = data.ForceLv;
                        weaponItem.EquipImage.sprite = CommonFunction.GetItemSprite(weaponItem.EquipmentDatas);
                    }

                }

                if (GameData.ArmorsDic.TryGetValue(data.CodedID, out ArmorDataModel armorData))
                {
                    var armorItem = EquipDataList.Where(x => x.GetComponent<EquipData>().PartID.Any(y => y == armorData.WearPartID)).FirstOrDefault();
                    if (armorItem != null)
                    {
                        armorItem.EquipmentDatas.Armor = armorData;
                        armorItem.EquipmentDatas.ForceLv = data.ForceLv;
                        armorItem.EquipImage.sprite = CommonFunction.GetItemSprite(armorItem.EquipmentDatas);
                    }
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
                CommonFunction.MessageHint(("獲得" + itemvaule.Name.GetText()), HintType.NormalItem);
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
                CommonFunction.MessageHint(("獲得" + itemvaule.Name.GetText()), HintType.NormalItem);
                item.Qty += qty;
            }
            else
                continue;
        }
    }

    /// <summary>
    /// 移除指定物品(道具類型)
    /// </summary>
    /// <param name="codeID"></param>
    /// <param name="Qty">可堆疊的物品需要額外設定數量</param>
    public void RemoveItem(string codeID, int Qty = 0)
    {
        var queryResult = BagItems.Where(x => x.EquipmentDatas.Item?.CodeID == codeID).FirstOrDefault();
        if (queryResult != null)
        {
            //若物品數量 大於 移除量 刪除指定數量即可
            if (queryResult.Qty > Qty && !Qty.Equals(0))
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
    /// 移除指定物品(防具、武器)
    /// </summary>
    /// <param name="equipmentData">物品資料</param>
    public void RemoveItem(EquipmentData equipmentData)
    {
        var queryResult = BagItems.Where(x => (x.EquipmentDatas.ItemCommonData?.CodeID == equipmentData.ItemCommonData.CodeID && x.EquipmentDatas.ForceLv == equipmentData.ForceLv)).FirstOrDefault();
        if (queryResult != null)
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
    {
        //背包物品重新排列 有資料的格數優先排列 物品=>武器=>防具
        BagItems = BagItems.OrderByDescending(x => x.EquipmentDatas.Item != null)
            .ThenByDescending(x => x.EquipmentDatas.Weapon != null)
            .ThenByDescending(x => x.EquipmentDatas.Armor != null).ToList();
    }

    /// <summary>
    /// 取得背包內指定物品的數量
    /// </summary>
    /// <param name="codeID"></param>
    /// <returns></returns>
    public int GetItemQtyFromBag(string codeID)
    {
        var queryResult = BagItems.Where(x => x.EquipmentDatas.Weapon?.CodeID == codeID || x.EquipmentDatas.Armor?.CodeID == codeID || x.EquipmentDatas.Item?.CodeID == codeID).FirstOrDefault();
        if (queryResult != null)
            return queryResult.Qty;
        else
            return 0;
    }

}
