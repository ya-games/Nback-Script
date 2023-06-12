using System.Threading;

namespace Assets._MyGame.Script.InGame.State
{
    /// <summary>
    /// UI更新時のステータスクラス
    /// Model側からPresenter側にUI更新の通知を送りawaitする際にこのフラグをWaitUntilする
    /// </summary>
    public class UIState
    {
        public UIStateType StateValue { get; set; }
        public CancellationToken Cancellation { get;private set; }
        public UIState(UIStateType state, CancellationToken cancellation)
        {
            StateValue = state;
            Cancellation = cancellation;
        }
    }
}
