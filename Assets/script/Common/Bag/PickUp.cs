using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/15
//  創建用途:  撿起物品腳本
//==========================================
public class PickUp : MonoBehaviour
{
    [Header("玩家總覽腳本"), SerializeField] private PlayerDataOverView playerDAtaOverView;

    //是否撿取的防呆
    public bool ispickup;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ispickup = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Item")
        {
            //確認當前未選擇到任何目標
            if (Input.GetKeyDown(KeyCode.Space) && !SelectTarget.Instance.CatchTarget)
            {
                if (!ispickup)
                {
                    ispickup = true;
                    other.GetComponent<BootysPresent>().BePickUP(playerDAtaOverView);
                }
            }
        }
    }
}
