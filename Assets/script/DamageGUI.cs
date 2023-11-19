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
    private Vector2 mPoint; //GUI坐標
    public float DestoryTime = 2.0f; //傷害數字消失時間

    void Start()
    {
        mTarget = transform.position; //獲取目標位置
        mScreen = Camera.main.WorldToScreenPoint(mTarget); //轉化為螢屏位置
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);
        StartCoroutine("Free"); //開啟一個協程
    }
    void Update()
    {
        this.transform.LookAt(this.transform.position + Camera.main.transform.rotation * Vector3.forward,
Camera.main.transform.rotation * Vector3.up);//讓傷害數字面對玩家
        transform.Translate(Vector3.up * 0.8f * Time.deltaTime); //傷害數字上移效果
        mTarget = transform.position;
        mScreen = Camera.main.WorldToScreenPoint(mTarget);
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);//實時變化位置
        DMGtext.text = DMGvalue.ToString();
    }
    IEnumerator Free()  //協程，傷害數字時間一到消失
    {
        yield return new WaitForSeconds(DestoryTime);
        Destroy(this.gameObject);
    }
}
