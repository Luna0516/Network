using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected GameObject _lockTarget;

    [SerializeField]
    protected Define.State _state = Define.State.Idle;

    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Die:
                    break;
                case Define.State.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case Define.State.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case Define.State.Skill:
                    anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                default:
                    break;
            }
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        switch (State)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
            default:
                break;
        }
    }

    public abstract void Init();
    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
}
