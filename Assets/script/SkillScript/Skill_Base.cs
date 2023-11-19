using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//==========================================
//  創建者:家豪
//  創建日期:2023/05/03
//  創建用途: 技能效果_基底
//==========================================

/// <summary>
/// 技能類別標籤
/// </summary>
public enum Category
{
    Buff,
    Passive,
    DeBuff,
    MeleeATK,
    RemoteATK,
    MageATK
}

public abstract class Skill_Base : MonoBehaviour
{

    #region GameData上的資料結構
    protected bool characteristic;                // True:主動、False:被動
    protected int multipleValue;                 // 多段傷害的次數 次數大於1需要填

    protected List<float> effectValue;            // 效果值 
    protected List<string> influenceStatus;       // 效果影響的屬性 (Buff)   
    protected List<string> addType;               // 加成運算的方式 Rate:乘法、Value:加法   
    protected string effectCategory;              // 標籤類型    
    protected List<string> additionalEffect;      // 額外附加效果標籤
    protected List<float> additionalEffectValue;  // 額外附加效果的值
    protected List<float> additionalEffectTime;   // 額外附加效果持續時間
    protected Dictionary<string, string> condition;             // 執行技能所需條件

    protected int effectRecive;
    protected int targetCount;                    // 目標數量 -4:範圍內所有怪物-3:範圍內所有敵軍、-2:範圍內所有敵方目標、-1:隊友與自身、0:自己
    protected float effectDurationTime;           // 效果持續時間
    protected float chantTime;                    // 詠唱時間

    protected string additionMode;                // 攻擊模式 戰鬥計算防禦方面使用 (近距離物理、遠距離物理找物防:魔法找魔防)
    protected float distance;                     // 技能範圍(施放者與目標間的距離值)
    protected float width;                        // 矩形範圍的寬
    protected float height;                       // 矩形範圍的長度
    protected float circleDistance;               // 圓形範圍 
    #endregion

    [Header("技能ID 用來從GameData找資料輸入"), SerializeField] protected string SkillName;

    protected Category category;

    /// <summary>
    /// 初始化技能資料
    /// </summary>
    public void InitSkillEffectData()
    {
        //獲取GameData技能資料
        var effectData = GameData.SkillsDataDic[SkillName];
        //轉換List
        TranslateListData(effectData);
        //設定其他資料
        characteristic = effectData.Characteristic;
        multipleValue = effectData.MultipleValue;
        effectCategory = effectData.EffectCategory;
        effectRecive = effectData.EffectRecive;
        targetCount = effectData.TargetCount;
        effectDurationTime = effectData.EffectDurationTime;
        chantTime = effectData.ChantTime;
        additionMode = effectData.AdditionMode;
        distance = effectData.Distance;
        width = effectData.Width;
        height = effectData.Height;
        circleDistance = effectData.CircleDistance;

    }
    /// <summary>
    /// 轉換資料 To List
    /// </summary>
    /// <param name="skillData"></param>
    protected void TranslateListData(JsonDataModel.SkillDataModel skillData)
    {
        effectValue = skillData.EffectValue.SetFloatList();
        addType = skillData.AddType.SetStringList();
        influenceStatus = skillData.InfluenceStatus.SetStringList();
        additionalEffect = skillData.AdditionalEffect.SetStringList();
        additionalEffectValue = skillData.AdditionalEffectValue.SetFloatList();
        additionalEffectTime = skillData.AdditionalEffectTime.SetFloatList();
        //condition = skillData.Condition.SetStringList();
        SplitConditionData(skillData.Condition);
    }

    /// <summary>
    /// 分割並輸入條件資料
    /// </summary>
    /// <param name="conditionID"></param>
    private void SplitConditionData(string conditionID)
    {
        if (string.IsNullOrEmpty(conditionID)) return;
        List<string> tempData = conditionID.SetStringList();
        foreach (string item in tempData)
        {
            var splitData = item.Split();
            condition.TrySetValue(splitData[0], splitData[1]);
        }
    }

    /// <summary>
    /// 效果所需條件是否達成判斷
    /// </summary>
    /// <returns></returns>
    protected bool CheckCondition()
    {
        bool[] checkResult = new bool[condition.Count];
        int i = 0;
        foreach (var item in condition)
        {
            checkResult[i] = DetailConditionProcess(item.Key, item.Value);
            i++;
        }


        return checkResult.All(x => x == true);
    }

    /// <summary>
    /// 詳細條件判斷處理
    /// </summary>
    /// <param name="key">條件名稱</param>
    /// <param name="value">條件判斷值</param>
    /// <returns></returns>
    protected bool DetailConditionProcess(string key, string value)
    {
        switch (key)
        {
            default:
            //裝備指定類型道具
            case "Equip":
                return ItemManager.Instance.EquipDataList.Any(x => x.WeaponData.TypeID.Contains(value) || x.ArmorData.TypeID.Contains(value));
            //在戰鬥狀態中
            case "InCombatStatus":
                //缺少戰鬥狀態判斷
                return false;
            //HP低於指定百分比
            case "HpLess":
                float conditionHP = PlayerData.MaxHP * float.Parse(value);
                return conditionHP < PlayerData.HP;
            //HP低於指定百分比
            case "HpMore":
                conditionHP = PlayerData.MaxHP * float.Parse(value);
                return conditionHP > PlayerData.HP;
            case "Close":
            //建立靠近單位的判斷(朝單位移動? 雙方距離縮短? 單位判斷與距離多少?)
                return false;
            case "Random":
            //缺乏隨機條件(目前有的資料 禁衛軍的"回擊好禮")
                return false;
        }
    }

    /// <summary>
    /// 技能施放開始
    /// </summary>
    protected abstract void SkillEffectStart();
    /// <summary>
    /// 技能施放結束
    /// </summary>
    protected abstract void SkillEffectEnd(string statusType, bool Rate, float value);
}
