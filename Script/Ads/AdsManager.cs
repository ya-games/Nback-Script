using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UniRx;
using System;

namespace Assets._MyGame.Script.Ads
{
    /// <summary>
    /// 広告管理クラス
    /// </summary>
    public class AdsManager : IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener,IDisposable
    {   
        //DI
        private AdsConfigSO _adsConfig;

        //Field
        private readonly Subject<Unit> _onAdsInitializationComplete = new();
        private readonly Subject<Unit> _onAdsShowComplete = new();
        private readonly Subject<Unit> _onAdsShowFailure = new();
        private readonly CompositeDisposable _disposable = new();

        public IObservable<Unit> OnAdsShowComplete => _onAdsShowComplete;
        public IObservable<Unit> OnAdsShowFailure => _onAdsShowFailure;
        public IObservable<Unit> OnAdsInitializationComplete => _onAdsInitializationComplete;
        public AdsManager(AdsConfigSO adsConfig)
        {
            _adsConfig = adsConfig;
            Initialize();

            //ストリームソースのDispose
            _onAdsInitializationComplete.AddTo(_disposable);
            _onAdsShowComplete.AddTo(_disposable);
            _onAdsShowFailure.AddTo(_disposable);
        }


        public void Initialize()
        {
            Advertisement.Initialize(_adsConfig.AndroidGameId, _adsConfig.TestMode, this);
        }

        public void Show()
        {
            if (Advertisement.isInitialized)
            {
                Advertisement.Show(_adsConfig.AndroidPlacementId, this);
            }
        }


        void IUnityAdsInitializationListener.OnInitializationComplete()
        {
            DebugLogger.Log("Unity Ads initialization complete.");
            _onAdsInitializationComplete.OnNext(default);
            Advertisement.Load(_adsConfig.AndroidPlacementId, this);
        }

        void IUnityAdsInitializationListener.OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            DebugLogger.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        void IUnityAdsLoadListener.OnUnityAdsAdLoaded(string adUnitId)
        {
            // Optionally execute code if the Ad Unit successfully loads content.
            DebugLogger.Log($"Unity Ads Loaded");

        }

        void IUnityAdsLoadListener.OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            DebugLogger.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        }

        void IUnityAdsShowListener.OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            DebugLogger.Log("OnUnityAdsShowFailure");
            _onAdsShowFailure.OnNext(default);
        }

        void IUnityAdsShowListener.OnUnityAdsShowStart(string placementId)
        {
            DebugLogger.Log("OnUnityAdsShowStart");
        }

        void IUnityAdsShowListener.OnUnityAdsShowClick(string placementId)
        {
            DebugLogger.Log("OnUnityAdsShowClick");
        }

        void IUnityAdsShowListener.OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            DebugLogger.Log("OnUnityAdsShowComplete");

            _onAdsShowComplete.OnNext(default);
        }
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

}