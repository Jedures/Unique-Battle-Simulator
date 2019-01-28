using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cameras
{
	public sealed class CamJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler 
	{
		#region vars
		
		#region public vars
		
		public Image background;
        public Image knob;
        
		public float sensitivity;
		public float zoomSpeed;
		
		private float rotateSpeed = 0.4f ;
		
		public Slider zoomIndicator;
		
		public Animator leftPanel;
	
		public Camera myCamera;
		
		#endregion
		
		#region private vars

		private Transform m_CameraController;
		private Vector3 m_Direction;
	
		private float m_StartX;
		private float m_StartY;
		
		private float m_StartRotationY;
		private float m_StartRotationX;
	
		private bool m_Joystick;
		
		[SerializeField] 
		private bool noTouch;

		[SerializeField]
		private bool isZoom = false;
	
		private static readonly int Hide = Animator.StringToHash("Hide");
		
		#endregion
		
		#endregion

		#region Unity Methods
		
		private void Start()
		{
			m_CameraController = myCamera.transform;
			
			m_Direction = Vector3.zero;
		}

		public void Update(){
		
			m_CameraController.Translate(Time.deltaTime * m_Direction * sensitivity);
		
			if(!leftPanel.GetBool(Hide))
				return;
			
			Zoom();

			foreach (var touch in Input.touches)
			{
				if(touch.position.x > Screen.width/2.0)
					Rotate(touch);
			}
		}
		
		#endregion
		
		#region Interfaces

		public void OnDrag(PointerEventData data)
		{
			Vector2 pos;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, data.position,
				data.pressEventCamera, out pos)) 
				return;
			
			m_Joystick = true;

			var rectTransform = background.rectTransform;
			var sizeDelta = rectTransform.sizeDelta;
			
			pos.x /= sizeDelta.x;	
			pos.y /= sizeDelta.y;	
			
			var pivot = rectTransform.pivot;
			
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			var x = (pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
			
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			var y = (pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;
			
			m_Direction = new Vector3(x, 0, y);
			
			if(m_Direction.magnitude > 1)
				m_Direction.Normalize();

			var rectTransform1 = background.rectTransform;
			var delta = rectTransform1.sizeDelta;
			
			knob.rectTransform.anchoredPosition = new Vector3(m_Direction.x * (delta.x/2.5f), m_Direction.z * (delta.y/2.5f));
		}
	
		public void OnPointerDown(PointerEventData data)
		{
			OnDrag(data);
		}
	
		public void OnPointerUp(PointerEventData data)
		{
			m_Direction = Vector3.zero;
		
			knob.rectTransform.anchoredPosition = Vector3.zero;
		
			m_Joystick = false;
		}
		
		#endregion
		
		#region Camera Controllers

		private void Zoom()
		{
			if (Input.touchCount == 0)
			{
				noTouch = true;
				isZoom = false;
			}

			if(Input.touchCount == 2)
			{	
				if(m_Joystick)
					return;
				
				var touchZero = Input.GetTouch(0);
				var touchOne = Input.GetTouch(1);

				var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				var z = (prevTouchDeltaMag - touchDeltaMag) * 0.001f * zoomSpeed;
			
				if((z > 0 && myCamera.fieldOfView < 100) || (z < 0 && myCamera.fieldOfView > 15))
				{
					myCamera.fieldOfView += z;
				}
			
				noTouch = false;
				isZoom = true;
				
				zoomIndicator.gameObject.SetActive(true);
				
				zoomIndicator.value = myCamera.fieldOfView;
				
			}
			else
			{
				zoomIndicator.gameObject.SetActive(false);
				isZoom = false;
			}
		}

		private void Rotate(Touch touch)
		{
			if (isZoom)
				return;
			
			Vector3 eulerAngles;
			if(touch.phase == TouchPhase.Began)
			{
				m_StartX = touch.position.x;
				m_StartY = touch.position.y;

				eulerAngles = m_CameraController.transform.eulerAngles;
				m_StartRotationX = eulerAngles.x;
				m_StartRotationY = eulerAngles.y;
			}

			if (touch.phase != TouchPhase.Moved) 
				return;
		
			eulerAngles = myCamera.transform.eulerAngles;
				
			m_CameraController.transform.eulerAngles = new Vector3(m_StartRotationX - (touch.position.y - m_StartY) * rotateSpeed, m_StartRotationY + ((touch.position.x - m_StartX) * rotateSpeed), 0);
		}
		
		#endregion
	}
}
