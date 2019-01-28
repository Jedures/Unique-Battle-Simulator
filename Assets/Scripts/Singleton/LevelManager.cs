using System.Collections.Generic;
using Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Singleton
{
	public class LevelManager : MonoBehaviour
	{
		#region vars
		
		#region public vars
		
		[HideInInspector]
		public static LevelManager _levelManager = null;
		
		[HideInInspector]
		public bool isStart = false;
	    
		[Header("Level Setting")]
		public List<GameObject> enemyPrefab;

		[SerializeField] 
		private int countEnemies;
	  
		[Header("UI Animators")] 
    	public List<Animator> animators;
		
        [Header("UI Panels")] 
	    public GameObject winPanel;
        public GameObject losePanel;

        public GameObject joysticPanel;
        
        public List<GameObject> spawnPoints = new List<GameObject>();
		
        #endregion
        
        #region private vars
        
        [SerializeField] 
        private bool isSandbox;
        
        [SerializeField] 
        private List<GameObject> enemies = new List<GameObject>();
		
        [SerializeField] 
		private List<GameObject> allies = new List<GameObject>();

		[Header("Level Settings")]
		[SerializeField]
		private int levelNumber = 0;

		[SerializeField]
		private int levelWin;

		private bool m_IsBattle = true;

		private int m_MoneyCash;

		private int m_EnemyCount;
		
		private static readonly int Close = Animator.StringToHash("Close");
		private static readonly int Hide = Animator.StringToHash("Hide");
		
		#endregion
		
		#endregion
		
		#region Unity Method

		private void Awake()
		{
			if (_levelManager == null) 
				_levelManager = this;
			else 
				Destroy(_levelManager);

			m_MoneyCash = Static.Money;
		}

		private void Start()
		{
			SpawnEnemies(countEnemies);
		}
		
		#endregion
		
		#region Lists Controller
		
		private void SpawnEnemies(int count)
		{
			spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("spawn"));

			if (isSandbox)
				count = spawnPoints.Count;
			
			for (var i = 0; i < count; i++)
			{
				var j = Random.Range(0, spawnPoints.Count);
				var pos = spawnPoints[j].transform;
				
				spawnPoints.Remove(spawnPoints[j]);
				
				AddEnemyToList(Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Count)], pos));
			}

			m_EnemyCount = enemies.Count;

		}

		public void DeleteEnemyFromList(GameObject unit)
		{
			enemies.Remove(unit);
			
			FinishGame();
		}
        		
		private void AddEnemyToList(GameObject unit) => enemies.Add(unit);

		public void DeleteAllieFromList(GameObject unit)
		{
			allies.Remove(unit);
			
			FinishGame();
		}
		
		public void AddAllieToList(GameObject unit) => allies.Add(unit);
		
		public List<GameObject> GetEnemyList() => enemies;

		public List<GameObject> GetAlliesList() => allies;
		
		#endregion
		
		#region Level Manager

		public void StartGame()
		{
			isStart = true;

			foreach (var animator in animators)
			{
				animator.SetTrigger(Close);
				animator.SetBool(Hide, true);
			}
			
			joysticPanel.SetActive(true);

			FinishGame();
		}

		public void Back(string scene) => SceneManager.LoadScene(scene);
		
		private void FinishGame()
		{
			if (!m_IsBattle)
				return;
			
			if (enemies.Count > 0 && allies.Count > 0) 
				return;

			if (enemies.Count <= 0 && allies.Count > 0)
				Win();
			else
				Lose();
		}

		private void Win()
		{
			m_IsBattle = false;
			
			Static.Money += levelWin;

			foreach (var allie in allies)
			{
				Static.Money += allie.GetComponent<PlayerController>().soldierCharacteristic.price;
			}
			
			winPanel.SetActive(true);

			if (_levelManager.isSandbox)
				return;

			Static.LevelComplete = levelNumber;
			
			Social.ReportScore(m_EnemyCount - enemies.Count, GPGSIds.leaderboard_kills, (bool success) => {});
			
			Static.Save();
		}

		private void Lose()
		{
			m_IsBattle = false; 

			if (!_levelManager.isSandbox)
				Static.Money = m_MoneyCash;
			
			losePanel.SetActive(true);
			
			Social.ReportScore(m_EnemyCount - enemies.Count, GPGSIds.leaderboard_kills, (bool success) => {});
			
			Static.Save();
		}
		
		#endregion
	}
}
