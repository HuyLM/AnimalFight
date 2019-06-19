using System;

namespace Gemmob.Common.Data {
    public partial class PersitenData : SecurePlayerPrefs {

        #region uint
        public static uint ToUInt(object value) {
            return Convert.ToUInt32(value);
        }

        public static uint GetUInt(string key, uint defaultValue = 0) {
            string value = GetString(key);
            if (!string.IsNullOrEmpty(value)) {
                return ToUInt(value);
            }
            return defaultValue;
        }

        public static void SetUInt(string key, uint value) {
            SetString(key, value.ToString());
        }
        #endregion

        #region ulong
        public static ulong ToULong(object value) {
            return Convert.ToUInt64(value);
        }

        public static ulong GetULong(string key, ulong defaultValue = 0) {
            string value = GetString(key);
            if (!string.IsNullOrEmpty(value)) {
                return ToULong(value);
            }
            return defaultValue;
        }

        public static void SetULong(string key, ulong value) {
            SetString(key, value.ToString());
        }
        #endregion
    }
}
