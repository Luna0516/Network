using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public System.Action KeyAction = null;

    public void OnUpdate()
    {
        if (Input.anyKey == false)
            return;

        if(KeyAction != null)
            KeyAction.Invoke();
    }
}
