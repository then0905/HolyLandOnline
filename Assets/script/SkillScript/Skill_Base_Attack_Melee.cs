using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/20
//  創建用途: 近距離攻擊型單體技能基底(無須額外效果)
//==========================================
public class Skill_Base_Attack_Melee : Skill_Base_Attack
{
    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defender = null)
    {
        base.SkillEffectStart(attacker, defender);
    }
}
