//==========================================
//  創建者:家豪
//  創建日期:2025/03/10
//  創建用途: 道具使用效果基底_裝備系列
//==========================================

using System;
using System.Collections.Generic;
using System.Linq;

public class ItemEffectBase_Equip : ItemEffectBase
{
    public ArmorDataModel ArmorData { get; set; }
    public WeaponDataModel WeaponData { get; set; }

    protected override List<ItemEffectData> itemEffectDataList => null;

    private EquipmentData equipmentData;
    /// <summary>
    /// 儲存此快捷欄的裝備資料(不可被更改的基準)
    /// </summary>
    public EquipmentData EquipmentData
    {
        get
        {
            return equipmentData;
        }
        set
        {
            equipmentData = new EquipmentData(value);
        }
    }

    /// <summary>
    /// 尋找裝備資料
    /// </summary>
    public Equipment TempEquipment
    {
        get
        {
            var bagData = BagManager.Instance.BagItems
                .Where(x => x.EquipmentDatas.ItemCommonData != null).ToList()
                .Find(x => x.EquipmentDatas.ItemCommonData.CodeID == EquipmentData.ItemCommonData.CodeID && x.EquipmentDatas.ForceLv == EquipmentData.ForceLv);

            var equipmentData = BagManager.Instance.EquipDataList
                                .Where(x => x.EquipmentDatas.ItemCommonData != null).ToList()
                                .Find(x => x.EquipmentDatas.ItemCommonData.CodeID == EquipmentData.ItemCommonData.CodeID && x.EquipmentDatas.ForceLv == EquipmentData.ForceLv);

            return bagData != null ? bagData : equipmentData;
        }
    }

    public override bool ItemEffectCanUse()
    {
        bool qtyBool = (BagManager.Instance.BagItems.Find(x => x.EquipmentDatas.ItemCommonData?.CodeID == TempEquipment.EquipmentDatas.ItemCommonData?.CodeID && x.EquipmentDatas.ForceLv == TempEquipment.EquipmentDatas.ForceLv) != null
            || BagManager.Instance.EquipDataList.Find(x => x.EquipmentDatas.ItemCommonData?.CodeID == TempEquipment.EquipmentDatas.ItemCommonData?.CodeID && x.EquipmentDatas.ForceLv == TempEquipment.EquipmentDatas.ForceLv) != null);
        bool CooldownBool = TempCooldownTime <= 0;
        //背包物品數量不足
        if (!qtyBool)
            CommonFunction.MessageHint(string.Format("TM_ItemEffectQtyError".GetText(), ItemName), HintType.Warning);
        //是否正在冷卻時間
        else if (TempCooldownTime > 0)
            CommonFunction.MessageHint(string.Format("TM_ItemEffectCoolDownError".GetText(), ItemName), HintType.Warning);

        return qtyBool & CooldownBool;
    }

    /// <summary>
    /// 確定物品是否正在被使用中
    /// </summary>
    /// <returns></returns>
    public bool CheckItemIsUse()
    {
        return BagManager.Instance.EquipDataList.Any(x => x.EquipmentDatas.ItemCommonData?.CodeID == TempEquipment.EquipmentDatas.ItemCommonData?.CodeID && x.EquipmentDatas.ForceLv == TempEquipment.EquipmentDatas.ForceLv);
    }

    protected override void ItemEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
        BagManager.Instance.BagItemEquip.HotKeyItemEquipProceoosr(TempEquipment);
    }

    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

    }

    /// <summary>
    /// 初始化道具資料
    /// </summary>
    public override void InitItemEffectData()
    {
        //獲取GameData技能資料
        if (GameData.ArmorsDic.TryGetValue(ItemID, out ArmorDataModel armorValue))
        {
            ArmorData = armorValue;
            ItemName = ArmorData.Name.GetText();
            ItemIntro = ArmorData.Intro.GetText();
        }
        else if (GameData.WeaponsDic.TryGetValue(ItemID, out WeaponDataModel weaponValue))
        {
            WeaponData = weaponValue;
            ItemName = WeaponData.Name.GetText();
            ItemIntro = WeaponData.Intro.GetText();
        }

        //因裝備不需要CD 但不設定的化為0 快捷鍵讀條會異常 故強制設定1
        CooldownTime = 1;

        //設定生成特效參考
        if (effectObj != null) InitItemEffect();
    }
}
