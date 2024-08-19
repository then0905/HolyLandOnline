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
public class HotKeyData : MonoBehaviour
{
    //鍵位
    public int Keyindex;
    //鍵位背景圖
    public Image Background;
    //放入的資料ID 供外部呼叫參考
    public string HotKeyDataID;
    //放入升級後的技能ID 供外部呼叫
    public string UpgradeSkillID;
    /// <summary>
    /// 設定快捷鍵
    /// </summary>
    /// <param name="skillIcon">技能圖示</param>
    /// <param name="skillID">技能名稱</param>
    public void SetSkill(Sprite skillIcon, string skillID, string upgradeSkillID = "")
    {
        //先檢查快捷鍵上是否已有此技能資料 有的話清除
        bool queryResult = SkillController.Instance.SkillHotKey.Any(x => x.HotKeyDataID.Contains(skillID));

        if (queryResult)
        {
            var targetQueryResult = SkillController.Instance.SkillHotKey.Where(x => x.HotKeyDataID.Contains(skillID)).ToList();
            targetQueryResult.ForEach(x => x.ClearHotKeyData());
        }


        //設定 技能圖片 與 技能名稱 與升級後的技能ID
        Background.sprite = skillIcon;
        HotKeyDataID = skillID;
        UpgradeSkillID = upgradeSkillID;
        //從Game取得該技能資料
        var getSkillData = GameData.SkillsUIDic[HotKeyDataID];
        //判斷空值
        if (getSkillData != null)
        {
            //設定清單索引目標 若輸入不是0鍵則-1 若是0則設定為9(最大索引值)
            int getIndexKey = Keyindex.Equals(0) ? (Keyindex + 9) : (Keyindex - 1);
            //寫入CD紀錄
            SkillController.Instance.Skillinformation.CDsec[getIndexKey] = getSkillData.CD;
            SkillController.Instance.Skillinformation.CDR[getIndexKey] = getSkillData.CD;
            SkillController.Instance.CoolDownRecord(getIndexKey);
        }
    }

    /// <summary>
    /// 清除此快捷鍵儲存的資料
    /// </summary>
    public void ClearHotKeyData()
    {
        //鍵位背景圖
        Background.sprite = HotKeyManager.Instance.HotKeyBackgroundSprite;
        //放入的技能資料
        HotKeyDataID = string.Empty;

        SkillController.Instance.Skillinformation.CDsec[Keyindex] = 0;
        SkillController.Instance.Skillinformation.CDR[Keyindex] = 0;
        SkillController.Instance.CoolDownRecord(Keyindex);
    }
}

