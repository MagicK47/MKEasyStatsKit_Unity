using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using UnityEngine.Events;


public class MKStatsSerializableData
{
	public string StatsID;

    public float InitDMinValue;
	public float InitDValue;
	public float InitDMaxValue;

	public float curBaseValue, curBaseMin, curBaseMax;
	public float curValue, curMin, curMax;

	public List<string> ActiveModifiers = new List<string>();
}

[Serializable]
public class MKStats
{
	public MKStatsInfo mKStatsInfo;

	public float InitDMinValue;
	public float InitDValue;
	public float InitDMaxValue;

	// Events
	public UnityEvent<float, float> onBaseMinValueChanged = new UnityEvent<float, float>();
	public UnityEvent<float, float> onBaseValueChanged = new UnityEvent<float, float>();
	public UnityEvent<float, float> onBaseMaxValueChanged = new UnityEvent<float, float>();
	public UnityEvent<float, float> onMinValueChanged = new UnityEvent<float, float>();
	public UnityEvent<float, float> onValueChanged = new UnityEvent<float, float>();
	public UnityEvent<float, float> onMaxValueChanged = new UnityEvent<float, float>();
	public UnityEvent<MKStatModifier> OnAddModifier=new UnityEvent<MKStatModifier>();
	public UnityEvent<MKStatModifier> OnRemoveModifier = new UnityEvent<MKStatModifier>();

    // Internal current values
    float curBaseValue, curBaseMin, curBaseMax;
	float curValue, curMin, curMax;










	public IReadOnlyList<MKStatModifier> NowActiveModifiers => ActiveModifiers.AsReadOnly();

	/// <summary>
	/// Returns a list of active modifiers on this Stat
	/// </summary>
	List<MKStatModifier> ActiveModifiers;



	public MKStats()
	{
		SetResInit();
		// Finalize

	}

	public MKStats(MKStatsInfo mKStatsInfo) : this()
	{
		this.mKStatsInfo = mKStatsInfo;
	}
	public MKStats(float baseValue, float minv, float maxv, MKStatsInfo mKStatsInfo) : this()
	{
		this.mKStatsInfo = mKStatsInfo;
		SetDefaultData(baseValue, minv, maxv);
	}


	// Clone method to create a copy of the current instance
	public MKStats CloneNew()
	{
        MKStats mKStats= new MKStats();
        mKStats.CloneFromOtherData(this);
        return mKStats;
    }
	public void CloneFromOtherData(MKStats other)
	{
		InitDMinValue = other.InitDMinValue;
		InitDValue = other.InitDValue;
		InitDMaxValue = other.InitDMaxValue;
		curBaseValue = other.curBaseValue;
		curBaseMin = other.curBaseMin;
		curBaseMax = other.curBaseMax;
		curValue = other.curValue;
		curMin = other.curMin;
		curMax = other.curMax;


		mKStatsInfo = other.mKStatsInfo;

		ActiveModifiers = new List<MKStatModifier>();
		for (int i = 0; i < other.ActiveModifiers.Count; i++)
		{
			ActiveModifiers.Add(other.ActiveModifiers[i].CloneNew(this));

		}
		ReCalcCurrentAllValue();
	}
    public string GetSaveJsonData()
	{
		MKStatsSerializableData mKStatsSerializableData = new MKStatsSerializableData();
		mKStatsSerializableData.StatsID = mKStatsInfo.StatsID;
        mKStatsSerializableData.InitDMinValue = this.InitDMinValue;
		mKStatsSerializableData.InitDValue = this.InitDValue;
		mKStatsSerializableData.InitDMaxValue = this.InitDMaxValue;
		mKStatsSerializableData.curBaseMin = this.curBaseMin;
		mKStatsSerializableData.curBaseValue = this.curBaseValue;
		mKStatsSerializableData.curBaseMax = this.curBaseMax;
		mKStatsSerializableData.curMin = this.curMin;
		mKStatsSerializableData.curValue = this.curValue;
		mKStatsSerializableData.curMax = this.curMax;

		for (int i = 0; i < ActiveModifiers.Count; i++)
		{

			mKStatsSerializableData.ActiveModifiers.Add(ActiveModifiers[i].GetSaveJsonData());

		}
		return JsonUtility.ToJson(mKStatsSerializableData);
	}
	public static MKStats LoadFromSerializableData(MKStatsSerializableData mKStatsSerializableData, MKStatsInfo mKStatsInfo)
	{
		MKStats mKStats = new MKStats(mKStatsSerializableData.InitDValue, mKStatsSerializableData.InitDMinValue, mKStatsSerializableData.InitDMaxValue, mKStatsInfo);

		mKStats.InitDMinValue = mKStatsSerializableData.InitDMinValue;
		mKStats.InitDValue = mKStatsSerializableData.InitDValue;
		mKStats.InitDMaxValue = mKStatsSerializableData.InitDMaxValue;
		mKStats.curBaseMin = mKStatsSerializableData.curBaseMin;
		mKStats.curBaseValue = mKStatsSerializableData.curBaseValue;
		mKStats.curBaseMax = mKStatsSerializableData.curBaseMax;
		mKStats.curMin = mKStatsSerializableData.curMin;
		mKStats.curValue = mKStatsSerializableData.curValue;
		mKStats.curMax = mKStatsSerializableData.curMax;

		mKStats.ActiveModifiers.Clear();

		for (int i = 0; i < mKStatsSerializableData.ActiveModifiers.Count; i++)
		{

			mKStats.ActiveModifiers.Add(MKStatModifier.LoadFromJsonData(mKStatsSerializableData.ActiveModifiers[i], mKStats));

		}
		mKStats.ReCalcCurrentAllValue();
		return mKStats;
	}

