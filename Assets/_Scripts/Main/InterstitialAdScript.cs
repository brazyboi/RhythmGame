using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;


public class InterstitialAdScript : MonoBehaviour
{
    private InterstitialAd interstitialAd;
    private bool isLoaded;
    void Start()
    {
        isLoaded = false;
        AudienceNetworkAds.Initialize();
        LoadInterstitial();
        
    }
    public void LoadInterstitial()
    {
        this.interstitialAd = new InterstitialAd("530781888165908_533251127918984");
        this.interstitialAd.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.interstitialAd.InterstitialAdDidLoad = (delegate () {
            Debug.Log("Interstitial ad loaded.");
            this.isLoaded = true;
        });
        interstitialAd.InterstitialAdDidFailWithError = (delegate (string error) {
            Debug.Log("Interstitial ad failed to load with error: " + error);
        });
        interstitialAd.InterstitialAdWillLogImpression = (delegate () {
            Debug.Log("Interstitial ad logged impression.");
        });
        interstitialAd.InterstitialAdDidClick = (delegate () {
            Debug.Log("Interstitial ad clicked.");
        });


        this.interstitialAd.interstitialAdDidClose = (delegate () {
            Debug.Log("Interstitial ad did close.");
            if (this.interstitialAd != null)
            {
                this.interstitialAd.Dispose();
            }
        });

        // Initiate the request to load the ad.
        this.interstitialAd.LoadAd();
    }
    public void ShowInterstitial()
    {
        if (this.isLoaded)
        {
            this.interstitialAd.Show();
            this.isLoaded = false;

        }
        else
        {
            Debug.Log("Interstitial Ad not loaded!");
        }
    }

    public IEnumerator showAdWhenReady()
    {
        Debug.Log("LOADED OR NOT: " + isLoaded);
        ShowInterstitial();
        while (!this.isLoaded)
        {
            Debug.Log("Is loading ad.");
            ShowInterstitial();
            yield return new WaitForSeconds(1.0f);
        }
    }

}
