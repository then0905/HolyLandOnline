using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageGUI : MonoBehaviour
{
    public Vector3 mTarget;
    public Vector3 mScreen;
    public string DMGvalue;


    public TMP_Text DMGtext;
    private Vector2 mPoint; //GUI����
    public float DestoryTime = 2.0f; //�ˮ`�Ʀr�����ɶ�

    void Start()
    {
        mTarget = transform.position; //����ؼЦ�m
        mScreen = Camera.main.WorldToScreenPoint(mTarget); //��Ƭ��ë̦�m
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);
        StartCoroutine("Free"); //�}�Ҥ@�Ө�{
    }
    void Update()
    {
        this.transform.LookAt(this.transform.position + Camera.main.transform.rotation * Vector3.forward,
Camera.main.transform.rotation * Vector3.up);//���ˮ`�Ʀr���缾�a
        transform.Translate(Vector3.up * 0.8f * Time.deltaTime); //�ˮ`�Ʀr�W���ĪG
        mTarget = transform.position;
        mScreen = Camera.main.WorldToScreenPoint(mTarget);
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);//����ܤƦ�m
        DMGtext.text = DMGvalue.ToString();
    }
    IEnumerator Free()  //��{�A�ˮ`�Ʀr�ɶ��@�����
    {
        yield return new WaitForSeconds(DestoryTime);
        Destroy(this.gameObject);
    }
}
