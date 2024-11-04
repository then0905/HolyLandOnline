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
    public override void EffectStart(ICombatant caster, ICombatant target, BuffComponent skillTarget)
    {
        //暫存輸入資料
        this.caster = caster;
        this.target = target;
        this.skilldata = skillTarget;
        //被施放者獲取效果
        target.GetBuffEffect(target, skillTarget.SkillOperationData);
        //生成玩家狀態效果物件
        CharacterStatusManager.Instance.InitCharacterStatusHintCheck(this);
        //禁止移動 禁止施放技能 禁止攻擊
        target.MoveEnable(false);
        target.SkillEnable(false);
        target.AttackEnable(false);
    }

    public override void EffectEnd()
    {
        //移除效果狀態
        this.skilldata.ReverseExecute(null);
        //恢復移動 恢復施放技能 恢復攻擊
        target.MoveEnable(true);
        target.SkillEnable(true);
        target.AttackEnable(true);
        if (gameObject)
            Destroy(gameObject);
    }
}
