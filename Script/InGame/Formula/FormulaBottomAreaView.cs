using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;
using VContainer;
using Assets._MyGame.Script.Audio;

namespace Assets._MyGame.Script.InGame.Formula
{
    /// <summary>
    /// 計算式下段View
    /// </summary>
    public class FormulaBottomAreaView : MonoBehaviour
    {

        [SerializeField]
        private RectTransform _formulaArea;

        [SerializeField]
        private CanvasGroup _formulaCanvasGroup; //背景以外の表示切替用


        [SerializeField]
        private TextMeshProUGUI _questionNoText;

        [SerializeField]
        private TextMeshProUGUI _formulaText;

        [SerializeField]
        private Image _answerFrame;

        [SerializeField]
        private TextMeshProUGUI _answerText;

        [SerializeField]
        private Image _correctImage;

        [SerializeField]
        private Image _inCorrectImage;


        //DI
        private AudioPlayer _audioPlayer;



        [Inject]
        public void Constructor(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }


        public void SetDefault()
        {
            //開始時アニメーションのため画面外に移動
            _formulaArea.anchorMin = new Vector2(-1, _formulaArea.anchorMin.y);
            _formulaArea.anchorMax = new Vector2(0, _formulaArea.anchorMax.y);


            _formulaText.text = "???=";
            _answerText.text = "?";
            _questionNoText.text = "No.";
            _formulaCanvasGroup.alpha = 0; //表示OFF

        }

        public async UniTask SetAnimationWhenStart(CancellationToken cancellation)
        {
            var r = _formulaArea;
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
            _formulaCanvasGroup.alpha = val ? 1 : 0;
        }

        public void SetQuestionNo(int no)
        {
            _questionNoText.text = $"No.{no.ToString()}";
            _answerText.text = "";
            _formulaText.text = "???=";
        }

        public void SetCorrectVisible(bool val)
        {
            _correctImage.gameObject.SetActive(val);

        }
        public void SetInCorrectVisible(bool val)
        {
            _inCorrectImage.gameObject.SetActive(val);
        }

        public void SetFormula(int questionNo, string formulaText)
        {
            _questionNoText.text = $"No.{questionNo.ToString()}";
            _formulaText.text = formulaText;
        }

        public void SetAnswer(string answerText)
        {
            _answerText.text = answerText;
        }

        public void SetAnimationWhenIncorrect()
        {
            _answerFrame.transform.DOPunchPosition(new Vector3(8f, 0, 0), 0.4f)
                .SetDelay(0.2f)
                .SetLink(gameObject);
        }

    }
}