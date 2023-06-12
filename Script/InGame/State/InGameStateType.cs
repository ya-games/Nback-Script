

namespace Assets._MyGame.Script.InGame.State
{

    /// <summary>
    /// ゲーム中のステータスの定義
    /// </summary>
    public enum InGameStateType
    {
        PREPARATION, //準備中
        CAN_ANSWER,  //回答可能
        ANSWERED,    //回答済み
        FINISHED     //終了
    }
}
