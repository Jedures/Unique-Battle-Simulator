using System.Globalization;
using Player;
using Singleton;
using Statics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LevelController
{
	public class SpawnAllies : MonoBehaviour
	{
		[Header("Camera")] 
		public UnityEngine.Camera myCamera;

		public Transform line;
		
		[Header("Prefabs")] 
		public GameObject playerPrefab;

		[FormerlySerializedAs("HealthText")] [Header("UI")]
		public Text healthText;
		public Text damageText;
		public Text priceText;

		public Text moneyText;

		private void Awake()
		{
			ChangeText();
		}
		
		private void SpawnPrefab()
		{
			if (playerPrefab.GetComponent<PlayerController>().soldierCharacteristic.price > Static.Money)
				return;
			
			RaycastHit hit;
			
			var ray = myCamera.ScreenPointToRay(Input.mousePosition);
			
			if (!Physics.Raycast(ray, out hit)) 
				return;
			
			if (EventSystem.current.IsPointerOverGameObject())
				return;
			
			if (!hit.transform.gameObject.CompareTag("terrain")) 
				return;

			if (hit.distance > Vector3.Distance(myCamera.transform.position, line.position))
				return;
			
			var position = hit.point;
			
			var player = Instantiate(playerPrefab, position, Quaternion.identity);
			
			Static.Money -= playerPrefab.GetComponent<PlayerController>().soldierCharacteristic.price;
			
			LevelManager._levelManager.AddAllieToList(player);
			
			ChangeText();
		}
		private void Update ()
		{
			if (LevelManager._levelManager.isStart)
				return;
			
			#if UNITY_EDITOR
			
			if (Input.GetMouseButtonDown(0)) 
				SpawnPrefab();
			
			#endif
			
			#if UNITY_ANDROID
			
			if (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Ended)
				SpawnPrefab();
			
			#endif
		}

		public void ChangePrefab(GameObject newPrefab)
		{
			playerPrefab = newPrefab;

			ChangeText();
		}

		private void ChangeText()
		{
			healthText.text = playerPrefab.GetComponent<PlayerController>().soldierCharacteristic.health.ToString(CultureInfo.CurrentCulture);
			damageText.text = playerPrefab.GetComponent<PlayerController>().soldierCharacteristic.damage.ToString(CultureInfo.CurrentCulture);
			priceText.text = playerPrefab.GetComponent<PlayerController>().soldierCharacteristic.price.ToString(CultureInfo.CurrentCulture);
			
			moneyText.text = Static.Money.ToString();
		}

	}
}
