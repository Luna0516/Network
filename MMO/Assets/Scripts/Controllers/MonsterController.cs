using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BaseController
{
    Stat _stat;

    public override void Init()
    {
        _stat = gameObject.GetOrAddComponent<Stat>();
        
        if(gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateIdle()
    {
        Debug.Log("Moster UpdateIdle -- Player Searching");
    }

    protected override void UpdateMoving()
    {
        Debug.Log("Moster UpdateMoving");
    }

    protected override void UpdateSkill()
    {
        Debug.Log("Moster UpdateSkill");
    }

    void OnHitEvent()
    {
        Debug.Log("Moster OnHitEvent");
    }
}
