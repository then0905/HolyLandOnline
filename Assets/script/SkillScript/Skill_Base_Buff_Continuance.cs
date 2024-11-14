using System.Linq;
using UnityEngine;
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:主動型buff技能(無須額外效果)
//==========================================
public class Skill_Base_Buff_Continuance : Skill_Base_Buff
{
    public override bool SkillCanUse(float tempMana)
    {
        bool basalBool = base.SkillCanUse(tempMana);
        //多判斷 當前是否有存在的Buff資料
        bool buffCheckBool = CharacterStatusManager.Instance.CharacterStatusHintDic.Any(x => x.CharacterStatusID == skillID);
        if (buffCheckBool) CommonFunction.MessageHint(string.Format("TM_SkillBuffError".GetText(), SkillName), HintType.Warning);
        return basalBool && !buffCheckBool;
    }

    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        SkillController.Instance.UsingSkill = true;
        PlayerDataOverView.Instance.CharacterMove.CharacterAnimator.SetTrigger("Skill");
        PlayerDataOverView.Instance.CharacterMove.CharacterAnimator.SetFloat("SkillID", SkillData.AnimaTrigger);
        SkillComponentList[0].Execute(attacker, defenfer);
        buffIsRun = true;
    }

    protected override void SkillEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {
        Debug.Log($"技能: {SkillName} 施放完畢，防呆已消除");
        SkillController.Instance.UsingSkillObj = null;
    }
}
