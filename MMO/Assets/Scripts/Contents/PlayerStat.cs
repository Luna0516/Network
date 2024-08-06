using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int _exp;
    [SerializeField]
    protected int _gold;

    public int Exp 
    {
        get => _exp;
        set 
        { 
            _exp = value;

            // LevelUp Check
            int level = Level;
            while (true)
            {
                Data.Stat stat;

                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;

                if (_exp < stat.totalExp)
                    break;

                level++;
            }

            if (level != Level)
            {
                Debug.Log("Level Up!");
                Level = level;
                SetStat(level);
            }
        } 
    }

    public int gold { get { return _gold; } set { _gold = value; } }

    private void Start()
    {
        _defense = 5;
        _moveSpeed = 5.0f;
        _gold = 0;
        _exp = 0;

        SetStat(1);
    }

    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
    }

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;

        Data.Stat stat = dict[1];

        _level = stat.level;
        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _attack = stat.attack;
    }
}
