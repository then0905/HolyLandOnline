using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  技能Icon的拖曳功能
//==========================================

public class DragSkill : MonoBehaviour
{
    //紀錄開始拖曳時的父級對象
    Transform beginDragtrans;
    //
    public Transform TopOfUnit { get; set; }
    //是否已生成Icon
    private bool CloneDrag;
    //拖曳生成的物件
    public GameObject SkillClone;
    //存取生成物件
    GameObject skillCloneIns;
    //快捷鍵Transform
    public Transform SkillHotKey { get; set; }
    //
    // public SkillRange Skillrange;
    //技能名稱
    private string skillID;
    //
    public SkillUI Callskillbase;

    /// <summary>
    /// 起始拖曳
    /// </summary>
    /// <param name="data"></param>
    public void BeginDrag(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;
        //if (data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>() == null) return;
        //判斷是否為被動技能
        if (Callskillbase.Characteristic)
        {
            //是否已生成
            if (!CloneDrag)
            {
                CloneDrag = true;
                skillCloneIns = Instantiate(SkillClone, transform.position, transform.rotation, SkillHotKey);
                skillCloneIns.transform.localScale = Vector3.one;
                skillCloneIns.GetComponent<SkillUI>().SkillUpgradeID = data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>().SkillUpgradeID;
                skillCloneIns.GetComponent<Image>().sprite = (data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>().SkillBeUpgrade ?
                data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>().SkillUpgradeIcon
                : data.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite);
            }
            //PointerEventData _ = data as PointerEventData;

            skillID = data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>().SkillID;

            //若技能升級  更改拖曳的技能圖鑑
            skillCloneIns.GetComponent<Image>().sprite =
                (data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>().SkillBeUpgrade ?
                data.pointerCurrentRaycast.gameObject.GetComponent<SkillUI>().SkillUpgradeIcon
                : data.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite);

            if (skillCloneIns.transform.parent == TopOfUnit) return;
            beginDragtrans = skillCloneIns.transform.parent;
            skillCloneIns.transform.SetParent(TopOfUnit);
        }
    }

    /// <summary>
    /// 拖曳中
    /// </summary>
    /// <param name="data"></param>
    public void Dragging(BaseEventData baseEventData)
    {
        if (CloneDrag)
        {
            //由RectTransform獲取正確的滑鼠座標
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(TopOfUnit.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out pos);

            skillCloneIns.GetComponent<RectTransform>().anchoredPosition = pos;
            skillCloneIns.transform.GetComponent<Image>().raycastTarget = false;
        }
    }

    /// <summary>
    /// 結束拖曳
    /// </summary>
    /// <param name="data"></param>
    public void EndDrag(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;
        if (data == null || !CloneDrag) return;

        GameObject go = data.pointerCurrentRaycast.gameObject;
        if (go == null || go.tag != "HotKey")
        {
            EndDragMethod(skillCloneIns);

            return;
        }
        SetSkillHotKey(go.GetComponent<HotKeyData>(),
            skillCloneIns.GetComponent<Image>().sprite, CommonFunction.LoadSkillPrefab(skillID), skillCloneIns.GetComponent<SkillUI>().SkillUpgradeID);

        go.GetComponent<Image>().raycastTarget = true;
    }

    /// <summary>
    /// 設置快捷鍵資料
    /// </summary>
    /// <param name="skillhotkey"></param>
    /// <param name="skillIcon"></param>
    /// <param name="skillID"></param>
    public void SetSkillHotKey(HotKeyData skillhotkey, Sprite skillIcon, IHotKey skillID, string upgradeSkillID = "")
    {
        skillhotkey.SetSkill(skillIcon, skillID, upgradeSkillID);
        EndDragMethod(skillCloneIns);
    }

    /// <summary>
    /// 結束拖曳
    /// </summary>
    /// <param name="clone"></param>
    public void EndDragMethod(GameObject clone)
    {
        //刪除生成物件
        Destroy(clone);
        CloneDrag = false;
    }
}
