using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能視窗生成技能
//==========================================

public class ClassAndSkill : MonoBehaviour
{

    #region 全域靜態變數

    public static ClassAndSkill Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ClassAndSkill>();
            return instance;
        }
    }

    private static ClassAndSkill instance;

    #endregion

    //玩家職業
    [HideInInspector] public string Job;
    //玩家等級
    [HideInInspector] public int LV;
    //需生成物件
    public GameObject SkillChild;
    //生成技能物件的父級
    public Transform InsSkillChildTrans;
    //儲存生成的Skill該有的資料
    private List<SkillUIModel> skillDataModelList = new List<SkillUIModel>();
    //儲存生成的SkillUI
    private List<SkillUI> skillUIList = new List<SkillUI>();

    [Header("設定UI物件裡的SkillHotKey"), SerializeField] private Transform skillHotKeyTrans;
    [Header("設定UI物件裡的TopOfUnit"), SerializeField] private Transform topOfUnit;
    //public void Start()
    //{
    //    Init();
    //}

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        skillDataModelList = new List<SkillUIModel>();
        skillUIList.ForEach(x => Destroy(x.gameObject));
        skillUIList.Clear();
        Job = PlayerDataOverView.Instance.PlayerData_.Job;
        LV = PlayerDataOverView.Instance.PlayerData_.Lv;

        //玩家職業可以取得的技能
        var jobSkillList = GameData.SkillsUIDic.Values.Where(x => x.Job.Contains(Job) && x.NeedLv <= LV).OrderBy(y => y.NeedLv).ToList();

        //儲存技能資料
        foreach (var data in jobSkillList)
        {
            skillDataModelList.Add(data);
        }
        InsSkillUI();
    }

    /// <summary>
    /// 滿足條件生成SkillUI物件
    /// </summary>
    private void InsSkillUI()
    {
        foreach (var item in skillDataModelList)
        {
            //生成SkillUI物件
            SkillUI skillUIobj = Instantiate(SkillChild, InsSkillChildTrans).GetComponent<SkillUI>();

            //設定此物件的資料
            skillUIobj.SkillName.text = item.Name;
            skillUIobj.SkillCostMage.text = item.CastMage.ToString();
            skillUIobj.SkillCD.text = item.CD.ToString();
            skillUIobj.SkillIntro.text = string.IsNullOrEmpty(skillUIobj.SkillUpgradeID) ? item.Intro : item.Intro + "\n" + skillUIobj.SkillUpgradeID;
            skillUIobj.Characteristic = item.Characteristic;
            skillUIobj.SkillIcon.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.SkillIcon + "/" + PlayerDataOverView.Instance.PlayerData_.Job, item.SkillID);

            skillUIobj.GetComponent<DragSkill>().TopOfUnit = topOfUnit;
            skillUIobj.GetComponent<DragSkill>().SkillHotKey = skillHotKeyTrans;

            //將資料添加於List
            skillUIList.Add(skillUIobj);
        }
        PassiveSkillManager.Instance.InitAllPassiveSkill(skillUIList);     //初始化被動技能
    }
}
