using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConeHit : MonoBehaviour
{

    public GameObject SkillConeCanvas;
    public GameObject SkillCone;
    public SkillController Skillrange;
    public SphereCollider ConeCollider;
    private void Update()
    {
        ConeCollider.radius = SkillCone.GetComponent<RectTransform>().sizeDelta.x / 2;
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag == "Monster")
    //    {
    //        Vector3 targetDir = other.transform.position - SkillConeCanvas.transform.position;
    //        Vector3 forward = SkillConeCanvas.transform.forward;
    //        float angle = Vector3.Angle(targetDir, forward);
    //        if (angle <= (360*SkillCone.GetComponent<Image>().fillAmount/2))
    //        {
    //            if (DataBase.Instance.SkillDB.DurinSskillUse && Skillrange.SkillConeImage.GetComponent<Image>().enabled)
    //            {
    //                print("®°§Î§ðÀ»½d³ò¤º");
    //                Skillrange.ManaCost(Skillrange.keyIndex);
    //            }               
    //        }
    //    }
    //}
}
