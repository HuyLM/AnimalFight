using UnityEngine;
using System;
using System.Reflection;

namespace Gemmob.Common {
    public class AutoAssignFields : MonoBehaviour {
        public Component component;

        void Awake() {
            Assign();
        }

        public void Assign() {
            if (component == null) {
                Debug.LogError("[AutoAssign] null main component!");
                return;
            }

            FetchValue(component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
            FetchValue(component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
        }

        private void FetchValue(MemberInfo[] members) {
            if (members == null || members.Length == 0) return;
            bool fieldInfo = members[0] != null && members[0] is FieldInfo;

            foreach (var info in members) {
                var attributes = info.GetCustomAttributes(typeof(AutoAssignAttribute), false);
                if (attributes.Length > 0) {
                    var att = attributes[0] as AutoAssignAttribute;
                    object value = null;
                    Type type = fieldInfo ? (info as FieldInfo).FieldType : (info as PropertyInfo).PropertyType;
                    switch (att.scope) {
                        case AutoAssignAttribute.Scope.Children:
                            value = component.GetComponentInChildren(type, true);
                            break;
                        case AutoAssignAttribute.Scope.Parent:
                            value = component.GetComponentInParent(type);
                            break;
                        case AutoAssignAttribute.Scope.Global:
                            value = GameObject.FindObjectOfType(type);
                            break;
                    }

                    if (fieldInfo) {
                        ((FieldInfo)info).SetValue(component, value);
                    }
                    else {
                        ((PropertyInfo)info).SetValue(component, value, null);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AutoAssignAttribute : Attribute {
        public Scope scope;

        public AutoAssignAttribute(Scope scope = Scope.Children) {
            this.scope = scope;
        }

        public enum Scope {
            Children,
            Parent,
            Global,
        }
    }
}