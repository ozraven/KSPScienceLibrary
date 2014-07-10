using System.Collections.Generic;
using UnityEngine;

public class LibraryView
{
    public LinkedList<LibraryRow> rows = new LinkedList<LibraryRow>();
}

public class LibraryRow
{
    public List<LibraryCell> cells = new List<LibraryCell>();
    public GUIStyle style;

    public LibraryRow(GUIStyle style)
    {
        this.style = style;
    }
}

public class LibraryCell
{
    public GUIContent content;
    public GUIStyle style;

    public LibraryCell(string content, GUIStyle style)
    {
        this.content = new GUIContent(content);
        this.style = style;
    }

    public LibraryCell(string content, string tooltip, GUIStyle style)
    {
        this.content = new GUIContent(content, tooltip);
        this.style = style;
    }

    public LibraryCell(GUIContent content, GUIStyle style)
    {
        this.content = content;
        this.style = style;
    }
}