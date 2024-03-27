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
public enum SkillEffectCategory
{
    /// <summary>
    /// 主動型Buff
    /// </summary>
    Buff,
    /// <summary>
    /// 被動Buff
    /// </summary>
    Passive,
    /// <summary>
    /// 純減益效果
    /// </summary>
    DeBuff,
    /// <summary>
    /// 攻擊型
    /// </summary>
    Attack
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
    protected List<string> additionalEffect = new List<string>();      // 額外附加效果標籤
    protected List<float> additionalEffectValue = new List<float>();  // 額外附加效果的值
    protected List<float> additionalEffectTime = new List<float>();   // 額外附加效果持續時間
    protected Dictionary<string, List<string>> condition = new Dictionary<string, List<string>>();             // 執行技能所需條件

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

    #region 供外部獲取資料的結構

    public bool Characteristic { get { return characteristic; } }             // True:主動、False:被動
    public int MultipleValue { get { return multipleValue; } }                 // 多段傷害的次數 次數大於1需要填

    public List<float> EffectValue { get { return effectValue; } }               // 效果值 
    public List<string> InfluenceStatus { get { return influenceStatus; } }        // 效果影響的屬性 (Buff)   
    public List<string> AddType { get { return addType; } }                // 加成運算的方式 Rate:乘法、Value:加法   
    public string EffectCategory { get { return effectCategory; } }                   // 標籤類型    
    public List<string> AdditionalEffect { get { return additionalEffect; } }      // 額外附加效果標籤
    public List<float> AdditionalEffectValue { get { return additionalEffectValue; } }  // 額外附加效果的值
    public List<float> AdditionalEffectTime { get { return additionalEffectTime; } }   // 額外附加效果持續時間
    public Dictionary<string, List<string>> Condition { get { return condition; } }              // 執行技能所需條件

    public int EffectRecive { get { return EffectRecive; } }
    public int TargetCount { get { return TargetCount; } }                        // 目標數量 -4:範圍內所有怪物-3:範圍內所有敵軍、-2:範圍內所有敵方目標、-1:隊友與自身、0:自己
    public float EffectDurationTime { get { return EffectDurationTime; } }              // 效果持續時間
    public float ChantTime { get { return ChantTime; } }                        // 詠唱時間

    public string AdditionMode { get { return AdditionMode; } }                    // 攻擊模式 戰鬥計算防禦方面使用 (近距離物理、遠距離物理找物防:魔法找魔防)
    public float Distance { get { return Distance; } }                        // 技能範圍(施放者與目標間的距離值)
    public float Width { get { return Width; } }                           // 矩形範圍的寬
    public float Height { get { return Height; } }                         // 矩形範圍的長度
    public float CircleDistance { get { return CircleDistance; } }                 // 圓形範圍 

    #endregion

    public string SkillName { get { return skillName; } }

    [Header("技能ID 用來從GameData找資料輸入"), SerializeField] protected string skillName;

    [Header("生成的動畫特效物件"), SerializeField] protected GameObject effectObj;

    protected SkillEffectCategory category;//技能類別標籤

    protected bool skillBeUpgrade = false;      //技能是否被升級

    /// <summary>
    /// 初始化技能資料
    /// </summary>
    /// <param name="costMaga">消耗魔力</param>
    public void InitSkillEffectData(int costMaga, bool skillUpgrade = false)
    {
        skillBeUpgrade = skillUpgrade;
        //獲取GameData技能資料
        var effectData = GameData.SkillsDataDic[skillName];

        //轉換List
        //TranslateListData(effectData);
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
        effectValue = effectData.EffectValue;
        addType = effectData.AddType;
        influenceStatus = effectData.InfluenceStatus;
        additionalEffect = effectData.AdditionalEffect;
        additionalEffectValue = effectData.AdditionalEffectValue;
        additionalEffectTime = effectData.AdditionalEffectTime;
        //扣除消耗魔力
        PlayerValueManager.Instance.ChangeMpEvent?.Invoke(costMaga * -1);

        //如果技能被升級 初始化技能時不執行 等待升級資訊後才執行
        if (!skillUpgrade)
            SkillEffectStart();
        //設定生成特效參考
        if (effectObj != null) InitSkillEffect(effectData.EffectTarget);
    }

