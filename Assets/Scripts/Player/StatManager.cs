using System;
using System.Collections;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    [SerializeField] Stat[] _stats;

	void Start()
	{
        foreach (Stat stat in _stats)
        {
            stat.UpdateBar();
        }
	}

	public void SetStat(string name, int value)
    {
        Stat stat = GetStat(name);
        if (stat == null) return;

        stat.Value = value;

        SetStat(name, stat);
    }

    public void SetStat(string name, Stat stat)
    {
        for (int i = 0; i < _stats.Length; i++)
        {
            if (_stats[i].Name == name)
            {
                stat.UpdateBar();
                _stats[i] = stat;
                return;
            }
        }
    }

    public Stat GetStat(string name)
    {
        foreach (Stat stat in _stats)
        {
            if (stat.Name == name) return stat;
        }
        return null;
    }
}
[Serializable]
public class Stat
{
    public string Name;

    public int Value;
    public int MinValue;
    public int MaxValue;
    public ProgressBar Bar;

    public void UpdateBar()
    {
        if (!Bar) return;

        Bar.Min = MinValue;
        Bar.Max = MaxValue;
        Bar.Value = Value;
    }
}
