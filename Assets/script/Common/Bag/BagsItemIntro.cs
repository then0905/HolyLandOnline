using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.ComponentModel;
using System.Linq;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/10
//  創建用途:  背包物品資訊(直接掛在顯示資訊上)
//==========================================
public class BagsItemIntro : MonoBehaviour
{
    public EventHandler<PointerEventData> BagItemOnClickEvent;

    //顯示資料
    protected IntroData IntroDataClass;

    //UI物件
    [Header("圖片Icon"), SerializeField] protected Image Image;
    [Header("道具名稱"), SerializeField] protected TextMeshProUGUI Name;
    [Header("材質種類"), SerializeField] protected TextMeshProUGUI Classification;
    [Header("穿戴部位"), SerializeField] protected TextMeshProUGUI Part;
    [Header("道具分類"), SerializeField] protected TextMeshProUGUI Type;
    [Header("所需等及"), SerializeField] protected TextMeshProUGUI Lv;
    [Header("強化等級"), SerializeField] protected TextMeshProUGUI Forge;
    [Header("影響數據"), SerializeField] protected TextMeshProUGUI Value;
    [Header("道具介紹"), SerializeField] protected TextMeshProUGUI Intro;

    [Header("快捷管理器")]
    public HotKeyManager HotKeyManager;

    //當前資訊呈現物件
    protected Equipment introItem;
    public Equipment IntroItem
    {
        get
        {
            return introItem;
        }
        set
        {
            introItem = value;
        }
    }

