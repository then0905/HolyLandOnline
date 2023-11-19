using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能UI架構(技能視窗的技能)
//==========================================
public class SkillUI : MonoBehaviour
{
    //技能名稱
    public TMP_Text SkillName;
    //技能消耗魔力
    public TMP_Text SkillCostMage;
    //技能冷卻時間
    public TMP_Text SkillCD;
    //技能介紹
    public TMP_Text SkillIntro;
    //技能圖示
    public Image SkillIcon;
    //數字
    public int Number;
    //是否主動技
    public bool Characteristic;
    
    void Start()
    {        
        //SkillName.text = DataBase.Instance.SkillDB.Skill[Number].SkillName;
        //SkillCD.text = DataBase.Instance.SkillDB.Skill[Number].SkillCoolDown.ToString();
        //SkillCostMage.text = DataBase.Instance.SkillDB.Skill[Number].CostMana.ToString();
        //SkillIntro.text = DataBase.Instance.SkillDB.Skill[Number].SkillIntro;
        //SkillIcon.sprite = DataBase.Instance.SkillDB.SkillIcon[Number];
        //Passive = DataBase.Instance.SkillDB.Skill[Number ].SkillIsPassive;
    }
}
