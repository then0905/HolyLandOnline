using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  戰鬥計算
//==========================================

public interface ICombatant
{
    /// <summary>名稱</summary>
    public string Name { get; }
    /// <summary>當前血量</summary>
    public int HP { get; set; }
    /// <summary>當前魔力</summary>
    public int MP { get; set; }
    /// <summary>等級</summary>
    public int LV { get; }
    /// <summary>攻擊力</summary>
    public int ATK { get; }
    /// <summary>攻擊模式</summary>
    public string GetAttackMode { get; set; }
    /// <summary>命中值</summary>
    public int Hit { get; }
    /// <summary>迴避值</summary>
    public int Avoid { get; }
    /// <summary>物理防禦值</summary>
    public int DEF { get; }
    /// <summary>魔法防禦值</summary>
    public int MDEF { get; }
    /// <summary>傷害減緩</summary>
    public int DamageReduction { get; }
    /// <summary>暴擊率</summary>
    public float Crt { get; }
    /// <summary>暴擊傷害</summary>
    public int CrtDamage { get; }
    /// <summary>暴擊抵抗</summary>
    public float CrtResistance { get; }
    /// <summary>異常狀態抗性</summary>
    public float DisorderResistance { get; }
    /// <summary>盾牌格檔率</summary>
    public float BlockRate { get; }
    /// <summary>屬性傷害增幅</summary>
    public float ElementDamageIncrease { get; }
    /// <summary>屬性傷害抵抗</summary>
    public float ElementDamageReduction { get; }
    /// <summary>GameObject參考</summary>
    public GameObject Obj { get; }
    /// <summary>物體是否為死亡狀態</summary>
    public bool IsDead { get; set; }
    /// <summary>
    /// 受到攻擊處理
    /// </summary>
    /// <param name="o"></param>
    /// <param name="attackerData">攻擊方</param>
    /// <param name="damage">傷害量</param>
    public void DealingWithInjuriesMethod(ICombatant attackerData, int damage);

    /// <summary>
    /// 賦予目標Buff處理
    /// </summary>
    /// <param name="target"></param>
    /// <param name="skillTarget"></param>
    public void GetBuffEffect(ICombatant target, SkillOperationData skillTarget);

    /// <summary>
    /// 移除目標Buff處理
    /// </summary>
    /// <param name="target"></param>
    /// <param name="skillTarget"></param>
    public void RemoveBuffEffect(ICombatant target, SkillOperationData skillTarget);
}

public class BattleOperation : MonoBehaviour
{

