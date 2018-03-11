using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityObject : ScriptableObject {

	public string abilityObject = "Ability Name Here";
	public int cost = 0;
	public string description;

	//NOTE: Player 0 = platforming scientist, Player 1 = scaling ghost
	public int player = 0;
	public float fireRate = .5f;
	public int damage = 10;
	public float range = 100;

}
