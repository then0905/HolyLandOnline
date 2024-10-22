using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:太久不可考
//  創建用途: 旋轉攝影機視角與縮放
//==========================================
public class CameraRotation : MonoBehaviour
{
    public GameObject CharacterCamera;
    public GameObject Character;
    public int ScrollSpeed = 5;
    public float rotateSpeed = 2f;
    public float maxRotAngle = 80f;
    public float minRotAngle = -60f;

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    private float minDistance = 10f;
    private float maxDistance = 45f;
    private float currentDistance; //追蹤當前距離

    private void Start()
    {
        // 計算初始距離
        currentDistance = Vector3.Distance(CharacterCamera.transform.position, Character.transform.position);

        // 獲取攝影機相對於角色的方向
        Vector3 direction = (CharacterCamera.transform.position - Character.transform.position).normalized;

        // 使用 LookRotation 來計算初始旋轉
        Quaternion lookRotation = Quaternion.LookRotation(-direction); // 注意這裡使用負方向
        Vector3 angles = lookRotation.eulerAngles;

        // 設置初始旋轉角度
        currentYRotation = angles.y;
        currentXRotation = angles.x;

        // 規範化 X 軸旋轉角度
        if (currentXRotation > 180f)
        {
            currentXRotation -= 360f;
        }

        // 確保初始角度在限制範圍內
        currentXRotation = Mathf.Clamp(currentXRotation, minRotAngle, maxRotAngle);

        // 立即更新攝影機位置和旋轉，確保初始狀態正確
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        // 滑鼠右鍵按下時
        if (Input.GetMouseButton(1))
        {
            //處理攝影機視角拖曳
            HandleRotation();
        }
        //處理攝影機視角縮放
        HandleZoom();
    }

    /// <summary>
    /// 處理攝影機視角縮放距離
    /// </summary>
    private void HandleZoom()
    {
        //獲取滾輪輸入值
        float wheel = Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        
        //沒有輸入值 不更新距離
        if (wheel.Equals(0)) return;

        // 更新距離
        float newDistance = currentDistance - wheel;

        // 檢查是否超出距離限制
        if (newDistance >= maxDistance || newDistance <= minDistance)
        {
            return;
        }

        currentDistance = newDistance;

        // 根據當前旋轉和新距離更新攝影機位置
        UpdateCameraPosition();
    }

    /// <summary>
    /// 處理攝影機視角旋轉
    /// </summary>
    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        // 更新旋轉角度
        currentYRotation -= (mouseX * -1);
        currentXRotation = Mathf.Clamp(currentXRotation - mouseY, minRotAngle, maxRotAngle);

        // 更新攝影機位置和旋轉
        UpdateCameraPosition();
    }

    /// <summary>
    /// 更新攝影機位置與旋轉
    /// </summary>
    private void UpdateCameraPosition()
    {
        // 計算旋轉
        Quaternion rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);

        // 計算攝影機位置：從目標位置沿相機前方方向偏移當前距離
        Vector3 direction = rotation * Vector3.back; // 使用 back 因為相機需要看向角色
        Vector3 position = Character.transform.position + direction * currentDistance;

        // 應用旋轉和位置
        CharacterCamera.transform.rotation = rotation;
        CharacterCamera.transform.position = position;
    }
}