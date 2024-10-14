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
        TutorialObjectController tempObj = Instantiate(tutorialObj);
        tempObj.InitTutorialCheck(tutorialID);
    }
}
