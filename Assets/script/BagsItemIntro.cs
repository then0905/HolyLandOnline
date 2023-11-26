using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/10
//  創建用途:  背包物品資訊(直接掛在顯示資訊上)
//==========================================
public class BagsItemIntro : MonoBehaviour
{
    //顯示資料
    protected IntroData IntroDataClass;

    //UI物件
    [Header("圖片Icon"), SerializeField] protected Image Image;
    [Header("道具名稱"), SerializeField] protected TextMeshProUGUI Name;
    [Header("材質種類"), SerializeField] protected TextMeshProUGUI Classification;
    [Header("穿戴部位"), SerializeField] protected TextMeshProUGUI Part;
    [Header("道具分類"), SerializeField] protected TextMeshProUGUI Type;
    [Header("所需等及"), SerializeField] protected TextMeshProUGUI Lv;
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
        public Sprite Icon;
    }
    /// <summary>
    /// 點擊事件
    /// </summary>
    /// <param name="BagsItem"></param>
    public void OnClick(RectTransform topOfUnit,PointerEventData BagsItem)
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
        if (introItem ==null && introItem != equipment)
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
                        Name = introItem.EquipmentDatas.Weapon.Name,
                        Classification = introItem.EquipmentDatas.Weapon.Classification,
                        Part = introItem.EquipmentDatas.Weapon.TackHand,
                        Type = introItem.EquipmentDatas.Weapon.Type,
                        Lv = introItem.EquipmentDatas.Weapon.LV.ToString(),
                        Content = introItem.EquipmentDatas.Weapon.Intro,
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
                        Name = introItem.EquipmentDatas.Armor.Name,
                        Classification = introItem.EquipmentDatas.Armor.Classification,
                        Part = introItem.EquipmentDatas.Armor.WearPart,
                        Type = introItem.EquipmentDatas.Armor.Type,
                        Lv = introItem.EquipmentDatas.Armor.NeedLv.ToString(),
                        Content = introItem.EquipmentDatas.Armor.Intro,
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
                        Name = introItem.EquipmentDatas.Item.Name,
                        Classification = introItem.EquipmentDatas.Item.Classification,
                        Part = introItem.EquipmentDatas.Item.TakeHand,
                        Type = introItem.EquipmentDatas.Item.Type,
                        Lv = introItem.EquipmentDatas.Item.LV.ToString(),
                        Content = introItem.EquipmentDatas.Item.Intro,
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
        Name.text = "道具名稱:" + introData.Name;
        Classification.text = "材質類型:" + introData.Classification;
        Part.text = "裝備部位:" + introData.Part;
        Type.text = "道具分類:" + introData.Type;
        Lv.text = "所需等級:" + introData.Lv;
        Intro.text = introData.Content;
        Image.sprite = introData.Icon;
    }
    #region 物品數據
    /// <summary>
    /// 顯示武器數據
    /// </summary>
    private void AddWeaponVaule()
    {
        if (introItem.EquipmentDatas.Weapon.MeleeATK != 0)
        {
            Value.text =
                "近距離攻擊力:" + introItem.EquipmentDatas.Weapon.MeleeATK + "\n"
                + "近距離命中值:" + introItem.EquipmentDatas.Weapon.MeleeHit + "\n";
        }
        if (introItem.EquipmentDatas.Weapon.RemoteATK != 0)
        {
            Value.text =
                 "遠距離攻擊力:" + introItem.EquipmentDatas.Weapon.RemoteATK + "\n"
                + "遠距離命中值:" + introItem.EquipmentDatas.Weapon.RemoteHit + "\n";

        }
        if (introItem.EquipmentDatas.Weapon.MageATK != 0)
        {
            Value.text =
                 "魔法攻擊力:" + introItem.EquipmentDatas.Weapon.MageATK + "\n"
                + "魔法命中值:" + introItem.EquipmentDatas.Weapon.MageHit + "\n";
        }
        Value.text += "武器攻擊速度:" + introItem.EquipmentDatas.Weapon.AS + "\n"
               + "武器暴擊率:" + introItem.EquipmentDatas.Weapon.Crt + "\n"
               + "武器暴擊傷害:" + introItem.EquipmentDatas.Weapon.CrtDamage + "\n"
               + "STR:" + introItem.EquipmentDatas.Weapon.STR + "\n"
               + "DEX:" + introItem.EquipmentDatas.Weapon.DEX + "\n"
               + "INT:" + introItem.EquipmentDatas.Weapon.Intro + "\n"
               + "AGI:" + introItem.EquipmentDatas.Weapon.AGI + "\n"
               + "VIT:" + introItem.EquipmentDatas.Weapon.VIT + "\n"
               + "WIS:" + introItem.EquipmentDatas.Weapon.WIS + "\n";
    }
    /// <summary>
    /// 顯示防具數據
    /// </summary>
    private void AddArmorVaule()
    {
        Value.text = "防禦值:" + introItem.EquipmentDatas.Armor.DEF + "\n"
                    + "迴避值:" + introItem.EquipmentDatas.Armor.Avoid + "\n"
                    + "魔法防禦值:" + introItem.EquipmentDatas.Armor.MDEF + "\n"
                    + "移動速度:" + introItem.EquipmentDatas.Armor.Speed + "\n"
                    + "暴擊抵抗:" + introItem.EquipmentDatas.Armor.CrtResistance + "\n"
                    + "傷害減緩:" + introItem.EquipmentDatas.Armor.DamageReduction + "\n"
                    + "HP最大值:" + introItem.EquipmentDatas.Armor.HP + "\n"
                    + "MP最大值:" + introItem.EquipmentDatas.Armor.MP + "\n"
                    + "HP自然回復:" + introItem.EquipmentDatas.Armor.HpRecovery + "\n"
                    + "MP自然回復:" + introItem.EquipmentDatas.Armor.MpRecovery + "\n"
                    + "STR:" + introItem.EquipmentDatas.Armor.STR + "\n"
                    + "DEX:" + introItem.EquipmentDatas.Armor.DEX + "\n"
                    + "INT:" + introItem.EquipmentDatas.Armor.INT + "\n"
                    + "AGI:" + introItem.EquipmentDatas.Armor.AGI + "\n"
                    + "VIT:" + introItem.EquipmentDatas.Armor.VIT + "\n"
                    + "WIS:" + introItem.EquipmentDatas.Armor.WIS + "\n"
                    + "屬性傷害減免:" + introItem.EquipmentDatas.Armor.ElementDamageReduction + "\n"
                    + "異常狀態抗性:" + introItem.EquipmentDatas.Armor.DisorderResistance + "\n";

    }
    /// <summary>
    /// 顯示盾牌數據
    /// </summary>
    void AddShieldVaule()
    {
        Value.text = "防禦值:" + introItem.EquipmentDatas.Weapon.DEF + "\n"
               + "魔法防禦值:" + introItem.EquipmentDatas.Weapon.MDEF + "\n"
               + "迴避值:" + introItem.EquipmentDatas.Weapon.Avoid + "\n"
               + "HP最大值:" + introItem.EquipmentDatas.Weapon.HP + "\n"
               + "MP最大值:" + introItem.EquipmentDatas.Weapon.MP + "\n"
               + "格檔率:" + introItem.EquipmentDatas.Weapon.BlockRate + "\n";
    }
    /// <summary>
    /// 顯示道具數據
    /// </summary>
    private void AddItemVaule()
    {
        Value.text = "作用值:" + introItem.EquipmentDatas.Item.Volume + "\n"
               + "冷卻時間:" + introItem.EquipmentDatas.Item.CD + "\n"
               + "持續時間:" + introItem.EquipmentDatas.Item.ActionTime + "\n";
    }
    #endregion
}
