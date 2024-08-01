using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;

    Vector3 _destPos;

    Texture2D _attackIcon;
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _stat = gameObject.GetOrAddComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Hand");
    }

    public enum PlayerState
    {
        Die,
        Moveing,
        Idle,
        Skill,  
    }

    PlayerState _state = PlayerState.Idle;

    private void UpdateIdle()
    {
        // 애니메이션 처리
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0);
    }

    private void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            _state = PlayerState.Idle;
        }
        else
        {
            NavMeshAgent agent = gameObject.GetOrAddComponent<NavMeshAgent>();

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);

            agent.Move(dir.normalized * moveDist);

            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if(Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                _state = PlayerState.Idle;
                return;
            }

            // transform.position += dir.normalized * moveDist;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            // transform.LookAt(_destPos);
        }

        // 애니메이션 처리
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _stat.MoveSpeed);
    }

    private void UpdateDie()
    {

    }

    void Update()
    {
        switch (_state)
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
            default:
                break;
        }

        UpdateMouseCursor();
    }

    void UpdateMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Moster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
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

    private void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, _mask))
        {
            _destPos = hit.point;
            _state = PlayerState.Moveing;

            if(hit.collider.gameObject.layer == (int)Define.Layer.Moster)
            {
                Debug.Log("Monster Click");
            }
            else
            {
                Debug.Log("Ground Click");
            }
        }
    }
}
