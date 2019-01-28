using System.Collections.Generic;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StoryMode
{
	public class LevelHide : MonoBehaviour
	{
		public List<GameObject> levelsButton;
		
		private void Awake()
		{
			for (var i = 0; i<Static.LevelComplete+1; i++)
				levelsButton[i].SetActive(true);
		}

		public void Hide()
		{
			foreach (var but in levelsButton)
			{
				but.SetActive(false);
			}	
		}

		public void BackToMenu() => SceneManager.LoadScene(0);
	}
}
