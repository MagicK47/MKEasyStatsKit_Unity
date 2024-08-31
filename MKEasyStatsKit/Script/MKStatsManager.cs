using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MKStatsManager : MonoBehaviour
{

    [HideInInspector]
    public List<MKStats> AllStats = new List<MKStats>();
    public bool AutoInit = true;
    public MKStatsGroup UseStatsGroup;
    public UnityEvent OnInitEnd;
    bool IsSetInit;




    private void Awake()
    {
        if (UseStatsGroup)
        {
            List<MKStats> AllGroupStats = UseStatsGroup.GetAllGroupStats(true);
            for (int i = 0; i < AllGroupStats.Count; i++)
            {
                AllStats.Add(AllGroupStats[i].CloneNew());
            }
        }
        for (int i = 0; i < AllStats.Count; i++)
        {
            if (AllStats[i].mKStatsInfo == null)
            {
                AllStats[i].mKStatsInfo = ScriptableObject.CreateInstance<MKStatsInfo>();
                AllStats[i].mKStatsInfo.StatsID = gameObject.name + "statid" + i;
                AllStats[i].mKStatsInfo.StatsDfName = "stat" + i;
            }
        }
        if (AutoInit)
        {
            SetInit();
        }
    }




    public void SetInit()
    {
        if (IsSetInit) { return; }
        IsSetInit = true;
        for (int i = 0; i < AllStats.Count; i++)
        {
            AllStats[i].SetResInit();
        }
        OnInitEnd?.Invoke();
    }

    public MKStats AddOneNewStat(MKStatsInfo mKStatsInfo)
    {
        if (AllStats == null) { AllStats = new List<MKStats>(); }
        MKStats mKStats = new MKStats(10, 0, 10, mKStatsInfo);
        AllStats.Add(mKStats);
        return mKStats;
    }

    public void RemoveOneStat(int id)
    {
        AllStats.RemoveAt(id);
    }
    public void RemoveOneStat(MKStats stats)
    {
        AllStats.Remove(stats);
    }

    public MKStats FindValidationStat(MKStatsInfo statsinfo)
    {
        if (AllStats == null) return null;

        foreach (MKStats stat in AllStats)
        {
            if (stat.mKStatsInfo && stat.mKStatsInfo == statsinfo)
            {
                return stat;
            }
            if (stat.mKStatsInfo && stat.mKStatsInfo.StatsID == statsinfo.StatsID)
            {
                return stat;
            }
        }

        return null;
    }

    public MKStats FindValidationStat(string statId)
    {
        if (AllStats == null) return null;

        foreach (MKStats stat in AllStats)
        {
            if (stat.mKStatsInfo && stat.mKStatsInfo.StatsID == statId)
            {
                return stat;
            }
        }

        return null;
    }

    public void GetNowAllModifier(List<MKStatModifier> OutAllModifierList)
    {
        if (OutAllModifierList == null) { OutAllModifierList = new List<MKStatModifier>(); } else { OutAllModifierList.Clear(); }
        for (int i = 0; i < AllStats.Count; i++)
        {

            if (AllStats[i] != null) { OutAllModifierList.AddRange(AllStats[i].NowActiveModifiers); }
        }

    }

    public void AddModToStats(string statId, MKStatModifier mKStatModifier)
    {
        MKStats mKStats = FindValidationStat(statId);
        if (mKStats != null) { mKStats.AddModifier(mKStatModifier); }
    }

    public void AddModToStats(MKStatsInfo statsinfo, MKStatModifier mKStatModifier)
    {
        MKStats mKStats = FindValidationStat(statsinfo);
        if (mKStats != null) { mKStats.AddModifier(mKStatModifier); }
    }

    public void RemoveModToStats(string statId, MKStatModifier mKStatModifier)
    {
        MKStats mKStats = FindValidationStat(statId);
        if (mKStats != null) { mKStats.RemoveModifier(mKStatModifier); }
    }

    public void RemoveModToStats(MKStatsInfo statsinfo, MKStatModifier mKStatModifier)
    {
        MKStats mKStats = FindValidationStat(statsinfo);
        if (mKStats != null) { mKStats.RemoveModifier(mKStatModifier); }
    }
    public void RemoveModToStatsFromSource(MKStatsInfo statsinfo, string sourceKey)
    {
        MKStats mKStats = FindValidationStat(statsinfo);
        if (mKStats != null) { mKStats.RemoveAllModifiersFromSource(sourceKey); }
    }
    public void RemoveModToStatsFromSource(string statsid, string sourceKey)
    {
        MKStats mKStats = FindValidationStat(statsid);
        if (mKStats != null) { mKStats.RemoveAllModifiersFromSource(sourceKey); }
    }
    public void RemoveAllModToStatsFromSource(string sourceKey)
    {
        if (AllStats == null) { return; }
        foreach (MKStats stat in AllStats)
        {
            if (stat != null) { stat.RemoveAllModifiersFromSource(sourceKey); }
        }

    }



    public string GetSaveJsonData()
    {
        MKStatsManagerSerializableData mKStatsManagerSerializableData = new MKStatsManagerSerializableData();
        for (int i = 0; i < AllStats.Count; i++)
        {

            mKStatsManagerSerializableData.AllStats.Add(AllStats[i].GetSaveJsonData());
        }
        return JsonUtility.ToJson(mKStatsManagerSerializableData);
    }

    public void LoadSaveDataFromJson(string jsondata)
    {
        MKStatsManagerSerializableData mKStatsManagerSerializableData = JsonUtility.FromJson<MKStatsManagerSerializableData>(jsondata);
        for (int i = 0; i < mKStatsManagerSerializableData.AllStats.Count; i++)
        {
            MKStatsSerializableData mKStatsSerializableData = JsonUtility.FromJson<MKStatsSerializableData>(mKStatsManagerSerializableData.AllStats[i]);
            for (int a = 0; a < AllStats.Count; a++)
            {
                if (AllStats[a].mKStatsInfo && AllStats[a].mKStatsInfo.StatsID == mKStatsSerializableData.StatsID)
                {
                    AllStats[a].CloneFromOtherData(MKStats.LoadFromSerializableData(mKStatsSerializableData, AllStats[a].mKStatsInfo));
                    break;
                }
            }
        }
    }
}

public class MKStatsManagerSerializableData
{
    public List<string> AllStats = new List<string>();
}
