
using System.Linq;
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:主動型buff技能(無須額外效果)
//==========================================
public class Skill_Base_Buff_Continuance : Skill_Base_Buff
{
    protected override CharacterStatusType characterStatusType => CharacterStatusType.Buff;

    public override bool SkillCanUse(float tempMana)
    {
        bool basalBool = base.SkillCanUse(tempMana);
        bool buffCheckBool = CharacterStatusManager.Instance.CharacterStatusHintDic.Any(x => x.CharacterStatusID == skillID);
        if (buffCheckBool) CommonFunction.MessageHint(string.Format("TM_SkillBuffError".GetText(), SkillName), HintType.Warning);
        return basalBool && !buffCheckBool;
    }

    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        SkillBuffEffectStart(attacker, defenfer);
        //if (gameObject != null)
        //    gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }

}
