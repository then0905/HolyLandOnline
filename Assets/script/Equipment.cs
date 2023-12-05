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
                Item = this.Item
            };
        }
        public WeaponDataModel Weapon;
        public ArmorDataModel Armor;
        public ItemDataModel Item;
    }
}
