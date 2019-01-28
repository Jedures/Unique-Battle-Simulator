using Enums;
using UnityEngine;

namespace Statics
{
	public static class Static
	{
		public static Localisation Language = Localisation.Rus;
		
		public static int Money = 160;

		public static int LevelComplete = 0;

		public static void Save()
		{
			PlayerPrefs.SetInt("Money", Money);
			
			PlayerPrefs.SetInt("LevelComplete", LevelComplete);
			
			PlayerPrefs.SetString("Language", Language.ToString());
		}

		public static void Load()
		{
			PlayerPrefs.DeleteAll();
			
			if (!PlayerPrefs.HasKey("Money"))
				return;
			
			Money = PlayerPrefs.GetInt("Money");

			LevelComplete = PlayerPrefs.GetInt("LevelComplete");

			Language = (Localisation)System.Enum.Parse(typeof(Localisation), PlayerPrefs.GetString("Language"));
		}
	}
}