    #region 靜態變數
    private static BattleOperation instance;
    public static BattleOperation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BattleOperation>();
            }
            return instance;
        }
    }
    #endregion

    private float hitValue;     //命中值
    private int hitRate;        //命中率

    //技能攻擊
    public Action<DamageComponent, ICombatant, ICombatant> SkillAttackEvent;
    //普通攻擊
    public Action<ICombatant, ICombatant> NormalAttackEvent;

    private void OnEnable()
    {
        SkillAttackEvent += BattleOperationStart;
        NormalAttackEvent += BattleOperationStart;
    }

    private void OnDisable()
    {
        SkillAttackEvent -= BattleOperationStart;
        NormalAttackEvent -= BattleOperationStart;
    }

    /// <summary>
    /// 獲取戰鬥對象
    /// </summary>    
    /// <param name="attacker">攻擊方</param>
    /// <param name="defender">防守方</param>
    private void BattleOperationStart(ICombatant attacker, ICombatant defender)
    {
        if (attacker != default(ICombatant) && defender != default(ICombatant))
            HitOrMiss(attacker, defender);
    }

    /// <summary>
    /// 獲取戰鬥對象
    /// </summary>    
    /// <param name="skillCompnent">攻擊技能資料</param>
    /// <param name="attacker">攻擊方</param>
    /// <param name="defender">防守方</param>
    public void BattleOperationStart(DamageComponent skillCompnent, ICombatant attacker, ICombatant defender)
    {
        //檢查空值
        if (skillCompnent != null && attacker != default(ICombatant) && defender != default(ICombatant))
            HitOrMiss(attacker, defender, skillCompnent);
    }

    /// <summary>
    ///  是否命中
    /// </summary>
    /// <param name="skillCompnent">攻擊技能資料</param>
    /// <param name="target">目標</param>
    public void HitOrMiss(ICombatant attacker, ICombatant defender, DamageComponent skillCompnent = null)
    {
        //有施放技能 取得技能運算的攻擊類型
        if (skillCompnent != null)
        {
            attacker.GetAttackMode = skillCompnent.SkillBase.SkillData.AdditionMode;
            print("技能倍率:" + skillCompnent.SkillOperationData.EffectValue);
        }
        else
        {
            PlayerDataOverView.Instance.GetAttackMode = (PlayerDataOverView.Instance.PlayerData_.NormalAttackRange >= 3 ? "RemoteATK" : "MeleeATK");
        }

        hitValue = attacker.Hit * 100 / (attacker.Hit + defender.Avoid);
        hitRate = (int)Mathf.Round(hitValue);
        // 命中率
        int isHit = UnityEngine.Random.Range(0, 101);

        // 是否命中
        if (isHit < hitRate)
            CrtOrNormal(attacker, defender, skillCompnent);
        else
            InstanceDmgGUI("Miss", false, defender.Obj);
    }

    /// <summary>
    /// 是否暴擊計算
    /// </summary>
    /// <param name="monsterBehaviour">怪物行為腳本</param>
    public void CrtOrNormal(ICombatant attacker, ICombatant defender, DamageComponent skillComponent = null)
    {
        //宣告暴擊率
        float CrtRate;
        //獲取暴擊率 (攻擊方暴擊率*100% / (攻擊方暴擊 + 被攻擊方暴擊抵抗))
        CrtRate = attacker.Crt * 100 / (attacker.Crt + defender.CrtResistance);

        //查看是否暴擊
        int isCrt = UnityEngine.Random.Range(0, 101);
        DmgOperation(isCrt < CrtRate, attacker, defender, skillComponent);
    }

    /// <summary>
    /// 格擋計算
    /// </summary>
    public void BlockRate()
    {

    }

    /// <summary>
    /// 傷害計算
    /// </summary>
    /// <param name="iscrt">是否暴擊</param>
    /// <param name="monsterBehaviour">怪物行為腳本</param>
    /// <param name="skillcompnent">技能資料</param>
    public void DmgOperation(bool iscrt, ICombatant attacker, ICombatant defender, DamageComponent skillcompnent = null)
    {
        float defRate;//防禦%
        float damage;//總傷害

        damage = attacker.ATK;

        //計算出防禦%
        defRate = 0.75f * (attacker.GetAttackMode == "MageATK" ? defender.MDEF : defender.DEF) / (attacker.LV + 9);

        //是否暴擊 取得傷害量
        if (iscrt)
            damage = (skillcompnent != null) ? damage * skillcompnent.SkillOperationData.EffectValue * (damage * 1.5f + attacker.CrtDamage) : damage * (damage * 1.5f + attacker.CrtDamage);
        else
            damage = (skillcompnent != null) ? damage * skillcompnent.SkillOperationData.EffectValue : damage;

        //傷害經過防禦減免的數值
        int finalDamage = (int)Mathf.Round((1 - defRate) * damage);

        //被攻擊者運算扣除生命
        defender.DealingWithInjuriesMethod(attacker, finalDamage);

        //生成傷害數字
        InstanceDmgGUI(finalDamage.ToString(), iscrt, defender.Obj);
    }

    /// <summary>
    /// 生成傷害數字
    /// </summary>
    /// <param name="dmgVaule">傷害值</param>
    /// <param name="iscrt">是否暴擊</param>
    /// <param name="target">生成數字參考對象</param>
    public void InstanceDmgGUI(string dmgVaule, bool iscrt, GameObject target)
    {
        //載入傷害數字prefab
        GameObject DamageGUI = Resources.Load("DMGtext") as GameObject;
        //取得傷害數字生成位置
        GameObject head = target.transform.GetChild(0).gameObject;

        //判別 是否命中 是否暴擊 產生不同顏色
        if (dmgVaule == "Miss")
        {
            DamageGUI.GetComponent<DamageGUI>().DMGtext.GetComponent<TMP_Text>().color = Color.white;
        }
        else
        {
            if (iscrt)
            {
                DamageGUI.GetComponent<DamageGUI>().DMGtext.GetComponent<TMP_Text>().color = Color.red;
            }
            else
            {
                DamageGUI.GetComponent<DamageGUI>().DMGtext.GetComponent<TMP_Text>().color = Color.yellow;
            }
        }

        //輸入傷害值
        DamageGUI.GetComponent<DamageGUI>().DMGvalue = dmgVaule;
        //生成傷害數字物件
        Instantiate(DamageGUI, head.transform.position, Quaternion.identity, head.transform);

        //還原技能施展防呆
        //SkillDisplayAction.Instance.UsingSkill = false;
    }
}
