using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//==========================================
//  創建者:家豪
//  創建日期:2025/03/12
//  創建用途: 常用資源的管理(加載、存取、卸載)
//==========================================
public static class CommonResourceManager
{
    /// <summary>初始化時的加載清單 <加載標籤,存取資源的字典> </summary>
    private static Dictionary<string, IDictionary> initLoadDic = new Dictionary<string, IDictionary>();

    /// <summary>讀取常用資源使用 依照標籤指向指定字典並拿取資源</summary>
    public static Dictionary<string, IDictionary> InitLoadDic => initLoadDic;

    #region 存放資源的字典區
    public static Dictionary<string, Sprite> SkillIconDic = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> WeaponIconDic = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> ArmorIconDic = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> ItemIconDic = new Dictionary<string, Sprite>();
    #endregion

    // 建構函式，初始化 Addressables 標籤對應字典
    static CommonResourceManager()
    {
        initLoadDic = new Dictionary<string, IDictionary>()
        {
            { GameConfig.SkillIcon, SkillIconDic },
            { GameConfig.WeaponIcon, WeaponIconDic },
            { GameConfig.ArmorIcon, ArmorIconDic },
            { GameConfig.ItemIcon, ItemIconDic }
        };
    }

    /// <summary>
    /// 初始化加載常用資源
    /// </summary>
    public static async Task InitLoadResource()
    {
        await LoadAllResources(initLoadDic);
        Debug.Log("常用資源加載完成");
    }

    /// <summary>
    /// 使用 Addressables 加載所有標籤對應的資源
    /// </summary>
    private static async Task LoadAllResources(Dictionary<string, IDictionary> initLoadDic)
    {
        List<Task> loadTasks = new List<Task>();

        foreach (var dicData in initLoadDic)
        {
            string label = dicData.Key; // Addressables 標籤
            IDictionary targetDic = dicData.Value; // 存放資源的字典
            loadTasks.Add(LoadResourceWithAddressables(label, targetDic));
        }

        await Task.WhenAll(loadTasks);
    }

    /// <summary>
    /// 使用 Addressables 非同步加載資源
    /// </summary>
    private static async Task LoadResourceWithAddressables(string label, IDictionary targetDic)
    {
        AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetsAsync<Sprite>(label, null);

        await handle.Task; // 等待加載完成

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var sprite in handle.Result)
            {
                targetDic[sprite.name] = sprite;
            }
            Debug.Log($"[{label}] 加載完成，共 {handle.Result.Count} 個資源");
        }
        else
        {
            Debug.LogError($"[{label}] 資源加載失敗");
        }
    }

    /// <summary>
    /// 卸載所有資源
    /// </summary>
    public static void ReleaseAllResources()
    {
        foreach (var dicData in initLoadDic)
        {
            string label = dicData.Key;
            Addressables.Release(label);
        }
        Debug.Log("所有 Addressables 資源已卸載");
    }
}
