using System;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using Assets._MyGame.Script.InGame;
using Assets._MyGame.Script.InGame.State;
using Assets._MyGame.Script.InGame.Formula;
using Assets._MyGame.Script.InGame.HandWrite;

/// <summary>
/// InGamePresenter
/// Model側から複数のPresenterにOnNextを発行し、複数UI側の処理を待つケースがあるため
/// このクラスを処理の受け口とする
/// </summary>
public class InGamePresenter : IInitializable, IDisposable
{
    private FormulaTopAreaPresenter _formulaTopAreaPresenter;
    private FormulaBottomAreaPresenter _formulaBottomAreaPresenter;
    private HandWriteAreaPresenter _handWriteAreaPresenter;
    private ReadyUsecase _readyUsecase;

    private readonly CompositeDisposable _disposable = new();

    public InGamePresenter(
        FormulaTopAreaPresenter formulaTopAreaPresenter,
        FormulaBottomAreaPresenter formulaBottomAreaPresenter,
        HandWriteAreaPresenter handWriteAreaPresenter,
        ReadyUsecase readyUsecase
        )
    {
        _formulaTopAreaPresenter = formulaTopAreaPresenter;
        _formulaBottomAreaPresenter = formulaBottomAreaPresenter;
        _handWriteAreaPresenter = handWriteAreaPresenter;
        _readyUsecase = readyUsecase;
    }
    public void Initialize()
    {
        _readyUsecase.OnStart.Subscribe(state =>OnStart(state).Forget()).AddTo(_disposable);
    }
    
    private async UniTask OnStart(UIState state)
    {        

        await (
            _formulaTopAreaPresenter.PlayAnimationWhenStart(state.Cancellation),
            _formulaBottomAreaPresenter.PlayAnimationWhenStart(state.Cancellation),
            _handWriteAreaPresenter.PlayAnimationWhenStart(state.Cancellation)
            );
        state.StateValue = UIStateType.FINISHED; //このフラグをModel側でWaitUntilして待つ
    }

    public void Dispose() => _disposable.Dispose();
}
