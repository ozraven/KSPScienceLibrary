using System.Collections.Generic;
using UnityEngine;

public class LibraryView
{
    public List<LibraryRow> rows = new List<LibraryRow>();
}

public class LibraryRow
{
    public GUIStyle style;
    public List<LibraryCell> cells = new List<LibraryCell>();

    public LibraryRow(GUIStyle style)
    {
        this.style = style;
    }
}

public class LibraryCell
{
    public string content;
    public GUIStyle style;

    public LibraryCell(string content, GUIStyle style)
    {
        this.content = content;
        this.style = style;
    }
}