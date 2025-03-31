using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//==========================================
//  創建者:家豪
//  創建日期:2024/07/22
//  創建用途: 地圖管理器
//==========================================
public class MapManager : MonoBehaviour
{
    #region 靜態變數

    private static MapManager instance;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MapManager>(true);
            return instance;
        }
    }

    /// <summary> 記錄地圖名稱</summary>
    public static string MapName => mapName;
    /// <summary> 記錄地圖名稱(當前地圖) </summary>
    private static string mapName;
    #endregion

    [Header("Loading")]
    [SerializeField] private Loading loading;

    [Header("轉移出現在畫面上文字的畫布"), SerializeField] private Canvas canvasMapText;
    public Canvas CanvasMapText => canvasMapText;

    [Header("進入遊戲場景須執行的事件")]
    [SerializeField] private UnityEvent onGameSceneLoad;

    private AreaData areaData;

    /// <summary>
    /// 登入時切換地圖、設定地圖的初始化
    /// </summary>
    public void Init(string mapName_)
    {
        //取得當前場景名稱
        //mapName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadMapAsync(mapName_));
    }


    /// <summary>
    /// 切換下個地圖
    /// </summary>
    /// <param name="mapName">場景名稱</param>
    public void NextMap(string mapName_)
    {
        StartCoroutine(LoadMapAsync(mapName_, false));
        //切換場景完成後 本地紀錄資料刷新點
        LoadPlayerData.SaveUserData();
    }

    /// <summary>
    /// 非同步加載場景處理
    /// </summary>
    /// <param name="mapName_">地圖名稱</param>
    /// <param name="isInit">是否為第一次進入遊戲的初始化</param>
    /// <returns></returns>
    private IEnumerator LoadMapAsync(string mapName_, bool isInit = true)
    {
        //檢查是否相同場景
        if (mapName_ != mapName)
        {
            //更新場景名稱
            mapName = mapName_;
            //儲存地圖資料
            areaData = GameData.AreaDataDic[mapName];
            //Loading遮罩開啟
            loading.Show();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapName_, LoadSceneMode.Single);
            // 允許場景立即激活
            asyncLoad.allowSceneActivation = true;

            //非同步加載場景處理
            while (!asyncLoad.isDone)
            {
                float progress = asyncLoad.progress;
                loading.UpdateProgress(progress);
                yield return null;
            }

            yield return StartCoroutine(PerformAdditionalTasks(isInit));

        }
        else
        {
            yield return StartCoroutine(PerformAdditionalTasks(isInit));
            //loading.Hide();
        }
    }

    /// <summary>
    /// 在這裡執行額外的加載任務，如預加載資源、初始化系統等
    /// </summary>
    /// <param name="isInit">是否為第一次進入遊戲的初始化</param>
    /// <returns></returns>
    private IEnumerator PerformAdditionalTasks(bool isInit)
    {
        PlayerDataOverView playerDataOverView = null;
        //生成玩家
        if (areaData != null)
        {
            playerDataOverView = Instantiate(CommonFunction.LoadObject<GameObject>(GameConfig.PlayerPrefab, "Player"),
                new Vector3(areaData.RecordPosX, areaData.RecordPosY, areaData.RecordPosZ),
                Quaternion.identity).GetComponent<PlayerDataOverView>();
        }
        else
        {
            playerDataOverView = Instantiate(CommonFunction.LoadObject<GameObject>(GameConfig.PlayerPrefab, "Player")).GetComponent<PlayerDataOverView>();
        }
        // 地圖需生成的NPC物件
        NpcManager.Instance.InitNpcManger(mapName);
        // 地圖需生成的怪物物件(單機 未進行伺服器同步地圖資料)
        MonsterManager.Instance.InitMonsterManger(mapName);

        if (!isInit)
            InitSequenceManager.Instance.SceneSwitchInit();

        //為場景上的 Canvas相機控制腳本設定相機
        List<CanvasCameraControll> cameraList = FindObjectsOfType<CanvasCameraControll>().ToList();
        if (cameraList.CheckAnyData())
            cameraList.ForEach(x => x.SetCamera());

        //一秒延遲
        yield return new WaitForSeconds(1f);

        //關閉Loading遮罩
        loading.Hide();

        //載入遊戲場景讀取完 執行事件
        onGameSceneLoad?.Invoke();
    }

    /// <summary>
    /// 回城處理
    /// </summary>
    public void RecordProcessor(ICombatant target)
    {
        //尋找最近的休息區
        string recordTarget = GameData.AreaDataDic[mapName].RecordMapTarget;
        //進入Loading
        if (mapName == recordTarget)
        {
            loading.Show();
        }
        else
        {
            NextMap(recordTarget);
        }
        //短暫無敵?不確定是否需要

        //Debug.Log("呼叫回城，回城系統暫未實裝");
    }
}
