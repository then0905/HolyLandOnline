using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TargetHit : MonoBehaviour
{
    public SkillController Skillrange;
    public GameObject SkillPlayerCircle;
    public SphereCollider PlayerCircleCollider;
    //private void Update()
    //{
    //    PlayerCircleCollider.radius = SkillPlayerCircle.GetComponent<RectTransform>().sizeDelta.x / 2;
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Monster")
    //    {
    //        if (DataBase.Instance.SkillDB.DurinSskillUse && Skillrange.SkillPlayerCricle.GetComponent<Image>().enabled)
    //        {
    //            print("���w���|�d��");
    //            Skillrange.ManaCost(Skillrange.keyIndex);
    //        }

    //    }
    //}
}
