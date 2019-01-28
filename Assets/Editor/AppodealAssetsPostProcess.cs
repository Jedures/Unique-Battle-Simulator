using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using Unity.Appodeal.Xcode.PBX;

public class AppodealAssetsPostProcess : AssetPostprocessor {

	static string[] Plugins = new string[] {
		"adcolony",
		"appodeal",
		"applovin",
		"chartboost",
		"facebook",
		"inmobi",
		"ironsource",
		"mobvistaalphab",
		"mobvistacommon",
		"mobvistainterstitial",
		"mobvistainterstitialvideo",
		"mobvistamvdownloads",
		"mobvistamvjscommon",
		"mobvistamvnative",
		"mobvistanativeex",
		"mobvistaplayercommon",
		"mobvistareward",
		"mobvistavideocommon",
		"mobvistavideofeeds",
		"mobvistavideojs",
		"mopub",
		"mytarget",
		"ogury",
		"startapp",
		"tapjoy",
		"unityads",
		"vungle",
		"yandex-metrica",
		"yandex-mobileads"
	};

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {

		#if UNITY_4 && UNITY_IPHONE
		if(PlayerSettings.apiCompatibilityLevel.Equals(ApiCompatibilityLevel.NET_2_0_Subset)) {
			if (EditorUtility.DisplayDialog("Appodeal Unity", "We have detected that you're using subset API compatibility level: " + PlayerSettings.apiCompatibilityLevel + " you should change it to NET_2_0 to be able using Ionic.ZIP.dll.", "Change it for me", "I'll do it")) {
				PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
			}
		}
		#endif

		#if UNITY_4
		if(File.Exists(Utils.FixSlashesInPath("Assets/Appodeal/SampleAndroidManifest.xml")) && !File.Exists(Utils.FixSlashesInPath("Assets/Plugins/Android/AndroidManifest.xml")) && moveManifest) {
			if (EditorUtility.DisplayDialog("Appodeal Unity", "We have detected that you have no AndroidManifest.xml inside Android folder, it should override Unity's settings to fix not clickable ads", "Place template for me", "Don't do that!")) {
				FileUtil.CopyFileOrDirectory(Utils.FixSlashesInPath("Assets/Appodeal/SampleAndroidManifest.xml"), Utils.FixSlashesInPath("Assets/Plugins/Android/AndroidManifest.xml"));
			}
		}
		#endif

		#if UNITY_5
		foreach(string plugin in Plugins) {
			string fullpath = Utils.FixSlashesInPath("Assets/Plugins/Android/" + plugin);
			if(File.Exists(fullpath)) {
				PluginImporter pluginImporter = AssetImporter.GetAtPath(fullpath) as PluginImporter;
				if(!pluginImporter.GetCompatibleWithPlatform(BuildTarget.Android)) {
					pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
				}
			}
		}

		string path = Utils.FixSlashesInPath("Assets/Plugins/Ionic.Zip.Unity.dll");
		if(File.Exists(path)) {
			PluginImporter ionicPluginImporter = AssetImporter.GetAtPath(path) as PluginImporter;
			if(!ionicPluginImporter.GetCompatibleWithEditor()) {
				ionicPluginImporter.SetCompatibleWithEditor(true);
			}
		}
		#endif
	}

	public static void reimportFolder(string path) {
		string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
		foreach(string file in files) {
			if (file.EndsWith(".DS_Store", System.StringComparison.Ordinal)) {
				continue;
			} else if (file.EndsWith(".meta", System.StringComparison.Ordinal)) {
				continue;
			} else {
				AssetDatabase.ImportAsset(Utils.FixSlashesInPath(file));
			}
		}
	}
}