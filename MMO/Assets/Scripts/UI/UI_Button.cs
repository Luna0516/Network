using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Base
{
    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum Images
    {
        ItemIcon,
    }

    enum GameObjects
    {
        TestObject,
    }

    private void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        Get<TextMeshProUGUI>((int)Texts.ScoreText).text = $"Bind Text";

        // 이미지 드래그 자동화 설정
        GameObject obj = GetImage((int)Images.ItemIcon).gameObject;
        UI_EventHandler evt = obj.GetComponent<UI_EventHandler>();
        evt.OnDragHandler += ((PointerEventData data) => { obj.transform.position = data.position; });
    }

    int _score = 0;

    public void OnButtonClicked()
    {
        _score++;

        GetText((int)Texts.ScoreText).text = $"Score : {_score}";
    }
}
