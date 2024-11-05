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
    //放入升級後的技能ID 供外部呼叫
    public string UpgradeSkillID;


    ////鍵位
    //public int Keyindex;
    ////鍵位背景圖
    //public Image Background;
    ////放入的資料ID 供外部呼叫參考
    //public string HotKeyDataID;


    /// <summary>
    /// 設定快捷鍵 (技能版)
    /// </summary>
    /// <param name="skillIcon">技能圖示</param>
    /// <param name="data">技能腳本資料</param>
    public void SetSkill(Sprite skillIcon, IHotKey data, string upgradeSkillID = "")
    {
        //先檢查快捷鍵上是否已有此技能資料 有的話清除
        bool queryResult = SkillController.Instance.SkillHotKey.Any(x => x.TempHotKeyData?.KeyID == data.KeyID);

        if (queryResult)
        {
            var targetQueryResult = SkillController.Instance.SkillHotKey.Where(x => x.TempHotKeyData?.KeyID == data.KeyID).ToList();
            targetQueryResult.ForEach(x => x.ClearHotKeyData());
        }

        //設定 技能圖片 與升級後的技能ID
        hotkeyBackground.sprite = skillIcon;
        hotkeyCdSliderImage.sprite = skillIcon;
        tempHotKeyData = Instantiate(((Skill_Base)data).gameObject).GetComponent<Skill_Base>();
        UpgradeSkillID = upgradeSkillID;

        //取得技能腳本資料
        Skill_Base skill_Base = (Skill_Base)tempHotKeyData;
        skill_Base.InitSkillEffectData(!string.IsNullOrEmpty(upgradeSkillID));

        //設定技能讀條
        hotkeyCdSlider.maxValue = skill_Base.CooldownTime;
        hotkeyCdSlider.value = skill_Base.CooldownTime;

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
        tempHotKeyData = null;

        hotkeyCdSlider.maxValue = 0;
        hotkeyCdSlider.value = 0;
    }

    /// <summary>
    /// 技能冷卻時間讀條處理
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
}

