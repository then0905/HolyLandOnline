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
public class SkillUI : MonoBehaviour, ISkillBase
{
    public string SkillID { get; set ;  }

    public float CooldownTime { get { return int.Parse(skillCD.text); } set { skillCD.text = value.ToString(); } }

    public int CastMage { get { return int.Parse(skillCostMage.text); } set { skillCostMage.text = value.ToString(); } }

    public string SkillName { get { return skillName.text; } set { skillName.text = value; skillIcon.sprite = CommonFunction.LoadSkillIcon(SkillID); } }

    public string SkillIntro { get { return skillIntro.text; } set { skillIntro.text = value; } }

    [Header("技能名稱文字"), SerializeField] private TMP_Text skillName;

    [Header("技能消耗魔力文字"), SerializeField] private TMP_Text skillCostMage;

    [Header("技能冷卻時間文字"), SerializeField] private TMP_Text skillCD;

    [Header("技能介紹文字"), SerializeField] private TMP_Text skillIntro;

    [Header("技能圖示"), SerializeField] private Image skillIcon;

    //數字
    public int Number;
    //是否主動技
    public bool Characteristic;

    /*技能升級內容*/

    //技能是否被升級
    public bool SkillBeUpgrade = false;
    //技能升級提示(增加在技能介紹) EX:已升級為...
    public string SkillUpgradeID = "";
    //技能升級後的Icon更新
    public Sprite SkillUpgradeIcon;
}
