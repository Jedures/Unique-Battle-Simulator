using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using Enums;
using Singleton;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
	public class MenuController : MonoBehaviour, IRewardedVideoAdListener
	{
		[Header("Settings")]
		public Slider volumeSlider;

		[Header("Money UI")] 
		public Text moneyText;
		
		public void StartStory() => SceneManager.LoadScene("Story");

		public void StartSandBox() => SceneManager.LoadScene("SandBox");

		public void Exit() => Application.Quit();

		private void Update() => moneyText.text =
			Static.Language == Localisation.Eng ? "Money: " + Static.Money : "Денег: " + Static.Money;
		
		public void ChangeLanguage()
		{
			var uiText = Resources.FindObjectsOfTypeAll<Language>();
			
			Static.Language = Static.Language != Localisation.Eng ? Localisation.Eng : Localisation.Rus;

			Static.Save();
			
			foreach (var language in uiText)
				language.ChangeLanguage();
		}

		public void ChangeVolume() => GameManager.gameManager.audioLevel = volumeSlider.value;

		public void Ads()
		{
			if (!Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
				return;
			
			Appodeal.show(Appodeal.REWARDED_VIDEO);
			
			Appodeal.setRewardedVideoCallbacks(this);
		}

		public void ShowLeaderBoard()
		{
			Social.ShowLeaderboardUI();
		}
		
		#region Appodeal Callbacks

		public void onRewardedVideoLoaded(bool precache)
		{
			throw new System.NotImplementedException();
		}

		public void onRewardedVideoFailedToLoad()
		{
			throw new System.NotImplementedException();
		}

		public void onRewardedVideoShown()
		{
			throw new System.NotImplementedException();
		}

		public void onRewardedVideoFinished(double amount, string name)
		{
			Static.Money += 150;
		}

		public void onRewardedVideoClosed(bool finished)
		{
			throw new System.NotImplementedException();
		}

		public void onRewardedVideoExpired()
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
		
	}
}
