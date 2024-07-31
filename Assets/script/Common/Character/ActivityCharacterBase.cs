using System;
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
        NpcBehavior npcBehavior = this as NpcBehavior;
        if (monsterBehaviour != null)
        {
            SetTargetInformation(monsterBehaviour);
        }
        else if (npcBehavior != null)
        {
            SetTargetInformation(npcBehavior);
        }
    }

    /// <summary>
    /// 設定目標信息(血量 等級..)
    /// </summary>
    protected void SetTargetInformation(NpcBehavior npcBehavior)
    {
        if (this is MonoBehaviour)
        {
            SelectTarget.Instance.TargetInformation.SetActive(true);
            SelectTarget.Instance.TargetHP.maxValue = npcBehavior.HP;
            SelectTarget.Instance.TargetHP.value = npcBehavior.HP;
            SelectTarget.Instance.TargetName.text = npcBehavior.Name;
            SelectTarget.Instance.TargetLV.text = 1.ToString();
            SelectTarget.Instance.TargetClass.text = "NPC";
            SelectTarget.Instance.TargetHP_MinAndMax.text = SelectTarget.Instance.TargetHP.value.ToString() + "/" + SelectTarget.Instance.TargetHP.maxValue.ToString();
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
            SelectTarget.Instance.TargetHP.maxValue = monster.MonsterValue.HP;
            SelectTarget.Instance.TargetHP.value = monster.HP;
            SelectTarget.Instance.TargetName.text = monster.MonsterValue.Name;
            SelectTarget.Instance.TargetLV.text = monster.MonsterValue.Lv.ToString();
            SelectTarget.Instance.TargetClass.text = monster.MonsterValue.Class;
            SelectTarget.Instance.TargetHP_MinAndMax.text = SelectTarget.Instance.TargetHP.value.ToString() + "/" + SelectTarget.Instance.TargetHP.maxValue.ToString();
        }
    }

    private void OnMouseDown()
    {
        if (SelectTarget.Instance.Targetgameobject == this)
        {
            if (this is NpcBehavior)
            {
                NpcBehavior behavior = (NpcBehavior)this;
                StartCoroutine(
                CommonFunction.DetectionRangeMethod(PlayerDataOverView.Instance.CharacterMove.gameObject,
                    gameObject, 7.5f,
                    () =>
                    {        //開啟自動尋路
                        PlayerDataOverView.Instance.CharacterMove.AutoNavToTarget = true;
                        float pursueDistance = Vector3.Distance(PlayerDataOverView.Instance.CharacterMove.gameObject.transform.position, gameObject.transform.position);
                        if (pursueDistance > 7.5f)
                        {
                            PlayerDataOverView.Instance.CharacterMove.Character.transform.LookAt(SelectTarget.Instance.Targetgameobject.transform);
                            PlayerDataOverView.Instance.CharacterMove.RunAnimation(true);
                            PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position =
                                Vector3.MoveTowards(PlayerDataOverView.Instance.CharacterMove.CharacterFather.transform.position,
                                SelectTarget.Instance.Targetgameobject.Povit.position,
                                PlayerDataOverView.Instance.CharacterMove.MoveSpeed);
                        }
                    }
                    ,()=> {
                        PlayerDataOverView.Instance.CharacterMove.RunAnimation(false);
                        behavior.NpcInit();
                    }));
            }
            else if (this is MonsterBehaviour)
            {
                NormalAttackSystem.Instance.StartNormalAttack(
                   PlayerDataOverView.Instance.CharacterMove.CharacterFather,
                   gameObject);
            }
        }
    }
}