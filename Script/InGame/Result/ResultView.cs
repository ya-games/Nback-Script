using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;
using VContainer;
using Assets._MyGame.Script.Audio;
using UniRx;
using Unity.Linq;
using Assets._MyGame.Script.Exercises;
using Assets._MyGame.Script.GameInformation;

namespace Assets._MyGame.Script.InGame.Result
{
    public class ResultView : MonoBehaviour
    {
        //Inspector
        [SerializeField]
        private Canvas _canvasResult;

        [SerializeField]
        private RectTransform _backgroundImage;

        [SerializeField]
        private RectTransform _panelContentRoot;

        [SerializeField]
        private TextMeshProUGUI _txtAnsnwerResult;

        [SerializeField]
        private TextMeshProUGUI _txtPercent;

        [SerializeField]
        private TextMeshProUGUI _txtIsClear;

        [SerializeField]
        private Image _imagePercent;

        [SerializeField]
        private Button _btnNext;

        [SerializeField]
        private Button _btnPrev;

        [SerializeField]
        private Button _btnRetry;

        [SerializeField]
        private LayoutGroup _layoutGroupCorrectImage;

        [SerializeField]
        private RectTransform _btnGroup;

        [SerializeField]
        private Image _prefImageCorrect;

        [SerializeField]
        private Image _prefImageInCorrect;

        [SerializeField]
        private Color _correctColor; //「合格」の背景色

        [SerializeField]
        private Color _inCorrectColor; //「不合格」の背景色

        //property
        public Button BtnNext => _btnNext;
        public Button BtnPrev => _btnPrev;
        public Button BtnRetry => _btnRetry;


        //DI
        private GameInfoSO _gameInfo;
        private AudioPlayer _audioPlayer;




        [Inject]
        public void Constructor(GameInfoSO gameInfo,AudioPlayer audioPlayer)
        {
            _gameInfo = gameInfo;
            _audioPlayer = audioPlayer;
        }

        public void SetDefault()
        {
            //開始時アニメーションのため画面外に移動
            _backgroundImage.anchorMin = new Vector2(-1, _backgroundImage.anchorMin.y);
            _backgroundImage.anchorMax = new Vector2(0, _backgroundImage.anchorMax.y);

            _panelContentRoot.gameObject.SetActive(false);
        }

        public async UniTask SetAnimationOnStart(CancellationToken cancellation)
        {
            var r = _backgroundImage;
            r.anchorMin = new Vector2(-1, r.anchorMin.y);
            r.anchorMax = new Vector2(0, r.anchorMax.y);
            var t1 = r.DOAnchorMin(new Vector2(0, r.anchorMin.y), 1f)
                .SetEase(Ease.OutCubic)
                .SetDelay(0.3f)
                .SetLink(gameObject).ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait,cancellationToken: cancellation);

            var t2 = r.DOAnchorMax(new Vector2(1, r.anchorMax.y), 1f)
                .SetEase(Ease.OutCubic)
                .SetDelay(0.3f)
                .SetLink(gameObject).ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait,cancellationToken: cancellation);

            await UniTask.WhenAll(t1, t2);

        }

        public void SetVisible(bool val)
        {
            //_formulaCanvasGroup.alpha = val ? 1 : 0;
            _canvasResult.gameObject.SetActive(val);
        }

        public async UniTask ShowAnswerResult(ExerciseResult result, CancellationToken cancellation)
        {
            _txtAnsnwerResult.text = string.Format("{0}問 ／ {1}問", 0, result.ExerciseCount.ToString());

            _panelContentRoot.gameObject.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cancellation);

            int correctAnswerCount = 0;

            foreach (var isCorrect in result.ResultList)
            {
                if (isCorrect)
                {
                    _layoutGroupCorrectImage.gameObject.Add(Instantiate(_prefImageCorrect));
                    _audioPlayer.PlaySe(AudioResource.ClipName.Correct);

                    correctAnswerCount++;
                    _txtAnsnwerResult.text = string.Format("{0}問 ／ {1}問", correctAnswerCount.ToString(), result.ExerciseCount.ToString());

                }
                else
                {
                    _layoutGroupCorrectImage.gameObject.Add(Instantiate(_prefImageInCorrect));
                }
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellation);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellation);
            _txtPercent.text = $"{result.CorrectPercent}%";
            _txtIsClear.text = result.IsPass ? "合格" : "不合格";
            if (result.IsPass)
            {
                _txtIsClear.text = "合格";
                _imagePercent.color = _correctColor;
            }
            else
            {
                _txtIsClear.text = "不合格";
                _imagePercent.color = _inCorrectColor;
                _btnNext.gameObject.SetActive(false);
            }

            if (_gameInfo.NbackLevel == GameInfoSO.MinNbackLevel) _btnPrev.gameObject.SetActive(false);
            if (_gameInfo.NbackLevel == GameInfoSO.MaxNbackLevel) _btnNext.gameObject.SetActive(false);


            _imagePercent.gameObject.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellation);

            _btnGroup.gameObject.SetActive(true);

        }


    }
}