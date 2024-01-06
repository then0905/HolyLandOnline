using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JsonDataModel;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  戰鬥計算
//==========================================
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

    /// <summary>
    /// 獲取戰鬥對象
    /// </summary>    
    /// <param name="skillBase">攻擊技能資料</param>
    /// <param name="target">目標</param>
    public void BattleOperationStart(Skill_Base_Attack skillBase, GameObject target)
    {
        //檢查空值
        if (skillBase != null && target != null)
            HitOrMiss(skillBase, target);
    }

    /// <summary>
    ///  是否命中
    /// </summary>
    /// <param name="skillBase">攻擊技能資料</param>
    /// <param name="target">目標</param>
    public void HitOrMiss(Skill_Base_Attack skillBase, GameObject target)
    {
        MonsterBehaviour monsterBehaviour = null;
        //取得 怪物行為腳本
        if (target.GetComponent<MonsterBehaviour>() != null)
            monsterBehaviour = target.GetComponent<MonsterBehaviour>();
        else
        {
            //獲取敵方資料
        }

        //判別技能模式
        switch (skillBase.EffectCategory)
        {

            case "MeleeATK":
                hitValue = PlayerData.MeleeHit * 100 / (PlayerData.MeleeHit + monsterBehaviour.MonsterValue.Avoid);
                hitRate = (int)Mathf.Round(hitValue);
                break;

            case "RemoteATK":
                hitValue = PlayerData.RemoteHit * 100 / (PlayerData.RemoteHit + monsterBehaviour.MonsterValue.Avoid);
                hitRate = (int)Mathf.Round(hitValue);
                break;

            case "MageATK":
                hitValue = PlayerData.MageHit * 100 / (PlayerData.MageHit + monsterBehaviour.MonsterValue.Avoid);
                hitRate = (int)Mathf.Round(hitValue);
                break;
            default:
                break;
        }

        // 命中率
        int isHit = Random.Range(0, 101);

        // 是否命中
        if (isHit < hitRate)
            CrtOrNormal(monsterBehaviour, skillBase);
        else
            InstanceDmgGUI("Miss", false, monsterBehaviour.gameObject);
    }

    /// <summary>
    /// 是否暴擊計算
    /// </summary>
    /// <param name="monsterBehaviour">怪物行為腳本</param>
    public void CrtOrNormal(MonsterBehaviour monsterBehaviour, Skill_Base_Attack skillBase)
    {
        //宣告暴擊率
        float CrtRate;    
        //獲取暴擊率(玩家暴擊率*100%/(玩家暴擊+對方暴擊抵抗))
        CrtRate = PlayerData.Crt * 100 / (PlayerData.Crt + monsterBehaviour.MonsterValue.CrtResistance);
        
        //查看是否暴擊
        int isCrt = Random.Range(0, 101);
        if (isCrt < CrtRate)
            DmgOperation(true, monsterBehaviour, skillBase);
        else
            DmgOperation(false, monsterBehaviour, skillBase);
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
    /// <param name="skillData">技能資料</param>
    public void DmgOperation(bool iscrt, MonsterBehaviour monsterBehaviour, Skill_Base_Attack skillData)
    {
        float defRate;//防禦%
        float damage;//總傷害

        //計算出防禦%
        defRate = 0.75f * monsterBehaviour.MonsterValue.DEF / (PlayerData.Lv + 9);

        //判別技能模式 獲取傷害類別
        switch (skillData.EffectCategory)
        {
            case "MeleeATK":
                damage = PlayerData.MeleeATK;

                break;
            case "RemoteATK":
                damage = PlayerData.RemoteATK;

                break;
            case "MageATK":
                damage = PlayerData.MageATK;

                break;

            default:
                damage = 0;
                break;
        }
        print("技能倍率:" + skillData.EffectValue[0]);

        //是否暴擊
        if (iscrt)
            damage = damage* skillData.EffectValue[0] * (damage * 1.5f + PlayerData.CrtDamage);
        else
            damage = damage* skillData.EffectValue[0];
        //目標扣除生命
        monsterBehaviour.CurrentHp -= (int)Mathf.Round((1 - defRate) * damage);
        //生成傷害數字
        InstanceDmgGUI(Mathf.Round((1 - defRate) * damage).ToString(), iscrt, monsterBehaviour.gameObject);
    }
    /// <summary>
    /// 生成傷害數字
    /// </summary>
    /// <param name="dmgVaule">傷害值</param>
    /// <param name="iscrt">是否暴擊</param>
    /// <param name="target">生成數字參考對象</param>
    public void InstanceDmgGUI(string dmgVaule, bool iscrt,GameObject target)
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
        //還原自動導航紀錄
        SkillDisplayAction.Instance.AutoNavToTarget = false;
        //怪物動畫(受傷)
        target.GetComponent<Animator>()?.SetTrigger("Injuried");
    }
}
