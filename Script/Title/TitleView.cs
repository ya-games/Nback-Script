using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine.EventSystems;
using Assets._MyGame.Script.GameInformation;

namespace Assets._MyGame.Script.Title
{
    /// <summary>
    /// タイトル画面view
    /// </summary>
    public class TitleView : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _adsButton;

        [SerializeField]
        private TextMeshProUGUI _adsErrorText;

        [SerializeField]
        private Button _privacyPolicyButton;

        [SerializeField]
        private Button _nextLevelButton;

        [SerializeField]
        public SliderManager _sliderExerciseAmount;

        [SerializeField]
        public SliderManager _sliderExerciseSpeed;

        [SerializeField]
        private HorizontalSelector _levelSelector;

        [SerializeField]
        private GameObject _privacyPolicyScrollPanel;

        [HideInInspector]
        public HorizontalSelector LevelSelector => _levelSelector;

        public int NbackCount => _levelSelector.index + 2; //Nbackレベル

        public GameInfoSO.Level ExerciseSpeedLevel
        {
            get
            {
                return Enum.IsDefined(typeof(GameInfoSO.Level), (int)_sliderExerciseSpeed.mainSlider.value)
                    ? (GameInfoSO.Level)(int)_sliderExerciseSpeed.mainSlider.value
                    : GameInfoSO.Level.Midium;
            }
        }

        public GameInfoSO.Level ExerciseAmountLevel
        {
            get
            {
                return Enum.IsDefined(typeof(GameInfoSO.Level), (int)_sliderExerciseAmount.mainSlider.value)
                    ? (GameInfoSO.Level)(int)_sliderExerciseAmount.mainSlider.value
                    : GameInfoSO.Level.Midium;
            }
        }

        public IObservable<Unit> OnClickStartAsObservable() => _startButton.OnClickAsObservable();

        public IObservable<Unit> OnClickAdsAsObservable() => _adsButton.OnClickAsObservable();

        public IObservable<Unit> OnClickPrivacyPolicyAsObservable() => _privacyPolicyButton.OnClickAsObservable();

        public IObservable<Unit> OnClickNextLevelButtonAsObservable() => _nextLevelButton.OnClickAsObservable();



        public void SetDefault(GameInfoSO gameInfo)
        {
            _adsErrorText.gameObject.SetActive(false);

            _sliderExerciseAmount.mainSlider.value = (int)gameInfo.ExerciseAmountLevel;
            _sliderExerciseSpeed.mainSlider.value = (int)gameInfo.ExerciseSpeedLevel;
        }

        public void DoScaleLoopStartButton()
        {
            _startButton.transform.DOScale(1.05f, 1.0f)
                    .SetEase(Ease.InCubic)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(gameObject);
        }

        public void DoScaleAdsButton()
        {
            _adsButton.transform.DOScale(1.1f, 0.2f)
                        .SetEase(Ease.InCubic)
                        .SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() => _adsButton.transform.localScale = Vector3.one)
                        .SetLink(gameObject);
        }

        public void ShowAdsErrorText()
        {
            _adsErrorText.gameObject.SetActive(true);
        }
        public void HideAdsErrorText()
        {
            _adsErrorText.gameObject.SetActive(false);
        }

        public void DestroyDupulicatedEventSystem()
        {
            //AdsManager(UnityAds)のAdvertisement.Initialize処理でEventSystemが追加されてしまうので、重複を避けるために削除する
            var es = FindObjectsOfType<EventSystem>();
            if (es.Length > 1)
            {
                for (int i = 1; i < es.Length; i++)
                {
                    Destroy(es[i].gameObject);
                }
            }

        }

    }
}