using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能快捷鍵紀錄
//==========================================

/// <summary>
/// 快捷鍵接口
/// </summary>
public interface IHotKey
{
    /// <summary>
    /// 快捷鍵當前資料ID
    /// </summary>
    string KeyID { get; }
}

public class HotKeyData : MonoBehaviour
{
    [Header("遊戲物件")]
    [SerializeField] private Image hotkeyBackground;
    [SerializeField] private Image hotkeyCdSliderImage;
    [SerializeField] private Slider hotkeyCdSlider;
    [Header("遊戲資料")]
    [SerializeField] private int keyindex;
    //暫存此快捷鍵資料 無論是技能還是道具
    private IHotKey tempHotKeyData;
    public IHotKey TempHotKeyData => tempHotKeyData;
    //快捷鍵管理器
    private HotKeyManager hotKeyManager;

    //放入升級後的技能ID 供外部呼叫
    public string UpgradeSkillID;

    private void Start()
    {
        hotKeyManager = HotKeyManager.Instance;
    }

    /// <summary>
    /// 設定快捷鍵 (技能版)
    /// </summary>
    /// <param name="skillIcon">技能圖示</param>
    /// <param name="data">技能腳本資料</param>
    public void SetSkill(Sprite skillIcon, IHotKey data, string upgradeSkillID = "")
    {
        hotKeyManager.PrepareHotKeySetting(data.KeyID);

        //設定 技能圖片 與升級後的技能ID
        hotkeyBackground.sprite = skillIcon;
        hotkeyCdSliderImage.sprite = skillIcon;
        tempHotKeyData = Instantiate(((Skill_Base)data).gameObject).GetComponent<Skill_Base>();
        UpgradeSkillID = upgradeSkillID;

        //取得技能腳本資料
        Skill_Base skill_Base = (Skill_Base)tempHotKeyData;
        skill_Base.InitSkillEffectData(upgradeSkillID);

        //設定技能讀條
        hotkeyCdSlider.maxValue = skill_Base.CooldownTime;
        hotkeyCdSlider.value = skill_Base.CooldownTime;

        //裝上新的技能到快捷鍵 重新刷新狀態 需要呼叫觸發技能條件檢查事件
        StatusOperation.Instance.StatusMethod();
    }

    /// <summary>
    /// 設定快捷鍵 (道具效果版)
    /// </summary>
    /// <param name="itemIcon">技能圖示</param>
    /// <param name="data">技能腳本資料</param>
    public void SetItemEffect(Sprite itemIcon, IHotKey data)
    {
        //先檢查快捷鍵上是否已有此技能資料 有的話清除
        hotKeyManager.PrepareHotKeySetting(data.KeyID);

        //設定 技能圖片 與升級後的技能ID
        hotkeyBackground.sprite = itemIcon;
        hotkeyCdSliderImage.sprite = itemIcon;
        tempHotKeyData = Instantiate(((ItemEffectBase)data).gameObject).GetComponent<ItemEffectBase>();

        //取得技能腳本資料
        ItemEffectBase itemEffect_Base = (ItemEffectBase)tempHotKeyData;
        itemEffect_Base.InitItemEffectData();

        //設定讀條
        hotkeyCdSlider.maxValue = itemEffect_Base.CooldownTime;
        hotkeyCdSlider.value = itemEffect_Base.CooldownTime;

        //裝上新的技能到快捷鍵 重新刷新狀態 需要呼叫觸發技能條件檢查事件
        StatusOperation.Instance.StatusMethod();
    }

    /// <summary>
    /// 清除此快捷鍵儲存的資料
    /// </summary>
    public void ClearHotKeyData()
    {
        //鍵位背景圖
        hotkeyBackground.sprite = HotKeyManager.Instance.HotKeyBackgroundSprite;
        //清除放入的技能資料
        if (tempHotKeyData is Skill_Base)
        {
            Destroy(((Skill_Base)tempHotKeyData).gameObject);

        }
        //清除放入的道具效果資料
        else if (tempHotKeyData is ItemEffectBase)
        {
            Destroy(((ItemEffectBase)tempHotKeyData).gameObject);
        }
        tempHotKeyData = null;

        hotkeyCdSlider.maxValue = 0;
        hotkeyCdSlider.value = 0;
    }

    /// <summary>
    /// 冷卻時間讀條處理
    /// </summary>
    /// <param name="cdTimer"></param>
    public void HotKeyCdProcessor(float cdTimer)
    {
        hotkeyCdSlider.value = cdTimer;
    }

    /// <summary>
    /// 因應升級技能效果更換圖示
    /// </summary>
    public void UpgradeSkillHotkeyDataProceoose(Sprite upgradeSkillSprite)
    {
        hotkeyBackground.sprite = upgradeSkillSprite;
        hotkeyCdSliderImage.sprite = upgradeSkillSprite;
    }

    /// <summary>
    /// 快捷鍵資料使用
    /// </summary>
    /// <param name="keyIndex"></param>
    public void HotKeyDataUse()
    {
        if (tempHotKeyData is Skill_Base skillBase)
        {
            SkillController.Instance.SkillUse(this);
        }
        else if (tempHotKeyData is ItemEffectBase itemBase)
        {
            //判斷 快捷鍵的資料是否為空值
            //if (SkillController.Instance.SkillHotKey[keyIndex].TempHotKeyData != null)
            //{ 
                //判斷道具數量是否足夠 以及 冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能)
                if (!itemBase.ItemEffectCanUse())
                    return;

                itemBase.ItemEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
            //}
        }
    }
}

