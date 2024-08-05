using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    [SerializeField]
    float _scanRange = 10.0f;

    [SerializeField]
    float _attackRange = 2.0f;

    Stat _stat;

    NavMeshAgent _agent;

    public override void Init()
    {
        _stat = gameObject.GetOrAddComponent<Stat>();
        _agent = gameObject.GetOrAddComponent<NavMeshAgent>();

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateIdle()
    {
        // 나중에 매니저를 이용한 플레이어
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distance = (player.transform.position - gameObject.transform.position).magnitude;
        if(distance <= _scanRange)
        {
            _lockTarget = player;
            State = Define.State.Moving;
            return;
        }
    }

    protected override void UpdateMoving()
    {
        // 플레이어가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            // 사정거리 일단 1
            if (distance <= _attackRange)
            {
                _agent.SetDestination(transform.position);
                State = Define.State.Skill;
                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            _agent.SetDestination(_destPos);
            _agent.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<Stat>();
            int damage = Mathf.Max(myStat.Attack - targetStat.Defense, 0);
            targetStat.HP -= damage;

            if(targetStat.HP > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
