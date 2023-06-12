using System.Diagnostics;

namespace Assets._MyGame.Script
{
    public static class DebugLogger
    {
        //Player設定 > スクリプトコンパイルに以下の定義を追加すると有効になる
        [Conditional("DEBUG_LOG_ON")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}