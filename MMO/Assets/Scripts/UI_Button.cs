using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Button : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _text;

    int _score = 0;

    public void OnButtonClicked()
    {
        _score++;
        _text.text = $"Score : {_score}";
    }
}
