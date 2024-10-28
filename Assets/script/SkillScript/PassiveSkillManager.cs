using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/10/17
//  創建用途:管理玩家擁有的被動技能
//==========================================
public class PassiveSkillManager : MonoBehaviour
{
    #region 全域靜態變數

    private static PassiveSkillManager instance;

    public static PassiveSkillManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PassiveSkillManager>();
            return instance;
        }
    }

    #endregion

    //儲存所有技能UI版
    public List<SkillUI> SkillUIList = new List<SkillUI>();

    //儲存生成的被動技能
    public List<Skill_Base_Buff_Passive> SkillPassiveBuffList = new List<Skill_Base_Buff_Passive>();

    /// <summary>
    /// 初始化所有被動技能
    /// </summary>
    public void InitAllPassiveSkill(List<SkillUI> skillUIList)
    {
        //清空已生成的被動技能
        if (SkillPassiveBuffList.Count > 1)
            SkillPassiveBuffList.ForEach(x => Destroy(x.gameObject));
        SkillPassiveBuffList.Clear();

        //由於要初始化所有被動技能 之前生產過的需要清空
        if (SkillUIList.Count > 1)
            SkillUIList.ForEach(x => Destroy(x.gameObject));
        SkillUIList.Clear();

        //找尋輸入的技能內 為被動技能的部分
        var getAllPassiveSkill = skillUIList.FindAll(x => !x.Characteristic);
        SkillUIList.AddRange(skillUIList);

        if (SkillPassiveBuffList.Count == 0)
            //執行被動技能效果
            getAllPassiveSkill.ForEach(passiveSkill =>
            {
                Skill_Base_Buff_Passive skillEffectObj = Instantiate(CommonFunction.LoadSkillPrefab(passiveSkill.SkillID)).GetComponent<Skill_Base_Buff_Passive>();

                //儲存生成的被動技能
                SkillPassiveBuffList.Add(skillEffectObj);

                //被動技能初始化
                skillEffectObj.GetComponent<Skill_Base>().InitSkillEffectData();
                //被動技能效果執行
                skillEffectObj.GetComponent<Skill_Base>().SkillEffect(PlayerDataOverView.Instance,PlayerDataOverView.Instance) ;
            });
    }

    /// <summary>
    /// 重新啟動被動技能 (目前在穿裝脫裝時使用)
    /// </summary>
    public void RestartPassiveSkill()
    {
        if (SkillPassiveBuffList.Count < 1) return;
        //SkillPassiveBuffList.ForEach(x => x.RestartSkillEffect());
        for (int i = 0; i < SkillPassiveBuffList.Count; i++)
        {
            if (SkillPassiveBuffList[i].gameObject != null)
                SkillPassiveBuffList[i].RestartSkillEffect();
        }
    }
}
