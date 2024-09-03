using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
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
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        // 이미지 드래그 자동화 설정 (수정)
        GameObject obj = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(obj, (PointerEventData data) => { obj.transform.position = data.position; }, Define.UIEvent.Drag);

        //Extension 연습
        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked);

        base.Init();
    }

    int _score = 0;

    public void OnButtonClicked(PointerEventData data)
    {
        _score++;

        GetText((int)Texts.ScoreText).text = $"Score : {_score}";
    }
}
