using UnityEngine;
using Unity.Appodeal.Xcode;
using Unity.Appodeal.Xcode.PBX;

using System;
using System.IO;

namespace Appodeal.Unity.Editor.iOS
{

    public class iOSPostprocessUtils : MonoBehaviour
    {
        static string suffix = ".framework";
        static string absoluteProjPath;

#if UNITY_4
        static string AppodealFramework = "Plugins/iOS/Appodeal.framework";
#endif

        static string[] frameworkList = new string[] {
        "AdSupport", "AudioToolbox", "AVFoundation", "CFNetwork", "CoreBluetooth",
        "CoreFoundation", "CoreGraphics", "CoreImage",
        "CoreLocation", "CoreMedia", "CoreMotion", "CoreTelephony",
        "CoreText", "EventKit", "EventKitUI", "GLKit",
        "ImageIO", "JavaScriptCore", "MediaPlayer", "MessageUI",
        "MobileCoreServices", "QuartzCore", "SafariServices", "Security",
        "Social", "StoreKit", "SystemConfiguration", "Twitter",
        "UIKit", "QuartzCore", "WebKit", "WatchConnectivity"
    };

        static string[] weakFrameworkList = new string[] {
        "CoreMotion", "WebKit", "Social"
    };


        static string[] platformLibs = new string[] {
        "libc++.dylib",
        "libz.dylib",
        "libsqlite3.dylib",
        "libxml2.2.dylib"
    };



        public static void PrepareProject(string buildPath)
        {
            Debug.Log("preparing your xcode project for appodeal");
            string projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            absoluteProjPath = Path.GetFullPath(buildPath);
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projPath));
            string target = project.TargetGuidByName("Unity-iPhone");

            AddProjectFrameworks(frameworkList, project, target, false);
            AddProjectFrameworks(weakFrameworkList, project, target, true);
            AddProjectLibs(platformLibs, project, target);
            project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            project.AddBuildProperty(target, "ENABLE_BITCODE", "YES");
            project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries");

            string apdFolder = "Adapters";
            string appodealPath = Path.Combine(Application.dataPath, "Appodeal");
            string adaptersPath = Path.Combine(appodealPath, apdFolder);
            if (Directory.Exists(adaptersPath))
            {
                foreach (string file in Directory.GetFiles(adaptersPath))
                {
                    if (Path.GetExtension(file).Equals(".zip"))
                    {
                        Debug.Log("unzipping:" + file);
                        ExtractZip(file, Path.Combine(absoluteProjPath, apdFolder));
                        AddAdaptersDirectory(apdFolder, project, target);
                    }
                }
            }
            string resourcesFolder = "InternalResources";
            string chZip = "CrashHunter.zip";
            string resourcesPath = Path.Combine(appodealPath, resourcesFolder);
            MacOSUnzip(Path.Combine(resourcesPath, chZip), buildPath);
            project.AppendShellScriptBuildPhase(target, "Run Script ChashHunter", "/bin/sh", "$PROJECT_DIR/CrashHunterScript.sh");

#if UNITY_4
        project.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/Plugins/iOS");
        project.SetBuildProperty (target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries");
        CopyAndReplaceDirectory ("Assets/" + AppodealFramework, Path.Combine(buildPath, "Frameworks/" + AppodealFramework));
        project.AddFileToBuild(target, project.AddFile("Frameworks/" + AppodealFramework, "Frameworks/" + AppodealFramework, PBXSourceTree.Source));
#endif

            File.WriteAllText(projPath, project.WriteToString());
        }

        static void MacOSUnzip(string source, string dest)
        {
            try {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "unzip",
                    Arguments = source + " -d " + dest
                };
                System.Diagnostics.Process proc = new System.Diagnostics.Process() { StartInfo = startInfo, };
                bool started = proc.Start();
            }
            catch(Exception e){
                Debug.Log(e.Message);
                ExtractZip(source, dest);
            }
        }

        protected static void AddProjectFrameworks(string[] frameworks, PBXProject project, string target, bool weak)
        {
            foreach (string framework in frameworks)
            {
                if (!project.ContainsFramework(target, framework))
                {
                    project.AddFrameworkToProject(target, framework + suffix, weak);
                }
            }
        }

        protected static void AddProjectLibs(string[] libs, PBXProject project, string target)
        {
            foreach (string lib in libs)
            {
                string libGUID = project.AddFile("usr/lib/" + lib, "Libraries/" + lib, PBXSourceTree.Sdk);
                project.AddFileToBuild(target, libGUID);
            }
        }

        public static void UpdatePlist(string buildPath)
        {
#if UNITY_4
        string plistPath = Path.Combine (buildPath, "Info.plist");
        PlistDocument plist = new PlistDocument ();
        plist.ReadFromString(File.ReadAllText (plistPath)); 
        PlistElementDict dict = plist.root.CreateDict ("NSAppTransportSecurity");
        dict.SetBoolean ("NSAllowsArbitraryLoads", true);
        File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }

        static void CopyAndReplaceDirectory(string srcPath, string dstPath)
        {
            if (Directory.Exists(dstPath))
            {
                Directory.Delete(dstPath);
            }
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }

            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath))
            {
                if (!file.Contains(".meta"))
                {
                    File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
                }
            }

            foreach (var dir in Directory.GetDirectories(srcPath))
            {
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
            }
        }

        static void ExtractZip(string filePath, string destFolder)
        {
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(filePath))
            {
                foreach (Ionic.Zip.ZipEntry z in zip)
                {
                    z.Extract(destFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        static void AddAdaptersDirectory(string path, PBXProject proj, string targetGuid)
        {
            if (path.EndsWith("__MACOSX", StringComparison.CurrentCultureIgnoreCase))
                return;

            if (path.EndsWith(".framework", StringComparison.CurrentCultureIgnoreCase))
            {
                proj.AddFileToBuild(targetGuid, proj.AddFile(path, path));
                string tmp = Utils.FixSlashesInPath(string.Format("$(PROJECT_DIR)/{0}", path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar))));
                proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", tmp);
                return;
            }
            else if (path.EndsWith(".bundle", StringComparison.CurrentCultureIgnoreCase))
            {
                proj.AddFileToBuild(targetGuid, proj.AddFile(path, path));
                return;
            }

            string fileName;
            bool libPathAdded = false;
            bool headPathAdded = false;

            string realDstPath = Path.Combine(absoluteProjPath, path);
            foreach (var filePath in Directory.GetFiles(realDstPath))
            {
                fileName = Path.GetFileName(filePath);

                if (fileName.EndsWith(".DS_Store", StringComparison.Ordinal))
                    continue;

                proj.AddFileToBuild(targetGuid, proj.AddFile(Path.Combine(path, fileName), Path.Combine(path, fileName), PBXSourceTree.Source));
                if (!libPathAdded && fileName.EndsWith(".a", StringComparison.Ordinal))
                {
                    proj.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", Utils.FixSlashesInPath(string.Format("$(PROJECT_DIR)/{0}", path)));
                    libPathAdded = true;
                }

                if (!headPathAdded && fileName.EndsWith(".h", StringComparison.Ordinal))
                {
                    proj.AddBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", Utils.FixSlashesInPath(string.Format("$(PROJECT_DIR)/{0}", path)));
                    headPathAdded = true;
                }
            }

            foreach (var subPath in Directory.GetDirectories(realDstPath))
            {
                AddAdaptersDirectory(Path.Combine(path, Path.GetFileName(subPath)), proj, targetGuid);
            }
        }
    }
}