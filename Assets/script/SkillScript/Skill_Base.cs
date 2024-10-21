using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2023/05/03
//  創建用途: 技能效果_基底
//==========================================

#region 接口區

/// <summary>
/// 技能基底接口
/// </summary>
public interface ISkillBase
{
    /// <summary>
    /// 技能ID
    /// </summary>
    string SkillID { get; }

    /// <summary>
    /// 技能名稱
    /// </summary>
    string SkillName { get; }

    /// <summary>
    /// 技能效果描述
    /// </summary>
    string SkillIntro { get; }

    /// <summary>
    /// 冷卻時間
    /// </summary>
    float CooldownTime { get; }

    /// <summary>
    /// 消耗魔力
    /// </summary>
    int CastMage { get; }
}

/// <summary>
/// 「技能」的效果接口
/// </summary>
public interface ISkillEffect : ISkillBase
{
    /// <summary>
    /// 該技能是否可以施放
    /// <param name="tempMana">施放者魔力值</param>
    /// </summary>
    bool SkillCanUse(float tempMana);

    /// <summary>
    /// 技能施放 效果實現
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="target">被施放者</param>
    void SkillEffect(ICombatant caster, ICombatant target);

    /// <summary>
    /// 暫存已進入的冷卻時間
    /// </summary>
    float TempCooldownTime { get; }

    /// <summary>
    /// 更新冷卻時間
    /// </summary>
    /// <param name="deltaTime">冷卻時間</param>
    IEnumerable UpdateCooldown(float deltaTime);
}

/// <summary>
/// 額外效果接口
/// </summary>
public interface IExtraEffect
{
    /// <summary>
    /// 額外效果ID
    /// </summary>
    string ExtraEffectIDName { get; }
    /// <summary>
    /// 額外效果名稱
    /// </summary>
    string ExtraEffectName { get; }
    /// <summary>
    /// 額外效果持續時間
    /// </summary>
    float Duration { get; }
    /// <summary>
    /// 開始附加額外效果
    /// </summary>
    /// <param name="target"></param>
    void Apply(ICombatant target);
    /// <summary>
    /// 額外效果更新時間
    /// </summary>
    /// <param name="target"></param>
    void Update(ICombatant target);
    /// <summary>
    /// 額外效果移除
    /// </summary>
    /// <param name="target"></param>
    void Remove(ICombatant target);
}

#endregion

#region Enum宣告區

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

/// <summary>
/// 控制效果類別標籤
/// </summary>
public enum CrownControlCategory
{
    /// <summary>
    /// 暈眩
    /// </summary>
    Stun,
    /// <summary>
    /// 禁錮
    /// </summary>
    Imprison,
    /// <summary>
    /// 壓制
    /// </summary>
    Suppress,
    /// <summary>
    /// 嘲諷
    /// </summary>
    Taunt,
    /// <summary>
    /// 擊退
    /// </summary>
    Repel,
    /// <summary>
    /// 擊飛
    /// </summary>
    Knockup,
    /// <summary>
    /// 托拽
    /// </summary>
    Drag,
    /// <summary>
    /// 凍結
    /// </summary>
    Freeze,
    /// <summary>
    /// 沉默
    /// </summary>
    Silence,
    /// <summary>
    /// 單挑
    /// </summary>
    Duel
}

/// <summary>
/// 負面效果類別標籤
/// </summary>
public enum DebuffCategory
{
    /// <summary>
    /// 破甲
    /// </summary>
    ArmorBreak,

    /// <summary>
    /// 耗弱
    /// </summary>
    MagicShred,

    /// <summary>
    /// 弱化
    /// </summary>
    Weaken,

    /// <summary>
    /// 出血
    /// </summary>
    Bleeding,

    /// <summary>
    /// 中毒
    /// </summary>
    Poisoned,

    /// <summary>
    /// 灼傷
    /// </summary>
    Scorched,

    /// <summary>
    /// 謙遜
    /// </summary>
    Thorns,

    /// <summary>
    /// 緩速
    /// </summary>
    SpeedSlow,

    /// <summary>
    /// 致盲
    /// </summary>
    Blind,

    /// <summary>
    /// 不死童話
    /// </summary>
    Lichform,

    /// <summary>
    /// 收割
    /// </summary>
    Harvest,

    /// <summary>
    /// 失明
    /// </summary>
    Darkness,

    /// <summary>
    /// 印記
    /// </summary>
    Marked,

    /// <summary>
    /// 烙印
    /// </summary>
    Branded,

    /// <summary>
    /// 創傷
    /// </summary>
    Wound
}

#endregion

