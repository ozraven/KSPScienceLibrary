// from http://wiki.unity3d.com/index.php?title=PopupList
// and some changes to integrate it in config window


using System;
using UnityEngine;

public class ComboBox
{
    private static bool forceToUnShow;
    private static int useControlID = -1;
    private bool isClickedComboButton;
    private Vector2 scrollVector;

    private int selectedItemIndex;

    public int List(string buttonText, string[] listContent, GUIStyle listStyle, int defaultSelectedItemIndex)
    {
        return List(new GUIContent(buttonText), listContent, "button", "box", listStyle, defaultSelectedItemIndex);
    }

    public int List(GUIContent buttonContent, string[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle, int defaultSelectedItemIndex)
    {
        selectedItemIndex = defaultSelectedItemIndex;
        if (forceToUnShow)
        {
            forceToUnShow = false;
            isClickedComboButton = false;
        }

        bool done = false;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.mouseUp:
                if (isClickedComboButton)
                    done = true;
                break;
        }

        if (GUILayout.Button(buttonContent, buttonStyle))
        {
            if (useControlID == -1)
            {
                useControlID = controlID;
                isClickedComboButton = false;
            }

            if (useControlID != controlID)
            {
                forceToUnShow = true;
                useControlID = controlID;
            }
            isClickedComboButton = true;
        }

        if (isClickedComboButton)
        {
            GUIContent guiContent = new GUIContent(listContent[0]);
            Rect rect = GUILayoutUtility.GetLastRect();
            Rect listRect = new Rect(rect.x, rect.y + listStyle.CalcHeight(guiContent, 1.0f), rect.width - 20, listStyle.CalcHeight(guiContent, 1.0f) * listContent.Length);
            Rect viewtRect = new Rect(rect.x, rect.y + listStyle.CalcHeight(guiContent, 1.0f), rect.width, listStyle.CalcHeight(guiContent, 1.0f) * Math.Min(4, listContent.Length));
            scrollVector = GUI.BeginScrollView(viewtRect, scrollVector, listRect, false, true);
            //GUI.Box(listRect, "", boxStyle);
            int newSelectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex, listContent, 1, listStyle);
            GUI.EndScrollView(true);

            if (done)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                if (!listRect.Contains(mousePosition) && viewtRect.Contains(mousePosition))
                    done = false;
                else
                    selectedItemIndex = newSelectedItemIndex;
            }
        }

        if (done)
            isClickedComboButton = false;

        return GetSelectedItemIndex();
    }

    public int GetSelectedItemIndex()
    {
        return selectedItemIndex;
    }
}