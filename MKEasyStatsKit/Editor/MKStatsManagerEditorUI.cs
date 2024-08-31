using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MKStatsManager))]
public class MKStatsManagerEditorUI : Editor
{    // 用于跟踪折叠状态的字典
    public static Dictionary<MKStats, bool> foldouts = new Dictionary<MKStats, bool>();

    // 用于跟踪折叠状态的字典
    public static Dictionary<MKStats, bool> statsDelogfoldouts = new Dictionary<MKStats, bool>();

    public static List<MKStats> NowCopyStatsList=new List<MKStats>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        MKStatsManager statsmanager = (MKStatsManager)target;
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);

       // List<MKStats> AllStats = statsmanager.AllStats;
       // List<MKStats> GroupAllStats = null;
        bool CanEditStatsList = true;
        if (statsmanager.UseStatsGroup != null)
        {
            if (!Application.isPlaying)
            {
                //AllStats = statsmanager.UseStatsGroup.AllStats;

               // CanEditStatsList = false;

                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.fontSize = 15;  // 设置字体大小
                labelStyle.alignment = TextAnchor.MiddleCenter;  // 设置文字居中
                GUI.color = Color.green;
                EditorGUILayout.LabelField("UseGroup:" + (string.IsNullOrEmpty(statsmanager.UseStatsGroup.GroupName) ? statsmanager.UseStatsGroup.name : statsmanager.UseStatsGroup.GroupName), labelStyle);
                GUI.color = Color.white;


                if (GUILayout.Button("EditGroup"))
                {
                    // 聚焦项目窗口
                    EditorUtility.FocusProjectWindow();

                    // 设置当前选中对象
                    Selection.activeObject = statsmanager.UseStatsGroup;
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("RemoveGroup"))
                {
                    statsmanager.UseStatsGroup = null;
                }
                GUI.backgroundColor = Color.white;
       
            }

        }
        if (statsmanager.UseStatsGroup == null)
        {
            if (GUILayout.Button("NewStatsGroup"))
            {
                // 创建新文件的代码逻辑
                string path = EditorUtility.SaveFilePanelInProject(
                    "Create New Stats Group",
                    "New Stats Group",
                    "asset",
                    "Please enter a file name"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    // 创建新的 ScriptableObject 实例
                    MKStatsGroup newStatsGroup = ScriptableObject.CreateInstance<MKStatsGroup>();
                    AssetDatabase.CreateAsset(newStatsGroup, path);
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = newStatsGroup;

                    // 更新属性
                    statsmanager.UseStatsGroup = newStatsGroup;
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            //AllStats = statsmanager.AllStats;
            //CanEditStatsList = true;
        }
        if (Application.isPlaying) { CanEditStatsList = false; }

        #region 属性展示
        if (statsmanager.UseStatsGroup && !Application.isPlaying)
        {
            List<MKStats> mKStats = statsmanager.UseStatsGroup.GetAllGroupStats(true);

           // mKStats.Reverse();
            ShowStatsInspectorGUI("Group Stats", mKStats, false, statsmanager.UseStatsGroup);
        }
        ShowStatsInspectorGUI("Stats", statsmanager.AllStats, CanEditStatsList, statsmanager);
        #endregion

        // 使场景数据发生变化，从而可以保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties(); // 应用属性的变更
    }
    // 绘制UI线的方法
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2; // 少量向左扩展，以更好地适应布局
        r.width += 6; // 少量向右扩展，以更好地适应布局
        EditorGUI.DrawRect(r, color);
    }


     

    public static void ShowStatsInspectorGUI(string titname,List<MKStats> AllStats,bool CanEditStatsList,UnityEngine.Object sendstatsm) {

        MKStatsManager statsmanager = sendstatsm as MKStatsManager;
        MKStatsGroup statsGroup = sendstatsm as MKStatsGroup;
        UnityEngine.Object targetm = statsmanager;
        if (statsmanager == null) { targetm = statsGroup; }

        if (CanEditStatsList)
        {
            if (NowCopyStatsList != null && NowCopyStatsList.Count > 0)
            {
                if (GUILayout.Button("Paste Stats List"))
                {
                    if (NowCopyStatsList != null && NowCopyStatsList.Count > 0)
                    {

                        // 显示对话框，并根据用户选择的结果执行不同的操作
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Paste Stats List", // 标题
                            "Are you sure you want to replace the current stats list?", // 提示信息
                            "Yes", // 确认按钮
                            "No"  // 取消按钮
                        );

                        if (confirmed)
                        {
                            // 记录更改以供撤销
                            Undo.RecordObject(targetm, "Paste Stats");


                            AllStats.Clear();
                            AllStats.AddRange(NowCopyStatsList);

                            // 标记对象已更改，以便编辑器知道保存它
                            EditorUtility.SetDirty(targetm);
                        }
                        else
                        {
                           
                        }
           
                    }
                }

            }
        }

        if (AllStats != null && AllStats.Count > 0)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            GUILayout.Space(20);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = 20;  // 设置字体大小
            labelStyle.alignment = TextAnchor.MiddleCenter;  // 设置文字居中
           // EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("--"+ titname + "--", labelStyle);
      

            //EditorGUILayout.EndHorizontal();
            // 垂直空间（顶部）
            DrawUILine(Color.gray);
            GUILayout.Space(10);
            for (int i = 0; i < AllStats.Count; i++)
            {

                var child = AllStats[i];
                if (child == null) continue;

                //string key = child.GetInstanceID().ToString();
                // Debug.Log(key+"/"+ foldouts.Count);
                if (!foldouts.ContainsKey(child))
                {
                    foldouts[child] = false;
                }
                if (!statsDelogfoldouts.ContainsKey(child))
                {
                    statsDelogfoldouts[child] = false;
                }
                EditorGUILayout.BeginHorizontal();
                // EditorGUILayout.ObjectField(child, typeof(MKQuestTaskGroup), false);

                GUI.backgroundColor = Color.blue;

                // 使用按钮代替折叠三角形
                string buttonLabel = foldouts[child] ? "▼" : "▶";
                if (GUILayout.Button(buttonLabel, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    foldouts[child] = !foldouts[child]; // 切换展开折叠状态
                }
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 15;

                /*
                if (child.mKStatsInfo)
                {
                    // 获取精灵的纹理
                    Sprite sprite = child.mKStatsInfo.GetShowIcon();
                    if (sprite != null)
                    {
                        // 通过计算得到纹理的矩形区域
                        //Rect spriteRect = sprite.rect;
                        //float aspectRatio = spriteRect.width / spriteRect.height;
                        //float previewHeight = 30; // 设置固定高度
                       // float previewWidth = previewHeight * aspectRatio;

                        // 绘制纹理
                        GUILayout.Box(sprite.texture, GUILayout.Width(30), GUILayout.Height(30));
                    }
                }*/

                string showdatav = "";
                GUI.color = child.mKStatsInfo ? Color.white : Color.red;
                if (GUILayout.Button(child.mKStatsInfo ? child.mKStatsInfo.GetShowName() + showdatav : "Stat" + i + showdatav+"(No StatInfo)", labelStyle, GUILayout.Height(30)))
                {
                    foldouts[child] = !foldouts[child]; // 切换展开折叠状态
                }
                GUI.color = Color.white;
              //EditorGUILayout.LabelField(string.IsNullOrEmpty(child.GroupName)? ("任务阶段" + (i+1)) : "阶段"+(i+1)+":"+ child.GroupName, labelStyle);



              GUI.backgroundColor = Color.red;
                GUI.enabled = CanEditStatsList;
                if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
                {
                    
                    // 记录更改以供撤销
                    Undo.RecordObject(targetm, "Remove Stat");

                    if (statsmanager) { statsmanager.RemoveOneStat(i); }
                    if (statsGroup) { statsGroup.RemoveOneStat(i); }

                    // 标记对象已更改，以便编辑器知道保存它
                    EditorUtility.SetDirty(targetm);

                    return; // 防止修改后继续迭代
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();


                GUI.backgroundColor = Color.white;
                if (foldouts[child])
                {
                    // EditorGUILayout.LabelField("属性信息");
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("GameObject Reference"));
                    GUI.enabled = CanEditStatsList;
                    if (child.mKStatsInfo == null)
                    {
                        EditorGUILayout.HelpBox("Need a StatInfo File!", MessageType.Error);


                        if (GUILayout.Button("NewStatInfo"))
                        {
                            // 创建新文件的代码逻辑
                            string path = EditorUtility.SaveFilePanelInProject(
                                "Create NewStatInfo",
                                "New Stat Info",
                                "asset",
                                "Please enter a file name"
                            );

                            if (!string.IsNullOrEmpty(path))
                            {
                                // 创建新的 ScriptableObject 实例
                                MKStatsInfo statinfo = ScriptableObject.CreateInstance<MKStatsInfo>();
                                AssetDatabase.CreateAsset(statinfo, path);
                                AssetDatabase.SaveAssets();
                                EditorUtility.FocusProjectWindow();
                                Selection.activeObject = statinfo;

                                // 更新属性
                                child.mKStatsInfo = statinfo;
                                EditorUtility.SetDirty(targetm);
                            }
                        }
                    }
                    child.mKStatsInfo = (MKStatsInfo)EditorGUILayout.ObjectField("StatInfo", child.mKStatsInfo, typeof(MKStatsInfo), false);
                    child.InitDMinValue = EditorGUILayout.FloatField("Default_MinValue", child.InitDMinValue);
                    child.InitDValue = EditorGUILayout.FloatField("Default_CurrentValue", child.InitDValue);
                    child.InitDMaxValue = EditorGUILayout.FloatField("Default_MaxValue", child.InitDMaxValue);

                  

                    GUI.enabled = true;
                    if (Application.isPlaying && !statsGroup)
                    {
                        Color isdebug = statsDelogfoldouts[child] ? Color.green : Color.gray;
                        GUI.backgroundColor = isdebug;

                        if (GUILayout.Button("Log", GUILayout.Width(40), GUILayout.Height(30)))
                        {
                            statsDelogfoldouts[child] = !statsDelogfoldouts[child]; // 切换展开折叠状态
                        }

                        if (statsDelogfoldouts[child])
                        {

                            // 暂时将 GUI 设置为不可编辑
                            GUI.enabled = false;
                            EditorGUILayout.FloatField("Runtime_MinValue", child.GetNowMinValue());
                            EditorGUILayout.FloatField("Runtime_CurrentValue", child.GetNowValue());
                            EditorGUILayout.FloatField("Runtime_MaxValue", child.GetNowMaxValue());
                            GUILayout.Space(5);
                            if (child.NowActiveModifiers.Count > 0)
                            {
                                labelStyle.fontStyle = FontStyle.Bold;
                                labelStyle.fontSize = 15;  // 设置字体大小
                                labelStyle.alignment = TextAnchor.MiddleLeft;  // 设置文字居中
                                EditorGUILayout.LabelField("--ActiveModifiers(" + child.NowActiveModifiers.Count + ")--", labelStyle);
                            }
                            for (int m = 0; m < child.NowActiveModifiers.Count; m++)
                            {
                                if (m>= child.NowActiveModifiers.Count) { continue; }
                                GUI.color = child.NowActiveModifiers[m].Value > 0 ? Color.green : Color.red;
                                labelStyle.fontStyle = FontStyle.Normal;
                                labelStyle.alignment = TextAnchor.MiddleLeft;
                                labelStyle.fontSize = 7;
                                string showinfo = "";
                                if (child.NowActiveModifiers[m].Type == MKStatModType.Add)
                                {
                                    showinfo = (child.NowActiveModifiers[m].Value > 0 ? "+ " : " ") + child.NowActiveModifiers[m].Value;

                                }
                                if (child.NowActiveModifiers[m].Type == MKStatModType.PercentAdd)
                                {
                                    showinfo = (child.NowActiveModifiers[m].Value > 0 ? "+ " : " ") + (child.NowActiveModifiers[m].Value * 100) + "%";
                                }
                                if (child.NowActiveModifiers[m].Type == MKStatModType.PercentMult)
                                {
                                    showinfo = "x " + (child.NowActiveModifiers[i].Value * 100) + "%";
                                }
                                string addinfo = "";
                                if (child.NowActiveModifiers[m].LastAppendMKStats!=null && child.NowActiveModifiers[m].LastAppendMKStats.mKStatsInfo) { addinfo = child.NowActiveModifiers[m].LastAppendMKStats.mKStatsInfo.GetShowName()+ " "; }
                                EditorGUILayout.LabelField(child.NowActiveModifiers[m].SourceKey+": "+ addinfo+ showinfo + " " + child.NowActiveModifiers[m].TargetType.ToString());
                                GUI.color = Color.white;
                            }
                            // 恢复 GUI 编辑状态
                            GUI.enabled = true;
                        }
                        GUI.backgroundColor = Color.white;
                    }
                }


                // 标记对象已更改
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(targetm);
                }

                DrawUILine(Color.gray);
            }
            if (CanEditStatsList)
            {
                if (GUILayout.Button("Copy All Stats"))
                {
                    NowCopyStatsList.Clear();
                    NowCopyStatsList.AddRange(AllStats);
                }
            }
        }
        GUILayout.Space(5);

        if (CanEditStatsList) {
            if (GUILayout.Button("New Stat", GUILayout.Height(20)))
            {
                MKStats child = null;
                if (statsmanager) { child = statsmanager.AddOneNewStat(null); }
                if (statsGroup) { child = statsGroup.AddOneNewStat(null); }

                EditorUtility.SetDirty(targetm);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();

                foldouts[child] = true;
                // Selection.activeObject = child;
            }
            GUILayout.Space(20);
        }

   
    }

  
}
