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
    /// 選取的怪物目標
    /// </summary>
    private GameObject monsterTarget
    {
        get { return monsterTarget; }

        set
        {
            if (value != null)
            {
                HitOrMiss();
            }
        }
    }

    /// <summary>
    /// 獲取戰鬥對象
    /// </summary>
    public void CatchBattleTarget(GameObject targetObj)
    {
        monsterTarget = targetObj;
    }

    /// <summary>
    /// 是否命中
    /// </summary>
    public void HitOrMiss()
    {
        //取得 怪物行為腳本
        MonsterBehaviour monsterBehaviour = monsterTarget.GetComponent<MonsterBehaviour>();
        //取得 技能資料
        SkillUIModel skillData = GameData.SkillsUIDic[SkillDisplayAction.Instance.SkillHotKey[SkillDisplayAction.Instance.KeyIndex].SkillName];

        //判別技能模式
        switch (skillData.Type)
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
            CrtOrNormal(monsterBehaviour, skillData);
        else
            InstanceDmgGUI("Miss", false);
    }

    /// <summary>
    /// 是否暴擊計算
    /// </summary>
    /// <param name="monsterBehaviour">怪物行為腳本</param>
    public void CrtOrNormal(MonsterBehaviour monsterBehaviour, SkillUIModel skillData)
    {
        float CrtRate;      //
        //
        CrtRate = PlayerData.Crt * 100 / (PlayerData.Crt + monsterBehaviour.MonsterValue.CrtResistance);
        //
        int isCrt = Random.Range(0, 101);
        //
        if (isCrt < CrtRate)
            DmgOperation(true, monsterBehaviour, skillData);
        else
            DmgOperation(false, monsterBehaviour, skillData);
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
    public void DmgOperation(bool iscrt, MonsterBehaviour monsterBehaviour, SkillUIModel skillData)
    {
        float defR;//防禦%
        float dmg;//總傷害

        //計算出防禦%
        defR = 0.75f * monsterBehaviour.MonsterValue.DEF / (PlayerData.Lv + 9);

        //判別技能模式
        switch (skillData.Type)
        {
            case "MeeleATK":
                dmg = PlayerData.MeleeATK;

                break;
            case "RemoteATK":
                dmg = PlayerData.RemoteATK;

                break;
            case "MageATK":
                dmg = PlayerData.MageATK;

                break;

            default:
                dmg = 0;
                break;
        }

        //是否暴擊
        if (iscrt)
            dmg = skillData.Damage * (dmg * 1.5f
                + PlayerData.CrtDamage);

        //目標扣除生命
        monsterBehaviour.CurrentHp -= (int)Mathf.Round((1 - defR) * dmg);
        //生成傷害數字
        InstanceDmgGUI(Mathf.Round((1 - defR) * dmg).ToString(), iscrt);
    }
    /// <summary>
    /// 生成傷害數字
    /// </summary>
    /// <param name="dmgVaule">傷害值</param>
    /// <param name="iscrt">是否暴擊</param>
    public void InstanceDmgGUI(string dmgVaule, bool iscrt)
    {
        //載入傷害數字prefab
        GameObject DamageGUI = Resources.Load("DMGtext") as GameObject;
        //取得傷害數字生成位置
        GameObject head = monsterTarget.transform.GetChild(0).gameObject;

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
        SkillDisplayAction.Instance.UsingSkill = false;
        //怪物動畫(受傷)
        monsterTarget.GetComponent<Animator>()?.SetTrigger("Injuried");
    }
}
