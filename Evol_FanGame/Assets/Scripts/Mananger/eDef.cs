using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum eChar { me, li, xu, bai, zhou, ling, none}
public enum ePosition { waiter, chef, explorer, rest, none}
public enum eCustomerStatus { enter, waitAtDoor, toTable, waitAtTable, eat, leave, none}
public enum eIngredient { decor, candy, iceCream, cookie, chocolate, cream, none}
public enum eIndividualObjs { waitLine, table0, table1, table2, table3, table4, table5, kitchen, none }
public enum eWaiterStates { toLine, toTable, toKitchen, none}
public enum eCarrying { customer, food, none}
public enum eObjStatus { foodReady, customerInLine, customerWaitForFood, tableEmpty, none}
public enum eMusic { cafe, heartbeat, bday, none}
public enum eSFX { click, doorbell, foodReady, typing, none}
public enum eCookingStatus { cooking, done, none}
public class NamedArrayAttribute : PropertyAttribute
{
    public Type TargetEnum;
    public NamedArrayAttribute(Type TargetEnum)
    {
        this.TargetEnum = TargetEnum;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Properly configure height for expanded contents.
        return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Replace label with enum name if possible.
        try
        {
            var config = attribute as NamedArrayAttribute;
            var enum_names = System.Enum.GetNames(config.TargetEnum);
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            var enum_label = enum_names.GetValue(pos) as string;
            // Make names nicer to read (but won't exactly match enum definition).
            enum_label = ObjectNames.NicifyVariableName(enum_label.ToLower());
            label = new GUIContent(enum_label);
        }
        catch
        {
            // keep default label
        }
        EditorGUI.PropertyField(position, property, label, property.isExpanded);
    }
}
#endif