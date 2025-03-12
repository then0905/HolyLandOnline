using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/12
//  創建用途: 常用資源的管理(加載、存取、卸載)
//==========================================
public static class CommonResourceManager
{
    /// <summary>初始化時的加載清單 <加載路徑,存取資源的字典> </summary>
    private static Dictionary<string, IDictionary> initLoadDic = new Dictionary<string, IDictionary>();
    /// <summary>讀取常用資源使用 依照資源路徑指向指定Dic並拿取資源</summary>
    public static Dictionary<string, IDictionary> InitLoadDic => initLoadDic;

    #region 存放資源的字典區

    /// <summary>將Resource裡的「技能圖示」資源做存取處理</summary>
    public static Dictionary<string, Sprite> SkillIconDic = new Dictionary<string, Sprite>();
    /// <summary>將Resource裡的「武器圖示」資源做存取處理</summary>
    public static Dictionary<string, Sprite> WeaponIconDic = new Dictionary<string, Sprite>();
    /// <summary>將Resource裡的「防具圖示」資源做存取處理</summary>
    public static Dictionary<string, Sprite> ArmorIconDic = new Dictionary<string, Sprite>();
    /// <summary>將Resource裡的「道具圖示」資源做存取處理</summary>
    public static Dictionary<string, Sprite> ItemIconDic = new Dictionary<string, Sprite>();

    #endregion

    // 建構函式 確保 SkillIconDic 先初始化
    static CommonResourceManager()
    {
        initLoadDic = new Dictionary<string, IDictionary>()
        {
            //技能圖片
            { GameConfig.SkillIcon, SkillIconDic },
            { GameConfig.WeaponIcon, WeaponIconDic },
            { GameConfig.ArmorIcon, ArmorIconDic },
            { GameConfig.ItemIcon, ItemIconDic }
        };
    }

    /// <summary>
    /// 初始化加載常用資源
    /// </summary>
    /// <returns></returns>
    public static async Task InitLoadResouce()
    {
        await LoadResourceTask();
    }

    private static async Task LoadResourceTask()
    {
        await LoadAllResources(initLoadDic);
        //加載完成
        Debug.Log("常用資源加載完成");
    }

    /// <summary>
    /// 加載資源
    /// </summary>
    /// <param name="initLoadDic">加載項目Dic</param>
    /// <returns></returns>
    private static async Task LoadAllResources(Dictionary<string, IDictionary> initLoadDic)
    {
        //建立所有Task
        List<Task> loadTasks = new List<Task>();

        //根據需要加載項目進行Task加載紀錄
        foreach (var dicData in initLoadDic)
        {
            //建立所有資源
            Object[] resources = Resources.LoadAll<Sprite>(dicData.Key);

            if (resources.Length == 0) return;

            foreach (Object resource in resources)
            {
                string assetPath = GetResourceFullPath(resource); // 記錄完整路徑
                loadTasks.Add(LoadResourceAsync(assetPath, dicData.Value));
            }
        }

        //回傳加載項目完畢
        await Task.WhenAll(loadTasks);
    }

    /// <summary>
    /// 非同步讀取需要加載的資源
    /// </summary>
    /// <param name="path">讀取路徑</param>
    /// <param name="targetDic">存放資源的Dic</param>
    /// <returns></returns>
    private static async Task LoadResourceAsync(string path, IDictionary targetDic)
    {
        ResourceRequest request = Resources.LoadAsync<Sprite>(path);
        while (!request.isDone)
        {
            await Task.Yield(); // 等一幀
        }

        if (request.asset != null)
        {
            targetDic[request.asset.name] = request.asset;
        }
        else
        {
            Debug.LogError($"資源 {path} 加載有問題");
        }
    }

    /// <summary>
    /// 獲取 Resources 內的完整資源路徑
    /// </summary>
    private static string GetResourceFullPath(Object resource)
    {
        string fullPath = GetPathInResources(resource);
        return fullPath;
    }

    /// <summary>
    /// 獲取資源在 Resources 內的相對路徑
    /// </summary>
    private static string GetPathInResources(Object obj)
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(obj);

        if (!string.IsNullOrEmpty(path))
        {
            int resourcesIndex = path.IndexOf("Resources/");
            if (resourcesIndex >= 0)
            {
                path = path.Substring(resourcesIndex + 10); // 去掉 "Resources/" 前綴
                path = path.Substring(0, path.LastIndexOf(".")); // 去掉 ".png" 等副檔名
                return path;
            }
        }
        return null;
    }
}
