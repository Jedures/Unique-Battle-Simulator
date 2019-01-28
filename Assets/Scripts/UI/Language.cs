using Enums;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Language : MonoBehaviour
	{
		private Text m_Text;
		
		public string rusText;
		public string engText;

		private void Start ()
		{
			m_Text = GetComponent<Text>();
			
			ChangeLanguage();

			Static.Save();
		}

		public void ChangeLanguage() => m_Text.text = Static.Language == Localisation.Rus ? rusText : engText;
	}
}
