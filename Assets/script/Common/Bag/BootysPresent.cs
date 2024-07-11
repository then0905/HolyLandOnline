using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Equipment;

//==========================================
//  創建者:    家豪
//  創建日期:  2023/06/15
//  創建用途:  掉落物的腳本
//==========================================
public class BootysPresent : MonoBehaviour
{
    [Header("物品圖片")]
    public Sprite ThisEquipmentImage;
    [Header("物品數量")]
    //紀錄物品數量
    public int Qty;
    [Header("物品資料"), SerializeField]
    public EquipmentData EquipmentDatas;

    [Header("金幣量")]
    public int Coins;

    /// <summary>
    /// 撿起掉落物處理
    /// </summary>
    public void BePickUP()
    {
        //根據掉落物的資料判斷該掉落物種類
        if (EquipmentDatas.Weapon != null)
            ItemManager.Instance.PickUp(ThisEquipmentImage, EquipmentDatas.Weapon);
        else if (EquipmentDatas.Armor != null)
            ItemManager.Instance.PickUp(ThisEquipmentImage, EquipmentDatas.Armor);
        else if (EquipmentDatas.Item != null)
            ItemManager.Instance.PickUp(ThisEquipmentImage, EquipmentDatas.Item, Qty);
        else if (!Coins.Equals(0))
            ItemManager.Instance.PickUp(Coins);
        //撿起物品 本地紀錄資料刷新點
        LoadPlayerData.SaveUserData();
        //撿起物品後清除
        Destroy(this.gameObject);
    }
}
