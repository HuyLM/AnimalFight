using System;
using System.Globalization;
using UnityEngine;

namespace Gemmob.Common.Data {
	public class SecurePlayerPrefs {
		public const string KeyPrefix = "ENC-";

		public const string ValueFloatPrefix = "0";
		public const string ValueIntPrefix = "1";
		public const string ValueStringPrefix = "2";

		public static bool IsEncryptedKey(string key) {
			return key.StartsWith(KeyPrefix);
		}


		public static string DecryptKey(string encryptedKey) {
			if (encryptedKey.StartsWith(KeyPrefix)) {
				var strippedKey = encryptedKey.Substring(KeyPrefix.Length);
				return Encryption.DecryptString(strippedKey);
			}

			throw new InvalidOperationException(
				"Could not decrypt item, no match found in known encrypted key prefixes");
		}


		public static void SetFloat(string key, float value) {
			var encryptedKey = Encryption.EncryptString(key);
			var encryptedValue = Encryption.EncryptFloat(value);
			PlayerPrefs.SetString(KeyPrefix + encryptedKey, ValueFloatPrefix + encryptedValue);
		}


		public static void SetInt(string key, int value) {
			var encryptedKey = Encryption.EncryptString(key);
			var encryptedValue = Encryption.EncryptInt(value);
			PlayerPrefs.SetString(KeyPrefix + encryptedKey, ValueIntPrefix + encryptedValue);
		}


		public static void SetString(string key, string value) {
			var encryptedKey = Encryption.EncryptString(key);
			var encryptedValue = Encryption.EncryptString(value);
			PlayerPrefs.SetString(KeyPrefix + encryptedKey, ValueStringPrefix + encryptedValue);
		}

		public static object GetValue(string encryptedKey, string encryptedValue) {
			if (encryptedValue.StartsWith(ValueFloatPrefix)) {
				return GetFloat(Encryption.DecryptString(encryptedKey.Substring(KeyPrefix.Length)));
			}

			if (encryptedValue.StartsWith(ValueIntPrefix)) {
				return GetInt(Encryption.DecryptString(encryptedKey.Substring(KeyPrefix.Length)));
			}

			if (encryptedValue.StartsWith(ValueStringPrefix)) {
				return GetString(Encryption.DecryptString(encryptedKey.Substring(KeyPrefix.Length)));
			}

			throw new InvalidOperationException(
				"Could not decrypt item, no match found in known encrypted key prefixes");
		}


		public static float GetFloat(string key, float defaultValue = 0.0f) {
			var encryptedKey = KeyPrefix + Encryption.EncryptString(key);
			var fetchedString = PlayerPrefs.GetString(encryptedKey);

			if (string.IsNullOrEmpty(fetchedString)) return defaultValue;
			fetchedString = fetchedString.Remove(0, 1);
			return Encryption.DecryptFloat(fetchedString);
		}

		public static int GetInt(string key, int defaultValue = 0) {
			var encryptedKey = KeyPrefix + Encryption.EncryptString(key);
			var fetchedString = PlayerPrefs.GetString(encryptedKey);

			if (string.IsNullOrEmpty(fetchedString)) return defaultValue;
			fetchedString = fetchedString.Remove(0, 1);
			return Encryption.DecryptInt(fetchedString);
		}

		public static string GetString(string key, string defaultValue = "") {
			var encryptedKey = KeyPrefix + Encryption.EncryptString(key);
			var fetchedString = PlayerPrefs.GetString(encryptedKey);

			if (string.IsNullOrEmpty(fetchedString)) return defaultValue;
			fetchedString = fetchedString.Remove(0, 1);
			return Encryption.DecryptString(fetchedString);
		}

		public static void SetBool(string key, bool value) {
			SetInt(key, value ? 1 : 0);
		}

		public static bool GetBool(string key, bool defaultValue = false) {
			return GetInt(key, defaultValue ? 1 : 0) == 1;
		}

		public static void SetEnum(string key, Enum value) {
			SetString(key, value.ToString());
		}

		public static T GetEnum<T>(string key, T defaultValue = default(T)) where T : struct {
			var stringValue = GetString(key);
			return !string.IsNullOrEmpty(stringValue) ? (T) Enum.Parse(typeof(T), stringValue) : defaultValue;
		}


		public static object GetEnum(string key, Type enumType, object defaultValue) {
			var value = GetString(key);
			return !string.IsNullOrEmpty(value) ? Enum.Parse(enumType, value) : defaultValue;
		}


		public static void SetDateTime(string key, DateTime value) {
			SetString(key, value.ToString("o", CultureInfo.InvariantCulture));
		}


		public static DateTime GetDateTime(string key, DateTime defaultValue = new DateTime()) {
			var stringValue = GetString(key);
			return !string.IsNullOrEmpty(stringValue)
				? DateTime.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
				: defaultValue;
		}


		public static void SetTimeSpan(string key, TimeSpan value) {
			SetString(key, value.ToString());
		}


		public static TimeSpan GetTimeSpan(string key, TimeSpan defaultValue = new TimeSpan()) {
			var stringValue = GetString(key);

			return !string.IsNullOrEmpty(stringValue) ? TimeSpan.Parse(stringValue) : defaultValue;
		}
	}
}