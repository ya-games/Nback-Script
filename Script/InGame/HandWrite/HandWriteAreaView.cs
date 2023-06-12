using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Assets._MyGame.Script.InGame.HandWrite
{
    /// <summary>
    /// 手書き領域View
    /// </summary>
    public class HandWriteAreaView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _messageAreaRect; //手書き領域のメッセージ部分のRoot

        [SerializeField]
        private TextMeshProUGUI _prepareMessageText;

        [SerializeField]
        private Image _circleImage; //開始アニメーション用

        [SerializeField]
        private Button _deleteButton;

        public Button DeleteButton => _deleteButton;


        public void SetDefault()
        {
        }

        //開始アニメーション
        public async UniTask SetAnimationOnStart(CancellationToken cancellation)
        {
            //円の拡大アニメーション -> 開始メッセージ表示
            await _circleImage.transform.DOScale(1000, 1f)
                     .SetEase(Ease.InSine)
                     .SetLink(gameObject)
                     //.OnComplete(()=>ShowPrepareMessage())
                     .ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: cancellation);

            ShowPrepareMessage();
        }
        //開始前メッセージ領域を表示
        public void ShowMessageArea()
        {
            _messageAreaRect.gameObject.SetActive(true);
        }

        //開始前メッセージ領域を非表示
        public void HideMessageArea()
        {
            _messageAreaRect.gameObject.SetActive(false);
        }


        public void ShowPrepareMessage()
        {
            _prepareMessageText.gameObject.SetActive(true);
        }
        public void HidePrepareMessage()
        {
            _prepareMessageText.gameObject.SetActive(false);
        }
    }

}