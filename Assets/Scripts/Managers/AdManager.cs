using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    void Start()
    {
    #if UNITY_ANDROID
            string appId = "ca-app-pub-4709635254723192~5674517136";
    #elif UNITY_IPHONE
        
    #else
            string appId = "unexpected_platform";
    #endif
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);
            this.RequestRewardedAd();
    }


    // Android Ads -
    private RewardedAd rewardedAd;

    // 보상형 광고
    private void RequestRewardedAd()
    {
        string adUnitId;
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-4709635254723192/2264372270";
            //adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator) // 테스트 광고 요청
            .Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        RequestRewardedAd();
    }

    Status status;
    void HandleUserEarnedReward(object sender, Reward args)
    {
        //NotEnough
        if(!status)
            status = GameObject.Find("Status").GetComponent<Status>();

        // 스테미나를 100 향상시킴
        float tempStamina = PlayerPrefs.GetFloat("Stamina");

        status.StatusIncrease(100f, ref tempStamina);
        PlayerPrefs.SetFloat("Stamina", tempStamina);

        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);

        //여기서 Reward가 생기는 모양 
    }

    // 광고는 30분에 한번씩 
    public void _ShowRewardedAd()
    {
        float LastTime = 0f;
        float OnTime = 0f;
        CalculateUtility.TimeToNumber(PlayerPrefs.GetString("ADTime"), ref LastTime);
        CalculateUtility.TimeToNumber(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"), ref OnTime);

        if (OnTime - LastTime > 30) 
        {
            if (this.rewardedAd.IsLoaded())
            {
                PlayerPrefs.SetString("ADTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));
                this.rewardedAd.Show();
            }
            else
            {
                Debug.Log("NOT Loaded Interstitial");
                RequestRewardedAd();
            }
        }
    }
}