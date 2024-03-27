using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonDataModel;
using System.Threading.Tasks;
using System.Threading;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/20
//  創建用途:  怪物行為設定呈現
//==========================================

public class MonsterBehaviour : ActivityCharacterBase
{
    //取得動畫
    [SerializeField]private Animator MonsterAnimator;

    public GameObject MonsterNameUI;

    protected int currentHp;
    public int CurrentHp {
        get {return currentHp; }
        set
        {
            if (value <= 0) 
            {
                value = 0;
                StartCoroutine(DeadBehaviourAsync());
            }
                currentHp = value;

            //若當前選擇的目標是此怪物 更新狀態
            if(SelectTarget.Instance.Targetgameobject == this)
            {
                SetTargetInformation(this);
            }
        }
    }
    public BootysHandle BootysHandling;
    public SelectTarget SelectTarget_;

    //
    public string MonsterName;
    //
    private MonsterDataModel monsterValue;
    public MonsterDataModel MonsterValue 
    {
        get
        {
                return monsterValue;
        }
    }

    public void Start()
    {
        Init();
    }

    /// <summary>
    /// 初始化 設定怪物數值與當前血量
    /// </summary>
    public void Init()
    {
        monsterValue = GameData.MonstersDataDic?[MonsterName];
        currentHp = monsterValue.HP;

        //測試用 執行後秒殺怪物
        //StartCoroutine(MonsterTest());
    }
    /// <summary>
    /// 怪物死亡後執行方法
    /// </summary>
    public IEnumerator DeadBehaviourAsync()
    {
        //設定動畫
        MonsterAnimator.SetBool("IsDead", true);

        //經驗值與掉落物處理
        BootysHandling.GetBootysData(monsterValue.MonsterCodeID, transform);

        PlayerData.Exp += monsterValue.EXP;

        //等待秒數 執行清除物件工作
        yield return new WaitForSeconds(3);

        //清除物件 清除鎖定目標避免空UI
        SelectTarget_.CatchTarget = false;
        Destroy(this.gameObject);

    }

    /// <summary>
    /// 測試使用(開始執行後怪物立即死亡)
    /// </summary>
    /// <returns></returns>
    public IEnumerator MonsterTest()
    {
        yield return new WaitForSeconds(0.5f);
        CurrentHp = 0;
    }
    private void Update()
    { 
        //每幀刷新 讓怪物身上的UI面對玩家
        MonsterNameUI.transform.LookAt(MonsterNameUI.transform.position + Camera.main.transform.rotation * Vector3.forward,
Camera.main.transform.rotation * Vector3.up);
    }
}
