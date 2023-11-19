using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ArrowHit : MonoBehaviour
{
    public SkillDisplayAction Skillrange;
    public GameObject SKillArrow;
    public BoxCollider ArrowCollider;
    //private void Update()
    //{
    //    ArrowCollider.size = new Vector3( SKillArrow.GetComponent<RectTransform>().sizeDelta.x, SKillArrow.GetComponent<RectTransform>().sizeDelta.y, 1);
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Monster")
    //    {            
    //            if (DataBase.Instance.SkillDB.DurinSskillUse && Skillrange.SkillArrowImage.GetComponent<Image>().enabled)
    //            {
    //                print("指定路徑範圍內");
    //                Skillrange.ManaCost(Skillrange.keyIndex);
    //            }

    //    }
    //}
}
