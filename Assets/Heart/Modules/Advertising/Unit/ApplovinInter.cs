﻿using System;
using Pancake.Common;

// ReSharper disable AccessToStaticMemberViaDerivedType
namespace Pancake.Monetization
{
    [Serializable]
    public class ApplovinInter : AdUnit
    {
        [NonSerialized] internal Action completedCallback;

        private bool _registerCallback;

        public override void Load()
        {
#if PANCAKE_ADVERTISING && PANCAKE_APPLOVIN
            if (Advertising.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
            if (!_registerCallback)
            {
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnAdLoaded;
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdLoadFailed;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaid;
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnAdDisplayed;
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHidden;
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdDisplayFailed;
                _registerCallback = true;
            }

            MaxSdk.LoadInterstitial(Id);
#endif
        }

        public override bool IsReady()
        {
#if PANCAKE_ADVERTISING && PANCAKE_APPLOVIN
            return !string.IsNullOrEmpty(Id) && MaxSdk.IsInterstitialReady(Id);
#else
            return false;
#endif
        }

        protected override void ShowImpl()
        {
#if PANCAKE_ADVERTISING && PANCAKE_APPLOVIN
            MaxSdk.ShowInterstitial(Id);
#endif
        }

        protected override void ResetChainCallback()
        {
            base.ResetChainCallback();
            completedCallback = null;
        }

        public override void Destroy() { }

#if PANCAKE_ADVERTISING && PANCAKE_APPLOVIN
        private void OnAdDisplayFailed(string unit, MaxSdkBase.ErrorInfo error, MaxSdkBase.AdInfo info) { C.CallActionClean(ref failedToDisplayCallback); }

        private void OnAdHidden(string unit, MaxSdkBase.AdInfo info)
        {
            Advertising.isShowingAd = false;
            C.CallActionClean(ref completedCallback);
            
            if (!string.IsNullOrEmpty(Id)) MaxSdk.LoadInterstitial(Id);  // ApplovinEnableRequestAdAfterHidden as true
        }

        private void OnAdDisplayed(string unit, MaxSdkBase.AdInfo info)
        {
            Advertising.isShowingAd = true;
            C.CallActionClean(ref displayedCallback);
        }

        private void OnAdRevenuePaid(string unit, MaxSdkBase.AdInfo info)
        {
            paidedCallback?.Invoke(info.Revenue,
                info.NetworkName,
                unit,
                info.AdFormat,
                EAdNetwork.Applovin.ToString());
        }

        private void OnAdLoadFailed(string unit, MaxSdkBase.ErrorInfo error) { C.CallActionClean(ref failedToLoadCallback); }

        private void OnAdLoaded(string unit, MaxSdkBase.AdInfo info) { C.CallActionClean(ref loadedCallback); }

#endif
    }
}