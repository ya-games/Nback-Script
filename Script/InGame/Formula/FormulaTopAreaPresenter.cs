using System;
using System.Threading;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Assets._MyGame.Script.InGame.Formula
{
    /// <summary>
    /// 計算式上段Presenter
    /// </summary>
    public class FormulaTopAreaPresenter : IInitializable, IDisposable
    {
        private FormulaTopAreaView _formulaAreaView;
        private ReadyUsecase _readyUsecase;
        private InGameUsecase _inGameUsecase;

        private readonly CompositeDisposable _disposable = new();

        public FormulaTopAreaPresenter(
            FormulaTopAreaView formulaAreaView,
            ReadyUsecase readyUsecase,
            InGameUsecase inGameUsecase
            )
        {
            _formulaAreaView = formulaAreaView;
            _readyUsecase = readyUsecase;
            _inGameUsecase = inGameUsecase;
        }
        public void Initialize()
        {
            //ReadyUsecase
            _readyUsecase.OnSetTopExercise.Subscribe(x =>
            {
                _formulaAreaView.SetVisible(true);
                _formulaAreaView.SetFormula(x.No, x.Formula);
            }).AddTo(_disposable);

            _readyUsecase.OnHide.Subscribe(_ => _formulaAreaView.SetVisible(false))
                .AddTo(_disposable);

            //InGameUsecase
            _inGameUsecase.OnSetTopExercise.Subscribe(x =>
            {
                _formulaAreaView.SetVisible(true);
                _formulaAreaView.SetFormula(x.No, x.Formula);
            }).AddTo(_disposable);

            _inGameUsecase.OnHide.Subscribe(_ => _formulaAreaView.SetVisible(false))
                .AddTo(_disposable);

            _formulaAreaView.SetDefault();
        }

        public async UniTask PlayAnimationWhenStart(CancellationToken cancellation)
        {
            await _formulaAreaView.SetAnimationWhenStart(cancellation);
        }


        public void Dispose() => _disposable.Dispose();
    }

}