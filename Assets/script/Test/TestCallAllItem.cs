using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/04
//  創建用途:測試使用 呼叫面板 生成遊戲中所有可生成的物品 並在場景中生成
//==========================================
public class TestCallAllItem : MonoBehaviour
{
    [Header("生成按鈕物件")]
    [SerializeField] private Equipment equipment;
    [SerializeField] private Transform weaponTrans; //武器生成參考
    [SerializeField] private Transform armorTrans; //防具生成參考
    [SerializeField] private Transform itemTrans; //道具生成參考

    private string tempItem;
    private void Awake()
    {
        foreach (var data in GameData.WeaponsDic)
        {
            Equipment tempEquipMent = Instantiate(equipment, weaponTrans);
            tempEquipMent.EquipmentDatas.Weapon = data.Value;
            tempEquipMent.EquipImage.sprite = CommonFunction.GetItemSprite(tempEquipMent.EquipmentDatas);
            tempEquipMent.gameObject.SetActive(true);
        }
        foreach (var data in GameData.ArmorsDic)
        {
            Equipment tempEquipMent = Instantiate(equipment, armorTrans);
            tempEquipMent.EquipmentDatas.Armor = data.Value;
            tempEquipMent.EquipImage.sprite = CommonFunction.GetItemSprite(tempEquipMent.EquipmentDatas);
            tempEquipMent.gameObject.SetActive(true);
        }
        foreach (var data in GameData.ItemsDic)
        {
            Equipment tempEquipMent = Instantiate(equipment, itemTrans);
            tempEquipMent.EquipmentDatas.Item = data.Value;
            tempEquipMent.EquipImage.sprite = CommonFunction.GetItemSprite(tempEquipMent.EquipmentDatas);
            tempEquipMent.gameObject.SetActive(true);
        }
    }
    public void SetItem(Equipment equipment)
    {
        tempItem = equipment.EquipmentDatas.ItemCommonData.CodeID;
    }
    public void ItemDrop()
    {
        BootysHandle.Instance.DropSpecifiedItem(tempItem);
    }
}
