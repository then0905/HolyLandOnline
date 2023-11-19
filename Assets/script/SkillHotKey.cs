using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能快捷鍵紀錄
//==========================================
public class SkillHotKey : MonoBehaviour
{
    //鍵位
    public int Keyindex;
    //鍵位背景圖
    public Image Background;
    //放入的技能資料
    public string SkillName;
    //public SkillRange Skillrange;

    /// <summary>
    /// 設定快捷鍵
    /// </summary>
    /// <param name="skillIcon">技能圖示</param>
    /// <param name="name">技能名稱</param>
    public void SetSkill(Sprite skillIcon, string name)
    {
        //設定 技能圖片 與 技能名稱
        Background.sprite = skillIcon;
        SkillName = name;
        //從Game取得該技能資料
        var getSkillData = GameData.SkillsUIDic[SkillName];
        //判斷空值
        if (getSkillData != null)
        {
            //寫入CD紀錄
            SkillDisplayAction.Instance.Skillinformation.CDsec[Keyindex] = getSkillData.CD;
            SkillDisplayAction.Instance.Skillinformation.CDR[Keyindex] = getSkillData.CD;
            SkillDisplayAction.Instance.CoolDownRecord(Keyindex);
        }
    }

}
