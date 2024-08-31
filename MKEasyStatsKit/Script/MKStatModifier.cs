using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MKStatModifierSerializableData {
    public float Value;
    public MKStatModType Type;
    public MKStatModTargetType TargetType;
    public int Order;

    public string SourceKey;
  //  public string NowMyMKStats;
}

public enum MKStatModType
{
	Add = 100,//addition 
    PercentAdd = 200,//Add by percentage
    PercentMult = 300,//Multiplication by percentage
}
public enum MKStatModTargetType
{
	Value,
	MaxValue,
	MinValue
}
[Serializable]
public struct MKStatModifier
{
    public  float Value;
    public  MKStatModType Type;
    public  MKStatModTargetType TargetType;
    public  int Order;//The larger the order, the lower the order, the lower the calculation
    public  string SourceKey;
    public MKStats LastAppendMKStats { get; private set; }

    public void SetNowAppendMKStats(MKStats mKStats)
    {
        LastAppendMKStats = mKStats;
    }
    public MKStatModifier(float value, MKStatModType type, MKStatModTargetType ttype, int order, string sourcekey,MKStats TargetStat)
    {
        Value = value;
        Type = type;
        TargetType = ttype;
        Order = order;
        SourceKey = sourcekey;
        LastAppendMKStats = TargetStat;
    }
    public MKStatModifier(MKStatModifier other)
    {
        Value = other.Value;
        Type = other.Type;
        TargetType = other.TargetType;
        Order = other.Order;
        SourceKey = other.SourceKey;
        LastAppendMKStats = other.LastAppendMKStats;
    }
    public MKStatModifier(float value, MKStatModType type, MKStatModTargetType ttype) : this(value, type, ttype, (int)type, null,null) { }

    public MKStatModifier(float value, MKStatModType type, MKStatModTargetType ttype, int order) : this(value, type, ttype, order, null, null) { }

    public MKStatModifier(float value, MKStatModType type, MKStatModTargetType ttype, string sourcekey) : this(value, type, ttype, (int)type, sourcekey,null) { }


    public MKStatModifier(float value, MKStatModType type, MKStatModTargetType ttype, int order, string sourcekey) : this(value, type, ttype, order, sourcekey, null) { }


    public void CloneFromOtherData(MKStatModifier mKStatModifier, MKStats NewMKStats)
    {

        Value = mKStatModifier.Value;
        Type = mKStatModifier.Type;
        TargetType = mKStatModifier.TargetType;
        Order = mKStatModifier.Order;
        SourceKey = mKStatModifier.SourceKey;
        if (NewMKStats != null)
        {
            LastAppendMKStats = NewMKStats;
        }
        else
        {
            LastAppendMKStats = mKStatModifier.LastAppendMKStats;
        }
    }

    public MKStatModifier CloneNew(MKStats NewMKStats)
    {

        MKStatModifier mKStatModifier = new MKStatModifier();
        mKStatModifier.CloneFromOtherData(this, NewMKStats);
        return mKStatModifier;
    }

    public string GetSaveJsonData()
    {
        MKStatModifierSerializableData mKStatModifierSerializableData = new MKStatModifierSerializableData();
        mKStatModifierSerializableData.Value = Value;
        mKStatModifierSerializableData.Type = Type;
        mKStatModifierSerializableData.TargetType = TargetType;
        mKStatModifierSerializableData.Order = Order;
        mKStatModifierSerializableData.SourceKey = SourceKey;
       // mKStatModifierSerializableData.NowMyMKStats = NowMyMKStats.GetSaveJsonData();
        return JsonUtility.ToJson(mKStatModifierSerializableData);
    }

    public static MKStatModifier LoadFromJsonData(string jsondata,MKStats targetstat)
    {
        MKStatModifier mKStatModifier= new MKStatModifier();
        MKStatModifierSerializableData mKStatModifierSerializableData = JsonUtility.FromJson<MKStatModifierSerializableData>(jsondata);
        mKStatModifier.Value = mKStatModifierSerializableData.Value;
        mKStatModifier.Type = mKStatModifierSerializableData.Type;
        mKStatModifier.TargetType = mKStatModifierSerializableData.TargetType;
        mKStatModifier.Order = mKStatModifierSerializableData.Order;
        mKStatModifier.SourceKey = mKStatModifierSerializableData.SourceKey;
        mKStatModifier.LastAppendMKStats = targetstat;

        return mKStatModifier;
    }



    /// <summary>
    /// Get unchanging value of modifier
    /// </summary>
    /// <param name="statVal"></param>
    /// <param name="stat"></param>
    /// <returns></returns>
    public float GetSustainedValue(float statVal, MKStats stat)
    {
        switch (Type)
        {
            case MKStatModType.PercentAdd:
                return statVal + (statVal * Value);
            case MKStatModType.PercentMult:
                return statVal * (statVal * Value);
            //  case MKStatModType.Subtract:
            //       return statVal - modVal;
            //  case MKStatModType.SubtractMultiplier:
            //      return statVal - (statVal * modVal);
            default:        // Fat
                return statVal + Value;
        }
    }



}

