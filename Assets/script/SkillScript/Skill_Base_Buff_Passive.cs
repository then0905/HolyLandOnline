
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:被動型buff技能(無須額外效果)
//==========================================
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Skill_Base_Buff_Passive : Skill_Base_Buff
{
    /*
     被動型技能幾乎為buff屬性(加強玩家)
     */

    private ICombatant caster;
    private ICombatant target;

    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        caster = attacker;
        target = defenfer;
        //SkillBuffEffectStart(attacker, defenfer);
        SkillComponentList[0].Execute(caster, target);
        buffIsRun = true;
    }

    /// <summary>
    /// 技能效果結束
    /// </summary>
    protected override void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {
        print("移除的被動技能:" + SkillName);

        //刪除自己
        if (this.gameObject)
        {
            if (this is Skill_Base_Buff_Passive)
                PassiveSkillManager.Instance.SkillPassiveBuffList.Remove(this);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 重新啟動技能效果 用來穿脫裝時 重新計算數值
    /// </summary>
    public virtual void RestartSkillEffect()
    {
        //SkillEffectEnd(caster, target);

        //SkillBuffEffectStart(caster, target);
    }

    public override void SkillBuffSub(string key, object value)
    {
        List<bool> checkCondtionOR = new List<bool>();
        List<bool> checkCondtionAND = new List<bool>();

        //檢查 技能運算資料清單
        foreach (var operationData in skillOperationList)
        {
            //檢查條件清單
            if (operationData.ConditionOR.CheckAnyData())
                checkCondtionOR.Add(CheckConditionOR(operationData.ConditionOR.Where(x => x.Contains(key)).ToList()));
            else if (operationData.ConditionAND.CheckAnyData())
                checkCondtionAND.Add(CheckConditionOR(operationData.ConditionAND.Where(x => x.Contains(key)).ToList()));
            else
            {
                checkCondtionOR.Add(true);
                continue;
            }
        }

        //OR條件任一達成 以及 AND條件全達成
        if (checkCondtionOR.CheckAnyData() && checkCondtionAND.CheckAnyData())
        {
            skillCondtionCheck = (checkCondtionOR.Any(x => x)&& checkCondtionAND.All(x => x));
            if(skillCondtionCheck)
                SkillEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
        }
        //OR條件任一達成 AND無條件資料
        else if(checkCondtionOR.CheckAnyData())
        {
            skillCondtionCheck = checkCondtionOR.Any(x => x);
            if (skillCondtionCheck)
                SkillEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
        }
        else
        {
            Debug.Log(string.Format("技能 : {0}  條件未達成", SkillName));
        }

        //if (checkCondtionOR.Any(x => x))
        //{
        //    skillCondtionCheck = true;
        //    SkillEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
        //}
        //else
        //{
        //    Debug.Log(string.Format("技能 : {0}  條件未達成", SkillName));
        //}
    }
}
