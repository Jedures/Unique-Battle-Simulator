using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Appodeal.Unity.Editor.iOS;

public class AppodealPostProcess : MonoBehaviour {

	[PostProcessBuild(100)]
	public static void OnPostProcessBuild (BuildTarget target, string path) {		
		if (target.ToString () == "iOS" || target.ToString () == "iPhone") {
            iOSPostprocessUtils.PrepareProject (path);
            iOSPostprocessUtils.UpdatePlist(path);
		}
	}

}