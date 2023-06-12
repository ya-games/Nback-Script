using System;
using System.Threading;
using VContainer.Unity;
using UniRx;
using Cysharp.Threading.Tasks;
using Assets._MyGame.Script.InGame.HandWrite;

namespace Assets._MyGame.Script.InGame.Formula
{
    /// <summary>
    /// 計算式下段Presenter
    /// </summary>
    public class FormulaBottomAreaPresenter : IInitializable, IDisposable
    {
        //DI
        private readonly FormulaBottomAreaView _formulaAreaView;
        private readonly InGameUsecase _inGameUsecase;
        private readonly HandWriter _handWriter;

        private readonly CompositeDisposable _disposable = new();

        //Field
        private const float _symbolDisplayDuration = 0.2f; //記号表示までの間隔

        public FormulaBottomAreaPresenter(
            FormulaBottomAreaView formulaAreaView,
            InGameUsecase inGameUsecase,
            HandWriter handWriter
            )
        {
            _formulaAreaView = formulaAreaView;
            _inGameUsecase = inGameUsecase;
            _handWriter = handWriter;
        }
        public void Initialize()
        {
            //InGameUsecase
            _inGameUsecase.OnCorrectAnswer.Subscribe(async x =>
            {
                _formulaAreaView.SetFormula(x.No, x.Formula);
                _formulaAreaView.SetAnswer(x.Answer);
                await UniTask.Delay(TimeSpan.FromSeconds(_symbolDisplayDuration));
                _formulaAreaView.SetCorrectVisible(true); //正解の丸を表示
            }).AddTo(_disposable);

            _inGameUsecase.OnIncorrectAnswer.Subscribe(async x =>
            {
                _formulaAreaView.SetFormula(x.No, x.Formula);
                _formulaAreaView.SetAnswer(x.Answer);
                await UniTask.Delay(TimeSpan.FromSeconds(_symbolDisplayDuration));
                _formulaAreaView.SetInCorrectVisible(true);
            }).AddTo(_disposable);

            _inGameUsecase.OnAnswerTimeOut.Subscribe(_ =>
            {
                _formulaAreaView.SetInCorrectVisible(true);
            }).AddTo(_disposable);

            _inGameUsecase.OnSetBottomExercise.Subscribe(x =>
            {
                //表示関連のリセット
                _formulaAreaView.SetVisible(true);
                _formulaAreaView.SetCorrectVisible(false);
                _formulaAreaView.SetInCorrectVisible(false);

                _formulaAreaView.SetQuestionNo(x.No);
            }).AddTo(_disposable);

            _inGameUsecase.OnHide.Subscribe(_ =>
            {
                _formulaAreaView.SetVisible(false);
            }).AddTo(_disposable);

            //HandWriter
            _handWriter.OnShowPlayerAnswer
                .Where(x => x >= 0) //推論不可の場合は-1を返しているので
                .Subscribe(x =>
            {

                _formulaAreaView.SetAnswer(x.ToString());
            }).AddTo(_disposable);

            _handWriter.OnIncorrectAnswer.Subscribe(_ =>
            {
                _formulaAreaView.SetAnimationWhenIncorrect();
            }).AddTo(_disposable);


            //初期値設定
            _formulaAreaView.SetDefault();
        }

        public async UniTask PlayAnimationWhenStart(CancellationToken cancellation)
        {
            await _formulaAreaView.SetAnimationWhenStart(cancellation);
        }

        public void Dispose() => _disposable.Dispose();
    }

}