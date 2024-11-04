using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

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
    /// 技能資料
    /// </summary>
    SkillData SkillData { get; set; }

    /// <summary>
    /// 技能組件資料清單
    /// </summary>
    List<ISkillComponent> SkillComponentList { get; set; }

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
    IEnumerator UpdateCooldown(float deltaTime);
}

/// <summary>
/// 技能組件接口
/// </summary>
public interface ISkillComponent
{
    /// <summary>
    /// 技能效果實現
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="target">被施放者</param>
    void Execute(ICombatant caster, ICombatant target);
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
    public string SkillID { get { return skillID; } }

    public string SkillName { get; protected set; }

    public string SkillIntro { get; protected set; }

    public float CooldownTime { get; protected set; }

    public int CastMage { get; protected set; }

    public SkillData SkillData { get; set; }

    public List<ISkillComponent> SkillComponentList { get; set; } = new List<ISkillComponent>();

    public virtual bool SkillCanUse(float tempMana)
    {
        //魔力是否足夠施放
        if (tempMana - CastMage <= 0)
            CommonFunction.MessageHint(string.Format("TM_SkillCastMageError".GetText(), SkillName), HintType.Warning);
        //是否正在冷卻時間
        else if (TempCooldownTime > 0)
            CommonFunction.MessageHint(string.Format("TM_SkillCoolDownError".GetText(), SkillName), HintType.Warning);

        return (tempMana - CastMage >= 0) && (TempCooldownTime <= 0);
    }

    public float TempCooldownTime { get; protected set; }

    public string KeyID => SkillID;

    protected bool skillBeUpgrade = false;      //技能是否被升級

    [Header("技能ID 用來從GameData找資料輸入"), SerializeField] protected string skillID;

    [Header("生成的動畫特效物件"), SerializeField] protected GameObject effectObj;

    [SerializeField] protected string skillCondtionHintText = "TM_SkillConditionError";   //使用者要施放技能但條件未達成 提示的字樣

    protected bool skillCondtionCheck = false;      //技能條件確認

    /// <summary>
    /// 回傳 技能條件檢查結果
    /// </summary>
    public bool SkillConditionCheck
    {
        get
        {
            if (!skillCondtionCheck)
                CommonFunction.MessageHint(skillCondtionHintText.GetText(), HintType.Warning);
            return skillCondtionCheck;
        }
    }

    /// <summary>
    /// 確認是否需要使用技能檢查
    /// </summary>
    protected bool UseSkillCheck
    {
        get
        {
            //若 計算資料 每項都需要OR條件 直接回傳true
            if (SkillData.SkillOperationDataList.All(x => x.ConditionOR.CheckAnyData()))
                return true;
            //回傳 有需要判斷條件(有任何一筆OR條件或是AND條件)
            return SkillData.SkillOperationDataList.Any(x => (x.ConditionOR.CheckAnyData()) ||
            (x.ConditionAND.CheckAnyData()));
        }
    }

    //取得 技能的所有運算資料
    protected List<SkillOperationData> skillOperationList => SkillData.SkillOperationDataList;

    private float dis;      //暫存當前玩家與目標的距離

    protected void OnEnable()
    {

    }

    protected void OnDestroy()
    {
        if (UseSkillCheck && PlayerDataOverView.Instance != null)
            SkillController.Instance.SkillConditionCheckEvent -= SkillBuffSub;
    }

    /// <summary>
    /// 初始化技能資料
    /// </summary>
    /// <param name="skillUpgrade">是否升級技能</param>
    /// <param name="caster">施放者</param>
    /// <param name="receiver">接收者</param>
    public virtual void InitSkillEffectData(bool skillUpgrade = false, ICombatant caster = null, ICombatant receiver = null)
    {
        skillBeUpgrade = skillUpgrade;

        //獲取GameData技能資料
        SkillData = GameData.SkillDataDic[skillID];

        CooldownTime = SkillData.CD;
        CastMage = SkillData.CastMage;

        SkillName = SkillData.Name.GetText();
        SkillIntro = SkillData.Intro.GetText();

        //設定技能組件接口
        SkillData.SkillOperationDataList.ForEach(x => SkillComponentList.Add(SkillComponent(x)));

        //設定生成特效參考
        if (effectObj != null) InitSkillEffect(SkillData.EffectTarget);

        //是否需要判斷條件 需要的話 訂閱事件
        if (UseSkillCheck)
            SkillController.Instance.SkillConditionCheckEvent += SkillBuffSub;
        else
            //不需要判斷條件 直接允許技能使用
            skillCondtionCheck = true;
    }

