using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;
    public static Managers Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

    // 구형
    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    // 구형
    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }

    // UI 매니저
    UIManager _ui = new UIManager();
    public static UIManager UI { get{return Instance._ui;}}

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (instance == null)
        {
            GameObject obj = GameObject.Find("@Managers");

            if (obj == null)
            {
                obj = new GameObject { name = "@Managers" };
                obj.AddComponent<Managers>();
            }

            DontDestroyOnLoad(obj);

            instance = obj.GetComponent<Managers>();
        }
    }
}
