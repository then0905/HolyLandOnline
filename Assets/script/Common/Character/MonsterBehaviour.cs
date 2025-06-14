using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 怪物行為Enum
/// </summary>
public enum MonserBehaviorEnum
{
    Attack,     //正在攻擊(戰鬥中)
    Pursue,      //追擊(被攻擊距離外的玩家攻擊，或是主動攻擊在範圍內偵查到玩家)
    Patrol      //偵查(在範圍內巡邏 隨機走動)
}

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/20
//  創建用途:  怪物行為設定呈現
//==========================================
public class MonsterBehaviour : ActivityCharacterBase
{
    //取得動畫
    [SerializeField] private Animator MonsterAnimator;

    [Header("遊戲物件")]
    //怪物UI
    public Canvas MonsterNameCanvas;
    public GameObject MonsterNameText;

    [Header("遊戲資料")]
    //怪物ID
    public string MonsterID;
    //怪物初始生成位置
    private Vector3 originpos;
    //停留最低時間
    private int monsterActivityIntervalMin;
    //停留最久時間
    private int monsterActivityIntervalMax;
    //怪物行為資料
    public MonserBehaviorEnum MonserBehaviorData;
    //怪物資料
    private MonsterDataModel monsterValue;
    public MonsterDataModel MonsterValue
    {
        get
        {
            return monsterValue;
        }
    }

    public bool MonsterIsAttack { get; set; }

    private float monsterAttackTimer;

    /// <summary>
    /// 戰鬥對象資料字典
    /// </summary>
    public Dictionary<ICombatant, int> BattleTargetDic = new Dictionary<ICombatant, int>();

    [SerializeField] private List<DebuffEffectBase> debuffEffectBases = new List<DebuffEffectBase>();
    /// <summary> 怪物身上接受的負面狀態清單 </summary>
    public List<DebuffEffectBase> DebuffEffectBases
    {
        get
        {
            return debuffEffectBases;
        }
        set
        {
            debuffEffectBases = value;
            if (SelectTarget.Instance.Targetgameobject == this)
            {
                SetTargetInformation(this);
            }
        }
    }

    //協程區
    public Coroutine MonsterPatrolCoroutine;        //偵查行為協程
    public Coroutine MonsterPursueCoroutine;        //追擊行為協程
    public Coroutine MonsterAttackCoroutine;        //攻擊行為協程
    public Coroutine CommonPursueCoroutine;     //追擊距離計算協程

    //怪物血量
    protected int hp;
    //怪物魔力
    protected int mp;

    public override string Name { get => monsterValue.Name.GetText(); }
    public override int HP
    {
        get { return hp; }
        set
        {
            if (value <= 0)
            {
                value = 0;
                IsDead = true;
            }
            else if (value == monsterValue.HP)
            {
                MonsterSetting(MonserBehaviorEnum.Patrol);
            }
            hp = value;

            //若當前選擇的目標是此怪物 更新狀態
            if (SelectTarget.Instance.Targetgameobject == this)
            {
                SetTargetInformation(this);
            }
        }
    }
    public override int MP
    {
        get { return mp; }
        set
        {
            mp = value;
        }
    }
    public override int LV { get => monsterValue.Lv; }
    public override int ATK { get => monsterValue.ATK; }
    public override string GetAttackMode { get { return monsterValue.AttackMode; } set { value = monsterValue.AttackMode; } }
    public override int Hit { get => monsterValue.Hit; }
    public override int Avoid { get => monsterValue.Avoid; }
    public override int DEF { get => monsterValue.DEF; }
    public override int MDEF { get => monsterValue.DEF; }
    public override int DamageReduction { get => monsterValue.DamageReduction; }
    public override float Crt { get => monsterValue.Crt; }
    public override int CrtDamage { get => monsterValue.CrtDamage; }
    public override float CrtResistance { get => monsterValue.CrtResistance; }
    public override float DisorderResistance { get => monsterValue.DamageReduction; }
    public override float BlockRate { get => monsterValue.BlockRate; }
    public override float ElementDamageIncrease { get => monsterValue.ElementDamageIncrease; }
    public override float ElementDamageReduction { get => monsterValue.ElementDamageReduction; }
    public override GameObject Obj { get => gameObject; }

