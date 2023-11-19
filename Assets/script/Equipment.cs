using UnityEngine;
using UnityEngine.UI;
using JsonDataModel;
using System;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/07
//  創建用途:  背包格物品資料
//==========================================
public class Equipment : MonoBehaviour
{
    [Header("需更換的圖片UI")]
    public Image EquipImage;
    [Header("物品資料"), SerializeField]
    public EquipmentData EquipmentDatas;

    [Serializable]
    ///<summary>  物品資料  </summary>
    public class EquipmentData 
    {
        public WeaponDataModel Weapon;
        public ArmorDataModel Armor;
        public ItemDataModel Item;
    }
}
