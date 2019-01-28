using AppodealAds.Unity.Api;
using Statics;
using UnityEngine;

namespace AppodealADS
{
	public class AppodealController : MonoBehaviour 
	{
		private void Awake()
		{
			const string appKey = "d2210e02d4e4d7de575df44a63a6584928e9f0568d9a14b3";
			
			Appodeal.initialize(appKey, Appodeal.REWARDED_VIDEO, true);
			
			Static.Load();
		}
	}
}

