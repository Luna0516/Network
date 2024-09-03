using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dictionary<int, GameObject> _player = new Dictionary<int, GameObject>();
// Dictionary<int, GameObject> _monster = new Dictionary<int, GameObject>();
// Dictionary<int, GameObject> _env = new Dictionary<int, GameObject>();

public class GameManager
{
    GameObject _player;
    HashSet<GameObject> _monster = new HashSet<GameObject>();

    public Action<int> OnSpawnEvent;

    public GameObject Player { get { return _player; } }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Player:
                _player = go;
                break;
            case Define.WorldObject.Monster:
                _monster.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Unknown:
            default:
                break;
        }

        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();

        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectTpye;
    }

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Player:
                if(_player == go)
                {
                    _player = null;
                }
                break;
            case Define.WorldObject.Monster:
                if(_monster.Contains(go))
                {
                    _monster.Remove(go);

                    if (OnSpawnEvent != null)
                        OnSpawnEvent.Invoke(-1);
                }
                break;
            case Define.WorldObject.Unknown:
            default:
                break;
        }

        Managers.Resource.Destroy(go);
    }
}
