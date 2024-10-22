using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/9/9
//  創建用途:角色基礎資訊(教學資料)
//==========================================
public partial class PlayerData /*: MonoBehaviour*/
{
    public List<string> PlayerTutorialList = new List<string>();

    /// <summary>
    /// 檢查玩家已完成的教學ID清單是否存在帶入的ID
    /// </summary>
    /// <param name="tutorialIDList">帶入ID</param>
    /// <returns></returns>
    public bool CheckPlayerTutorialID(TutorialIDData tutorialIDList)
    {
        //若沒有 已儲存的教學ID (新玩家，未進行任何教學) 直接回傳true
        if (PlayerTutorialList.Count < 1) return true;
        
        //宣告 已教學過的ID
        List<string> haveList = new List<string>();
        //宣告 未教學過的ID
        List<string> dontHaveLsit = new List<string>();

        //教學需要尋找玩家 未教學的ID (取得玩家是否有 此教學不可教學過的ID)
        if (tutorialIDList.NotIncludedID != null && tutorialIDList.NotIncludedID.Count > 0)
            haveList = PlayerTutorialList.Intersect(tutorialIDList.NotIncludedID).ToList();
        //教學需要尋找玩家 已教學的ID (取得玩家是否有 此教學需要教學過的ID)
        if (tutorialIDList.IncludedID != null && tutorialIDList.IncludedID.Count > 0)
            dontHaveLsit = PlayerTutorialList.Intersect(tutorialIDList.IncludedID).ToList();

        //回傳 此次教學 需要玩家並沒有教過的ID(null或是0) 以及 此次教學 需要玩家有教過的ID (不可為null且有資料)
        return ((haveList == null || haveList.Count <= 0) && (dontHaveLsit != null && dontHaveLsit.Count > 0));
    }

    /// <summary>
    /// 紀錄完成的教學ID
    /// </summary>
    /// <param name="tutorialIDList">完成的教學ID清單</param>
    public void SaveTargetID(params string[] tutorialIDList)
    {
        foreach (var tutorial in tutorialIDList)
        {
            //檢查是否有重複寫入
            if (!PlayerTutorialList.Any(x => x == tutorial))
            {
                //紀錄教學ID
                PlayerTutorialList.Add(tutorial);
                //教學完成寫入ID 本地紀錄資料刷新點
                LoadPlayerData.SaveUserData();
            }
            else
            {
                CommonFunction.MessageHint(string.Format("錯誤:已有存在的教學ID", tutorial), HintType.Warning);
            }
        }
    }
}
