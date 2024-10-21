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

    float dis;

    //記錄追擊協程
    public Coroutine SkillChasingCoroutine;

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

    ///// <summary>
    ///// 鍵入技能
    ///// </summary>
    ///// <param name="inputNumber">技能快捷鍵</param>
    //public void SkillUse(int inputNumber)
    //{
    //    //判斷 技能是否有在使用中 或是 快捷鍵的資料是否為空值
    //    if (!string.IsNullOrEmpty(SkillHotKey[inputNumber].HotKeyDataID))
    //    {
    //        //紀錄輸入的鍵位
    //        keyIndex = inputNumber;
    //        //var skillData = GameData.SkillsDataDic.Where(x => x.Value.Name.Contains(SkillHotKey[inputNumber].SkillName)).Select(x => x.Value).FirstOrDefault();
    //        //取得 該技能UI版資料
    //        var skillUIData = GameData.SkillsUIDic.Where(x => x.Key.Contains(SkillHotKey[inputNumber].HotKeyDataID)).Select(x => x.Value).FirstOrDefault();

    //        //判斷是否魔力足夠 以及 冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能)

    //        if (PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage < 0)
    //        {
    //            CommonFunction.MessageHint("魔力不足...", HintType.Warning);
    //            return;
    //        }
    //        if (Skillinformation.CDR[keyIndex] < Skillinformation.CDsec[keyIndex])
    //        {
    //            CommonFunction.MessageHint("該技能冷卻時間未完成!", HintType.Warning);
    //            return;
    //        }
    //        if (UsingSkill)
    //        {
    //            CommonFunction.MessageHint("當前有技能正在使用中!", HintType.Warning);
    //            return;
    //        }

    //        switch (skillUIData.Type)
    //        {
    //            //指向技能類型
    //            case "Arrow":
    //                //若已選取目標 接近目標到可施放範圍
    //                if (SelectTarget.Instance.CatchTarget)
    //                    SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
    //                else
    //                    //若未選取 顯示該技能範圍
    //                    SkillArrow(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
    //                break;

    //            //指定技能類型
    //            case "Target":
    //                if (SelectTarget.Instance.CatchTarget)
    //                    SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
    //                else
    //                    SkillTarget(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
    //                break;

    //            //圓圈型範圍技能類型
    //            case "Circle":
    //                if (SelectTarget.Instance.CatchTarget)
    //                    SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
    //                else
    //                    SkillCircle(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
    //                break;

    //            //扇型範圍技能類型
    //            case "Cone":
    //                if (SelectTarget.Instance.CatchTarget)
    //                    SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
    //                else
    //                    SkillCone(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
    //                break;
    //        }
    //    }
    //}

    /// <summary>
    /// 鍵入技能
    /// </summary>
    /// <param name="inputNumber">技能快捷鍵</param>
    public void SkillUse(int inputNumber)
    {
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

            if (UsingSkill)
            {
                CommonFunction.MessageHint("當前有技能正在使用中!", HintType.Warning);
                return;
            }

            //若已選取目標 接近目標到可施放範圍
            if (SelectTarget.Instance.CatchTarget)
                SkillChasingCoroutine = StartCoroutine(skill_Base.SkillDistanceCheck());
            else
            {
                switch (skill_Base.Type)
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

    /// <summary>
    /// 取得玩家與目標間的距離
    /// </summary>
    /// <returns></returns>
    private float DistanceWithTarget()
    {
        if (SelectTarget.Instance.CatchTarget)
            dis = Vector3.Distance(SelectTarget.Instance.Targetgameobject.Povit.position, PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position);
        else
            return 0;
        return dis;
    }

    /// <summary>
    /// 顯示 指向技 相關設定
    /// </summary>
    /// <param name="skillData">技能資料</param>
    public void SkillArrow(Skill_Base skillUIData, string UpgradeSkillID = "")
    {
        //設定範圍圖示長寬
        SkillArrowImage.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.Width, skillUIData.Height);

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
        SkillPlayerCricle.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.Distance, skillUIData.Distance);
        //設定圓圈型範圍技的圖示半徑
        SkillTargetCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.CircleDistance, skillUIData.CircleDistance);

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
        SkillPlayerCricle.GetComponent<RectTransform>().sizeDelta = new Vector2(skillUIData.Distance, skillUIData.Distance);

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
    /// 設定快捷鍵上的技能冷卻與資料(裝上技能時呼叫)
    /// </summary>
    /// <param name="indexKey">快捷鍵鍵位</param>
    //public void CoolDownRecord(int indexKey)
    //{
    //    Skillinformation.SkillSlider[indexKey].maxValue = Skillinformation.CDsec[indexKey];
    //    Skillinformation.SkillSlider[indexKey].value = Skillinformation.CDR[indexKey];
    //}

    /// <summary>
    /// 處理技能冷卻讀秒
    /// </summary>
    /// <param name="indexKey">快捷鍵鍵位</param>
    //public IEnumerator ProcessorSkillCoolDown(SkillUIModel skillUIData)
    //{
    //    //取得快捷鍵上 有此技能資料的鍵位
    //    var tempHotKey = SkillHotKey.Where(x => x.HotKeyDataID.Contains(skillUIData.SkillID)).Select(x => x.Keyindex).FirstOrDefault();
    //    //設定清單索引目標 若輸入不是0鍵則-1 若是0則設定為9(最大索引值)
    //    int getIndexKey = tempHotKey.Equals(0) ? (tempHotKey + 9) : (tempHotKey - 1);
    //    Skillinformation.CDR[getIndexKey] = 0;
    //    while (Skillinformation.CDR[getIndexKey] < Skillinformation.CDsec[getIndexKey])
    //    {
    //        Skillinformation.CDR[getIndexKey] += 0.1f;
    //        Skillinformation.SkillSlider[getIndexKey].value = Skillinformation.CDR[getIndexKey];
    //        //print(skillUIData.Name + "當前冷卻:" + Skillinformation.CDR[getIndexKey] + "總時間:" + Skillinformation.CDsec[getIndexKey]);
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    ///// <summary>
    ///// 確認玩家是否進入可施放範圍
    ///// </summary>
    //public IEnumerator SkillDistanceCheck(SkillUIModel skillUIData, string UpgradeSkillID = "")
    //{
    //    DistanceWithTarget();
    //    //是否有在使用的技能 是否魔力足夠 以及 該技能冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能) 
    //    if (!UsingSkill && PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage >= 0 && Skillinformation.CDR[keyIndex] >= Skillinformation.CDsec[keyIndex])
    //    {
    //        UsingSkill = true;
    //        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
    //        //若還沒進入施放距離則移動玩家
    //        while (dis > skillUIData.Distance)
    //        {
    //            DistanceWithTarget();
    //            //PlayerDataOverView.Instance.CharacterMove.Character.transform.LookAt(SelectTarget.Instance.Targetgameobject.transform);

    //            // 取得角色面相目標的方向
    //            Vector3 direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
    //            // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
    //            direction.y = 0;
    //            // 如果 direction 的長度不為零，設定角色的朝向
    //            if (direction != Vector3.zero)
    //                PlayerDataOverView.Instance.CharacterMove.Character.transform.rotation = Quaternion.LookRotation(direction);

    //            //跑步動畫
    //            PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);
    //            //設定移動座標
    //            PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
    //                Vector3.MoveTowards(PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position,
    //                SelectTarget.Instance.Targetgameobject.Povit.position,
    //                PlayerDataOverView.Instance.CharacterMove.MoveSpeed);

    //            yield return new WaitForEndOfFrame();
    //        }
    //        PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
    //        CallSkillEffect(skillUIData, UpgradeSkillID);
    //    }
    //    else
    //    {
    //        if (PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage < 0)
    //            CommonFunction.MessageHint("魔力不足...", HintType.Warning);
    //        else if (Skillinformation.CDR[keyIndex] < Skillinformation.CDsec[keyIndex])
    //            CommonFunction.MessageHint("該技能冷卻時間未完成!", HintType.Warning);

    //    }
    //}

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
            CommonFunction.MessageHint("正在取消攻擊目標", HintType.Warning);
        }
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = false;
    }
}