    public override void MoveEnable(bool enable)
    {
        base.MoveEnable(enable);
        //偵查協程 取消
        if (MonsterPatrolCoroutine != null && !enable)
        {
            StopCoroutine(MonsterPatrolCoroutine);
            MonsterPatrolCoroutine = null;
        }
        //追擊行為協程 取消
        if (MonsterPursueCoroutine != null && !enable)
        {
            StopCoroutine(MonsterPursueCoroutine);
            MonsterPursueCoroutine = null;
        }
        //追擊距離協程 取消
        if (CommonPursueCoroutine != null && !enable)
        {
            StopCoroutine(CommonPursueCoroutine);
            CommonPursueCoroutine = null;
        }
    }

    public override void SkillEnable(bool enable)
    {
        base.SkillEnable(enable);
    }

    public override void AttackEnable(bool enbale)
    {
        base.AttackEnable(enabled);
        //攻擊協程 取消
        if (MonsterAttackCoroutine != null && !enbale)
        {
            StopCoroutine(MonsterAttackCoroutine);
            MonsterAttackCoroutine = null;
        }
    }

    /// <summary>
    /// 檢查是否可以移動
    /// </summary>
    /// <returns></returns>
    private bool CanMove()
    {
        return (!MonsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonsterAttack")) && (!MonsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonsterInjuried"));
    }

    public void Start()
    {
        //設定怪物頭上文字轉移到 渲染文字的畫布
        MonsterNameText.transform.SetParent(MapManager.Instance.CanvasMapText.transform);
    }

    private void OnDestroy()
    {
        //移除怪物名稱文字
        if (MonsterNameText.gameObject != null)
            Destroy(MonsterNameText.gameObject);
    }

    /// <summary>
    /// 初始化 設定怪物數值與當前血量
    /// </summary>
    public void Init(Vector3 originPos)
    {
        originpos = originPos;
        monsterValue = GameData.MonstersDataDic?[MonsterID];
        monsterActivityIntervalMin = (int)GameData.GameSettingDic["MonsterActivityIntervalMin"].GameSettingValue;
        monsterActivityIntervalMax = (int)GameData.GameSettingDic["MonsterActivityIntervalMax"].GameSettingValue;
        HP = monsterValue.HP;
        monsterAttackTimer = 1f / monsterValue.AtkSpeed;       //寫入普通攻擊間隔
        //測試用 執行後秒殺怪物
        //StartCoroutine(MonsterTest());
    }

    /// <summary>
    /// 怪物死亡後執行方法
    /// </summary>
    /// <returns></returns>
    public IEnumerator DeadBehaviourAsync()
    {
        //檢查是否有任務需要狩獵該怪物
        MissionManager.Instance.MissionScheduleCheck.Invoke(null, MonsterID);
        //設定動畫
        MonsterAnimator.SetBool("IsDead", true);

        //依照傷害量 降冪排序 所有造成傷害的玩家資料
        List<ICombatant> combatants = BattleTargetDic.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        //掉落物處理
        BootysHandle.Instance.GetBootysData(monsterValue.MonsterCodeID, transform, combatants);
        //經驗值處理
        ExpProcessor(monsterValue.ExpSplit);

        //清除物件 清除鎖定目標避免空UI
        SelectTarget.Instance.CatchTarget = false;

        //等待秒數 執行清除物件工作
        yield return new WaitForSeconds(3);

        //設定怪物重生
        MonsterManager.Instance.SetMonsterRebirth(originpos, this);

        if (this.gameObject != null)
            Destroy(this.gameObject);
    }

    /// <summary>
    /// 測試使用(開始執行後怪物立即死亡)
    /// </summary>
    /// <returns></returns>
    public IEnumerator MonsterTest()
    {
        yield return new WaitForSeconds(0.5f);
        HP = 0;
    }

    private void Update()
    {
        if (IsDead) return;
        //獲取世界轉換成螢幕的座標
        Vector3 screenPosition = PlayerDataOverView.Instance.CharacterMove.CharacterCamera.WorldToScreenPoint(HeadTrans.position);
        //計算怪物物件與攝影機的距離
        float distance = Vector3.Distance(HeadTrans.position, PlayerDataOverView.Instance.CharacterMove.CharacterCamera.transform.position);
        // 將屏幕坐標轉換為Canvas中的本地坐標
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            MapManager.Instance.CanvasMapText.GetComponent<RectTransform>(),
            screenPosition,
            MapManager.Instance.CanvasMapText.worldCamera,
            out Vector2 localPoint);

        //計算縮放距離
        float scale = Mathf.Clamp(2.0f - (distance * 0.02f), 0.5f, 2.0f);
        //若在玩家身後則不顯示
        MonsterNameText.gameObject.SetActive(screenPosition.z > 0);
        //設定文字座標
        MonsterNameText.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(localPoint.x, localPoint.y, 0);
        //設定文字旋轉
        MonsterNameText.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
        // 設定文字大小
        MonsterNameText.transform.localScale = new Vector3(scale, scale, scale);

    }

    /// <summary>
    /// 怪物行為設定
    /// </summary>
    /// <param name="monsterEnumData"></param>
    public void MonsterSetting(MonserBehaviorEnum monsterEnumData)
    {
        //若怪物死亡 不進行任何行為
        if (IsDead) return;
        MonserBehaviorData = monsterEnumData;
        BreakAnyCoroutine();
        switch (monsterEnumData)
        {
            //戰鬥中
            case MonserBehaviorEnum.Attack:
                MonsterAttackCoroutine = StartCoroutine(MonsterAttack());
                break;
            //偵查 巡邏
            case MonserBehaviorEnum.Patrol:
                MonsterPatrolCoroutine = StartCoroutine(MonsterPatrol());
                break;
            //追擊
            case MonserBehaviorEnum.Pursue:
                MonsterPursueCoroutine = StartCoroutine(MonsterPursue());
                break;
        }
    }

    /// <summary>
    /// 中斷任何協程
    /// </summary>
    public void BreakAnyCoroutine()
    {
        if (MonsterPatrolCoroutine != null)
        {
            StopCoroutine(MonsterPatrolCoroutine);
            MonsterPatrolCoroutine = null;
        }
        if (MonsterAttackCoroutine != null)
        {
            StopCoroutine(MonsterAttackCoroutine);
            MonsterAttackCoroutine = null;
        }
        if (MonsterPursueCoroutine != null)
        {
            StopCoroutine(MonsterPursueCoroutine);
            MonsterPursueCoroutine = null;
        }
    }

    /// <summary>
    /// 怪物巡邏協程
    /// </summary>
    /// <returns></returns>
    private IEnumerator MonsterPatrol()
    {
        if (!MoveIsEnable.Equals(0)) yield break;
        yield return new WaitForSeconds(Random.Range(monsterActivityIntervalMin, monsterActivityIntervalMax + 1));
        if (!MoveIsEnable.Equals(0)) yield break;
        while (true)
        {
            Vector2 randomPoint = Random.insideUnitCircle * monsterValue.ActivityScope;
            Vector3 destination = originpos + new Vector3(randomPoint.x, 0, randomPoint.y);

            while (Vector3.Distance(destination, transform.position) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, monsterValue.WalkSpeed);

                Vector3 direction = (destination - transform.position).normalized;
                direction.y = 0; // 將 Y 軸設為 0，僅保留 X 和 Z 軸的方向

                Quaternion targetRotation = Quaternion.LookRotation(direction); // 計算目標旋轉
                float targetYRotation = targetRotation.eulerAngles.y; // 取得目標旋轉的 Y 軸旋轉值
                Quaternion rotationOnlyY = Quaternion.Euler(0, targetYRotation, 0); // 建立只包含 Y 軸旋轉的四元數
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationOnlyY, 360 * Time.deltaTime); // 使用 RotateTowards 逐漸旋轉

                MonsterAnimator.SetBool("IsWalking", true);
                yield return null;
            }

            MonsterAnimator.SetBool("IsWalking", false);
            yield return new WaitForSeconds(Random.Range(monsterActivityIntervalMin, monsterActivityIntervalMax + 1));
        }
    }

    /// <summary>
    /// 怪物追擊協程
    /// </summary>
    /// <returns></returns>
    private IEnumerator MonsterPursue()
    {
        if (!MoveIsEnable.Equals(0)) yield break;
        ActivityCharacterBase targetObj = BattleTargetDic.FirstOrDefault().Key as ActivityCharacterBase;
        if (targetObj != null)
        {
            CommonPursueCoroutine = StartCoroutine(CommonFunction.DetectionRangeMethod(povit.gameObject,
                targetObj.Povit.gameObject, monsterValue.AttackRange,
                () =>
                {
                    //怪物當前已追擊距離
                    float pursueDistance = Vector3.Distance(originpos, targetObj.Povit.gameObject.transform.position);
                    if (pursueDistance > monsterValue.PursueRange)
                    {
                        //Debug.Log("怪物:" + name + " 當前狀態 :" + "放棄追擊" + "怪物最遠可追擊距離 :" + monsterValue.PursueRange + "當前與目標差距 :" + pursueDistance);
                        MonsterAnimator.SetBool("IsWalking", false);
                        BreakAnyCoroutine();
                        StopCoroutine(CommonPursueCoroutine);

                        //超出怪物可追擊範圍 設定怪物重生
                        MonsterManager.Instance.SetMonsterRebirth(originpos, this, false);

                        if (this.gameObject != null)
                            Destroy(this.gameObject);
                    }
                    else
                    {
                        //Debug.Log("怪物:" + name + " 當前狀態 :" + "追擊" + " 當前距離 :" + pursueDistance);
                        MonsterAnimator.SetBool("IsWalking", true);

                        //調整面向
                        transform.LookAt(targetObj.transform);

                        transform.position = Vector3.MoveTowards(Povit.position, targetObj.Povit.position, monsterValue.RunSpeed);
                    }
                },
                () =>
                {
                    if (!targetObj.IsDead)
                        //Debug.Log("怪物:" + name + " 當前狀態 :" + "追擊成功 切換 攻擊");
                        MonsterSetting(MonserBehaviorEnum.Attack);
                    else
                        //玩家死亡後改為巡邏
                        MonsterSetting(MonserBehaviorEnum.Patrol);
                }));
        }

        yield return null;
    }

    /// <summary>
    /// 怪物攻擊協程
    /// </summary>
    /// <returns></returns>
    private IEnumerator MonsterAttack()
    {
        if (!AttackIsEnable.Equals(0)) yield break;
        //怪物攻擊內容 動畫 傷害設定 攻擊計時器等等...
        if (!MonsterIsAttack)
        {
            MonsterIsAttack = true;
            //取得對象
            var getTarget = BattleTargetDic.FirstOrDefault().Key;

            //調整面向
            transform.LookAt(getTarget.Obj.transform);
            //Quaternion targetRotation = Quaternion.LookRotation(getTarget.Obj.transform.position); // 計算目標旋轉
            //float targetYRotation = targetRotation.eulerAngles.y; // 取得目標旋轉的 Y 軸旋轉值
            //transform.rotation = Quaternion.Euler(0, targetYRotation, 0); // 直接旋轉到目標角度

            MonsterAnimator.SetTrigger("Attack");
            //攻擊計時器協程
            StartCoroutine(CommonFunction.Timer(monsterAttackTimer, null, (() => { MonsterIsAttack = false; })));
            //Debug.Log("怪物:" + name + " 當前狀態 :" + "攻擊");

            // 等待一段時間，以確保動畫已經開始播放
            yield return new WaitForSeconds(0.1f);

            //攻擊計算
            BattleOperation.Instance.NormalAttackEvent.Invoke(this, getTarget);
            // 等待動畫播放完畢
            yield return new WaitUntil(() => /*!MonsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonsterAttack")*/CanMove());

            //Debug.Log("怪物:" + name + " 當前狀態 :" + "攻擊完畢");
            //BreakAnyCoroutine();
            //攻擊完成重新設定追擊
            MonsterSetting(MonserBehaviorEnum.Pursue);
        }
        else
        {
            yield return null;
            //Debug.Log("怪物:" + name + " 當前狀態 :" + "攻擊 但攻擊計時未完成 切換追擊再次等待攻擊");
            MonsterSetting(MonserBehaviorEnum.Pursue);
        }
    }

    public override void DealingWithHealMethod(ICombatant battleData, int value, bool animTrigger = true)
    {
        //血量改動
        HP += value;
    }

    public override void DealingWithMageMethod(ICombatant battleData, int value, bool animTrigger = true)
    {
        //魔力改動
        MP += value;
    }

    public override void DealingWithInjuriesMethod(ICombatant battleData, int damage, bool animTrigger = true)
    {
        StartCoroutine(DealingWithInjuriesCoroutine(battleData, damage, animTrigger));
    }

    /// <summary>
    /// 怪物受到攻擊後執行的協程
    /// </summary>
    /// <param name="battleData"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    public IEnumerator DealingWithInjuriesCoroutine(ICombatant battleData, int damage, bool animTrigger)
    {
        //檢查此次傷害造成者是否已經在紀錄中
        if (!BattleTargetDic.ContainsKey(battleData))
            BattleTargetDic.Add(battleData, damage * -1);
        else
            BattleTargetDic[battleData] = damage * -1;

        //血量扣除
        HP += damage;

        if (animTrigger)
            //怪物動畫(受傷)
            MonsterAnimator.SetTrigger("Injuried");

        // 等待一段時間，以確保動畫已經開始播放
        yield return new WaitForSeconds(0.1f);

        // 等待動畫播放完畢
        yield return new WaitUntil(() => !MonsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonsterInjuried"));

        // 當動畫播放完成後，繼續執行後續的操作
        BattleTargetDic = BattleTargetDic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, y => y.Value);

        MonsterSetting(MonserBehaviorEnum.Pursue);
    }


    /// <summary>
    /// 經驗值處理
    /// </summary>
    /// <param name="setSplit">True : 造成傷害者依照比例分割 False: 有傷害者同樣的Exp</param>
    private void ExpProcessor(bool setSplit)
    {
        if (setSplit)
        {
            foreach (var item in BattleTargetDic)
            {
                //將經驗值依照傷害比例 分割給玩家
                int tempExp = (int)MathF.Round((float)monsterValue.HP / (float)item.Value, 1);
                item.Key.Obj.GetComponent<PlayerDataOverView>().PlayerData_.Exp += tempExp;
                item.Key.Obj.GetComponent<PlayerDataOverView>().ExpProcessor();
            }
        }
        else
        {
            foreach (var item in BattleTargetDic)
            {
                item.Key.Obj.GetComponent<PlayerDataOverView>().PlayerData_.Exp += monsterValue.EXP;
                item.Key.Obj.GetComponent<PlayerDataOverView>().ExpProcessor();
            }
        }
    }


    public override void GetDebuff(DebuffEffectBase debuffEffectBase)
    {
        base.GetDebuff(debuffEffectBase);

        //將此次獲得的debuff加入清單
        DebuffEffectBases.Add(debuffEffectBase);
        //更新選取目標資訊呈現
        if (SelectTarget.Instance.Targetgameobject == this)
        {
            SetTargetInformation(this);
        }
    }

    public override void RemoveDebuff(DebuffEffectBase debuffEffectBase)
    {
        base.RemoveDebuff(debuffEffectBase);

        //將此次需要移除的debuff移除清單
        DebuffEffectBases.Remove(debuffEffectBase);
        //若物件還存在 清除
        if (debuffEffectBase.gameObject)
            Destroy(debuffEffectBase.gameObject);
        //更新選取目標資訊呈現
        if (SelectTarget.Instance.Targetgameobject == this)
        {
            SetTargetInformation(this);
        }
        MonsterSetting(MonserBehaviorEnum.Pursue);
    }

    protected override void CharacterIsDead()
    {
        Debug.Log("怪物死亡");
        //移除怪物名稱文字
        if (MonsterNameText.gameObject != null)
            Destroy(MonsterNameText.gameObject);
        BreakAnyCoroutine();
        StartCoroutine(DeadBehaviourAsync());
    }
}
