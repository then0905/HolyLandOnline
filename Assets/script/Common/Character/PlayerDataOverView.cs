using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/04/21
//  創建用途: 玩家資料總覽
//==========================================
public class PlayerDataOverView : ActivityCharacterBase
{
    #region 全域靜態變數

    private static PlayerDataOverView instance;
    public static PlayerDataOverView Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerDataOverView>(true);
            }
            return instance;
        }
    }

    #endregion

    [Header("遊戲物件")]
    public Character_move CharacterMove;

    //血條
    [HideInInspector] public Slider HpSlider;
    //魔力條
    [HideInInspector] public Slider MpSlider;
    //經驗條
    [HideInInspector] public Slider ExpSlider;
    //血條文字
    [HideInInspector] public TMP_Text HpText;
    //魔力文字
    [HideInInspector] public TMP_Text MpText;
    //等級文字
    [HideInInspector] public TMP_Text LvText;
    //玩家名稱文字
    [HideInInspector] public TMP_Text NameText;

    //暫存玩家座標(寫入及讀取玩家本地資料專用)
    public Vector3 TempPlayerPos
    {
        get { return gameObject.transform.position; }
        set { gameObject.transform.position = value; }
    }

    [Header("遊戲資料")]
    public PlayerData PlayerData_ = new PlayerData();

    private ICombatant lastAttacker;        //紀錄最後攻擊者(死亡時發送訊息)

    //public override string GetAttackMode { get; set; }
    public override int HP
    {
        get => PlayerData_.HP;
        set
        {
            //判斷增加的值是否超出血量Range範圍 設定滿血 或 死亡 或更動數值
            if (value > PlayerData_.MaxHP)
            {
                //滿血
                value = PlayerData_.MaxHP;
            }
            if (value < 0)
            {
                //死亡 
                value = 0;
                //呼叫死亡事件
                IsDead = true;
            }
            PlayerData_.HP = value;
        }
    }
    public override int MP
    {
        get => PlayerData_.MP; set
        {
            //判斷增加的值是否超出魔力Range範圍 設定滿魔 或更動數值
            if (value > PlayerData_.MaxMP)
            {
                //滿魔
                value = PlayerData_.MaxMP;
            }
            //正常更動
            PlayerData_.MP = value;
        }
    }
    public override int ATK
    {
        get
        {
            return GetAttackMode switch
            {
                "RemoteATK" => PlayerData_.RemoteATK,
                "MageATK" => PlayerData_.MageATK,
                "MeleeATK" => PlayerData_.MeleeATK,
                _ => 0
            };
        }
    }
    public override int Hit
    {
        get
        {
            return GetAttackMode switch
            {
                "RemoteATK" => PlayerData_.RemoteHit,
                "MageATK" => PlayerData_.MageHit,
                "MeleeATK" => PlayerData_.MeleeHit,
                _ => 0
            };
        }
    }
    public override string Name { get => PlayerData_.PlayerName; }
    public override int LV { get => PlayerData_.Lv; }
    public override int Avoid { get => PlayerData_.Avoid; }
    public override int DEF { get => PlayerData_.DEF; }
    public override int MDEF { get => PlayerData_.MDEF; }
    public override int DamageReduction { get => PlayerData_.DamageReduction; }
    public override float Crt { get => PlayerData_.Crt; }
    public override int CrtDamage { get => PlayerData_.CrtDamage; }
    public override float CrtResistance { get => PlayerData_.CrtResistance; }
    public override float DisorderResistance { get => PlayerData_.DisorderResistance; }
    public override float BlockRate { get => PlayerData_.BlockRate; }
    public override float ElementDamageIncrease { get => PlayerData_.ElementDamageIncrease; }
    public override float ElementDamageReduction { get => PlayerData_.ElementDamageReduction; }
    public override GameObject Obj { get => gameObject; }

    public override void SkillEnable(bool enable)
    {
        base.SkillEnable(enable);
        SkillController skillController = SkillController.Instance;
        //若有正在施放追擊的協程 取消
        if (skillController.SkillChasingCoroutine != null && !enable)
        {
            StopCoroutine(skillController.SkillChasingCoroutine);
            skillController.SkillChasingCoroutine = null;
        }
    }

    public override void AttackEnable(bool enbale)
    {
        base.AttackEnable(enabled);
        NormalAttackSystem normalAttackSystem = NormalAttackSystem.Instance;
        //若有正在施放追擊的協程 取消
        if (normalAttackSystem.NormalAttackCoroutine != null && !enbale)
        {
            StopCoroutine(normalAttackSystem.NormalAttackCoroutine);
            normalAttackSystem.NormalAttackCoroutine = null;
        }
    }

    [Header("事件區")]
    public Action UIrefresh;        //刷新玩家UI的事件
    public Action<int> ChangeHpEvent;        //刷新玩家HP的事件
    public Action<int> ChangeMpEvent;        //刷新玩家MP的事件


    private void OnEnable()
    {
        CharacterMove.ControlCharacterEvent += NormalAttackSystem.Instance.StopNormalAttack;
        CharacterMove.ControlCharacterEvent += SkillController.Instance.StopSkillChasingTarge;
        SelectTarget.Instance.CharacterCamera = CharacterMove.CharacterCamera;
    }

    private void OnDisable()
    {
        CharacterMove.ControlCharacterEvent -= NormalAttackSystem.Instance.StopNormalAttack;
        CharacterMove.ControlCharacterEvent -= SkillController.Instance.StopSkillChasingTarge;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        RefreshExpAndLv();
        //呼叫自然恢復魔力與血量
        StartCoroutine(RecoveryHpCoroutine());
        StartCoroutine(RecoveryMpCoroutine());

        UIrefresh += RefreshExpAndLv;
        UIrefresh += PlayerDataPanelProcessor.Instance.SetPlayerDataContent;
        ChangeHpEvent += ChangePlayerHp;
        ChangeMpEvent += ChangePlayerMp;
        ExpProcessor();
    }

    /// <summary>
    /// 自然恢復生命計時
    /// </summary>
    /// <returns></returns>
    public IEnumerator RecoveryHpCoroutine()
    {
        float hpRecoverSec = GameData.GameSettingDic["HpRecoverySec"].GameSettingValue;
        while (hpRecoverSec > 0)
        {
            hpRecoverSec -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        ChangeHpEvent.Invoke(PlayerData_.HP_Recovery);

        yield return StartCoroutine(RecoveryHpCoroutine());
    }

    /// <summary>
    /// 自然恢復魔力計時
    /// </summary>
    /// <returns></returns>
    public IEnumerator RecoveryMpCoroutine()
    {
        float hpRecoverSec = GameData.GameSettingDic["MpRecoverySec"].GameSettingValue;
        while (hpRecoverSec > 0)
        {
            hpRecoverSec -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        ChangeMpEvent.Invoke(PlayerData_.MP_Recovery);

        yield return StartCoroutine(RecoveryMpCoroutine());
    }

    /// <summary>
    /// 設定UI資料 血量 魔力 經驗值?
    /// </summary>
    private void RefreshExpAndLv()
    {
        //獲取等級
        LvText.text = PlayerData_.Lv.ToString();
        //獲取玩家名稱
        NameText.text = PlayerData_.PlayerName;

        //獲取最大生命值與魔力
        HpSlider.maxValue = PlayerData_.MaxHP;
        MpSlider.maxValue = PlayerData_.MaxMP;

        //獲取當前生命值與魔力
        HpSlider.value = HP;
        MpSlider.value = MP;

        //帶入UI Slider
        HpText.text = HpSlider.value + "/" + HpSlider.maxValue;
        MpText.text = MpSlider.value + "/" + MpSlider.maxValue;

        LoadPlayerData.SaveUserData();
    }

    /// <summary>
    /// 經驗值更動時的處理
    /// </summary>
    public void ExpProcessor()
    {
        //若經驗值小於0 強迫設定為0 (死亡扣除經驗避免扣超過0)
        if (PlayerData_.Exp <= 0)
            PlayerData_.Exp = 0;

        //設定經驗值條資料
        ExpSlider.value = PlayerData_.Exp;
        ExpSlider.maxValue = GameData.ExpAndLvDic.Where(x => x.Key.Contains(PlayerData_.Lv.ToString())).Select(x => x.Value).FirstOrDefault().EXP;

        //若玩家經驗值>最大經驗值條 為 升級事件
        if (PlayerData_.Exp >= ExpSlider.maxValue)
        {
            //更新經驗值條(扣除當前最大經驗值)
            PlayerData_.Exp -= int.Parse(ExpSlider.maxValue.ToString());
            PlayerData_.Lv++;
            //呼叫刷新與升等
            LVup();
        }
    }

    /// <summary>
    /// 升級處理
    /// </summary>
    private void LVup()
    {
        ClassAndSkill.Instance.Init();//技能視窗初始化
        StatusOperation.Instance.StatusMethod();//使用者資料刷新
        RefreshExpAndLv();
        //回復玩家生命值與魔力
        PlayerData_.HP = PlayerData_.MaxHP;
        PlayerData_.MP = PlayerData_.MaxMP;
        //更新技能 暫時寫在這 以後不確定會不會再學技能這塊增加NPC或什麼道具學習
        Init();
    }

    /// <summary>
    /// 更動玩家血量
    /// </summary>
    /// <param name="value">更動值</param>
    private void ChangePlayerHp(int value)
    {
        HP = value + HP;
        UIrefresh?.Invoke();
    }
    /// <summary>
    /// 更動玩家魔力
    /// </summary>
    /// <param name="value">更動值</param>
    private void ChangePlayerMp(int value)
    {
        MP = value + MP;
        UIrefresh?.Invoke();
    }

    public override void DealingWithInjuriesMethod(ICombatant attackerData, int damage, bool animTrigger = true)
    {
        lastAttacker = attackerData;
        ChangeHpEvent.Invoke(damage);
    }

    public override void DealingWithHealMethod(ICombatant attackerData, int value, bool animTrigger = true)
    {
        ChangeHpEvent.Invoke(value);
    }

    public override void DealingWithMageMethod(ICombatant attackerData, int value, bool animTrigger = true)
    {
        ChangeMpEvent.Invoke(value);
    }

    public override void GetBuffEffect(ICombatant target, OperationData operationData)
    {
        if (operationData is SkillOperationData skillOperationData)
            StatusOperation.Instance.SkillEffectStatusOperation(skillOperationData.InfluenceStatus, (skillOperationData.AddType == "Rate"), skillOperationData.EffectValue);
        else if (operationData is ItemEffectData itemOperationData)
            StatusOperation.Instance.SkillEffectStatusOperation(itemOperationData.InfluenceStatus, (itemOperationData.AddType == "Rate"), itemOperationData.EffectValue);
    }

    public override void RemoveBuffEffect(ICombatant target, OperationData operationData)
    {
        if (operationData is SkillOperationData skillOperationData)
            StatusOperation.Instance.SkillEffectStatusOperation(skillOperationData.InfluenceStatus, (skillOperationData.AddType == "Rate"), skillOperationData.EffectValue * -1);
        else if (operationData is ItemEffectData itemOperationData)
            StatusOperation.Instance.SkillEffectStatusOperation(itemOperationData.InfluenceStatus, (itemOperationData.AddType == "Rate"), itemOperationData.EffectValue * -1);
    }

    public override void GetDebuff(DebuffEffectBase debuffEffectBase)
    {
        base.GetDebuff(debuffEffectBase);
        CharacterStatusManager.Instance.InitCharacterStatusHintCheck(debuffEffectBase);
    }

    protected async override void CharacterIsDead()
    {
        //操作相關禁用
        MoveEnable(false);
        SkillEnable(false);
        AttackEnable(false);
        //播放死亡動畫
        CharacterMove.CharacterAnimator.SetTrigger("IsDead");
        //獲取死亡動畫時間
        float animationTime = CharacterMove.CharacterAnimator.GetCurrentAnimatorStateInfo(0).length;
        await Task.Delay((int)(animationTime * 1000));
        //顯示死亡視窗
        PlayerDataPanelProcessor.Instance.CallDeadWindow(lastAttacker);
        //獲取死亡扣除經驗值倍率
        float expDeathPenalty = GameData.GameSettingDic["ExpDeathPenalty"].GameSettingValue;
        PlayerData_.Exp -= (int)MathF.Round((1f - (PlayerData_.Exp * expDeathPenalty)), 0, MidpointRounding.AwayFromZero);
    }
}