    /// <summary>
    /// 分割並輸入條件資料
    /// </summary>
    /// <param name="conditionID"></param>
    private void SplitConditionData(List<string> conditionID)
    {
        if (conditionID == null) return;
        List<string> tempValue = new List<string>();
        string tempKey = "";
        foreach (string item in conditionID)
        {
            //分割出條件(key)與判斷值(value)
            var splitData = item.Split('_');
            
            //判斷key值是否一樣 不一樣清空value
            if (tempKey != splitData[0])
                tempValue = new List<string>();

            tempKey = splitData[0];
            tempValue.Add(splitData[1]);
            condition.TrySetValue(tempKey, tempValue);
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
    protected bool DetailConditionProcess(string key, List<string> value)
    {
        switch (key)
        {
            default:
            //裝備指定類型道具
            case "Equip":
                foreach (var itemData in ItemManager.Instance.EquipDataList)
                {
                    if (itemData.EquipmentDatas.Weapon != null)
                    {
                        if (value.Any(x => x.Contains(itemData.EquipmentDatas.Weapon.TypeID)))
                            return true;
                    }
                    else if (itemData.EquipmentDatas.Armor != null)
                    {
                        if (value.Any(x => x.Contains(itemData.EquipmentDatas.Armor.TypeID)))
                            return true;
                    }
                }
                return false;
            //在戰鬥狀態中
            case "InCombatStatus":
                //缺少戰鬥狀態判斷
                return false;
            //HP低於指定百分比
            //case "HpLess":
            //    float conditionHP = PlayerData.MaxHP * float.Parse(value);
            //    return conditionHP < PlayerData.HP;
            //HP低於指定百分比
            //case "HpMore":
            //    conditionHP = PlayerData.MaxHP * float.Parse(value);
            //    return conditionHP > PlayerData.HP;
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
    protected abstract void SkillEffectEnd();

    /// <summary>
    /// 技能施放結束 For AnimationEvent
    /// </summary>
    public void SkillEndForAnimation()
    {
        SkillEffectEnd();
    }

    /// <summary>
    /// 實例化技能特效物件
    /// </summary>
    /// <param name="targetReference">生成目標參考</param>
    protected void InitSkillEffect(string targetReference)
    {
        GameObject obj;
        switch (targetReference)
        {
            default:
            case "Self":
                obj = Instantiate(effectObj, Character_move.Instance.Character.transform);
                break;
            case "Target":
                obj = Instantiate(effectObj, SelectTarget.Instance.Targetgameobject.transform);
                break;
                //case "TargetArea":
                //case "Team":
        }
        obj.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 從被動獲得技能升級效果 刷新技能內容
    /// </summary>
    /// <param name="skillUpgrade"></param>
    public void GetSkillUpgradeEffect(string skillUpgradeID)
    {
        //獲取GameData技能資料
        var effectData = GameData.SkillsDataDic[skillUpgradeID];
        //設定其他資料
        //characteristic = effectData.Characteristic;
        multipleValue = effectData.MultipleValue;
        //effectCategory = effectData.EffectCategory;
        effectRecive = effectData.EffectRecive;
        targetCount = effectData.TargetCount;
        effectDurationTime = effectData.EffectDurationTime;
        chantTime = effectData.ChantTime;
        additionMode = effectData.AdditionMode;
        distance = effectData.Distance;
        width = effectData.Width;
        height = effectData.Height;
        circleDistance = effectData.CircleDistance;
        effectValue = effectData.EffectValue;
        addType = effectData.AddType;
        influenceStatus = effectData.InfluenceStatus;
        additionalEffect = effectData.AdditionalEffect;
        additionalEffectValue = effectData.AdditionalEffectValue;
        additionalEffectTime = effectData.AdditionalEffectTime;
        //升級資訊完成 執行程式
        skillBeUpgrade = false;
        SkillEffectStart();
    }
}
