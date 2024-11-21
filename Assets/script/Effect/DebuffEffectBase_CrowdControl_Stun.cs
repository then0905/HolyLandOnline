using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/04
//  創建用途: 負面效果基底_控制狀態_暈眩
//==========================================
public class DebuffEffectBase_CrowdControl_Stun : DebuffEffectBase_CrowdControl
{
    public override void EffectStart(ICombatant caster, ICombatant target, SkillComponent skillTarget)
    {
        //暫存輸入資料
        this.caster = caster;
        this.target = target;
        this.skillComponent = skillTarget;
        //被施放者獲取效果
        target.GetDebuff(this);
    }

    public override void EffectEnd()
    {
        //移除效果狀態
        (this.skillComponent as CrowdControlSkillComponent).ReverseExecute(this);
        //恢復移動 恢復施放技能 恢復攻擊
        //target.MoveEnable(true);
        //target.SkillEnable(true);
        //target.AttackEnable(true);
        //if (gameObject)
        //    Destroy(gameObject);
    }
}
