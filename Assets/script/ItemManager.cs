using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JsonDataModel;
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

    public List<EquipData> EquipDataList = new List<EquipData>();       //身上裝備資料
    [HideInInspector] public List<Equipment> BagItems = new List<Equipment>();     //背包格數
    public TextMeshProUGUI CoinText;       //金幣Text

    [Header("生成背包格數的Content"), SerializeField] private Transform bagContent;
    [Header("背包格數生成量"), SerializeField] private int bagCount;
    [Header("背包格預置物"), SerializeField] private GameObject bagObject;

    private void Awake()
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
    public void PickUp(Sprite ItemImage, ItemDataModel itemvaule)
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
                item.GetComponent<Equipment>().EquipmentDatas.Item = itemvaule;
                return;
            }
            else
                continue;
        }
    }

    /// <summary>
    /// 獲取金幣
    /// </summary>
    /// <param name="coin"></param>
    public void PickUp(int coin)
    {
        CoinText.text += coin.ToString();
        PlayerData.Coin = int.Parse(CoinText.text);
    }
}
