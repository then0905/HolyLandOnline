using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHit : MonoBehaviour
{
    public SkillDisplayAction Skillrange;
    public GameObject SkillTargetCircle;
    public SphereCollider CircleCollider;
    //private void Update()
    //{
    //    CircleCollider.radius = SkillTargetCircle.GetComponent<RectTransform>().sizeDelta.x / 2;
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Monster")
    //    {
    //        if (DataBase.Instance.SkillDB.DurinSskillUse && Skillrange.SkillTargetCircle.GetComponent<Image>().enabled)
    //        {
    //            print("指定路徑範圍內");
    //            Skillrange.ManaCost(Skillrange.);
    //        }

    //    }
    //}
}
