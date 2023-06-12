using System;
using UniRx;

namespace Assets._MyGame.Script.InGame.State
{   
    /// <summary>
    /// ゲーム中のステータス
    /// </summary>
    public class InGameState:IDisposable
    {
        private ReactiveProperty<InGameStateType> _currentGameState = new(InGameStateType.PREPARATION);
        private readonly CompositeDisposable _disposable = new();

        public InGameState()
        {            
            _currentGameState.AddTo(_disposable);            
        }
        public bool IsState(InGameStateType targetState)
        {
            return _currentGameState.Value == targetState;
        }

        public void SetState(InGameStateType inGameStateType)
        {
            _currentGameState.Value = inGameStateType;
        }

        public IObservable<InGameStateType> OnChangeState() => _currentGameState;

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
