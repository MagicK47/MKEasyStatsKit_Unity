using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TestMKStatsSet : MonoBehaviour
{
    public MKStatsManager mKStatsManager; // Manager for handling all stats
    public Renderer PlayeBodyRenderer; // Renderer for the player's body
    public Image HealthBarUI; // UI image for the health bar
    public Text HealthBarTextUI; // UI text for displaying health value
    public Image EnergyBarUI; // UI image for the energy bar
    public Text EnergyBarTextUI; // UI text for displaying energy value
    public Image ExpBarUI; // UI image for the experience bar
    public Text ExpBarTextUI; // UI text for displaying experience value
    public Text LevelTextUI; // UI text for displaying level
    public Text PowerTextUI; // UI text for displaying power value
    public Text ShowAllModifierTextUI; // UI text for displaying all modifiers
    public Text SaveAndLoadTipUI; // UI text for displaying save and load tips

    List<MKStatModifier> NowAllModifier = new List<MKStatModifier>(); // List of all current modifiers
    MKStats Level; // Stat for level
    MKStats Exp; // Stat for experience
    MKStats Health; // Stat for health
    MKStats Energy; // Stat for energy
    MKStats Power; // Stat for power

    string SavePath; // Path for saving data

    private void Start()
    {
        SavePath = Application.persistentDataPath + "/TestMKStatsSave.data"; // Define the save path

        mKStatsManager.SetInit(); // Initialize the stats manager

        // Find and validate the stats
        Health = mKStatsManager.FindValidationStat("Health");
        Energy = mKStatsManager.FindValidationStat("Energy");
        Power = mKStatsManager.FindValidationStat("Power");
        Level = mKStatsManager.FindValidationStat("Level");
        Exp = mKStatsManager.FindValidationStat("Exp");

        UpdateUI(); // Update the UI with current stats

        // Add listeners for stat value changes
        Level.onValueChanged.AddListener(OnLevelChange);
        Exp.onValueChanged.AddListener(OnExpChange);

        Health.onValueChanged.AddListener(delegate { UpdateUI(); });
        Health.onMaxValueChanged.AddListener(delegate { UpdateUI(); });

        Energy.onValueChanged.AddListener(delegate { UpdateUI(); });

        Power.onValueChanged.AddListener(delegate { UpdateUI(); });

        // Add listeners for adding and removing modifiers
        Health.OnAddModifier.AddListener(delegate { UpdateUI(); });
        Health.OnRemoveModifier.AddListener(delegate { UpdateUI(); });

        Energy.OnAddModifier.AddListener(delegate { UpdateUI(); });
        Energy.OnRemoveModifier.AddListener(delegate { UpdateUI(); });

        Power.OnAddModifier.AddListener(delegate { UpdateUI(); });
        Power.OnRemoveModifier.AddListener(delegate { UpdateUI(); });
    }

    private void Update()
    {
        // Regenerate energy over time
        if (Energy.GetNowValue() < Energy.GetNowMaxValue())
        {
            float nowbasevalue = Energy.GetNowBaseValue();
            nowbasevalue += Time.deltaTime * 10;
            Energy.SetNowBaseValue(nowbasevalue);
        }

        // Gradually reset the player's body color to white
        if (PlayeBodyRenderer.material.color != Color.white)
        {
            Color c = PlayeBodyRenderer.material.color;
            c = Color.Lerp(c, Color.white, Time.deltaTime * 3);
            PlayeBodyRenderer.material.color = c;
        }
    }

    public void UpdateUI()
    {
        // Update the health bar and text
        HealthBarTextUI.text = Health.GetNowValue().ToString("f0") + "/" + Health.GetNowMaxValue().ToString("f0");
        HealthBarUI.fillAmount = Health.GetNowValue() / Health.GetNowMaxValue();

        // Update the energy bar and text
        EnergyBarTextUI.text = Energy.GetNowValue().ToString("f0") + "/" + Energy.GetNowMaxValue().ToString("f0");
        EnergyBarUI.fillAmount = Energy.GetNowValue() / Energy.GetNowMaxValue();

        // Update the experience bar and text
        ExpBarTextUI.text = "XP: " + Exp.GetNowValue().ToString("f0") + "/" + GetLevelUpNeexExp((int)Level.GetNowValue()).ToString("f0");
        ExpBarUI.fillAmount = Exp.GetNowValue() / GetLevelUpNeexExp((int)Level.GetNowValue());

        // Update the power text and level text
        PowerTextUI.text = "Power:" + Power.GetNowValue() + "";
        LevelTextUI.text = "Lv:" + Level.GetNowValue();

        // Display all current modifiers
        mKStatsManager.GetNowAllModifier(NowAllModifier);
        ShowAllModifierTextUI.text = "";
        for (int i = 0; i < NowAllModifier.Count; i++)
        {
            string showcolor = "#49FF00";
            if (NowAllModifier[i].Value < 0) { showcolor = "#FF2200"; }
            string showinfo = "";
            if (NowAllModifier[i].Type == MKStatModType.Add)
            {
                showinfo = (NowAllModifier[i].Value > 0 ? "+ " : " ") + NowAllModifier[i].Value;
            }
            if (NowAllModifier[i].Type == MKStatModType.PercentAdd)
            {
                showinfo = (NowAllModifier[i].Value > 0 ? "+ " : " ") + (NowAllModifier[i].Value * 100) + "%";
            }
            if (NowAllModifier[i].Type == MKStatModType.PercentMult)
            {
                showinfo = "x " + (NowAllModifier[i].Value * 100) + "%";
            }
            ShowAllModifierTextUI.text += "<color=" + showcolor + ">" + NowAllModifier[i].SourceKey + ": " + (NowAllModifier[i].LastAppendMKStats.mKStatsInfo.GetShowName()) + " " + showinfo + "</color>\n\n";
        }
    }

    public void OnExpChange(float oldv, float newv)
    {
        // Handle experience change and level up if necessary
        float nowneedexp = GetLevelUpNeexExp((int)Level.GetNowValue());
        bool IsUpLevel = newv >= nowneedexp;
        if (IsUpLevel)
        {
            Level.SetNowBaseValue(Level.GetNowBaseValue() + 1);
            Exp.SetNowBaseValue(Exp.GetNowBaseValue() - nowneedexp);
        }
        UpdateUI();
    }

    public void OnLevelChange(float oldv, float newv)
    {
        // Handle level change and update stats accordingly
        if (newv > oldv)
        {
            Health.SetNowBaseMaximum(Health.GetNowBaseMaxValue() + 20);
            Health.SetToNowMaximum();
            Energy.SetNowBaseMaximum(Energy.GetNowBaseMaxValue() + 50);
            Energy.SetToNowMaximum();
            Power.SetNowBaseValue(Power.GetNowBaseValue() + 1);
            SaveAndLoadTipUI.text = "Level Up !";
            PlayeBodyRenderer.material.color = Color.yellow;
        }
        UpdateUI();
    }

    public void HitHealth()
    {
        // Decrease health and change player's body color to red
        Health.SetNowBaseValue(Health.GetNowBaseValue() - 10);
        PlayeBodyRenderer.material.color = Color.red;
    }

    public void AddExp(int v)
    {
        // Add experience points
        Exp.SetNowBaseValue(Exp.GetNowBaseValue() + v);
    }

    public void AddHealth()
    {
        // Increase health and decrease energy, change player's body color to green
        Health.SetNowBaseValue(Health.GetNowBaseValue() + 5);
        CostEnergy(10);
        PlayeBodyRenderer.material.color = Color.green;
    }

    public void CostEnergy(float v)
    {
        // Decrease energy
        Energy.SetNowBaseValue(Energy.GetNowBaseValue() - v);
    }

    public void AddMaxHealthBuff()
    {
        // Add a buff to increase maximum health
        Health.AddModifier(new MKStatModifier(0.5f, MKStatModType.PercentAdd, MKStatModTargetType.MaxValue,0, "UpHeathBuff"));
        CostEnergy(35);
        PlayeBodyRenderer.material.color = Color.blue;
    }

    public void RemoveMaxHealthBuff()
    {
        // Remove the health buff
        mKStatsManager.RemoveAllModToStatsFromSource("UpHeathBuff");
    }

    public void AddPowerBuff()
    {
        // Add a buff to increase power
        Power.AddModifier(new MKStatModifier(0.5f, MKStatModType.PercentAdd, MKStatModTargetType.Value, "UpPowerBuff"));
        CostEnergy(30);
        PlayeBodyRenderer.material.color = Color.blue;
    }

    public void RemovePowerBuff()
    {
        // Remove the power buff
        mKStatsManager.RemoveAllModToStatsFromSource("UpPowerBuff");
    }

    public void AddWeakBuff()
    {
        // Add a debuff to decrease power and energy
        Power.AddModifier(new MKStatModifier(-0.2f, MKStatModType.PercentAdd, MKStatModTargetType.Value, "WeakBuff"));
        Energy.AddModifier(new MKStatModifier(-0.5f, MKStatModType.PercentAdd, MKStatModTargetType.MaxValue, "WeakBuff"));
        CostEnergy(15);
        PlayeBodyRenderer.material.color = Color.blue;
    }

    public void RemoveWeakBuff()
    {
        // Remove the weak debuff
        mKStatsManager.RemoveAllModToStatsFromSource("WeakBuff");
    }

    public void ClearAlldModifier()
    {
        // Clear all modifiers and change player's body color to blue
        RemoveMaxHealthBuff();
        RemovePowerBuff();
        RemoveWeakBuff();
        PlayeBodyRenderer.material.color = Color.blue;
    }

    public void SaveNow()
    {
        // Save current stats to a file
        File.WriteAllText(SavePath, mKStatsManager.GetSaveJsonData());
        SaveAndLoadTipUI.text = "Player stats is Save!";
    }

    public void LoadNow()
    {
        // Load stats from a file
        if (!File.Exists(SavePath))
        {
            SaveAndLoadTipUI.text = "Null save data!";
            return;
        }
        string loads = File.ReadAllText(SavePath);
        mKStatsManager.LoadSaveDataFromJson(loads);
        SaveAndLoadTipUI.text = "Player stats is Load!";
        UpdateUI();
    }

    int GetLevelUpNeexExp(int level)
    {
        // Calculate the experience needed to level up
        float neexexp = 100 + Mathf.Pow(level - 1, 2);
        return (int)neexexp;
    }
}
