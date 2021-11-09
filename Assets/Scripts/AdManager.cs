using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yodo1.MAS;

public class AdManager : MonoBehaviour
{
    private int buttonIndex = -1;
    // Start is called before the first frame update

    public static AdManager instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Yodo1U3dMas.InitializeSdk();

        Yodo1U3dMas.SetBannerAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) => {
            Debug.Log("[Yodo1 Mas] BannerdDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Banner ad has been closed.");
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Banner ad has been shown.");
                    break;
                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Banner ad error, " + error.ToString());
                    break;
            }
        });

        Yodo1U3dMas.SetRewardedAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) => {
            Debug.Log("[Yodo1 Mas] RewardVideoDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Reward video ad has been closed.");
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Reward video ad has shown successful.");
                    break;
                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Reward video ad error, " + error);
                    break;
                case Yodo1U3dAdEvent.AdReward:
                    switch (buttonIndex)
                    {
                        case 1: // next level button
                            LoadNextScene();
                            break;
                    }
                    LoadNextScene();
                    Debug.Log("[Yodo1 Mas] Reward video ad reward, give rewards to the player.");
                    break;
            }

        });
    }

    public void ShowBannerAd()
    {
        int align = Yodo1U3dBannerAlign.BannerBottom;
        Yodo1U3dMas.ShowBannerAd(align);
    }

    public void ShowRewardedAds(int buttonIndex)
    {
        this.buttonIndex = buttonIndex;
        bool isLoaded = Yodo1U3dMas.IsRewardedAdLoaded();
        if (isLoaded)
        {
            Yodo1U3dMas.ShowRewardedAd();
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void CloseBannerAd()
    {
        Yodo1U3dMas.DismissBannerAd();
    }
}
