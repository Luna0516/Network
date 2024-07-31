using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();
    }

    private void Start()
    {
        StartCoroutine(CheckData());
    }

    IEnumerator CheckData()
    {
        yield return new WaitForSeconds(2);
        Dictionary<int, Stat> dict = Managers.Data.StatDict;
    }

    public override void Clear()
    {

    }
}
