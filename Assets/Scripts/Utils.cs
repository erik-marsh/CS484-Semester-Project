using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
	public static void ExitUnity()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();	
		#endif
	}
}
