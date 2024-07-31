using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/07/30
//  創建用途: Canvas的相機控制
//==========================================
public class CanvasCameraControll : MonoBehaviour
{
    public void SetCamera()
    {
        GetComponent<Canvas>().worldCamera = PlayerDataOverView.Instance.CharacterMove.CharacterCamera;
    }
}
