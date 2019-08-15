using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Gemmob.EditorTools {
	public class ScriptingDefine {
        private const string ProductionBuildString = "PRODUCTION_BUILD";

        [DidReloadScripts]
		public static void Init() {
			ShowLogForTagetGroup(GetBuildTargetGroup());
		}

		public static BuildTargetGroup GetBuildTargetGroup() {
#if UNITY_ANDROID
			return BuildTargetGroup.Android;
#endif
#if UNITY_STANDALONE
            return BuildTargetGroup.Standalone;
    
    #endif

#if UNITY_IOS
            return BuildTargetGroup.iOS;
    
#endif
		}

		public static void EnableScriptingDefineFlag(bool flag, string name, List<string> defines) {
			if (flag && !defines.Contains(name)) {
				defines.Add(name);
			}

			if (!flag && defines.Contains(name)) {
				defines.Remove(name);
			}
		}

		public static void SaveScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup, string[] defines) {
			SaveScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defines));
		}

		public static void SaveScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup, string define) {
			string defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

			if (!defineBuild.Equals(define)) {
				Debug.LogFormat("Set define symbols for group {0} [{1}]->[{2}]", buildTargetGroup, defineBuild, define);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, define);
			}
		}

		private static void ShowLogForTagetGroup(BuildTargetGroup targetGroup) {
			string defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
			var defines = new List<string>(defineBuild.Split(';'));
			//if (!defines.Contains(Logs.EnableLogsString)) {
			//	defines.Add(Logs.EnableLogsString);
			//}

			SaveScriptingDefineSymbolsForGroup(targetGroup, defines.ToArray());
		}

        public static void EnableProductionString(bool enable, BuildTargetGroup targetGroup) {
            string defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defines = new List<string>(defineBuild.Split(';'));
            EnableScriptingDefineFlag(enable, ProductionBuildString, defines);
            EnableScriptingDefineFlag(!enable, Logs.EnableLogsString, defines);
            SaveScriptingDefineSymbolsForGroup(targetGroup, defines.ToArray());
        }

        public static void EnableLogString(bool enable, BuildTargetGroup targetGroup) {
            string defineBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defines = new List<string>(defineBuild.Split(';'));
            EnableScriptingDefineFlag(enable, Logs.EnableLogsString, defines);
            SaveScriptingDefineSymbolsForGroup(targetGroup, defines.ToArray());
        }
	}
}