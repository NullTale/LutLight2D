using UnityEditor;
using UnityEngine;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalDrawer : PropertyDrawer
    {
        private const float  k_ToggleWidth = 18;
        private const string k_ValueName   = "value";
        private const string k_EnabledName = "enabled";

        // =======================================================================
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(k_ValueName);
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(k_ValueName);
            var enabledProperty = property.FindPropertyRelative(k_EnabledName);

            position.width -= k_ToggleWidth;
            using (new EditorGUI.DisabledGroupScope(!enabledProperty.boolValue))
                EditorGUI.PropertyField(position, valueProperty, label, true);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var togglePos = new Rect(position.x + position.width + EditorGUIUtility.standardVerticalSpacing, position.y, k_ToggleWidth, EditorGUIUtility.singleLineHeight);
            enabledProperty.boolValue = EditorGUI.Toggle(togglePos, new GUIContent(string.Empty, "Enabled"), enabledProperty.boolValue);

            EditorGUI.indentLevel = indent;
        }
    }
}