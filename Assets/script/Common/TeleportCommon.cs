using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/07/29
//  創建用途: 傳送門通用設定
//==========================================
[RequireComponent(typeof(BoxCollider))]
public class TeleportCommon : MonoBehaviour
{
    [Header("傳送門前往地圖ID")]
    [SerializeField] private string teleportMapName;
    public string TeleportMapName =>teleportMapName;

    private void OnCollisionEnter(Collision collision)
    {
        //檢查碰觸的物體是否為玩家
        if (collision.gameObject.GetComponent<PlayerDataOverView>() != null) 
        {
            //呼叫切換場景
            MapManager.Instance.NextMap(teleportMapName);
        }
    }
}