	public static MKStats LoadFromJsonData(string jsondata, MKStatsInfo mKStatsInfo)
	{
		MKStatsSerializableData mKStatsSerializableData = JsonUtility.FromJson<MKStatsSerializableData>(jsondata);
		return LoadFromSerializableData(mKStatsSerializableData, mKStatsInfo);
	}
	public void SetDefaultData(float baseValue, float minv, float maxv)
	{
		InitDValue = baseValue;
		InitDMinValue = minv;
		InitDMaxValue = maxv;
		ReCalcCurrentAllValue();
	}

	public void SetResInit()
	{
		ActiveModifiers = new List<MKStatModifier>();
		SetNowBaseMaximum(InitDMaxValue);
		SetNowBaseMinimum(InitDMinValue);
		SetNowBaseValue(InitDValue);
		ReCalcCurrentAllValue();
	}

	public void SetNowBaseMaximum(float value)
	{
		if (curBaseMax == value) return;

		float oldValue = curBaseMax;
		curBaseMax = value;
		onBaseMaxValueChanged?.Invoke(oldValue, value);
		ReCalcCurrentMax();
		ReCalcCurrentValue();
	}

	public void SetNowBaseMinimum(float value)
	{
		if (curBaseMin == value) return;

		float oldValue = curBaseMin;
		curBaseMin = value;
		onBaseMinValueChanged?.Invoke(oldValue, value);
		ReCalcCurrentMin();
		ReCalcCurrentValue();
	}

	public void SetNowBaseValue(float value)
	{
		if (curBaseValue == value) return;

		float oldValue = curBaseValue;
		value = Mathf.Clamp(value, GetNowMinValue(), GetNowMaxValue());
		curBaseValue = value;
		onBaseValueChanged?.Invoke(oldValue, value);
		ReCalcCurrentValue();
	}

	public void SetToNowMaximum()
	{
		SetNowBaseValue(GetNowMaxValue());
	}
	public float GetNowBaseValue()
	{
		return curBaseValue;
	}
	public float GetNowBaseMinValue()
	{

		return curBaseMin;
	}

	public float GetNowBaseMaxValue()
	{

		return curBaseMax;
	}
	public float GetNowValue()
	{
		return curValue;
	}
	public float GetNowMinValue()
	{

		return curMin;
	}

	public float GetNowMaxValue()
	{

		return curMax;
	}
	private void ReCalcCurrentAllValue()
	{
		ReCalcCurrentMax();
		ReCalcCurrentMin();
		ReCalcCurrentValue();
	}

	private void ReCalcCurrentMax()
	{
		ActiveModifiers.Sort(CompareModifierOrder);

		float v = curBaseMax;
		foreach (MKStatModifier mod in ActiveModifiers)
		{

			if (mod.TargetType == MKStatModTargetType.MaxValue)
			{
				v = mod.GetSustainedValue(v, this);
			}

		}
		float endv = (float)Math.Round(v, 4);

		if (endv == curMax) { return; }
		float oldv = curMax;
		curMax = endv;
		onMaxValueChanged?.Invoke(oldv, curMax);
	}

	private void ReCalcCurrentMin()
	{
		ActiveModifiers.Sort(CompareModifierOrder);

		float v = curBaseMin;
		foreach (MKStatModifier mod in ActiveModifiers)
		{
			if (mod.TargetType == MKStatModTargetType.MinValue)
			{
				v = mod.GetSustainedValue(v, this);
			}
		}
		float endv = (float)Math.Round(v, 4);

		if (endv == curMin) { return; }
		float oldv = curMin;
		curMin = endv;
		onMinValueChanged?.Invoke(oldv, curMin);
	}


	private void ReCalcCurrentValue()
	{
		ActiveModifiers.Sort(CompareModifierOrder);

		float v = curBaseValue;
		foreach (MKStatModifier mod in ActiveModifiers)
		{
			if (mod.TargetType == MKStatModTargetType.Value)
			{
				v = mod.GetSustainedValue(v, this);
			}
		}
		float endv = (float)Math.Round(v, 4);
		endv = Mathf.Clamp(endv, GetNowMinValue(), GetNowMaxValue());
		if (endv == curValue) { return; }
		float oldv = curValue;
		curValue = endv;
		onValueChanged?.Invoke(oldv, curValue);
	}


	public virtual void AddModifier(MKStatModifier mod)
	{
        mod.SetNowAppendMKStats(this);
        ActiveModifiers.Add(mod);
		ReCalcCurrentAllValue();
		OnAddModifier?.Invoke(mod);
	}

	public virtual bool RemoveModifier(MKStatModifier mod)
	{
		if (ActiveModifiers.Remove(mod))
		{
			ReCalcCurrentAllValue();
			OnRemoveModifier?.Invoke(mod);
			return true;
		}
		return false;
	}

	public virtual bool RemoveAllModifiersFromSource(string sourceKey)
	{

		var allremove = ActiveModifiers.FindAll(mod => mod.SourceKey == sourceKey);

		if (allremove.Count > 0)
		{
			for (int i = 0; i < allremove.Count; i++)
			{
				RemoveModifier(allremove[i]);
			}
			return true;
		}
		return false;
	}

	protected virtual int CompareModifierOrder(MKStatModifier a, MKStatModifier b)
	{
		if (a.Order < b.Order)
			return -1;
		else if (a.Order > b.Order)
			return 1;
		return 0; //if (a.Order == b.Order)
	}


}
