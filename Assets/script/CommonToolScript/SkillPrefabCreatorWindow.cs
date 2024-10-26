using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public enum SkillPrefabType
{
    Skill_Base_Attack_Single,        // 普通攻擊
    Skill_Base_Attack_Mutiple,       // 範圍攻擊
    Skill_Base_Buff_Passive,         // 被動技能
    Skill_Base_Buff_SkillUpgrade,    // 強化指定技能的被動技能
    Skill_Base_Buff_Continuance,     // 主動型buff
}

[AddComponentMenu("HLO_Tool/技能預製物快速建置工具")]
public class SkillPrefabCreatorWindow : EditorWindow
{
    private string jobID = "";
    private int skillCount = 1;
    private Vector2 scrollPosition;
    private string prefabPath = "Assets/Prefabs/SkillPrefab";
    private bool showSkillFields = false;

    // 保存每個技能的類型和ID
    private List<SkillEntry> skillEntries = new List<SkillEntry>();

    private class SkillEntry
    {
        public SkillPrefabType skillType;
        public string skillID;
    }

    [MenuItem("HLO_Tool/技能預製物生成工具")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<SkillPrefabCreatorWindow>("技能預製物生成工具");
        window.minSize = new Vector2(300, 400);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("技能預製物生成設置", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Job ID輸入
        EditorGUILayout.LabelField("Job ID", EditorStyles.boldLabel);
        jobID = EditorGUILayout.TextField("輸入Job ID", jobID);

        GUILayout.Space(10);

        // 技能數量輸入
        EditorGUILayout.LabelField("技能數量", EditorStyles.boldLabel);
        skillCount = EditorGUILayout.IntField("輸入技能數量", skillCount);
        skillCount = Mathf.Max(1, skillCount); // 確保數量至少為1

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // 生成技能欄位的按鈕
        if (GUILayout.Button("生成技能欄位", GUILayout.Height(30)))
        {
            showSkillFields = true;
            // 初始化或調整技能條目列表
            while (skillEntries.Count < skillCount)
            {
                skillEntries.Add(new SkillEntry
                {
                    skillType = SkillPrefabType.Skill_Base_Attack_Single,
                    skillID = $"{jobID}_{skillEntries.Count}"
                });
            }
            while (skillEntries.Count > skillCount)
            {
                skillEntries.RemoveAt(skillEntries.Count - 1);
            }
        }

        // 顯示技能欄位
        if (showSkillFields)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("技能設置", EditorStyles.boldLabel);

            for (int i = 0; i < skillEntries.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"技能 {i + 1}", EditorStyles.boldLabel);

                // 技能類型選擇
                skillEntries[i].skillType = (SkillPrefabType)EditorGUILayout.EnumPopup(
                    "技能類型",
                    skillEntries[i].skillType
                );

                // 技能ID顯示（自動生成）
                skillEntries[i].skillID = $"{jobID}_{i+1}";
                EditorGUILayout.LabelField($"技能ID: {skillEntries[i].skillID}");

                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }

            // 生成所有技能預製物的按鈕
            if (GUILayout.Button("生成所有技能預製物", GUILayout.Height(30)))
            {
                CreateAllSkillPrefabs();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateAllSkillPrefabs()
    {
        if (string.IsNullOrEmpty(jobID))
        {
            EditorUtility.DisplayDialog("錯誤", "請輸入Job ID", "確定");
            return;
        }

        foreach (var entry in skillEntries)
        {
            CreateSkillPrefab(entry);
        }
    }

    private void CreateSkillPrefab(SkillEntry entry)
    {
        string[] prefabFiles = Directory.GetFiles(prefabPath, "SkillEffect_Mod.prefab", SearchOption.AllDirectories);
        GameObject obj = null;

        foreach (string prefabPath in prefabFiles)
        {
            string unityPath = prefabPath.Replace('\\', '/');
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);

            if (prefab != null)
            {
                obj = Instantiate(prefab);
                break;
            }
        }

        if (obj != null)
        {
            SetupSkillProperties(obj, entry);
        }
        else
        {
            Debug.LogError("找不到模板預製物：SkillEffect_Mod.prefab");
        }
    }

    private void SetupSkillProperties(GameObject obj, SkillEntry entry)
    {
        Skill_Base skillbase;

        switch (entry.skillType)
        {
            case SkillPrefabType.Skill_Base_Attack_Single:
                skillbase = obj.AddComponent<Skill_Base_Attack_Single>();
                break;
            case SkillPrefabType.Skill_Base_Attack_Mutiple:
                skillbase = obj.AddComponent<Skill_Base_Attack_Mutiple>();
                break;
            case SkillPrefabType.Skill_Base_Buff_Passive:
                skillbase = obj.AddComponent<Skill_Base_Buff_Passive>();
                break;
            case SkillPrefabType.Skill_Base_Buff_SkillUpgrade:
                skillbase = obj.AddComponent<Skill_Base_Buff_SkillUpgrade>();
                break;
            case SkillPrefabType.Skill_Base_Buff_Continuance:
                skillbase = obj.AddComponent<Skill_Base_Buff_Continuance>();
                break;
            default:
                skillbase = null;
                break;
        }

        if (skillbase != null)
        {
            FieldInfo fieldInfo = typeof(Skill_Base).GetField("skillID",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(skillbase, entry.skillID);
            }
            skillbase.gameObject.name = "SkillEffect_" + entry.skillID;
            EditorUtility.SetDirty(obj);
        }
    }
}