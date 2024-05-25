using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
//==========================================
//  創建者:    家豪
//  創建日期:  2023/08/05
//  創建用途:  選取目標功能UI
//==========================================
public class SelectTarget : MonoBehaviour
{
    #region 全域靜態變數

    public static SelectTarget Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SelectTarget>();
            return instance;
        }
    }

    private static SelectTarget instance;

    #endregion

    //玩家攝影機
    private Camera characterCamera;
    // 是否抓取到目標
    public bool CatchTarget
    {
        get { return _catchTarget; }

        set
        {
            _catchTarget = value;
            if (!value)
            {
                TargetInformation.SetActive(false);
                Targetgameobject = null;
            }
            Targetgameobject?.BeenSelected();
        }
    }
    private bool _catchTarget;

    //目標資訊物件
    public GameObject TargetInformation;
    //目標名稱
    public TMP_Text TargetName;
    //目標血條
    public Slider TargetHP;
    //目標血條文字 當前血量/最大血量
    public TMP_Text TargetHP_MinAndMax;
    //目標等級
    public TMP_Text TargetLV;
    //目標類型(怪物)
    public TMP_Text TargetClass;

    public RaycastHit hit;
    //根據此腳本來記錄選擇目標
    public ActivityCharacterBase Targetgameobject;

    private void Start()
    {
        characterCamera = PlayerDataOverView.Instance.CharacterMove.CharacterCamera.GetComponent<Camera>();
    }
    void Update()
    {
        // 檢查是否有視窗開啟
        if (PanelManager.Instance.CheckPanelStatus())
        {
            HandleMouseClick();
        }

        // 檢查是否按下Escape鍵來取消選取
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CatchTarget = false;
            Targetgameobject = null;
        }
    }
    /// <summary>
    /// 點擊處理
    /// </summary>
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ActivityCharacterBase selectedObject = hit.collider.GetComponent<ActivityCharacterBase>();

                if (selectedObject != null)
                {
                    // 確保選取到的新目標與當前目標不同
                    if (selectedObject != Targetgameobject)
                    {
                        Targetgameobject = selectedObject;
                        CatchTarget = true;
                    }
                }
                else
                {
                    // 沒有選取到有效目標
                    CatchTarget = false;
                }
            }
            else
            {
                // Raycast沒有擊中任何物體
                CatchTarget = false;
            }
        }
    }
}
