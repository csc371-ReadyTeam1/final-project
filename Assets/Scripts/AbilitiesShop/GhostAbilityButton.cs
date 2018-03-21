using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
	public class GhostAbilityButton : MonoBehaviour {
		public PlayerGhostController pgc;
		public int bulletNumBuff;

		public Text name;
		public Text description;

		public AudioSource source;

		// Use this for initialization
		/* Contributors: Megan Washburn */
		void Start () {
			source = GetComponent<AudioSource> ();
			SetButton ();
		}

		/* Contributors: Megan Washburn */
		void SetButton()
		{
//			name.text = pgc.abilities[abilityNum].name;
//			description.text = pgc.abilities[abilityNum].description;
		}

		/* Contributors: Megan Washburn */
		public void OnClick()
		{
			pgc.numBullets = bulletNumBuff;
		}
	}
}