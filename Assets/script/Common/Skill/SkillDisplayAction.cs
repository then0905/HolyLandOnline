using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能施放前置工作
//==========================================
public class SkillDisplayAction : MonoBehaviour
{
    #region 靜態變數
    private static SkillDisplayAction instance;
    public static SkillDisplayAction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkillDisplayAction>();
            }
            return instance;
        }
    }
    #endregion

    [HideInInspector] public GameObject UsingSkillObj;  //儲存當前施放的技能

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
    public SkillInformation Skillinformation;

    [Header("技能相關參考")]
    public HotKeyData[] SkillHotKey = new HotKeyData[10];

    [Header("指向技攻擊範圍")]
    public Canvas SkillArrowCanvas;             //指向技畫布
    public Image SkillArrowImage;               //指向技圖示

    [Header("攻擊範圍與圓圈形範圍技")]
    public Image SkillTargetCircle;             //圓圈型範圍技圖示
    public Image SkillPlayerCricle;             //攻擊範圍技圖示
    public Canvas SkillTargetCanvas;            //圓圈型範圍技畫布
    public Vector3 PosUp;
    public float maxSkillTargetDistance;        //技能最大範圍

    [Header("扇形攻擊範圍")]
    public Canvas SkillConeCanvas;              //扇形範圍技畫布
    public Image SkillConeImage;                //扇形範圍技圖示

    float dis;

    //[Header("攻擊範圍判定")]
    //public bool TargetMove;
    //public bool ConeMove;
    //public bool CircleMove;
    //public bool ArrowMove;

    /// <summary>
    /// 快捷欄上的技能冷卻紀錄
    /// </summary>
    [Serializable]
    public class SkillInformation
    {
        public List<float> CDsec = new List<float>();           //當前CD
        public List<float> CDR = new List<float>();             //冷卻CD
        public List<Slider> SkillSlider = new List<Slider>();   //Slider
    }

    //記錄追擊協程
    public Coroutine SkillChasingCoroutine;

    private void Start()
    {
        //關閉所有範圍圖示
        SkillArrowImage.GetComponent<Image>().enabled = false;
        SkillTargetCircle.GetComponent<Image>().enabled = false;
        SkillPlayerCricle.GetComponent<Image>().enabled = false;
        SkillConeImage.GetComponent<Image>().enabled = false;
    }

    private void OnEnable()
    {
        PlayerDataOverView.Instance.CharacterMove.ControlCharacterEvent += StopSkillChasingTarge;
    }

    private void OnDisable()
    {
        if (PlayerDataOverView.Instance)
            PlayerDataOverView.Instance.CharacterMove.ControlCharacterEvent -= StopSkillChasingTarge;

    }

    void Update()
    {
        ////刷新hit
        //RaycastHit hit;
        //Ray ray = CharacterCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        ////指向技顯示 並跟隨滑鼠
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    Postion = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        //}

        ////範圍技顯示
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    if (hit.collider.gameObject != this.gameObject)
        //    {
        //        PosUp = new Vector3(hit.point.x, 10f, hit.point.z);
        //        Postion = hit.point;
        //    }
        //}

        ////指向技&扇形技作用
        //Quaternion transRot = Quaternion.LookRotation(Postion - CharacterTransform.position);
        //transRot.eulerAngles = new Vector3(0, transRot.eulerAngles.y, transRot.eulerAngles.z);
        //SkillArrowCanvas.transform.rotation = Quaternion.Lerp(transRot, SkillArrowCanvas.transform.rotation, 0);
        //SkillConeCanvas.transform.rotation = Quaternion.Lerp(transRot, SkillConeCanvas.transform.rotation, 0);

        ////範圍技作用
        //var hitPosDir = (hit.point - CharacterTransform.transform.position).normalized;
        //float distance = Vector3.Distance(hit.point, CharacterTransform.transform.position);
        //maxSkillTargetDistance = SkillTargetCircle.GetComponent<RectTransform>().rect.width;
        //distance = Mathf.Min(distance, maxSkillTargetDistance);

        //var newHitPos = CharacterTransform.transform.position + hitPosDir * distance;
        //SkillTargetCanvas.transform.position = newHitPos;

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
        //判斷 技能是否有在使用中 或是 快捷鍵的資料是否為空值
        if (!string.IsNullOrEmpty(SkillHotKey[inputNumber].HotKeyDataID))
        {
            //紀錄輸入的鍵位
            keyIndex = inputNumber;
            //var skillData = GameData.SkillsDataDic.Where(x => x.Value.Name.Contains(SkillHotKey[inputNumber].SkillName)).Select(x => x.Value).FirstOrDefault();
            //取得 該技能UI版資料
            var skillUIData = GameData.SkillsUIDic.Where(x => x.Key.Contains(SkillHotKey[inputNumber].HotKeyDataID)).Select(x => x.Value).FirstOrDefault();

            //判斷是否魔力足夠 以及 冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能)

            if (PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage < 0)
            {
                CommonFunction.MessageHint("魔力不足...", HintType.Warning);
                return;
            }
            if (Skillinformation.CDR[keyIndex] < Skillinformation.CDsec[keyIndex])
            {
                CommonFunction.MessageHint("該技能冷卻時間未完成!", HintType.Warning);
                return;
            }
            if (UsingSkill)
            {
                CommonFunction.MessageHint("當前有技能正在使用中!", HintType.Warning);
                return;
            }

            switch (skillUIData.Type)
            {
                //指向技能類型
                case "Arrow":
                    //若已選取目標 接近目標到可施放範圍
                    if (SelectTarget.Instance.CatchTarget)
                        SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
                    else
                        //若未選取 顯示該技能範圍
                        SkillArrow(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
                    break;

                //指定技能類型
                case "Target":
                    if (SelectTarget.Instance.CatchTarget)
                        SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
                    else
                        SkillTarget(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
                    break;

                //圓圈型範圍技能類型
                case "Circle":
                    if (SelectTarget.Instance.CatchTarget)
                        SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
                    else
                        SkillCircle(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
                    break;

                //扇型範圍技能類型
                case "Cone":
                    if (SelectTarget.Instance.CatchTarget)
                        SkillChasingCoroutine = StartCoroutine(SkillDistanceCheck(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID));
                    else
                        SkillCone(skillUIData, SkillHotKey[inputNumber].UpgradeSkillID);
                    break;
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
    public void SkillArrow(SkillUIModel skillUIData, string UpgradeSkillID = "")
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
    public void SkillCone(SkillUIModel skillUIData, string UpgradeSkillID = "")
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
    public void SkillCircle(SkillUIModel skillUIData, string UpgradeSkillID = "")
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
    public void SkillTarget(SkillUIModel skillUIData, string UpgradeSkillID = "")
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
    public void CallSkillEffect(SkillUIModel skillUIData, string UpgradeSkillID = "")
    {
        string queryResule =
        GameData.SkillsDataDic.Where(x => x.Value.Name.Contains(skillUIData.Name)).Select(x => x.Value.SkillID).FirstOrDefault();
        GameObject effectObj = CommonFunction.LoadObject<GameObject>("SkillPrefab", "SkillEffect_" + queryResule);

        //生成技能效果物件
        UsingSkillObj = Instantiate(effectObj);
        //初始化技能效果物件
        UsingSkillObj.GetComponent<Skill_Base>().InitSkillEffectData(skillUIData.CastMage, UpgradeSkillID != "",
            PlayerDataOverView.Instance, SelectTarget.Instance.Targetgameobject.GetComponent<ICombatant>());

        //若技能效果為升級版 執行升級效果
        if (UpgradeSkillID != "")
            UsingSkillObj.GetComponent<Skill_Base>().GetSkillUpgradeEffect(UpgradeSkillID, PlayerDataOverView.Instance, SelectTarget.Instance.Targetgameobject.GetComponent<ICombatant>());
        //技能進入冷卻 併計時
        StartCoroutine(ProcessorSkillCoolDown(skillUIData));
        //if (PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage >= 0 && Skillinformation.CDR[keyIndex] >= Skillinformation.CDsec[keyIndex])
        //{
        //    //關閉所有畫布
        //    SkillArrowImage.GetComponent<Image>().enabled = false;
        //    SkillTargetCircle.GetComponent<Image>().enabled = false;
        //    SkillPlayerCricle.GetComponent<Image>().enabled = false;
        //    SkillConeImage.GetComponent<Image>().enabled = false;

        //    //扣除魔力
        //    PlayerDataOverView.Instance.PlayerData_.MP -= skillUIData.CastMage;
        //    //技能進入冷卻並執行讀秒
        //    Skillinformation.CDR[keyIndex] = 0;
        //    SkillCD(keyIndex);
        //    //技能已施放完成
        //    UsingSkill = true;
        //    //執行角色施放該技能動畫
        //    characterAnimator.SetTrigger(skillUIData.AnimaTrigger);
        //}
    }

    ///// <summary>
    ///// 施放技能扣除魔力
    ///// </summary>
    ///// <param name="skillData">鍵入的技能資料</param>
    //public void ManaCost(SkillDataModel skillData, SkillUIModel skillUIData)
    //{
    //    //判斷是否魔力足夠 以及 冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能)
    //    if (PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage >= 0 && Skillinformation.CDR[keyIndex] >= Skillinformation.CDsec[keyIndex])
    //    {
    //        //關閉所有畫布
    //        SkillArrowImage.GetComponent<Image>().enabled = false;
    //        SkillTargetCircle.GetComponent<Image>().enabled = false;
    //        SkillPlayerCricle.GetComponent<Image>().enabled = false;
    //        SkillConeImage.GetComponent<Image>().enabled = false;

    //        //扣除魔力
    //        PlayerDataOverView.Instance.PlayerData_.MP -= skillUIData.CastMage;
    //        //技能進入冷卻並執行讀秒
    //        Skillinformation.CDR[keyIndex] = 0;
    //        SkillCD(keyIndex);
    //        //技能已施放完成
    //        UsingSkill = true;
    //        //執行角色施放該技能動畫
    //        characterAnimator.SetTrigger(skillUIData.AnimaTrigger);
    //    }
    //}

    /// <summary>
    /// 設定快捷鍵上的技能冷卻與資料(裝上技能時呼叫)
    /// </summary>
    /// <param name="indexKey">快捷鍵鍵位</param>
    public void CoolDownRecord(int indexKey)
    {
        Skillinformation.SkillSlider[indexKey].maxValue = Skillinformation.CDsec[indexKey];
        Skillinformation.SkillSlider[indexKey].value = Skillinformation.CDR[indexKey];
    }

    /// <summary>
    /// 處理技能冷卻讀秒
    /// </summary>
    /// <param name="indexKey">快捷鍵鍵位</param>
    public IEnumerator ProcessorSkillCoolDown(SkillUIModel skillUIData)
    {
        //取得快捷鍵上 有此技能資料的鍵位
        var tempHotKey = SkillHotKey.Where(x => x.HotKeyDataID.Contains(skillUIData.SkillID)).Select(x => x.Keyindex).FirstOrDefault();
        //設定清單索引目標 若輸入不是0鍵則-1 若是0則設定為9(最大索引值)
        int getIndexKey = tempHotKey.Equals(0) ? (tempHotKey + 9) : (tempHotKey - 1);
        Skillinformation.CDR[getIndexKey] = 0;
        while (Skillinformation.CDR[getIndexKey] < Skillinformation.CDsec[getIndexKey])
        {
            Skillinformation.CDR[getIndexKey] += 0.1f;
            Skillinformation.SkillSlider[getIndexKey].value = Skillinformation.CDR[getIndexKey];
            //print(skillUIData.Name + "當前冷卻:" + Skillinformation.CDR[getIndexKey] + "總時間:" + Skillinformation.CDsec[getIndexKey]);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 確認玩家是否進入可施放範圍
    /// </summary>
    public IEnumerator SkillDistanceCheck(SkillUIModel skillUIData, string UpgradeSkillID = "")
    {
        DistanceWithTarget();
        //是否有在使用的技能 是否魔力足夠 以及 該技能冷卻時間是否完成刷新(防止玩家重複按指扣除魔力並沒有施放技能) 
        if (!UsingSkill && PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage >= 0 && Skillinformation.CDR[keyIndex] >= Skillinformation.CDsec[keyIndex])
        {
            UsingSkill = true;
            PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
            //若還沒進入施放距離則移動玩家
            while (dis > skillUIData.Distance)
            {
                DistanceWithTarget();
                PlayerDataOverView.Instance.CharacterMove.Character.transform.LookAt(SelectTarget.Instance.Targetgameobject.transform);
                PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);

                PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
                    Vector3.MoveTowards(PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position,
                    SelectTarget.Instance.Targetgameobject.Povit.position,
                    PlayerDataOverView.Instance.CharacterMove.MoveSpeed);

                yield return new WaitForEndOfFrame();
            }
            PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
            CallSkillEffect(skillUIData, UpgradeSkillID);
        }
        else
        {
            if (PlayerDataOverView.Instance.PlayerData_.MP - skillUIData.CastMage < 0)
                CommonFunction.MessageHint("魔力不足...", HintType.Warning);
            else if (Skillinformation.CDR[keyIndex] < Skillinformation.CDsec[keyIndex])
                CommonFunction.MessageHint("該技能冷卻時間未完成!", HintType.Warning);

        }
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
            CommonFunction.MessageHint("正在取消攻擊目標", HintType.Warning);
        }
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = false;
    }
}
