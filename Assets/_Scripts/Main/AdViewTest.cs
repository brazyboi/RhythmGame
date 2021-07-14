using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

public class AdViewTest : MonoBehaviour
{

    private AdView adView;
    // Start is called before the first frame update
    void Start()
    {
        LoadBanner();
    }

    public void LoadBanner()
    {
        if (this.adView)
        {
            this.adView.Dispose();
        }

        this.adView = new AdView("TEST_AD_TYPE#YOUR_PLACEMENT_ID", AdSize.BANNER_HEIGHT_50);
        this.adView.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.adView.AdViewDidLoad = (delegate () {
            Debug.Log("Banner loaded.");
            this.adView.Show(100);
        });
        adView.AdViewDidFailWithError = (delegate (string error) {
            Debug.Log("Banner failed to load with error: " + error);
        });
        adView.AdViewWillLogImpression = (delegate () {
            Debug.Log("Banner logged impression.");
        });
        adView.AdViewDidClick = (delegate () {
            Debug.Log("Banner clicked.");
        });

        // Initiate a request to load an ad.
        adView.LoadAd();

        this.adView.AdViewDidLoad = (delegate () {
            Debug.Log("Banner loaded.");
            this.adView.Show(100);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
