
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
        if (!buffIsRun)
        {
            print("啟動的被動技能:" + SkillName);
            caster = attacker;
            target = defenfer;
            //SkillBuffEffectStart(attacker, defenfer);
            SkillComponentList[0].Execute(caster, target);
            buffIsRun = true;
        }
    }

    /// <summary>
    /// 技能效果結束
    /// </summary>
    protected override void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

        if (buffIsRun)
        {
            print("移除的被動技能:" + SkillName);
         
            //以相同的組件 與 持續時間分組
            var group = skillOperationList.GroupBy(x => new {
                x.SkillComponentID,
                ConditionKey = string.Join(",", x.ConditionOR.OrderBy(c => c))
            });
            foreach (var groupData in group)
            {
                CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(groupData.ToArray());
            }
            //SkillBuffEffectStart(attacker, defenfer);
            //CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(skillOperationList.ToArray());
            buffIsRun = false;
        }

        ////刪除自己
        //if (this.gameObject)
        //{
        //    if (this is Skill_Base_Buff_Passive)
        //        PassiveSkillManager.Instance.SkillPassiveBuffList.Remove(this);
        //    Destroy(this.gameObject);
        //}
    }

    /// <summary>
    /// 重新啟動技能效果 用來穿脫裝時 重新計算數值
    /// </summary>
    public virtual void RestartSkillEffect()
    {
        //SkillEffectEnd(caster, target);

        //SkillBuffEffectStart(caster, target);
    }

    public override void SkillBuffSub(string key)
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

        //OR條件任一達成 以及 AND條件全達成
        if (checkCondtionOR.CheckAnyData() && checkCondtionAND.CheckAnyData())
        {
            skillCondtionCheck = (checkCondtionOR.Any(x => x) && checkCondtionAND.All(x => x));
            if (skillCondtionCheck)
                SkillEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
            else
                SkillEffectEnd(null, null);
        }
        //OR條件任一達成 AND無條件資料
        else if (checkCondtionOR.CheckAnyData())
        {
            skillCondtionCheck = checkCondtionOR.Any(x => x);
            if (skillCondtionCheck)
                SkillEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
            else
                SkillEffectEnd(null, null);
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
