using System.Collections;
using UnityEngine;
using UnityEditor;

public class MakeAbilityObject  {

	[MenuItem("Assets/Create/Ability Object")]
	public static void Create()
	{
		AbilityObject asset = ScriptableObject.CreateInstance<AbilityObject> ();
		AssetDatabase.CreateAsset (asset, "Assets/NewAbilityObject.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}
