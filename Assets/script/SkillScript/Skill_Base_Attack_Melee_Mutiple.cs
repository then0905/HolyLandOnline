using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/12/16
//  創建用途: 近距離攻擊型群體技能基底(無額外效果)
//==========================================
public class Skill_Base_Attack_Melee_Mutiple : Skill_Base_Attack
{
    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        List<ICombatant> getTargetList = GetAttackType(attacker);
        foreach (var target in getTargetList)
        {
            base.SkillEffectStart(attacker, target);
        }
        PlayerDataOverView.Instance.CharacterMove.CharacterAnimator.SetTrigger(skillID);
    }

    protected override void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {
        SkillController.Instance.SkillDistanceReverse();
        base.SkillEffectEnd();
    }

    /// <summary>
    /// 依照技能範圍類型 設定攻擊距離與偵測範圍可攻擊目標
    /// </summary>
    protected List<ICombatant> GetAttackType(ICombatant attacker)
    {
        switch (SkillData.Type)
        {
            //圓形範圍
            case "Circle":
                return null;
            //指定方向 方形範圍
            case "Arrow":
                var skillOperation = SkillData.SkillOperationDataList.Where(x => (x.EffectRecive.Equals(1) || x.EffectRecive.Equals(-1)) && (!x.TargetCount.Equals(0) || !x.TargetCount.Equals(-1))).FirstOrDefault();
                return SkillController.Instance.SkillArrowImage.GetComponent<ArrowHit>().SetSkillSize(this, skillOperation);
            //指定方向 扇形範圍
            case "Cone":
                return null;
            default:
                return null;
        }
    }
}
