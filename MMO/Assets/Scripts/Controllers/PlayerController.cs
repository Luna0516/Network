using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;

    Vector3 _destPos;

    NavMeshAgent _agent;

    void Start()
    {
        _stat = gameObject.GetOrAddComponent<PlayerStat>();
        _agent = gameObject.GetOrAddComponent<NavMeshAgent>();

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
    }

    public enum PlayerState
    {
        Die,
        Moveing,
        Idle,
        Skill,  
    }

    [SerializeField]
    PlayerState _state = PlayerState.Idle;

    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case PlayerState.Die:
                    break;
                case PlayerState.Moveing:
                    anim.SetFloat("speed", _stat.MoveSpeed);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Idle:
                    anim.SetFloat("speed", 0);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Skill:
                    anim.SetBool("attack", true);
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateIdle()
    {
    }

    private void UpdateMoving()
    {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if(_lockTarget != null)
        {
            float distance = (_destPos - transform.position).magnitude;
            // 사정거리 일단 1
            if(distance <= 1)
            {
                State = PlayerState.Skill;
                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = PlayerState.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);

            _agent.Move(dir.normalized * moveDist);

            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false)
                    State = PlayerState.Idle;
                return;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    private void UpdateDie()
    {

    }

    private void UpdateSkill()
    {
        State = PlayerState.Skill;
    }

    void OnHitEvent()
    {
        State = PlayerState.Moveing;
    }

    void Update()
    {
        switch (State)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moveing:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Skill:
                UpdateSkill();
                break;
            default:
                break;
        }
    }

    //void OnKeyboard()
    //{
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.5f);
    //        transform.position += Vector3.forward * Time.deltaTime * _speed;
    //    }

    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.5f);
    //        transform.position += Vector3.back * Time.deltaTime * _speed;
    //    }

    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.5f);
    //        transform.position += Vector3.right * Time.deltaTime * _speed;
    //    }

    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.5f);
    //        transform.position += Vector3.left * Time.deltaTime * _speed;
    //    }

    //    _moveToDest = false;
    //}

    int _mask = (1 << (int)Define.Layer.Ground | 1 << (int)Define.Layer.Moster);
    GameObject _lockTarget;

    private void OnMouseEvent(Define.MouseEvent evt)
    {
        if (State == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if(raycastHit)
                    {
                        _destPos = hit.point;
                        State = PlayerState.Moveing;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Moster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                if (_lockTarget != null)
                    _destPos = _lockTarget.transform.position;
                else if (raycastHit)
                    _destPos = hit.point;
                break;
            //case Define.MouseEvent.PointerUp:
            //    _lockTarget = null;
            //    break;
            case Define.MouseEvent.Click:
                break;
            default:
                break;
        }
    }
}
