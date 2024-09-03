using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using UnityEngine.AI;

public class CursorController : MonoBehaviour
{

    Texture2D _attackIcon;
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;

    int _mask = (1 << (int)Define.Layer.Ground | 1 << (int)Define.Layer.Moster);

    private void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Hand");
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
            return;

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
}