    [Serializable]
    public class IntroData
    {
        public string Name;
        public string Classification;
        public string Part;
        public string Type;
        public string Lv;
        public string Content;
        public int ForgeLv;
        public Sprite Icon;
    }
    /// <summary>
    /// 點擊事件
    /// </summary>
    /// <param name="BagsItem"></param>
    public void OnClick(RectTransform topOfUnit, PointerEventData BagsItem)
    {
        //檢查是否為背包格子
        if (BagsItem.pointerCurrentRaycast.gameObject.tag != "BagItem" && BagsItem.pointerCurrentRaycast.gameObject.tag != "Equip")
            return;
        //物品資訊顯示位置跟隨鼠標
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(topOfUnit, Input.mousePosition, Camera.main, out pos);
        GetComponent<RectTransform>().anchoredPosition = pos;

        //獲取此次點擊的物品資料
        Equipment equipment = BagsItem.pointerCurrentRaycast.gameObject.GetComponent<Equipment>();

        //檢查是否重複點擊同件物品
        if (introItem == null && introItem != equipment)
        {
            //若不是則記錄物品資料
            introItem = equipment;

            //檢查空值
            if (introItem.EquipmentDatas != null)
            {
                //物品為武器類
                if (introItem.EquipmentDatas.Weapon != null)
                {
                    //設定基本資訊
                    SetItemIntro(new IntroData
                    {
                        Name = introItem.EquipmentDatas.Weapon.Name.GetText(),
                        Classification = introItem.EquipmentDatas.Weapon.Classification,
                        Part = introItem.EquipmentDatas.Weapon.TackHand,
                        Type = introItem.EquipmentDatas.Weapon.Type,
                        Lv = introItem.EquipmentDatas.Weapon.LV.ToString(),
                        Content = introItem.EquipmentDatas.Weapon.Intro.GetText(),
                        ForgeLv = introItem.EquipmentDatas.ForceLv,
                        Icon = introItem.EquipImage.sprite
                    });

                    //設定武器數據或盾牌數據
                    if (introItem.EquipmentDatas.Weapon.Type == "盾牌")
                        AddShieldVaule();
                    else
                        AddWeaponVaule();

                    //顯示物品資訊介面
                    gameObject.SetActive(true);
                }
                //物品為防具類
                else if (introItem.GetComponent<Equipment>().EquipmentDatas.Armor != null)
                {
                    //設定基本資訊
                    SetItemIntro(new IntroData
                    {
                        Name = introItem.EquipmentDatas.Armor.Name.GetText(),
                        Classification = introItem.EquipmentDatas.Armor.Classification,
                        Part = introItem.EquipmentDatas.Armor.WearPart,
                        Type = introItem.EquipmentDatas.Armor.Type,
                        Lv = introItem.EquipmentDatas.Armor.NeedLv.ToString(),
                        Content = introItem.EquipmentDatas.Armor.Intro.GetText(),
                        ForgeLv = introItem.EquipmentDatas.ForceLv,
                        Icon = introItem.EquipImage.sprite
                    });
                    //設定防具數據
                    AddArmorVaule();
                    //顯示物品資訊介面
                    gameObject.SetActive(true);
                }
                //物品為道具類
                else if (introItem.GetComponent<Equipment>().EquipmentDatas.Item != null)
                {
                    //設定基本資訊
                    SetItemIntro(new IntroData
                    {
                        Name = introItem.EquipmentDatas.Item.Name.GetText(),
                        Classification = introItem.EquipmentDatas.Item.Classification,
                        Part = introItem.EquipmentDatas.Item.TakeHand,
                        Type = introItem.EquipmentDatas.Item.Type,
                        Lv = introItem.EquipmentDatas.Item.LV.ToString(),
                        Content = introItem.EquipmentDatas.Item.Intro.GetText(),
                        Icon = introItem.EquipImage.sprite
                    });
                    //設定物品資訊
                    AddItemVaule();
                    //顯示物品資訊介面
                    gameObject.SetActive(true);
                }
                else
                {
                    //若非以上類型 設定空值
                    gameObject.SetActive(false);
                    introItem = null;
                }
            }
        }
        else
        {
            //空值並關閉物件
            gameObject.SetActive(false);
            introItem = null;
        }
    }
    /// <summary>
    /// 設定物品基本資訊
    /// </summary>
    /// <param name="introData">顯示資料</param>
    private void SetItemIntro(IntroData introData)
    {
        Name.text = "TM_ItemName".GetText(true) + introData.Name;
        Classification.text = "TM_Classification".GetText(true) + introData.Classification;
        Part.text = "TM_WearPart".GetText(true) + introData.Part;
        Type.text = "TM_ItemType".GetText(true) + introData.Type;
        Lv.text = "TM_NeedLv".GetText(true) + introData.Lv;
        Forge.text = string.Format("TM_ForgeLV".GetText(),introData.ForgeLv);
        Intro.text = introData.Content;
        Image.sprite = introData.Icon;
    }
    #region 物品數據
    /// <summary>
    /// 顯示武器數據
    /// </summary>
    private void AddWeaponVaule()
    {
        ForgeData forgeData = introItem.EquipmentDatas.Weapon.ForgeConfigList.Where(x => x.ForgeLv.Equals(introItem.EquipmentDatas.ForceLv)).FirstOrDefault();
        if (forgeData != null)
        {
            if (introItem.EquipmentDatas.Weapon.MeleeATK != 0)
            {
                Value.text =
                    "TM_MeleeATK".GetText(true) + introItem.EquipmentDatas.Weapon.MeleeATK +
                    (forgeData.MeleeATK.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MeleeATK)) + "\n"
                    + "TM_MeleeHit".GetText(true) + introItem.EquipmentDatas.Weapon.MeleeHit +
                     (forgeData.MeleeHit.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MeleeHit)) + "\n";
            }
            if (introItem.EquipmentDatas.Weapon.RemoteATK != 0)
            {
                Value.text =
                     "TM_RemoteATK".GetText(true) + introItem.EquipmentDatas.Weapon.RemoteATK +
                      (forgeData.RemoteATK.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.RemoteATK)) + "\n"
                    + "TM_RemoteHit".GetText(true) + introItem.EquipmentDatas.Weapon.RemoteHit +
                     (forgeData.RemoteHit.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.RemoteHit)) + "\n";

            }
            if (introItem.EquipmentDatas.Weapon.MageATK != 0)
            {
                Value.text =
                     "TM_MageATK".GetText(true) + introItem.EquipmentDatas.Weapon.MageATK +
                     (forgeData.MageATK.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MageATK)) + "\n"
                    + "TM_MAgeHit".GetText(true) + introItem.EquipmentDatas.Weapon.MageHit +
                     (forgeData.MageHit.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MageHit)) + "\n";
            }
            Value.text += "TM_AS".GetText(true) + introItem.EquipmentDatas.Weapon.AS + "\n"
                   + "TM_Crt".GetText(true) + introItem.EquipmentDatas.Weapon.Crt + (forgeData.Crt.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.Crt)) + "\n"
                   + "TM_CrtDamage".GetText(true) + introItem.EquipmentDatas.Weapon.CrtDamage + (forgeData.CrtDamage.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.CrtDamage)) + "\n"
                   + "TM_STR".GetText(true) + introItem.EquipmentDatas.Weapon.STR + (forgeData.STR.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.STR)) + "\n"
                   + "TM_DEX".GetText(true) + introItem.EquipmentDatas.Weapon.DEX + (forgeData.DEX.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.DEX)) + "\n"
                   + "TM_INT".GetText(true) + introItem.EquipmentDatas.Weapon.INT + (forgeData.INT.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.INT)) + "\n"
                   + "TM_AGI".GetText(true) + introItem.EquipmentDatas.Weapon.AGI + (forgeData.AGI.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.AGI)) + "\n"
                   + "TM_VIT".GetText(true) + introItem.EquipmentDatas.Weapon.VIT + (forgeData.VIT.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.VIT)) + "\n"
                   + "TM_WIS".GetText(true) + introItem.EquipmentDatas.Weapon.WIS + (forgeData.WIS.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.WIS));
        }
        else
        {
            if (introItem.EquipmentDatas.Weapon.MeleeATK != 0)
            {
                Value.text =
                    "TM_MeleeATK".GetText(true) + introItem.EquipmentDatas.Weapon.MeleeATK + "\n"
                    + "TM_MeleeHit".GetText(true) + introItem.EquipmentDatas.Weapon.MeleeHit + "\n";
            }
            if (introItem.EquipmentDatas.Weapon.RemoteATK != 0)
            {
                Value.text =
                     "TM_RemoteATK".GetText(true) + introItem.EquipmentDatas.Weapon.RemoteATK + "\n"
                    + "TM_RemoteHit".GetText(true) + introItem.EquipmentDatas.Weapon.RemoteHit + "\n";

            }
            if (introItem.EquipmentDatas.Weapon.MageATK != 0)
            {
                Value.text =
                     "TM_MageATK".GetText(true) + introItem.EquipmentDatas.Weapon.MageATK + "\n"
                    + "TM_MAgeHit".GetText(true) + introItem.EquipmentDatas.Weapon.MageHit + "\n";
            }
            Value.text += "TM_AS".GetText(true) + introItem.EquipmentDatas.Weapon.AS + "\n"
                   + "TM_Crt".GetText(true) + introItem.EquipmentDatas.Weapon.Crt + "\n"
                   + "TM_CrtDamage".GetText(true) + introItem.EquipmentDatas.Weapon.CrtDamage + "\n"
                   + "TM_STR".GetText(true) + introItem.EquipmentDatas.Weapon.STR + "\n"
                   + "TM_DEX".GetText(true) + introItem.EquipmentDatas.Weapon.DEX + "\n"
                   + "TM_INT".GetText(true) + introItem.EquipmentDatas.Weapon.INT + "\n"
                   + "TM_AGI".GetText(true) + introItem.EquipmentDatas.Weapon.AGI + "\n"
                   + "TM_VIT".GetText(true) + introItem.EquipmentDatas.Weapon.VIT + "\n"
                   + "TM_WIS".GetText(true) + introItem.EquipmentDatas.Weapon.WIS + "\n";
        }
    }
    /// <summary>
    /// 顯示防具數據
    /// </summary>
    private void AddArmorVaule()
    {
        ForgeData forgeData = introItem.EquipmentDatas.Armor.ForgeConfigList.Where(x => x.ForgeLv.Equals(introItem.EquipmentDatas.ForceLv)).FirstOrDefault();
        if (forgeData != null)
        {
            Value.text = "TM_DEF".GetText(true) + introItem.EquipmentDatas.Armor.DEF +
                    (forgeData.DEF.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.DEF)) + "\n"
                    + "TM_Avoid".GetText(true) + introItem.EquipmentDatas.Armor.Avoid +
                    (forgeData.Avoid.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.Avoid)) + "\n"
                    + "TM_MDEF".GetText(true) + introItem.EquipmentDatas.Armor.MDEF +
                    (forgeData.MDEF.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MDEF)) + "\n"
                    + "TM_Speed".GetText(true) + introItem.EquipmentDatas.Armor.Speed +
                    (forgeData.Speed.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.Speed)) + "\n"
                    + "TM_CrtResistance".GetText(true) + introItem.EquipmentDatas.Armor.CrtResistance +
                    (forgeData.CrtResistance.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.CrtResistance)) + "\n"
                    + "TM_DamageReduction".GetText(true) + introItem.EquipmentDatas.Armor.DamageReduction +
                    (forgeData.DamageReduction.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.DamageReduction)) + "\n"
                    + "TM_MaxHP".GetText(true) + introItem.EquipmentDatas.Armor.HP +
                    (forgeData.HP.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.HP)) + "\n"
                    + "TM_MaxMP".GetText(true) + introItem.EquipmentDatas.Armor.MP +
                    (forgeData.MP.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MP)) + "\n"
                    + "TM_HP_Recovery".GetText(true) + introItem.EquipmentDatas.Armor.HpRecovery +
                    (forgeData.HpRecovery.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.HpRecovery)) + "\n"
                    + "TM_MP_Recovery".GetText(true) + introItem.EquipmentDatas.Armor.MpRecovery +
                    (forgeData.MpRecovery.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MpRecovery)) + "\n"
                    + "TM_STR".GetText(true) + introItem.EquipmentDatas.Armor.STR +
                    (forgeData.STR.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.STR)) + "\n"
                    + "TM_DEX".GetText(true) + introItem.EquipmentDatas.Armor.DEX +
                    (forgeData.DEX.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.DEX)) + "\n"
                    + "TM_INT".GetText(true) + introItem.EquipmentDatas.Armor.INT +
                    (forgeData.INT.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.INT)) + "\n"
                    + "TM_AGI".GetText(true) + introItem.EquipmentDatas.Armor.AGI +
                    (forgeData.AGI.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.AGI)) + "\n"
                    + "TM_VIT".GetText(true) + introItem.EquipmentDatas.Armor.VIT +
                    (forgeData.VIT.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.VIT)) + "\n"
                    + "TM_WIS".GetText(true) + introItem.EquipmentDatas.Armor.WIS +
                    (forgeData.WIS.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.WIS)) + "\n"
                    + "TM_ElementDamageReduction".GetText(true) + introItem.EquipmentDatas.Armor.ElementDamageReduction +
                    (forgeData.ElementDamageReduction.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.ElementDamageReduction)) + "\n"
                    + "TM_DisorderResistance".GetText(true) + introItem.EquipmentDatas.Armor.DisorderResistance +
                    (forgeData.DisorderResistance.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.DisorderResistance)) + "\n";
        }
        else
        {
            Value.text = "TM_DEF".GetText(true) + introItem.EquipmentDatas.Armor.DEF + "\n"
                 + "TM_Avoid".GetText(true) + introItem.EquipmentDatas.Armor.Avoid + "\n"
                 + "TM_MDEF".GetText(true) + introItem.EquipmentDatas.Armor.MDEF + "\n"
                 + "TM_Speed".GetText(true) + introItem.EquipmentDatas.Armor.Speed + "\n"
                 + "TM_CrtResistance".GetText(true) + introItem.EquipmentDatas.Armor.CrtResistance + "\n"
                 + "TM_DamageReduction".GetText(true) + introItem.EquipmentDatas.Armor.DamageReduction + "\n"
                 + "TM_MaxHP".GetText(true) + introItem.EquipmentDatas.Armor.HP + "\n"
                 + "TM_MaxMP".GetText(true) + introItem.EquipmentDatas.Armor.MP + "\n"
                 + "TM_HP_Recovery".GetText(true) + introItem.EquipmentDatas.Armor.HpRecovery + "\n"
                 + "TM_MP_Recovery".GetText(true) + introItem.EquipmentDatas.Armor.MpRecovery + "\n"
                 + "TM_STR".GetText(true) + introItem.EquipmentDatas.Armor.STR + "\n"
                 + "TM_DEX".GetText(true) + introItem.EquipmentDatas.Armor.DEX + "\n"
                 + "TM_INT".GetText(true) + introItem.EquipmentDatas.Armor.INT + "\n"
                 + "TM_AGI".GetText(true) + introItem.EquipmentDatas.Armor.AGI + "\n"
                 + "TM_VIT".GetText(true) + introItem.EquipmentDatas.Armor.VIT + "\n"
                 + "TM_WIS".GetText(true) + introItem.EquipmentDatas.Armor.WIS + "\n"
                 + "TM_ElementDamageReduction".GetText(true) + introItem.EquipmentDatas.Armor.ElementDamageReduction + "\n"
                 + "TM_DisorderResistance".GetText(true) + introItem.EquipmentDatas.Armor.DisorderResistance + "\n";
        }
    }
    /// <summary>
    /// 顯示盾牌數據
    /// </summary>
    void AddShieldVaule()
    {
        ForgeData forgeData = introItem.EquipmentDatas.Weapon.ForgeConfigList.Where(x => x.ForgeLv.Equals(introItem.EquipmentDatas.ForceLv)).FirstOrDefault();
        if (forgeData != null)
        {
            Value.text = "TM_DEF".GetText(true) + introItem.EquipmentDatas.Weapon.DEF +
                    (forgeData.DEF.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.DEF)) + "\n"
               + "TM_MDEF".GetText(true) + introItem.EquipmentDatas.Weapon.MDEF +
                    (forgeData.MDEF.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MDEF)) + "\n"
               + "TM_Avoid".GetText(true) + introItem.EquipmentDatas.Weapon.Avoid +
                    (forgeData.Avoid.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.Avoid)) + "\n"
               + "TM_MaxHP".GetText(true) + introItem.EquipmentDatas.Weapon.HP +
                    (forgeData.HP.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.HP)) + "\n"
               + "TM_MaxMP".GetText(true) + introItem.EquipmentDatas.Weapon.MP +
                    (forgeData.MP.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.MP)) + "\n"
               + "TM_BlockRate".GetText(true) + introItem.EquipmentDatas.Weapon.BlockRate +
                    (forgeData.BlockRate.Equals(0) ? "" : string.Format("TM_ForgeAppend".GetText(), forgeData.BlockRate)) + "\n";
        }
        else
        {
            Value.text = "TM_DEF".GetText(true) + introItem.EquipmentDatas.Weapon.DEF + "\n"
              + "TM_MDEF".GetText(true) + introItem.EquipmentDatas.Weapon.MDEF + "\n"
              + "TM_Avoid".GetText(true) + introItem.EquipmentDatas.Weapon.Avoid + "\n"
              + "TM_MaxHP".GetText(true) + introItem.EquipmentDatas.Weapon.HP + "\n"
              + "TM_MaxMP".GetText(true) + introItem.EquipmentDatas.Weapon.MP + "\n"
              + "TM_BlockRate".GetText(true) + introItem.EquipmentDatas.Weapon.BlockRate + "\n";
        }
    }
    /// <summary>
    /// 顯示道具數據
    /// </summary>
    private void AddItemVaule()
    {
        var itemData = introItem.EquipmentDatas.Item;
        Value.text = "TM_Volume".GetText(true) + (itemData.Volume.Count > 0 ? itemData.Volume[0] + "~" + itemData.Volume[itemData.Volume.Count - 1] : itemData.Volume[0]) + "\n"
               + "TM_CD".GetText(true) + itemData.CD + "\n"
               + "TM_Duration".GetText(true) + itemData.ActionTime + "\n";
    }
    #endregion
}