public abstract class Skill_Base : MonoBehaviour, ISkillEffect, IHotKey
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

    public int EffectRecive { get { return effectRecive; } }
    public int TargetCount { get { return targetCount; } }                        // 目標數量 -4:範圍內所有怪物-3:範圍內所有敵軍、-2:範圍內所有敵方目標、-1:隊友與自身、0:自己
    public float EffectDurationTime { get { return effectDurationTime; } }              // 效果持續時間
    public float ChantTime { get { return chantTime; } }                        // 詠唱時間
    public string Type { get; set; }            // 技能類型
    public string AdditionMode { get { return additionMode; } }                    // 攻擊模式 戰鬥計算防禦方面使用 (近距離物理、遠距離物理找物防:魔法找魔防)
    public float Distance { get { return distance; } }                        // 技能範圍(施放者與目標間的距離值)
    public float Width { get { return width; } }                           // 矩形範圍的寬
    public float Height { get { return height; } }                         // 矩形範圍的長度
    public float CircleDistance { get { return circleDistance; } }                 // 圓形範圍 

    #endregion

    public string SkillID { get { return skillID; } }

    public string SkillName { get; protected set; }

    public string SkillIntro { get; protected set; }

    public float CooldownTime { get; protected set; }

    public int CastMage { get; protected set; }

    public bool SkillCanUse(float tempMana)
    {
        if (tempMana - CastMage <= 0)
            CommonFunction.MessageHint(string.Format("技能：{0}  所需消耗魔力不足...", SkillName), HintType.Warning);
        else if (TempCooldownTime > 0)
            CommonFunction.MessageHint(string.Format("技能：{0}  正在冷卻時間中...", SkillName), HintType.Warning);

        return (tempMana - CastMage >= 0) && (TempCooldownTime <= 0);
    }

    public Sprite SkillIcon { get; protected set; }

    public float TempCooldownTime { get; protected set; }

    public string KeyID => SkillID;


    [Header("技能ID 用來從GameData找資料輸入"), SerializeField] protected string skillID;

    [Header("生成的動畫特效物件"), SerializeField] protected GameObject effectObj;

    protected SkillEffectCategory category;//技能類別標籤

    protected bool skillBeUpgrade = false;      //技能是否被升級

    private float dis;      //暫存當前玩家與目標的距離

    /// <summary>
    /// 初始化技能資料
    /// </summary>
    /// <param name="costMaga">消耗魔力</param>
    /// <param name="skillUpgrade">是否升級技能</param>
    /// <param name="caster">施放者</param>
    /// <param name="receiver">接收者</param>
    public virtual void InitSkillEffectData(bool skillUpgrade = false, ICombatant caster = null, ICombatant receiver = null)
    {
        skillBeUpgrade = skillUpgrade;
        //獲取GameData技能資料
        SkillDataModel effectData = GameData.SkillsDataDic[skillID];

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

        CooldownTime = effectData.CD;
        CastMage = effectData.CastMage;
        Type = effectData.Type;

        SkillName = effectData.Name;
        SkillIntro = effectData.Intro;

        //如果技能被升級 初始化技能時不執行 等待升級資訊後才執行
        //if (!skillUpgrade)
        //    SkillEffectStart(caster, receiver);
        //設定生成特效參考
        if (effectObj != null) InitSkillEffect(effectData.EffectTarget);
    }

    /// <summary>
    /// 確認玩家是否進入可施放範圍
    /// </summary>
    public IEnumerator SkillDistanceCheck()
    {
        DistanceWithTarget();
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
        //若還沒進入施放距離則移動玩家
        while (dis > Distance)
        {
            DistanceWithTarget();

            // 取得角色面相目標的方向
            Vector3 direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
            // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
            direction.y = 0;
            // 如果 direction 的長度不為零，設定角色的朝向
            if (direction != Vector3.zero)
                PlayerDataOverView.Instance.CharacterMove.Character.transform.rotation = Quaternion.LookRotation(direction);

            //跑步動畫
            PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);
            //設定移動座標
            PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
                Vector3.MoveTowards(PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position,
                SelectTarget.Instance.Targetgameobject.Povit.position,
                PlayerDataOverView.Instance.CharacterMove.MoveSpeed);

            yield return new WaitForEndOfFrame();
        }
        PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
        SkillController.Instance.UsingSkill = true;
        SkillEffect(PlayerDataOverView.Instance,SelectTarget.Instance.Targetgameobject);
    }

    /// <summary>
    /// 取得玩家與目標間的距離
    /// </summary>
    /// <returns></returns>
    private float DistanceWithTarget()
    {
        if (SelectTarget.Instance.CatchTarget)
            dis = Vector3.Distance(SelectTarget.Instance.Targetgameobject.Povit.position, PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position);
        else
            return 0;
        return dis;
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
    /// <param name="caster">施放者</param>
    /// <param name="receiver">被施放者</param>
    protected abstract void SkillEffectStart(ICombatant caster = null, ICombatant receiver = null);

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
                obj = Instantiate(effectObj, PlayerDataOverView.Instance.CharacterMove.Character.transform);
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
    /// <param name="skillUpgradeID">技能升級目標ID</param>
    public void GetSkillUpgradeEffect(string skillUpgradeID, ICombatant caster = null, ICombatant receiver = null)
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
        SkillEffect(caster, receiver);
    }

    public void SkillEffect(ICombatant caster = null, ICombatant target = null)
    {
        SkillEffectStart(caster, target);
    }

    public virtual IEnumerable UpdateCooldown(float deltaTime)
    {
        //暫存冷卻時間
        TempCooldownTime = deltaTime;
        while (TempCooldownTime > 0)
        {
            TempCooldownTime -= 0.1f;
            //更新技能冷卻讀條
            //skillCdSlider.value = CooldownTime - TempCooldownTime;
            var hotkeyData = SkillController.Instance.SkillHotKey.Where(x => (x.TempHotKeyData is Skill_Base) && ((object)x.TempHotKeyData == this)).FirstOrDefault();
            hotkeyData.HotKeyCdProcessor(CooldownTime - TempCooldownTime);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
