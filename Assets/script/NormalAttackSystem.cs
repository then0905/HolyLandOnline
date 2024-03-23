
using UnityEngine;
using UnityEngine.Rendering;

//==========================================
//  創建者:家豪
//  創建日期:2024/02/15
//  創建用途: 普通攻擊系統
//==========================================
public class NormalAttackSystem : MonoBehaviour
{
    #region 全域靜態變數

    //攻擊速度區間 每秒AttackSpeedTimer攻擊1下
    public static float AttackSpeedTimer;

    private void OnEnable()
    {
        Character_move.Instance.ControlCharacterEvent += StopNormalAttack;
    }
    private void OnDisable()
    {
        Character_move.Instance.ControlCharacterEvent -= StopNormalAttack;
    }

    /// <summary>
    /// 普通攻擊允許
    /// </summary>
    public static bool AttackAllow;

    public static NormalAttackSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<NormalAttackSystem>();
            return instance;
        }
    }

    private static NormalAttackSystem instance;

    #endregion

    //自動攻擊協程
    public Coroutine NormalAttackCoroutine;

    /// <summary>
    /// 啟動普通攻擊
    /// </summary>
    public void StartNormalAttack(GameObject player, GameObject target)
    {
        if (!AttackAllow)
            //需要進行判斷距離
            NormalAttackCoroutine = StartCoroutine(CommonFunction.DetectionRangeMethod(player, target, PlayerData.NormalAttackRange, ChasingTarget, RunAttack));
        else
            //正在攻擊或是不可普通攻擊狀態
            Instantiate(CommonFunction.MessageHint("正在攻擊或是不可普通攻擊狀態", HintType.Warning));
    }

    /// <summary>
    /// 執行普通攻擊
    /// </summary>
    public void RunAttack()
    {
        Character_move.Instance.RunAnimation(false);
        AttackAllow = true;
        //依照攻擊速度調整動畫播放速度
        Character_move.Instance.ControCharacterAnimationEvent.Invoke(AttackSpeedTimer, "NormalAttack");
        StartCoroutine(CommonFunction.Timer(AttackSpeedTimer, null, (() => { AttackAllow = false; Character_move.Instance.AutoNavToTarget = false; })));
        //執行攻擊運算
        BattleOperation.Instance.NormalAttackEvent(SelectTarget.Instance.Targetgameobject.gameObject);
    }

    /// <summary>
    /// 追擊目標
    /// </summary>
    public void ChasingTarget()
    {
        //開啟自動尋路
        Character_move.Instance.AutoNavToTarget = true;

        Character_move.Instance.Character.transform.LookAt(SelectTarget.Instance.Targetgameobject.transform);
        Character_move.Instance.RunAnimation(true);

        Character_move.Instance.CharacterFather.transform.position =
            Vector3.MoveTowards(Character_move.Instance.CharacterFather.transform.position,
            SelectTarget.Instance.Targetgameobject.Povit.position,
            Character_move.Instance.MoveSpeed);
    }

    /// <summary>
    /// 停止普通攻擊狀態
    /// </summary>
    public void StopNormalAttack()
    {
        if (NormalAttackCoroutine != null) {
            StopCoroutine(NormalAttackCoroutine);
            NormalAttackCoroutine= null;
            Instantiate(CommonFunction.MessageHint("正在取消攻擊目標", HintType.Warning));
        }
        Character_move.Instance.AutoNavToTarget = false;
    }
}
