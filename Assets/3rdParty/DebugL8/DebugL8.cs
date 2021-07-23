/**
 *  日志模块
 *  @author 徐方磊
 *  1.有一个逻辑控制开关 EnableLog和一个编译控制开关 LOG
 *      1.1.EnableLog关闭后，日志将不会输出，但是构造日志的传参gc仍然存在
 *      1.2.LOG编译选项关闭后，日志将不会输出，构造日志的传参gc也不存在
 *      1.3.输出Log时，要注意尽量不要在传参前拼接字符串，否则相应的gc无法避免，例如下面例子中推荐a，不推荐b
 *      a：
 *          DebugL8.LogError("这里出了点问题：{0}", e.statcktrace)
 *      b:
 *          string a = string.format("这里出了点问题：{0}", e.statcktrace)
 *          DebugL8.LogError(a)
 *      
 *  2.正式打最终包的时候可以通过去除LOG编译选项来彻底关闭日志功能 
 * 
 */
using System;
using UnityEngine;
using System.Diagnostics;

public static class DebugL8
{
    public const string LOG_SYMBOL = "LOG";
    [Conditional(LOG_SYMBOL)]
    public static void Log(object message)
    {
        DebugL8.Log(message, null);
    }

    [Conditional(LOG_SYMBOL)]
    public static void Log(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogError(object message)
    {
        DebugL8.LogError(message, null);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogError(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogWarning(object message)
    {
        DebugL8.LogWarning(message, null);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    [Conditional(LOG_SYMBOL)]
    public static void Log(string str, params object[] args)
    {
        if (args.Length > 0)
        {
            str = string.Format(str, args);
        }

        UnityEngine.Debug.Log(str);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogWarning(string str, params object[] args)
    {
        if (args.Length > 0)
        {
            str = string.Format(str, args);
        }
        UnityEngine.Debug.LogWarning(str);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogError(string str, params object[] args)
    {
        if (args.Length > 0)
        {
            str = string.Format(str, args);
        }
        UnityEngine.Debug.LogError(str);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
    {
        string text = string.Format(format, args);
        UnityEngine.Debug.LogError(text, context);
    }

    [Conditional(LOG_SYMBOL)]
    public static void DebugBreak()
    {
        UnityEngine.Debug.Break();
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        string text = string.Format(format, args);
        UnityEngine.Debug.Log(text, context);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogFormat(string format, params object[] args)
    {
        string text = string.Format(format, args);
        UnityEngine.Debug.Log(text);
    }

    [Conditional(LOG_SYMBOL)]
    public static void ClearDeveloperConsole()
    {
        UnityEngine.Debug.ClearDeveloperConsole();
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogWarningFormat(string format, params object[] args)
    {
        string text = string.Format(format, args);
        UnityEngine.Debug.LogWarningFormat(text, new object[0]);
    }

    [Conditional(LOG_SYMBOL)]
    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
    {
        string text = string.Format(format, args);
        UnityEngine.Debug.LogWarningFormat(context, text, new object[0]);
    }
}
