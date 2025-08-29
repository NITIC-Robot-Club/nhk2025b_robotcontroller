using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AndButton))]
public class AndButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float[] widths = { position.width * 0.3f, position.width * 0.4f, position.width * 0.3f };

        // AndButtonList enum
        var andbuttonProp = property.FindPropertyRelative("andbutton");
        position.width = widths[0];
        EditorGUI.PropertyField(position, andbuttonProp, GUIContent.none);

        // Button
        var buttonProp = property.FindPropertyRelative("button");
        position.x += position.width;
        position.width = widths[1];
        EditorGUI.PropertyField(position, buttonProp, GUIContent.none);

        // AndPanelList enum
        var panelProp = property.FindPropertyRelative("panel");
        position.x += position.width;
        position.width = widths[2];
        EditorGUI.PropertyField(position, panelProp, GUIContent.none);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}