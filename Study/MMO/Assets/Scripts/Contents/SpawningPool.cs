using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _mosterCount = 0;

    int _reserveCount = 0;

    [SerializeField]
    int _keepMosterCount = 0;

    [SerializeField]
    Vector3 _spawnPos;

    [SerializeField]
    float _spawnRadius = 15.0f;

    [SerializeField]
    float _spawnTime = 5.0f;

    private void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    private void Update()
    {
        while(_reserveCount + _mosterCount < _keepMosterCount)
        {
            StartCoroutine(ReserveSpawn());
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;

        yield return new WaitForSeconds(Random.Range(0,_spawnTime));

        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");

        NavMeshAgent agent = obj.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitCircle * Random.Range(0, _spawnRadius);
            randDir.y = 0.0f;

            randPos = _spawnPos + randDir;

            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(randPos, path))
                break;

            yield return null;
        }

        obj.transform.position = randPos;

        _reserveCount--;
    }

    public void AddMonsterCount(int value)
    {
        _mosterCount += value;
    }

    public void SetKeepMonsterCount(int count)
    {
        _keepMosterCount = count;
    }
}
