using System.Collections.Generic;
using Enums;
using Statics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DialogueSystem
{
	public class Dialogue : MonoBehaviour
	{
		private int m_I = -1;

		public bool isStoryLevel = true;

		public int levelNumber = 1;
		
		[Header("Dialog Elements")]
		public List<Sprite> sprites;

		public List<Sprite> backgroundSprites;
		
		public List<string> dialogueRus;
		public List<string> dialigueEng;

		[Header("UI")] 
		public Text dialogText;
		public Image dialogSprite;
		public Image background;

		public string levelName;

		private int m_LevelDialog = 0;

		public void firstClick()
		{
			Next();
		}

		public void FirstClick(int lvl)
		{
			m_LevelDialog = lvl;
			
			Next();
		}

		private void Start()
		{
			if(isStoryLevel)
				return;

			Next();
		}
		
		public void Next()
		{	
			if ((m_LevelDialog != levelNumber) && (isStoryLevel))
				return;
			
			if (m_I == -1)
			{
				dialogText.gameObject.SetActive(true);
				dialogSprite.gameObject.SetActive(true);
			}
			if (sprites.Count > m_I + 1)
			{
				m_I++;
				
				dialogText.text = Static.Language == Localisation.Rus ? dialogueRus[m_I] : dialigueEng[m_I];
				dialogSprite.sprite = sprites[m_I];
				
				if(!isStoryLevel)
					return;
				
				background.sprite = backgroundSprites[m_I];
			}
			else 
				SceneManager.LoadScene(levelName);
		}
	}
}
