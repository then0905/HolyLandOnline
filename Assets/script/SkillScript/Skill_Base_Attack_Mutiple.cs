using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//==========================================
//  創建者:家豪
//  創建日期:2023/12/16
//  創建用途: 攻擊型群體技能基底(無額外效果)
//==========================================
public class Skill_Base_Attack_Mutiple : Skill_Base_Attack
{
    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        List<ICombatant> getTargetList = GetAttackType(attacker, defenfer);
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
    protected List<ICombatant> GetAttackType(ICombatant attacker, ICombatant defenfer)
    {
        var skillOperation = SkillData.SkillOperationDataList.Where(x => (x.EffectRecive.Equals(1) || x.EffectRecive.Equals(-1)) && (!x.TargetCount.Equals(0) || !x.TargetCount.Equals(-1))).FirstOrDefault();
        switch (SkillData.Type)
        {
            //圓形範圍
            case "Circle":
                return SkillController.Instance.SkillPlayerCricle.GetComponent<CircleHit>().SetSkillSize(this, skillOperation, attacker, defenfer);
            //指定方向 方形範圍
            case "Arrow":
                return SkillController.Instance.SkillArrowImage.GetComponent<ArrowHit>().SetSkillSize(this, skillOperation, attacker, defenfer);
            //指定方向 扇形範圍
            case "Cone":
                return null;
            default:
                return null;
        }
    }
}
