
//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:主動型buff技能
//==========================================
public class Skill_Base_Buff_Continuance : Skill_Base_Buff
{
    protected override void SkillEffectStart()
    {
        SkillBuffEffectStart();
        //if (gameObject != null)
        //    gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }

}
