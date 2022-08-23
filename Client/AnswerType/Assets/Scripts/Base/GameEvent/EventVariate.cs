//2018.09.18    关林
//游戏事件 变量定义

#region 事件ID
public enum EEventID
{
    GameState,
    GameMode,

    //逻辑刷新
    RefreshCurrency,    //刷新货币
    
    RefreshGameMode,  //刷新游戏玩法
    
    /// <summary>刷新免费提示</summary>
    RefreshFreeTip,
    
    RefreshEventPage,
    /// <summary>刷新游戏界面现金</summary>
    RefreshBogus,

    /// <summary>刷新主页财神气泡</summary>
    RefreshGameMainArpuBubble,
    RefreshPosition,

    //刷新新手签到ui
    RefreshNewbieSignUI,

    //语言
    EID_ChangeLanguage,

    UIOpen,
    UIClose,


    //UITurnAD,
}
#endregion


#region 事件参数
public class EventParam
{

}

//一个参数的类型，简单的都可以用这个泛型类
public class EventParam<T> : EventParam
{
    public T _param { get; private set; }

    public EventParam(T param)
    {
        this._param = param;
    }
}

//两个参数的类型，简单的都可以用这个泛型类
public class EventParam<T, T2> : EventParam
{
    public T _param { get; private set; }
    public T2 _param2 { get; private set; }

    public EventParam(T param, T2 param2)
    {
        this._param = param;
        this._param2 = param2;
    }
}

//三个参数的类型，简单的都可以用这个泛型类
public class EventParam<T, T2, T3> : EventParam
{
    public T _param { get; private set; }
    public T2 _param2 { get; private set; }
    public T3 _param3 { get; private set; }

    public EventParam(T param, T2 param2, T3 param3)
    {
        this._param = param;
        this._param2 = param2;
        this._param3 = param3;
    }
}

#endregion
