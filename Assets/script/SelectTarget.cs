using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//==========================================
//  創建者:    家豪
//  創建日期:  2023/08/05
//  創建用途:  選取目標功能UI
//==========================================
public class SelectTarget : MonoBehaviour
{
    //玩家攝影機
    public GameObject CharacterCamera;
    //是否抓取到目標
    public bool CatchTarget;

    public GameObject TargetInformation;
    //
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
    public Selectable Targetgameobject;
    //怪物行為腳本
    public MonsterBehaviour Monsterbehavior;
    //快捷鍵管理
    public HotKeyManager HotKeyManager;
    void Update()
    {
        //檢查是否有視窗開啟
        if (!HotKeyManager.BagWindow.activeSelf && !HotKeyManager.SkillsWindow.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CharacterCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    //獲取是否可選擇腳本
                    Selectable selectobj = hit.collider.GetComponent<Selectable>();
                    if (selectobj != null)
                    {
                        CatchTarget = true;
                        Targetgameobject = selectobj;
                    }
                    else
                    {
                        CatchTarget = false;
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CatchTarget = false;
        }
        //若有選取到目標 每真刷新(之後可能寫在有戰鬥狀態時更新 不要寫在Update)
        if (CatchTarget)
        {
            TargetInformation.SetActive(true);
            if (hit.collider.gameObject.tag == "Monster")
            {
                Monsterbehavior = Targetgameobject.GetComponent<MonsterBehaviour>();
            }
            if (Monsterbehavior != null)
            {
                SetTargetInformation();
            }
        }
        else
        {
            TargetInformation.SetActive(false);
            Targetgameobject = null;
        }
    }

    /// <summary>
    /// 設定目標信息(血量 等級..)
    /// </summary>
    private void SetTargetInformation()
    {
        TargetHP.value = Monsterbehavior.CurrentHp;
        TargetName.text = Monsterbehavior.MonsterValue.Name;
        TargetHP.maxValue = Monsterbehavior.MonsterValue.HP;
        TargetLV.text = Monsterbehavior.MonsterValue.Lv.ToString();
        TargetClass.text = Monsterbehavior.MonsterValue.Class;
        TargetHP_MinAndMax.text = TargetHP.value.ToString() + "/" + TargetHP.maxValue.ToString();
    }
}
