using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/03/14
//  創建用途: 活體人物基底
//==========================================
public class ActivityCharacterBase : MonoBehaviour
{
    [Header("角色中心點"), SerializeField] protected Transform povit;
    public Transform Povit
    {
        get { return povit; }
    }
    [Header("角色頭部 傷害數字參考"), SerializeField] protected Transform headTrans;
    public Transform HeadTrans
    {
        get { return headTrans; }
    }
    [Header("可否被選取"), SerializeField]
    protected bool canBeChoose;
    public bool CanBeChoose => canBeChoose;

    public void BeenSelected()
    {
        if (!canBeChoose) return;
        MonsterBehaviour monsterBehaviour = this as MonsterBehaviour;
        if (monsterBehaviour != null)
        {
            SetTargetInformation(monsterBehaviour);
        }
    }

    /// <summary>
    /// 設定目標信息(血量 等級..)
    /// </summary>
    protected void SetTargetInformation(MonsterBehaviour monster)
    {
        if (this is MonoBehaviour)
        {
            SelectTarget.Instance.TargetInformation.SetActive(true);
            SelectTarget.Instance.TargetHP.value = monster.HP;
            SelectTarget.Instance.TargetName.text = monster.MonsterValue.Name;
            SelectTarget.Instance.TargetHP.maxValue = monster.MonsterValue.HP;
            SelectTarget.Instance.TargetLV.text = monster.MonsterValue.Lv.ToString();
            SelectTarget.Instance.TargetClass.text = monster.MonsterValue.Class;
            SelectTarget.Instance.TargetHP_MinAndMax.text = SelectTarget.Instance.TargetHP.value.ToString() + "/" + SelectTarget.Instance.TargetHP.maxValue.ToString();
        }
    }

    private void OnMouseDown()
    {
        if (SelectTarget.Instance.Targetgameobject == this)
        {
            NormalAttackSystem.Instance.StartNormalAttack(
               PlayerDataOverView.Instance.CharacterMove.CharacterFather,
               gameObject);
        }
    }
}