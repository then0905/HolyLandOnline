using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/12/16
//  創建用途: 近距離攻擊型群體技能基底(無額外效果)
//==========================================
public class Skill_Base_Attack_Melee_Mutiple : Skill_Base_Attack
{
    /// <summary>
    /// 攻擊標籤
    /// </summary>
    public readonly AttackCategory AttackType = AttackCategory.MeleeATK;

    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        //CheckGetAnyTarget(AttackType);
        SkillController.Instance.SkillArrowImage.GetComponent<ArrowHit>().SetSkillSize(this, skillID);
        PlayerDataOverView.Instance.CharacterMove.CharacterAnimator.SetTrigger(skillID);
    }
    protected override void SkillEffectEnd()
    {
        SkillController.Instance.SkillDistanceReverse();
        Destroy(gameObject);
    }
}
