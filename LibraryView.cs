using System.Collections.Generic;
using UnityEngine;

public class LibraryView
{
    public List<LibraryRow> rows = new List<LibraryRow>();
}

public class LibraryRow
{
    public List<LibraryCell> cells = new List<LibraryCell>();
}

public class LibraryCell
{
    public string content;
    public GUIStyle style;
}