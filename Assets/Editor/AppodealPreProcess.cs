using UnityEditor;
using UnityEditor.Build;
//using Appodeal.Unity.Editor.Android;

class AppodealPreProcess : IPreprocessBuild //exists only from unity 5.6
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.Android)
        {
            //ManifestMod.UpdateOguryManifest();
        }
    }
}