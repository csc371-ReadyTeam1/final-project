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
		void Start () {
			source = GetComponent<AudioSource> ();
			SetButton ();
		}

		void SetButton()
		{
//			name.text = pgc.abilities[abilityNum].name;
//			description.text = pgc.abilities[abilityNum].description;
		}

		public void OnClick()
		{
			pgc.numBullets = bulletNumBuff;
		}

		// Update is called once per frame
		void Update () {

		}
	}
}