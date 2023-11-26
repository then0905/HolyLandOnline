using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/20
//  創建用途: 近距離攻擊型技能基底
//==========================================
public class Skill_Base_Attack_Melee : Skill_Base_Attack
{
    /// <summary>
    /// 攻擊標籤
    /// </summary>
    public readonly AttackCategory AttackType = AttackCategory.MeleeATK;

    protected override void SkillEffectStart()
    {
        CheckGetAnyTarget(AttackType);
    }
    protected override void SkillEffectEnd(string statusType = "", bool Rate = false, float value = 0)
    {
        Destroy(gameObject);
    }

}
