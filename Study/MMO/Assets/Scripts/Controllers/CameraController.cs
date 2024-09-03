using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta;

    [SerializeField]
    GameObject _player;

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    private void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (_player.IsValid() == false)
                return;

            if (Physics.Raycast(_player.transform.position, _delta, out RaycastHit hitInfo, _delta.magnitude, 1 << (int)Define.Layer.Block))
            {
                float dist = (hitInfo.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
