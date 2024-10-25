
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:被動型buff技能(無須額外效果)
//==========================================
public class Skill_Base_Buff_Passive : Skill_Base_Buff
{
    /*
     被動型技能幾乎為buff屬性(加強玩家)
     */

    //角色狀態訊息類別標籤
    protected override CharacterStatusType characterStatusType => CharacterStatusType.Passive;

    private ICombatant caster;
    private ICombatant target;

    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        caster = attacker;
        target = defenfer;
        SkillBuffEffectStart(attacker, defenfer);
        if (gameObject != null)
            gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }

    private void OnDestroy()
    {
        //if (gameObject != null)
        //    SkillEffectEnd(caster, target);
    }

    /// <summary>
    /// 重新啟動技能效果 用來穿脫裝時 重新計算數值
    /// </summary>
    public virtual void RestartSkillEffect()
    {
        SkillEffectEnd(caster, target);

        SkillBuffEffectStart(caster, target);
    }
}
