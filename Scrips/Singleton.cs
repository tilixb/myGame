using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 理论上_instance不应该为null，这里可以提示我们哪个单例类出问题了
                Debug.Log(typeof(T).ToString() + " is NULL");
                // Do something...
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T; // 基类引用this转化为派生类T引用
    }
}