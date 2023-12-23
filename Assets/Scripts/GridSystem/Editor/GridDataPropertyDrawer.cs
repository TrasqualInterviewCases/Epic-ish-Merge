using Gameplay.GridSystem;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GridData))]
public class GridDataPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        SerializedProperty width = property.FindPropertyRelative("Width");
        SerializedProperty height = property.FindPropertyRelative("Height");
        SerializedProperty cellSize = property.FindPropertyRelative("CellSize");
        SerializedProperty center = property.FindPropertyRelative("Center");

        var widthRect = new Rect(position.x, position.y + 30, position.width, 20);
        var hightRect = new Rect(position.x, position.y + 60, position.width, 20);
        var cellSizeRect = new Rect(position.x, position.y + 90, position.width, 20);
        var centerRect = new Rect(position.x, position.y + 120, position.width, 20);

        width.intValue = EditorGUI.IntField(widthRect, new GUIContent("Width"), width.intValue);
        height.intValue = EditorGUI.IntField(hightRect, new GUIContent("Height"), height.intValue);

        width.intValue = Mathf.Max(0, width.intValue);
        height.intValue = Mathf.Max(0, height.intValue);

        cellSize.floatValue = EditorGUI.FloatField(cellSizeRect, new GUIContent("CellSize"), cellSize.floatValue);

        center.vector3Value = EditorGUI.Vector3Field(centerRect, new GUIContent("Center"), center.vector3Value);

        SerializedProperty columns = property.FindPropertyRelative("Columns");

        Rect newPosition = position;
        newPosition.height = 20f;
        newPosition.y += 160f;

        if (columns.arraySize != height.intValue)
        {
            columns.arraySize = height.intValue;
        }

        for (int i = 0; i < height.intValue; i++)
        {
            SerializedProperty row = columns.GetArrayElementAtIndex(height.intValue - 1 - i).FindPropertyRelative("Row");

            if (row.arraySize != width.intValue)
            {
                row.arraySize = width.intValue;
            }

            newPosition.width = position.width / width.intValue;

            for (int j = 0; j < width.intValue; j++)
            {
                SerializedProperty enumElement = row.GetArrayElementAtIndex(j);

                if (enumElement.enumValueIndex == 0)
                {
                    enumElement.enumValueIndex = 2;
                }

                enumElement.enumValueIndex = (int)(GridCellState)(EditorGUI.EnumPopup(newPosition, (GridCellState)enumElement.enumValueIndex));
                newPosition.x += newPosition.width;
            }

            newPosition.x = position.x;
            newPosition.y += 18f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty height = property.FindPropertyRelative("Height");
        return 18f * (height.intValue + 1) + 130f;
    }
}
