using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能施放控制器
//==========================================
public class SkillController : MonoBehaviour
{
    #region 靜態變數
    private static SkillController instance;
    public static SkillController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkillController>();
            }
            return instance;
        }
    }
    #endregion

    [HideInInspector] public Skill_Base UsingSkillObj;  //儲存當前施放的技能

    //防止動畫未結束播放，因鍵入下一個技能施放指令而中斷動畫或是產生BUG的防呆
    private bool usingSkill = false;
    public bool UsingSkill //是否正在施放技能
    {
        get
        {
            return usingSkill;
        }
        set
        {
            usingSkill = value;
        }
    }

    [Header("冷卻時間紀錄")]
    private int keyIndex;                        //紀錄當前鍵位

    /// <summary>
    /// 外部取得當前鍵位索引值
    /// </summary>
    public int KeyIndex
    {
        get
        {
            return keyIndex;
        }
    }

    [Header("技能相關參考")]
    public HotKeyData[] SkillHotKey = new HotKeyData[10];

    [HideInInspector] public Image SkillArrowImage;               //指向技圖示
    [HideInInspector] public Image SkillTargetCircle;             //圓圈型範圍技圖示
    [HideInInspector] public Image SkillPlayerCricle;             //攻擊範圍技圖示
    [HideInInspector] public Image SkillConeImage;                //扇形範圍技圖示

    //記錄追擊協程
    public Coroutine SkillChasingCoroutine;

    public Action<string> SkillConditionCheckEvent;     //發生特定事情後 檢查技能條件是否達成

    void Update()
    {
        //按下快捷鍵
        if (Input.GetKeyDown("1"))
        {
            SkillUse(0);
        }
        if (Input.GetKeyDown("2"))
        {
            SkillUse(1);
        }
        if (Input.GetKeyDown("3"))
        {
            SkillUse(2);
        }
        if (Input.GetKeyDown("4"))
        {
            SkillUse(3);
        }
        if (Input.GetKeyDown("5"))
        {
            SkillUse(4);
        }
        if (Input.GetKeyDown("6"))
        {
            SkillUse(5);
        }
        if (Input.GetKeyDown("7"))
        {
            SkillUse(6);
        }
        if (Input.GetKeyDown("8"))
        {
            SkillUse(7);
        }
        if (Input.GetKeyDown("9"))
        {
            SkillUse(8);
        }
        if (Input.GetKeyDown("0"))
        {
            SkillUse(9);
        }
    }

    /// <summary>
    /// 鍵入技能
    /// </summary>
    /// <param name="inputNumber">技能快捷鍵</param>
    public void SkillUse(int inputNumber)
    {
        if (!PlayerDataOverView.Instance.SkillIsEnable.Equals(0))
        {
            CommonFunction.MessageHint("TM_SkillEnableError".GetText(), HintType.Warning);
            return;
        }
        //判斷 快捷鍵的資料是否為空值
        if (SkillHotKey[inputNumber].TempHotKeyData != null)
        {
            //紀錄輸入的鍵位
            keyIndex = inputNumber;
            //取得 該技能UI版資料
            Skill_Base skill_Base = (Skill_Base)SkillHotKey[inputNumber].TempHotKeyData;

            //判斷是否魔力足夠 以及 冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能)
            if (!skill_Base.SkillCanUse(PlayerDataOverView.Instance.PlayerData_.MP))
                return;
            Debug.Log("攻擊狀態:" + NormalAttackSystem.AttackAllow + "使用技能中:" + UsingSkill);
            if (UsingSkill || NormalAttackSystem.AttackAllow)
            {
                CommonFunction.MessageHint("TM_SkillIsUsing".GetText(), HintType.Warning);
                return;
            }

            if (skill_Base.SkillConditionCheck)
            {
                //Buff類型 直接施放
                if (skill_Base.SkillData.Type == "Buff")
                {
                    skill_Base.SkillEffect(PlayerDataOverView.Instance, PlayerDataOverView.Instance);
                }
                //剩餘類型 需要設定目標
                else
                {
                    //若已選取目標 接近目標到可施放範圍
                    if (SelectTarget.Instance.CatchTarget)
                    {
                        SkillChasingCoroutine = StartCoroutine(skill_Base.SkillDistanceCheck());
                    }
                    else
                    {
                        switch (skill_Base.SkillData.Type)
                        {
                            //指向技能類型
                            case "Arrow":
                                //若未選取 顯示該技能範圍
                                SkillArrow(skill_Base, SkillHotKey[inputNumber].UpgradeSkillID);
                                break;

                            //指定技能類型
                            case "Target":
                                SkillTarget(skill_Base, SkillHotKey[inputNumber].UpgradeSkillID);
                                break;

                            //圓圈型範圍技能類型
                            case "Circle":
                                SkillCircle(skill_Base, SkillHotKey[inputNumber].UpgradeSkillID);
                                break;

                            //扇型範圍技能類型
                            case "Cone":
                                SkillCone(skill_Base, SkillHotKey[inputNumber].UpgradeSkillID);
                                break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 顯示 指向技 相關設定
    /// </summary>
    /// <param name="skillData">技能資料</param>
    public void SkillArrow(Skill_Base skillUIData, string UpgradeSkillID = "")
    {
        //設定範圍圖示長寬
        SkillArrowImage.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.SkillData.Width, skillUIData.SkillData.Height);

        //顯示指向技畫布並隱藏其他型畫布
        SkillArrowImage.GetComponent<Image>().enabled = true;
        SkillTargetCircle.GetComponent<Image>().enabled = false;
        SkillPlayerCricle.GetComponent<Image>().enabled = false;
        SkillConeImage.GetComponent<Image>().enabled = false;

        //點擊左鍵對指定方向施放技能
        if (SkillArrowImage.GetComponent<Image>().enabled && Input.GetMouseButton(0))
        {
            UsingSkill = true;
            CallSkillEffect(skillUIData, UpgradeSkillID);
        }
    }

    /// <summary>
    /// 顯示 扇型技 相關設定
    /// </summary>
    /// <param name="skillData">技能資料</param>
    public void SkillCone(Skill_Base skillUIData, string UpgradeSkillID = "")
    {
        //SkillConeImage.GetComponent<RectTransform>().sizeDelta = new Vector2(DataBase.Instance.SkillDB.Skill[SkillDBindex].SkillAmbitWidth, DataBase.Instance.SkillDB.Skill[SkillDBindex].SkillAmbitHeight);

        //顯示扇型範圍畫布並隱藏其他型畫布
        SkillArrowImage.GetComponent<Image>().enabled = false;
        SkillTargetCircle.GetComponent<Image>().enabled = false;
        SkillPlayerCricle.GetComponent<Image>().enabled = false;
        SkillConeImage.GetComponent<Image>().enabled = true;

        //點擊左鍵對指定範圍施放技能
        if (SkillConeImage.GetComponent<Image>().enabled && Input.GetMouseButton(0))
        {
            UsingSkill = true;
            CallSkillEffect(skillUIData, UpgradeSkillID);
        }
    }

    /// <summary>
    /// 顯示 圓形技 相關設定
    /// </summary>
    /// <param name="skillData">技能資料</param>
    public void SkillCircle(Skill_Base skillUIData, string UpgradeSkillID = "")
    {
        //設定範圍圖示半徑
        SkillPlayerCricle.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.SkillData.Distance, skillUIData.SkillData.Distance);
        //設定圓圈型範圍技的圖示半徑
        SkillTargetCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.SkillData.CircleDistance, skillUIData.SkillData.CircleDistance);

        //顯示圓形範圍技與攻擊範圍圖示 隱藏其他圖示
        SkillArrowImage.GetComponent<Image>().enabled = false;
        SkillTargetCircle.GetComponent<Image>().enabled = true;
        SkillPlayerCricle.GetComponent<Image>().enabled = true;
        SkillConeImage.GetComponent<Image>().enabled = false;

        //點擊左鍵對指定範圍施放技能
        if (SkillTargetCircle.GetComponent<Image>().enabled && Input.GetMouseButton(0))
        {
            UsingSkill = true;
            CallSkillEffect(skillUIData, UpgradeSkillID);
        }
    }

    /// <summary>
    /// 顯示 指向技 相關設定
    /// </summary>
    /// <param name="skillData">技能資料</param>
    public void SkillTarget(Skill_Base skillUIData, string UpgradeSkillID = "")
    {
        //設定範圍圖示半徑
        SkillPlayerCricle.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.SkillData.Distance, skillUIData.SkillData.Distance);

        //顯示攻擊範圍圖示 隱藏其他圖示
        SkillArrowImage.GetComponent<Image>().enabled = false;
        SkillTargetCircle.GetComponent<Image>().enabled = false;
        SkillPlayerCricle.GetComponent<Image>().enabled = true;
        SkillConeImage.GetComponent<Image>().enabled = false;

        //點擊左鍵對指定範圍施放技能
        if (SkillTargetCircle.GetComponent<Image>().enabled && Input.GetMouseButton(0))
            //若選取到目標執行
            if (SelectTarget.Instance.Targetgameobject != null)
            {
                UsingSkill = true;
                CallSkillEffect(skillUIData, UpgradeSkillID);
            }
            else
                return;
    }

    /// <summary>
    /// 呼叫技能特效物件
    /// </summary>
    /// <param name="skillData">鍵入的技能資料</param>
    public void CallSkillEffect(Skill_Base skillData, string UpgradeSkillID = "")
    {
        //生成技能效果物件
        UsingSkillObj = skillData;

        //若技能效果為升級版 執行升級效果
        if (UpgradeSkillID != "")
            UsingSkillObj.GetSkillUpgradeEffect(UpgradeSkillID, PlayerDataOverView.Instance, SelectTarget.Instance.Targetgameobject.GetComponent<ICombatant>());
    }

    /// <summary>
    /// 還原技能範圍碰撞器與圖片
    /// </summary>
    public void SkillDistanceReverse()
    {
        SkillArrowImage.GetComponent<Image>().enabled = false;
        SkillTargetCircle.GetComponent<Image>().enabled = false;
        SkillPlayerCricle.GetComponent<Image>().enabled = false;
        SkillConeImage.GetComponent<Image>().enabled = false;
        SkillArrowImage.GetComponent<BoxCollider>().enabled = false;
        SkillTargetCircle.GetComponent<SphereCollider>().enabled = false;
        SkillPlayerCricle.GetComponent<SphereCollider>().enabled = false;
        SkillConeImage.GetComponent<SphereCollider>().enabled = false;
    }

    /// <summary>
    /// 停止技能攻擊追擊目標
    /// </summary>
    public void StopSkillChasingTarge()
    {
        if (SkillChasingCoroutine != null)
        {
            StopCoroutine(SkillChasingCoroutine);
            SkillChasingCoroutine = null;
            CommonFunction.MessageHint("TM_CancelAttackTarget".GetText(), HintType.Warning);
        }
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = false;
    }
}
