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
		/* Contributors: Megan Washburn */
		void Start () {
			source = GetComponent<AudioSource> ();
			SetButton ();
		}

		/* Contributors: Megan Washburn */
		void SetButton()
		{
//			name.text = ppc.abilities[abilityNum].name;
//			description.text = pgc.abilities[abilityNum].description;
		}

		/* Contributors: Megan Washburn */
		public void OnClick()
		{
//			ppc.stunDuration += stunTime; //will have negative buff value
			es.lifeTime += shieldLifetime;
			source.Play ();
		}
	}
}