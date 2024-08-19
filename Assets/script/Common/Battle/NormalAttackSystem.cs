
using UnityEditor.Experimental.GraphView;
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
            NormalAttackCoroutine = StartCoroutine(CommonFunction.DetectionRangeMethod(player, target,
                PlayerDataOverView.Instance.PlayerData_.NormalAttackRange, ChasingTarget,
                () => { RunAttack(PlayerDataOverView.Instance, target.GetComponent<ICombatant>()); }));
        else
            //正在攻擊或是不可普通攻擊狀態
            CommonFunction.MessageHint("正在攻擊或是不可普通攻擊狀態", HintType.Warning);
    }

    /// <summary>
    /// 執行普通攻擊
    /// </summary>
    public void RunAttack(ICombatant attacker = null, ICombatant defender = null)
    {
        if (defender.IsDead) return;
        PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
        AttackAllow = true;
        //依照攻擊速度調整動畫播放速度
        PlayerDataOverView.Instance.CharacterMove.ControCharacterAnimationEvent.Invoke(AttackSpeedTimer, "NormalAttack");
        StartCoroutine(CommonFunction.Timer(AttackSpeedTimer, null, (() => { AttackAllow = false; PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = false; })));
        //執行攻擊運算
        BattleOperation.Instance.NormalAttackEvent.Invoke(attacker, defender);
    }

    /// <summary>
    /// 追擊目標
    /// </summary>
    public void ChasingTarget()
    {
        //開啟自動尋路
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;

        // 取得角色面相目標的方向
        Vector3 direction = SelectTarget.Instance.Targetgameobject.transform.position - PlayerDataOverView.Instance.CharacterMove.Character.transform.position;
        // 鎖定y軸的旋轉 避免角色在x軸和z軸上傾斜
        direction.y = 0;
        // 如果 direction 的長度不為零，設定角色的朝向
        if (direction != Vector3.zero)
            PlayerDataOverView.Instance.CharacterMove.Character.transform.rotation = Quaternion.LookRotation(direction);

        PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);

        PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
            Vector3.MoveTowards(PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position,
            SelectTarget.Instance.Targetgameobject.Povit.position,
            PlayerDataOverView.Instance.CharacterMove.MoveSpeed);
    }

    /// <summary>
    /// 停止普通攻擊狀態
    /// </summary>
    public void StopNormalAttack()
    {
        if (NormalAttackCoroutine != null)
        {
            StopCoroutine(NormalAttackCoroutine);
            NormalAttackCoroutine = null;
            CommonFunction.MessageHint("正在取消攻擊目標", HintType.Warning);
        }
        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = false;
    }
}
