using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    class Test
    {
        public int Id = 0;
    }

    // Coroutine Test
    // 1. 함수의 상태를 저장/복윈 가능!
        // -> 엄청 오래 걸리는 작업을 잠시 끊거나
        // -> 원하는 타이밍에 함수를 잠시 Stop/복원하는 경우
    // 2. return -> 우리가 원하는 타입으로 가능(class도 가능)
    class CoroutineTest : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            for(int i = 0; i < 1000000; i++)
            {
                if(i % 10000 == 0)
                    yield return null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();


        CoroutineTest test = new CoroutineTest();
        foreach(var t in test)
        {
            Test value = (Test)t;
            Debug.Log(value.Id);
        }
    }

    public override void Clear()
    {

    }
}
