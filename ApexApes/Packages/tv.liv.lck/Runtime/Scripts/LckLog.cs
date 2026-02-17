using System.IO;
using System.Runtime.CompilerServices;
using Liv.Lck.Core;
using Liv.Lck.Settings;

namespace Liv.Lck
{
  public enum LogLevel : int
  {
    None,
    Error,
    Warning,
    Info,
  }

  internal static class LckLog
  {
    public static void Log(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
      if(ShouldPrint(LogLevel.Info))
      {
        UnityEngine.Debug.Log(message);
      }
      LckCore.Log(LogType.Info, message, memberName, GetFileName(filePath), lineNumber);
    }

    public static void LogWarning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
      if(ShouldPrint(LogLevel.Warning))
      {
        UnityEngine.Debug.LogWarning(message);
      }
      LckCore.Log(LogType.Warning, message, memberName, GetFileName(filePath), lineNumber);
    }

    public static void LogError(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
      if(ShouldPrint(LogLevel.Error))
      {
        UnityEngine.Debug.LogError(message);
      }
      LckCore.Log(LogType.Error, message, memberName, GetFileName(filePath), lineNumber);
    }

    [System.Diagnostics.Conditional("LCK_TRACE")]
    public static void LogTrace(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
      UnityEngine.Debug.Log(message);
      LckCore.Log(LogType.Trace, message, memberName, GetFileName(filePath), lineNumber);
    }

    private static bool ShouldPrint(LogLevel level)
    {
      return (int)LckSettings.Instance.BaseLogLevel >= (int)level;
    }

    private static string GetFileName(string filePath)
    {
#if UNITY_EDITOR
      return Path.GetFileName(filePath);
#else
      int lastSlashIndex = filePath.LastIndexOfAny(new char[] { '/', '\\' });
      if (lastSlashIndex >= 0 && lastSlashIndex < filePath.Length - 1)
      {
        return filePath.Substring(lastSlashIndex + 1);
      }
      return filePath;
#endif
    }
  }
}
