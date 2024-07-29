using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);

        foreach(Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        // 실제 인벤토리 정보를 참고해서
        for(int i = 0; i < 10; i++)
        {
            GameObject item = Managers.Resource.Instantiate("UI/Scene/UI_Inven_Item", gridPanel.transform);

            UI_Inven_Item invenItem = Utill.GetOrAddComponent<UI_Inven_Item>(item);
            invenItem.SetInfo($"Sword {i}");
        }
    }
}
