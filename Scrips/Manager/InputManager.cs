using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputAction m_action;

    //public Action<Vector2> moveAction;
    //public Action behaviour1, behaviour2;
    public Action<Vector2> mouseDragAction;
    public Action leftClickStarted, rightClickStarted;
    public Action leftClickPerformed, rightClickPerformed;
    public Action leftClickCanceled, rightClickCanceled;

    protected override void Awake()
    {
        base.Awake();
        m_action = new PlayerInputAction();

        // m_action.Player.behaviour1.performed += ctx => { behaviour1?.Invoke();};
        // m_action.Player.behaviour2.performed += ctx => { behaviour2?.Invoke();};

        //performed被执行
        m_action.Player.LeftClick.started += ctx => { if(haveInput) leftClickStarted?.Invoke(); };
        m_action.Player.RightClick.started += ctx => { if(haveInput) rightClickStarted?.Invoke(); };
        m_action.Player.LeftClick.performed += ctx => { if(haveInput) leftClickPerformed?.Invoke(); };
        m_action.Player.RightClick.performed += ctx => { if(haveInput) rightClickPerformed?.Invoke(); };
        m_action.Player.LeftClick.canceled += ctx => { if(haveInput) leftClickCanceled?.Invoke(); };
        m_action.Player.RightClick.canceled += ctx => { if(haveInput) rightClickCanceled?.Invoke(); };
    }

    public void OnEnable()
    {
        m_action.Enable();
    }

    public void OnDisable()
    {
        m_action.Disable();
    }

    void Update()
    {
        //moveAction?.Invoke(m_action.Player.Move.ReadValue<Vector2>());
        if(haveInput) mouseDragAction?.Invoke(m_action.Player.MousePosition.ReadValue<Vector2>());
    }


    private bool haveInput=false;
    public void NoInput()
    {
        haveInput = false;
        
        m_action.Player.LeftClick.Reset();
        m_action.Player.RightClick.Reset();
        
    }

    public void HaveInput()
    {
        haveInput = true;
        // Reset the input actions
        m_action.Player.LeftClick.Reset();
        m_action.Player.RightClick.Reset();
    }
}