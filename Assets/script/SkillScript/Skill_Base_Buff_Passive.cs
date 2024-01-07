
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:被動型buff技能(無須額外效果)
//==========================================
public  class Skill_Base_Buff_Passive : Skill_Base_Buff
{
    /*
     被動型技能幾乎為buff屬性(加強玩家)
     */
    protected override void SkillEffectStart()
    {
        SkillBuffEffectStart();
        print("施放的被動技能效果:"+skillName);
        if (gameObject != null)
            gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }
}
