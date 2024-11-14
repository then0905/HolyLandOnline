using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/20
//  創建用途:攻擊型技能基底(分類)
//==========================================
public abstract class Skill_Base_Attack : Skill_Base
{
    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defender = null)
    {
        SkillController.Instance.UsingSkill = true;
        SkillController.Instance.UsingSkillObj = this;
        base.SkillEffectStart(attacker, defender);
    }

    protected override void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {
        Debug.Log($"技能: {SkillName} 施放完畢，防呆已消除");
        SkillController.Instance.UsingSkillObj = null;
        //if (gameObject)
        //    Destroy(gameObject);
    }
}
