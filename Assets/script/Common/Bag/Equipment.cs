using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/07
//  創建用途:  背包格物品資料
//==========================================


[Serializable]
///<summary>  物品資料  </summary>
public class EquipmentData
{
    /// <summary>
    /// 複製一個新的實例使用(防止當原本的資料要被清空時 連帶拖曳目標的資料也會被清空)
    /// </summary>
    /// <returns></returns>
    public EquipmentData Clone()
    {
        return new EquipmentData
        {
            Weapon = this.Weapon,
            Armor = this.Armor,
            Item = this.Item,
            Qty = this.Qty
        };
    }
    public WeaponDataModel Weapon;
    public ArmorDataModel Armor;
    public ItemDataModel Item;

    /// <summary>物品數量</summary>
    public int Qty;
    /// <summary>強化等級</summary>
    public int ForceLv;

    /// <summary>取得此物品簡略的基本資訊</summary>
    public IItemBasal ItemCommonData
    {
        get
        {
            IItemBasal itemBasal = null;
            if (Weapon != null)
                itemBasal = Weapon;
            else if (Armor != null)
                itemBasal = Armor;
            else if (Item != null)
                itemBasal = Item;
            return itemBasal;
        }
    }

    /// <summary>物品基礎資料(用於Json紀錄使用)</summary>
    public EquipmentDataToJson EquipmentDataToJson_
    {
        get
        {
            EquipmentDataToJson tempData = new EquipmentDataToJson();
            if (Weapon != null)
                tempData = new EquipmentDataToJson()
                {
                    CodedID = Weapon.CodeID,
                    Type = Weapon.ClassificationID,
                    Qty = Qty,
                    ForceLv = ForceLv
                };
            if (Armor != null)
                tempData = new EquipmentDataToJson()
                {
                    CodedID = Armor.CodeID,
                    Type = Armor.ClassificationID,
                    Qty = Qty,
                    ForceLv = ForceLv
                };
            if (Item != null)
                tempData = new EquipmentDataToJson()
                {
                    CodedID = Item.CodeID,
                    Type = Item.ClassificationID,
                    Qty = Qty,
                    ForceLv = ForceLv
                };
            return tempData;
        }
    }
}

[Serializable]
///<summary>  物品資料ForJson序列化  </summary>
public class EquipmentDataToJson
{
    public string CodedID;
    public string Type;
    public int Qty;
    public int ForceLv;
}

[RequireComponent(typeof(EventTrigger))]
public class Equipment : MonoBehaviour
{
    [Header("需更換的圖片UI")]
    public Image EquipImage;

    [Header("物品數量")]
    //文字UI
    [SerializeField] private TextMeshProUGUI equipQty;

    [Header("設定物品資料 NPC、Player")] public string EquipmentType = "Player";

    /// <summary>紀錄物品數量資料 </summary>
    public int Qty
    {
        get
        {
            return int.Parse(equipQty.text);
        }
        set
        {
            EquipmentDatas.Qty = value;
            equipQty.text = value.ToString();
            equipQty.gameObject.SetActive(EquipmentDatas.ItemCommonData != null ? EquipmentDatas.ItemCommonData.Stackability: false);
            equipQty.raycastTarget = false;
        }
    }

    [Header("物品資料"), SerializeField]
    public EquipmentData EquipmentDatas;
}
