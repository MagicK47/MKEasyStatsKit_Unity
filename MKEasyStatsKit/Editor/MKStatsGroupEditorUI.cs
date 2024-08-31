using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MKStatsGroup))]
public class MKStatsGroupEditorUI : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        MKStatsGroup statsmanager = (MKStatsGroup)target;



        #region 属性展示
        if (statsmanager.ParentGroup)
        {
            List<MKStats> mKStats = statsmanager.GetAllParentGroupStats();
            //mKStats.Reverse();
            MKStatsManagerEditorUI.ShowStatsInspectorGUI("Parent Group Stats", mKStats, false, statsmanager.ParentGroup);
        }
        MKStatsManagerEditorUI.ShowStatsInspectorGUI("Group Stats", statsmanager.AllStats, true, statsmanager);

        #endregion

        // 使场景数据发生变化，从而可以保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties(); // 应用属性的变更
    }
}
