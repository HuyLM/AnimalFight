using System;
using UnityEditor;
using UnityEngine;

namespace Gemmob.EditorTools {
	internal class BuildConfig : ScriptableObject {
		public static readonly Options DefaultOptions = new Options();
        public static readonly Options DefaultOpenOptions = new Options { autoOpen = true};

		public class Options {
			public bool isDevelop;
            public bool isBuildAPK;
			public bool autorun;
			public bool autoOpen;
		}


		[Flags]
		public enum CustomBuildTarget {
			Android = (1 << 0),
			iOS = (1 << 1),
			Standalone = (1 << 30),
		}

        public enum BuildAchietecture { ARMv7, ARM64, X86, ARM_All, All }
        
		[Serializable]
		public class ScriptingConfig {
			[SerializeField] public string key;
			[SerializeField] public bool develop;
			[SerializeField] public bool release;
			[SerializeField] public CustomBuildTarget target;
		}

		[SerializeField] public ScriptingConfig[] scriptingDefines;
		[SerializeField] public bool enableIap;
		[SerializeField] public bool enableAds;
		[SerializeField] public bool enableFirebase;
		[SerializeField] public BuildOptions developOptions;
		[SerializeField] public BuildOptions productionOptions;
		[SerializeField] public string keyAliasName;
		[SerializeField] public string keyAliasPass;
		[SerializeField] public string BuildVersion;
		[SerializeField] public int BuildVerCode;
		[SerializeField] public string KeyStorePass;
		[SerializeField] public string UnityBuildVersion;
		[SerializeField] public string GroupSkypeId;
		[SerializeField] public bool AndroidIl2CppForRelease;
        [SerializeField] public bool iOsIl2CppForRelease;

        [SerializeField] public BuildAchietecture achitecture;

        [SerializeField] public Texture2D iOSIcon;
		[SerializeField] public Texture2D androidIcon;
		[SerializeField] public Sprite iOsSplashBackground;
		[SerializeField] public Sprite androidSplashBackground;
		[SerializeField] public Sprite iOsSplashLogo;
		[SerializeField] public Sprite androidSplashLogo;
		[SerializeField] public iOSTargetDevice iOSTargetDevice;
		[SerializeField] public iOSSdkVersion iOSSdkVersion;
		[SerializeField] public string iOSTargetOSVersion;
		[SerializeField] public int buildTimeout;
		[SerializeField] public string buildNode;
	}
}