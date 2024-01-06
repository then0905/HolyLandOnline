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

    //儲存所有被動技能
    public List<SkillUI> SkillBuffList = new List<SkillUI>();

    /// <summary>
    /// 初始化所有被動技能
    /// </summary>
    public void InitAllPassiveSkill(List<SkillUI> skillUIList)
    {
        //由於要初始化所有被動技能 之前生產過的需要清空
        if (SkillBuffList.Count > 1)
            SkillBuffList.ForEach(x => Destroy(x.gameObject));
        SkillBuffList.Clear();
        
        //找尋輸入的技能內 為被動技能的部分
        var getAllPassiveSkill = skillUIList.Where(x => x.Characteristic == false).ToList();
        SkillBuffList.AddRange(skillUIList);
       
        //執行被動技能效果
        getAllPassiveSkill.ForEach(passiveSkill =>
        {
            string queryResule =
                GameData.SkillsDataDic.Where(x => x.Value.Name.Contains(passiveSkill.SkillName.text)).Select(x => x.Value.SkillID).FirstOrDefault();
            GameObject effectObj = CommonFunction.LoadObject<GameObject>("SkillPrefab", "SkillEffect_" + queryResule);
            GameObject skillEffectObj = Instantiate(effectObj);
            skillEffectObj.GetComponent<Skill_Base>().InitSkillEffectData(0);
        });
    }
}
