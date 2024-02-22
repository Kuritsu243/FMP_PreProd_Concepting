using System;
using UnityEditor;
using UnityEngine;
using Player;
using Weapons;

namespace EditorExtensions
{
    // src: https://discussions.unity.com/t/custom-inspector-if-bool-is-true-then-show-variable/178698/3
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]

    public sealed class ShowIfProjectileAttribute : PropertyAttribute
    {

        public string ConditionalField;
        public BaseWeapon.ShootingType ExpectedValue;
        public bool HideInInspector;

        public ShowIfProjectileAttribute(string conditionalField, BaseWeapon.ShootingType expectedValue, bool hideInInspector)
        {
            this.ConditionalField = conditionalField;
            this.ExpectedValue = expectedValue;
            this.HideInInspector = hideInInspector;
        }

    }
    
    public class EnumHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            var condition = (ShowIfAttribute) attribute;
            var enabled = GetConditionalSourceField(property, condition);
            GUI.enabled = enabled;
            EditorGUI.BeginProperty(position, label, property);
            if (enabled)
                EditorGUI.PropertyField(position, property, label, true);
            else if (!condition.HideInInspector)
                EditorGUI.PropertyField(position, property, label, false);
            else return;
#endif
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            var condition = (ShowIfAttribute) attribute;
            var enabled = GetConditionalSourceField(property, condition);

            if (enabled)
                return EditorGUI.GetPropertyHeight(property, label, true);
            else
            {
                if (!condition.HideInInspector)
                    return EditorGUI.GetPropertyHeight(property, label, false);
                else
                    return -EditorGUIUtility.standardVerticalSpacing;
            }
#else

            return 0f;
#endif
        }

        private static bool GetConditionalSourceField(SerializedProperty property, ShowIfAttribute condition)
        {
#if UNITY_EDITOR
            var enabled = false;
            var propertyPath = property.propertyPath;
            var conditionPath = propertyPath.Replace(property.name, condition.ConditionalSourceField);
            var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                enabled = sourcePropertyValue.boolValue;
                enabled = enabled == condition.ExpectedValue;
            }
            else
            {
                var warning =
                    $"ConditionalHideAttribute: Field not found [{condition.ConditionalSourceField}] " +
                    $"in {property.propertyPath}";
                warning += "Ensure the condition name has been entered correctly";
                Debug.LogWarning(warning);
            }

            return enabled;
#else
            return false;
#endif
        }
    }

}




// [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    // public sealed class ShowIfShootingAttribute : PropertyAttribute
    // {
    //     public string ConditionalSourceField;
    //     public BaseWeapon.ShootingType ExpectedValue;
    //     public bool HideInInspector;
    //
    //     public ShowIfShootingAttribute(string conditionalSourceField, BaseWeapon.ShootingType expectedValue, bool hideInInspector)
    //     {
    //         this.ConditionalSourceField = conditionalSourceField;
    //         this.ExpectedValue = expectedValue;
    //         this.HideInInspector = hideInInspector;
    //     }
    // }

//     public class EnumHidePropertyDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
// #if UNITY_EDITOR
//             ShowIfAttribute condHAtt = (ShowIfAttribute)attribute;
//             bool enabled = GetConditionalShootingSourceField(property, condHAtt);
//             GUI.enabled = enabled;
//
//             if (enabled)
//                 EditorGUI.PropertyField(position, property, label, true);
//         
//             else if (!condHAtt.HideInInspector)
//                 EditorGUI.PropertyField(position, property, label, false);
//         
//             else return;
// #endif
//         }
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
// #if UNITY_EDITOR
//             ShowIfAttribute condHAtt = (ShowIfAttribute)attribute;
//             bool enabled = GetConditionalShootingSourceField(property, condHAtt);
//
//             if (enabled)
//                 return EditorGUI.GetPropertyHeight(property, label, true);
//             else
//             {
//                 if (!condHAtt.HideInInspector)
//                     return EditorGUI.GetPropertyHeight(property, label, false);
//                 else
//                     return -EditorGUIUtility.standardVerticalSpacing;
//             }
// #else
//         return 0f;
// #endif
//         }
//
//         private bool GetConditionalShootingSourceField(SerializedProperty property, ShowIfAttribute condHAtt)
//         {
// #if UNITY_EDITOR
//             bool enabled = false;
//             string propertyPath = property.propertyPath;
//             string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
//             SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
//
//             if (sourcePropertyValue != null)
//             {
//                 enabled = sourcePropertyValue.boolValue;
//                 enabled = enabled == condHAtt.ExpectedValue;
//             }
//             else
//             {
//                 string warning =
//                     $"ConditionalHideAttribute: Boolean field not found [{condHAtt.ConditionalSourceField}] " +
//                     $"in {property.propertyPath}";
//                 warning += "Ensure the condition name has been entered correctly";
//                 Debug.LogWarning(warning);
//             }
//
//             return enabled;
// #else
//         return false;
// #endif
//         }
//     }
// }