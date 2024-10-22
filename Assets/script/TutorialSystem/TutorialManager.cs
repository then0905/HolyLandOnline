using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/9/9
//  創建用途:教學系統管理器
//==========================================
public class TutorialManager : MonoBehaviour
{
    #region 靜態變數

    private static TutorialManager instance;
    public static TutorialManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TutorialManager>(true);
            return instance;
        }
    }

    #endregion

    [Header("遊戲物件")]
    [SerializeField] private TutorialObjectController tutorialObj;        //教學物件

    [Header("教學ID帶入")]
    [SerializeField] private string tutorialID;

    /// <summary>
    /// 教學啟動
    /// </summary>
    public void RunTutorial()
    {
        //取得教學 ID判斷組合資料
        var tutorialSystemData = GameData.TutorialDataDic[tutorialID].TutorialIDList[0];

        //檢查是否符合判斷
        if (PlayerDataOverView.Instance.PlayerData_.CheckPlayerTutorialID(tutorialSystemData))
        {
            //生成教學物件
            TutorialObjectController tempObj = Instantiate(tutorialObj);
            //初始化教學
            tempObj.InitTutorial(tutorialID);
        }
    }
}
