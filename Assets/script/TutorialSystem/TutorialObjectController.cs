
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/9/9
//  創建用途:教學物件控制
//==========================================
public class TutorialObjectController : MonoBehaviour
{
    public static bool IsTutorial;

    private TutorialSystemData tutorialSystemData;

    private int tutorialIndex;      //紀錄當前是教學第幾段設定
    private int tutorialMaxIndex;      //紀錄當前教學最大段設定
    private List<GameObject> tempActorList = new List<GameObject>();        //暫存當前教學產生的演出物件

    [Header("畫布")]
    [SerializeField] private Canvas canvas;
    [Header("黑色背景遮罩 按鈕事件")]
    [SerializeField] private Button blackMaskBtn;
    [Header("光圈圖片")]
    [SerializeField] private RectTransform lightCricleTrans;
    [SerializeField] private Button lightCricleBtn; //光圈按鈕Component
    [Header("圓形光圈參考")]
    [SerializeField] private Sprite lightCricleSprite;
    [Header("光圈位置參考")]
    [SerializeField] private Transform maskImgTrans;
    public Transform MaskImgTrans { get { return maskImgTrans; } set { maskImgTrans = value; } }
    [Header("對話框")]
    [SerializeField] private GameObject dialog;
    [SerializeField] private TextMeshProUGUI dialogText;


    /// <summary>
    /// 初始化教學
    /// </summary>
    /// <param name="tutorialID">教學ID</param>
    public void InitTutorial(string tutorialID)
    {
        //畫布裝上相機
        canvas.worldCamera = Camera.main;
        //取得此教學資料
        tutorialSystemData = GameData.TutorialDataDic[tutorialID];

        //需要取得從0開始的索引值 所以-1
        tutorialMaxIndex = tutorialSystemData.TutorialBasalSettingList.Count - 1;
        tutorialIndex = 0;
        //正在教學中標記
        IsTutorial = true;
        
        //教學資料設定
        TutorialDataSetting();
    }

    /// <summary>
    /// 教學資料設定
    /// </summary>
    /// <param name="data"></param>
    public void TutorialDataSetting()
    {
        if (tutorialSystemData == null)
        {
            Debug.LogError("報錯通知 並無教學資料");
            return;
        }

        //取得此次教學內容
        TutorialBasalSettingData tempData = tutorialSystemData.TutorialBasalSettingList[tutorialIndex];

        //光圈 大小 形狀 設定
        lightCricleTrans.gameObject.SetActive(tempData.ClickLock);
        lightCricleTrans.sizeDelta = new Vector2(tempData.ClickRadio, tempData.ClickRadio);
        lightCricleTrans.GetComponent<ReverseMask>().sprite = tempData.ApertureIsSquare ? null : lightCricleSprite;
        //是否強制點擊光圈的位置
        lightCricleTrans.GetComponent<ReverseMask>().raycastTarget = tempData.ApertureEnable;
        //光圈與黑色背景遮罩 按鈕事件條整
        blackMaskBtn.enabled = !tempData.ApertureEnable;
        lightCricleBtn.enabled = tempData.ApertureEnable;
        //光圈位置參考
        if (maskImgTrans != null)
        {
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)lightCricleTrans.parent, // 父級 RectTransform
                RectTransformUtility.WorldToScreenPoint(Camera.main, maskImgTrans.position), // 將世界座標轉為螢幕座標
                Camera.main, // 用來參考 UI 的相機
                out uiPosition // 輸出 UI 當前的本地點
            );

            lightCricleTrans.anchoredPosition = uiPosition;
        }
        //lightCricleTrans.anchoredPosition = new Vector2(maskImgTrans.position.x, maskImgTrans.position.y);

        //對話框位置
        dialog.GetComponent<RectTransform>().anchoredPosition = new Vector2(tempData.DiaologPosX, tempData.DiaologPosY);
        //設定對話框文字內容
        dialogText.text = DialogTextFormatProcessor(tempData.TutorialDialog, tempData.TutorialDialogFormat != null ? tempData.TutorialDialogFormat.ToArray() : null);

        //設定是否需要暫停時間
        Time.timeScale = (tempData.TimeScaleStop ? 0 : 1);

        //獲取演出物件
        if (tutorialSystemData.TutorialActorList != null)
        {
            var tempActors = tutorialSystemData.TutorialActorList.ToArray();
            //設定演出物件        
            TutorialActorSetting(tempActors);
        }
    }

    /// <summary>
    /// 對話框文字合併處理
    /// </summary>
    /// <param name="dialogFormat">需要合併的字串資料</param>
    public string DialogTextFormatProcessor(string dialogText, params string[] dialogFormat)
    {
        //宣告 儲存需合併的字串資料
        List<string> tempFormat = new List<string>();

        //若沒有需要合併的字串 直接回傳對話文字
        if (dialogFormat == null)
            return dialogText.GetText();
        else
            //尋找需合併的字串資料
            foreach (var formatTarget in dialogFormat)
            {
                switch (formatTarget)
                {
                    //玩家ID
                    case "PlayerName":
                        tempFormat.Add(PlayerDataOverView.Instance.PlayerData_.PlayerName);
                        break;
                    default:
                        break;
                }
            }

        //回傳處理合併過後的資料
        return string.Format(dialogText.GetText(), tempFormat.ToArray());
    }

    /// <summary>
    /// 設定教學內容所需的演出物件
    /// </summary>
    /// <param name="actors"></param>
    public void TutorialActorSetting(params TutorialActorData[] actors)
    {
        //清空物件
        if (tempActorList.CheckAnyData())
        {
            tempActorList.ForEach(x => Destroy(x.gameObject));
            tempActorList.Clear();
        }

        if (actors.Length < 1 || actors == null) return;

        foreach (var actor in actors)
        {
            //生成此教學需要的演出物件
            GameObject tempActor = Instantiate(CommonFunction.LoadObject<GameObject>(actor.Path, actor.Name));
            //設定演出物件的圖層
            List<SpriteRenderer> actorRenderList = tempActor.transform.GetComponentsInChildren<SpriteRenderer>().ToList();
            if (actorRenderList.CheckAnyData())
            {
                actorRenderList.ForEach(x => x.sortingOrder = actor.LayoutOrder);
                actorRenderList.ForEach(x => x.sortingLayerName = actor.LayoutName);
                actorRenderList.ForEach(x => x.gameObject.layer = LayerMask.NameToLayer(actor.LayoutName));
            }
            //設定演出物件位置
            tempActor.GetComponent<RectTransform>().anchoredPosition = new Vector2(actor.PosX, actor.PosY);
            //加入暫存
            tempActorList.Add(tempActor);
        }
    }

    /// <summary>
    /// 呼叫同一個教學的下一個內容
    /// </summary>
    public void CallNextTutorialIndex()
    {
        //若已到教學索引最大值 => 教學將完成
        if (tutorialIndex.Equals(tutorialMaxIndex))
        {
            //紀錄ID
            PlayerDataOverView.Instance.PlayerData_.SaveTargetID(tutorialSystemData.SaveTutorialID);

            //清空物件
            if (tempActorList.CheckAnyData())
            {
                tempActorList.ForEach(x => Destroy(x.gameObject));
                tempActorList.Clear();
            }

            //教學結束標記
            IsTutorial = false;
            
            //清除物件
            Destroy(this.gameObject);
        }
        else
        {
            //教學索引+1
            tutorialIndex += 1;
            //設定新索引的教學資料
            TutorialDataSetting();
        }
    }
}