    //測試方法 目前測試暈眩效果 需要修改或測試效果 反註解依需求修改即可
    //private void Update()
    //{
    //    if (Input.GetKeyUp("f"))
    //    {
    //        var test = GameData.SkillDataDic["Warrior_1"].SkillOperationDataList.Where(x => x.InfluenceStatus == "Stun").FirstOrDefault();
    //        ISkillComponent testC = SkillComponent(test);
    //        testC.Execute(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
    //    }
    //}

    /// <summary>
    /// 技能施放開始
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="receiver">被施放者</param>
    protected virtual void SkillEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
        SkillComponentList.ForEach(x => x.Execute(caster, receiver));
    }

    /// <summary>
    /// 技能施放結束
    /// </summary>
    protected abstract void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null);

    /// <summary>
    /// 技能施放結束 For AnimationEvent
    /// </summary>
    public void SkillEndForAnimation()
    {
        SkillEffectEnd();
    }

    public void SkillEffect(ICombatant caster = null, ICombatant target = null)
    {
        PlayerDataOverView.Instance?.ChangeMpEvent?.Invoke(-1 * CastMage);
        if (!CooldownTime.Equals(0))
            StartCoroutine(UpdateCooldown(CooldownTime));
        SkillEffectStart(caster, target);
    }

    /// <summary>
    /// 訂閱使用 角色狀態改變時呼叫 檢查條件查看是否達成條件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public virtual void SkillBuffSub(string key)
    {
        List<bool> checkCondtionOR = new List<bool>();
        List<bool> checkCondtionAND = new List<bool>();

        //檢查 技能運算資料清單
        foreach (var operationData in skillOperationList)
        {
            var listOR = operationData.ConditionOR?.Where(x => x.Contains(key)).ToList();
            var listAND = operationData.ConditionAND?.Where(x => x.Contains(key)).ToList();

            //發送的訂閱類型非此被動技能的類型 不進行檢查
            if (!listOR.CheckAnyData() && !listAND.CheckAnyData())
                return;

            //檢查條件清單
            if (listOR.CheckAnyData())
                checkCondtionOR.Add(CheckConditionOR(listOR));
            if (listAND.CheckAnyData())
                checkCondtionAND.Add(CheckConditionAND(listAND));
        }
        //設定技能是否允許使用 (OR條件任一達成 以及 AND條件全達成或是沒有任何AND資料)
        skillCondtionCheck = (checkCondtionAND.CheckAnyData()) ? (checkCondtionOR.Any(x => x) && checkCondtionAND.All(x => x)) : checkCondtionOR.Any(x => x);
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
        var effectData = GameData.SkillDataDic[skillUpgradeID];
        SkillData.CastMage = effectData.CastMage;
        SkillData.CD = effectData.CD;
        SkillData.ChantTime = effectData.ChantTime;
        SkillData.AnimaTrigger = effectData.AnimaTrigger;
        SkillData.Type = effectData.Type;
        SkillData.EffectTarget = effectData.EffectTarget;
        SkillData.Distance = effectData.Distance;
        SkillData.Width = effectData.Width;
        SkillData.Height = effectData.Height;
        SkillData.CircleDistance = effectData.CircleDistance;
        SkillData.Damage = effectData.Damage;
        SkillData.AdditionMode = effectData.AdditionMode;
        //升級資訊完成 執行程式
        skillBeUpgrade = false;
        SkillEffect(caster, receiver);
    }

    #region 技能運作功能類方法

    /// <summary>
    /// 確認玩家是否進入可施放範圍
    /// </summary>
    public IEnumerator SkillDistanceCheck()
    {
        Vector3 direction;
        DistanceWithTarget();
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
        //若還沒進入施放距離則移動玩家
        while (dis > SkillData.Distance)
        {
            DistanceWithTarget();

            // 取得角色面相目標的方向
            direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
            // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
            direction.y = 0;
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
        // 取得角色面相目標的方向
        direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
        // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
        direction.y = 0;
        PlayerDataOverView.Instance.CharacterMove.Character.transform.rotation = Quaternion.LookRotation(direction);
        //SkillController.Instance.UsingSkill = true;
        SkillEffect(PlayerDataOverView.Instance, SelectTarget.Instance.Targetgameobject);
    }

    /// <summary>
    /// 取得玩家與目標間的距離
    /// </summary>
    /// <returns></returns>
    private float DistanceWithTarget()
    {
        if (SelectTarget.Instance.CatchTarget)
            dis = Vector3.Distance(SelectTarget.Instance.Targetgameobject.Povit.position, PlayerDataOverView.Instance.Povit.transform.position);
        else
            return 0;
        return dis;
    }

    /// <summary>
    /// 詳細條件判斷處理
    /// </summary>
    /// <param name="key">條件名稱</param>
    /// <param name="value">條件判斷值</param>
    /// <returns></returns>
    protected List<bool> DetailConditionProcess(List<string> conditions)
    {
        //宣告 判斷條件清單
        List<bool> finalResult = new List<bool>();
        //宣告 儲存條件判斷字典
        List<KeyValuePair<string, object>> conditionDetail = new List<KeyValuePair<string, object>>();

        foreach (string condition in conditions)
        {
            KeyValuePair<string, object> tempData = new KeyValuePair<string, object>(condition.Split('_')[0], condition.Split('_')[1]);
            conditionDetail.Add(tempData);
        }

        //根據處理完的字典資料 判斷條件是否達成
        foreach (var condtionData in conditionDetail)
        {
            switch (condtionData.Key)
            {
                //不含有任何條件判斷
                default:
                    finalResult.Add(false);
                    break;
                //裝備指定類型武器
                case "EquipWeapon":
                    //取得裝備的武器資料
                    var allWeaponData = ItemManager.Instance.EquipDataList.Where(x => x.EquipmentDatas.Weapon != null).ToList();

                    //沒有任何資料直接回傳False
                    if (!allWeaponData.CheckAnyData())
                        finalResult.Add(false);
                    else
                        finalResult.Add(allWeaponData.Any(x => x.EquipmentDatas.Weapon.TypeID == condtionData.Value.ToString()));
                    if (skillID == "Knight_6")
                        Debug.Log(finalResult);
                    break;

                //副手裝備指定類型
                case "EquipLeft":
                    //取得裝備的武器資料
                    allWeaponData = ItemManager.Instance.EquipDataList.Where(x => x.EquipmentDatas.Weapon != null).ToList();

                    //沒有任何資料直接回傳False
                    if (!allWeaponData.CheckAnyData())
                        finalResult.Add(false);
                    else
                    {
                        //尋找左手格子資料
                        var leftItemData = allWeaponData.Where(x => x.GetComponent<EquipData>().PartID.Any(x => x == "LeftHand")).FirstOrDefault();


                        if (leftItemData != null)
                            //判斷是不是指定類型
                            finalResult.Add(leftItemData.EquipmentDatas.Weapon.TypeID == condtionData.Value.ToString());
                        else
                        {
                            Debug.Log("沒有找到左手資料 錯誤了啦>>>>>>>>>>>>>");
                            finalResult.Add(false);
                        }
                    }
                    break;

                //防具裝備指定類型
                case "EquipArmor":
                    //取得裝備的防具資料
                    var allArmorData = ItemManager.Instance.EquipDataList.Where(x => x.EquipmentDatas.Armor != null).ToList();

                    //沒有任何資料直接回傳False
                    if (!allArmorData.CheckAnyData())
                        finalResult.Add(false);
                    else
                        finalResult.Add(allArmorData.Count.Equals(5) && allArmorData.All(x => x.EquipmentDatas.Armor.TypeID == condtionData.Value.ToString()));
                    break;

                    //在戰鬥狀態中
                    //case "InCombatStatus":
                    //缺少戰鬥狀態判斷
                    //return false;
                    //HP低於指定百分比
                    //case "HpLess":
                    //    float conditionHP = PlayerData.MaxHP * float.Parse(value);
                    //    return conditionHP < PlayerData.HP;
                    //HP低於指定百分比
                    //case "HpMore":
                    //    conditionHP = PlayerData.MaxHP * float.Parse(value);
                    //    return conditionHP > PlayerData.HP;
                    //case "Close":
                    //建立靠近單位的判斷(朝單位移動? 雙方距離縮短? 單位判斷與距離多少?)
                    //return false;
                    //case "Random":
                    //缺乏隨機條件(目前有的資料 禁衛軍的"回擊好禮")
                    //return false;
            }
        }
        return finalResult;
    }

    public virtual IEnumerator UpdateCooldown(float deltaTime)
    {
        //暫存冷卻時間
        TempCooldownTime = deltaTime;
        while (TempCooldownTime > 0)
        {
            TempCooldownTime -= 0.1f;
            //更新技能冷卻讀條
            //skillCdSlider.value = CooldownTime - TempCooldownTime;
            var hotkeyData = SkillController.Instance.SkillHotKey.Where(x => (x.TempHotKeyData is Skill_Base) && ((object)x.TempHotKeyData == this)).FirstOrDefault();
            if (hotkeyData == null)
            {
                Debug.Log("快捷鍵上沒有正在施放的技能 :" + SkillName);
                yield break;
            }
            hotkeyData.HotKeyCdProcessor(CooldownTime - TempCooldownTime);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 效果所需條件是否達成判斷 OR版
    /// </summary>
    /// <returns></returns>
    protected bool CheckConditionOR(List<string> condtions)
    {
        if (!condtions.CheckAnyData())
            return (false);

        List<bool> checkResult = new List<bool>();

        checkResult.AddRange(DetailConditionProcess(condtions));

        //回傳條件結果
        return checkResult.Any(x => x == true);
    }

    /// <summary>
    /// 效果所需條件是否達成判斷 AND版
    /// </summary>
    /// <returns></returns>
    protected bool CheckConditionAND(List<string> condtions)
    {
        List<bool> checkResult = new List<bool>();

        checkResult.AddRange(DetailConditionProcess(condtions));

        //回傳條件結果
        return checkResult.All(x => x == true);
    }

    /// <summary>
    /// 取得技能組件
    /// </summary>
    /// <param name="skillComponentID"></param>
    /// <returns></returns>
    public ISkillComponent SkillComponent(SkillOperationData skillOperationData)
    {
        switch (skillOperationData.SkillComponentID)
        {
            case "Damage":
                return new DamageSkillComponent(this, skillOperationData);

            case "MultipleDamage":
                return new MultipleDamageSkillComponent(this, skillOperationData);

            case "ElementtDamage":
                return new ElementtDamageSkillComponent(this, skillOperationData);

            case "DotDamage":
                return new DotDamageSkill(this, skillOperationData);

            case "CrowdControl":
                return new CrowdControlSkillComponent(this, skillOperationData);

            case "Channeled":
                return new ChanneledSkillComponent(this, skillOperationData);

            case "Utility":
                return new UtilitySkillComponent(this, skillOperationData);

            case "Teleportation":
                return new TeleportationSkillComponent(this, skillOperationData);

            case "Lunge":
                return new LungeSkillComponent(this, skillOperationData);

            case "Charge":
                return new ChargeSkillComponent(this, skillOperationData);

            case "UpgradeSkill":
                return new UpgradeSkillComponent(this, skillOperationData);

            case "EnhanceSkill":
                return new EnhanceSkillComponent(this, skillOperationData);

            case "InheritDamage":
                return new InheritDamegeSkillComponent(this, skillOperationData);

            case "PassiveBuff":
                return new PassiveBuffSkillComponent(this, skillOperationData);

            case "ContinuanceBuff":
                return new ContinuanceBuffComponent(this, skillOperationData);

            case "AdditiveBuff ":
                return new AdditiveBuffComponent(this, skillOperationData);

            case "Debuff":
                return new DebuffComponent(this, skillOperationData);

            default:
                return null;
        }
    }

    #endregion 
}
