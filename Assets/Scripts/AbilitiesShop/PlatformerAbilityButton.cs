using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
	public class PlatformerAbilityButton : MonoBehaviour {
		public PlayerPlatformerController ppc;
		// Will be negative to decrease time stunned
		public float stunTime; 

		public EnergyShield es;
		// Will be positive to increase shield duration
		public float shieldLifetime;

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
//			name.text = ppc.abilities[abilityNum].name;
//			description.text = pgc.abilities[abilityNum].description;
		}

		public void OnClick()
		{
//			ppc.stunDuration += stunTime; //will have negative buff value
			es.lifeTime += shieldLifetime;
			source.Play ();
		}

		// Update is called once per frame
		void Update () {

		}
	}
}