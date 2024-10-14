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
        if (PlayerTutorialList.Count < 1) return true;

        var haveList = PlayerTutorialList.Intersect(tutorialIDList.NotIncludedID).ToList();
        var dontHaveLsit = PlayerTutorialList.Intersect(tutorialIDList.IncludedID).ToList();

        return ((haveList != null && haveList.Count > 0) && (dontHaveLsit == null || dontHaveLsit.Count <= 0));
    }

    /// <summary>
    /// 紀錄完成的教學ID
    /// </summary>
    /// <param name="tutorialIDList">完成的教學ID清單</param>
    public void SaveTargetID(params string[] tutorialIDList)
    {
        foreach (var tutorial in tutorialIDList)
        {
            if (!PlayerTutorialList.Any(x => x == tutorial))
            {
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
