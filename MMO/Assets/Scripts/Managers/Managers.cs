using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance
    {
        get
        {
            Init();
            return s_instance;
        }
    }

    // 구형
    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    // Pool 매니저
    PoolManager _pool = new PoolManager();
    public static PoolManager Pool { get { return Instance._pool; } }

    // 구형
    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }

    // Scene 매니저
    SceneManagerEx _scene = new SceneManagerEx();
    public static SceneManagerEx Scene { get { return Instance._scene; } }

    // Sound 매니저
    SoundManager _sound = new SoundManager();
    public static SoundManager Sound { get {  return Instance._sound; } }

    // UI 매니저
    UIManager _ui = new UIManager();
    public static UIManager UI { get { return Instance._ui; } }

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
        if (s_instance == null)
        {
            GameObject obj = GameObject.Find("@Managers");

            if (obj == null)
            {
                obj = new GameObject { name = "@Managers" };
                obj.AddComponent<Managers>();
            }

            DontDestroyOnLoad(obj);

            s_instance = obj.GetComponent<Managers>();

            s_instance._pool.Init();
            s_instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();

        Pool.Clear();
    }
}
