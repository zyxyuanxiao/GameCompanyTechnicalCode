using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LogSystem
{
  public class rdtGuiTree<T>
  {
    private GUIContent m_tempContent = new GUIContent();
    private GUIContent m_content = new GUIContent();
    private const float ROW_HEIGHT = 16f;
    private const float INDENT_WIDTH = 13f;
    private const float FOLDOUT_WIDTH = 12f;
    private const float BASE_INDENT = 2f;
    private const float ICON_WIDTH = 16f;
    private const float SCROLLBAR_WIDTH = 16f;
    private const float TREE_BOTTOM_MARGIN = 2f;
    private const float SPACE_BETWEEN_ICON_AND_TEXT = 2f;
    private Vector2 m_scrollPosition;
    private rdtGuiTree<T>.Node m_root;
    private rdtGuiTree<T>.SelectedNodeCollection m_selectedNodes;
    private GUIStyle m_foldoutStyle;
    private GUIStyle m_lineStyle;
    private GUIStyle m_disabledLineStyle;
    private GUIStyle m_boldLineStyle;
    private GUIStyle m_selectionStyle;
    private GUIStyle m_noDataStyle;
    private Rect m_scrollViewRect;
    private bool m_hasFocus;
    private rdtExpandedCache m_expandedCache;
    private string m_filter;
    private List<rdtGuiTree<T>.Node> m_visibleNodes;
    private bool m_visibleNodesDirty;

    public rdtExpandedCache ExpandedCache
    {
      get
      {
        return this.m_expandedCache;
      }
    }

    public rdtGuiTree<T>.SelectedNodeCollection SelectedNodes
    {
      get
      {
        return this.m_selectedNodes;
      }
    }

    public string Filter
    {
      get
      {
        return this.m_filter;
      }
      set
      {
        this.m_visibleNodesDirty |= !string.Equals(this.m_filter, value);
        string filter = this.m_filter;
        this.m_filter = value;
        if (!string.IsNullOrEmpty(value) || string.IsNullOrEmpty(filter))
          return;
        this.EnsureVisible((List<rdtGuiTree<T>.Node>) this.SelectedNodes);
      }
    }

    public event Action SelectedNodesChanged;

    public event Action SelectedNodesDeleted;

    public rdtGuiTree()
    {
      this.m_expandedCache = new rdtExpandedCache();
      this.m_root = new rdtGuiTree<T>.Node("root", (rdtGuiTree<T>.Node) null, this.m_expandedCache);
      this.m_root.Depth = -1;
      this.m_selectedNodes = new rdtGuiTree<T>.SelectedNodeCollection();
      this.m_selectedNodes.ListChanged += new Action<ObservableList<rdtGuiTree<T>.Node>>(this.OnSelectedNodesChanged);
      this.m_visibleNodesDirty = true;
    }

    private void OnSelectedNodesChanged(ObservableList<rdtGuiTree<T>.Node> obj)
    {
      if (this.SelectedNodesChanged == null)
        return;
      this.SelectedNodesChanged();
    }

    public rdtGuiTree<T>.Node FindNode(T data)
    {
      return this.BuildFlatList(this.m_root.Children, true).FirstOrDefault<rdtGuiTree<T>.Node>((Func<rdtGuiTree<T>.Node, bool>) (x => x.Data.Equals((object) data)));
    }

    public rdtGuiTree<T>.Node FindNode(string name)
    {
      return this.BuildFlatList(this.m_root.Children, true).FirstOrDefault<rdtGuiTree<T>.Node>((Func<rdtGuiTree<T>.Node, bool>) (x => x.Name.Equals(name)));
    }

    public rdtGuiTree<T>.Node AddNode(T data, bool enabled = true)
    {
      rdtGuiTree<T>.Node node = new rdtGuiTree<T>.Node(data, this.m_root, this.m_expandedCache);
      node.Enabled = enabled;
      this.m_root.Children.Add(node);
      this.m_visibleNodesDirty = true;
      return node;
    }

    public rdtGuiTree<T>.Node AddNode(string name)
    {
      rdtGuiTree<T>.Node node = new rdtGuiTree<T>.Node(name, this.m_root, this.m_expandedCache);
      node.Enabled = true;
      this.m_root.Children.Add(node);
      this.m_visibleNodesDirty = true;
      return node;
    }

    public void Clear()
    {
      this.m_root.Children.Clear();
      this.SelectedNodes.Clear();
      this.m_visibleNodesDirty = true;
    }

    public void Draw(Rect rect, bool windowHasFocus)
    {
      this.RefreshVisibleNodes();
      if (Event.current.type == UnityEngine.EventType.Repaint)
        this.m_scrollViewRect = rect;
      this.m_hasFocus &= windowHasFocus;
      this.InitStyles();
      this.ProcessKeyboardInput();
      Vector2 totalSize = this.GetTotalSize();
      Rect viewRect = new Rect(0.0f, 0.0f, rect.width, totalSize.y);
      if ((double) viewRect.height > (double) rect.height)
        viewRect.width -= 16f;
      this.m_scrollPosition = GUI.BeginScrollView(rect, this.m_scrollPosition, viewRect);
      int first;
      int last;
      this.GetFirstLastRowVisible(out first, out last);
      for (int rowIndex = first; rowIndex <= last; ++rowIndex)
        this.DrawNode(this.m_visibleNodes[rowIndex], rowIndex, viewRect.width);
      GUI.EndScrollView();
      Event current = Event.current;
      switch (current.type)
      {
        case UnityEngine.EventType.MouseDown:
          this.m_hasFocus = this.m_scrollViewRect.Contains(Event.current.mousePosition);
          if (!this.m_hasFocus)
            break;
          this.SelectedNodes.Clear();
          current.Use();
          break;
        case UnityEngine.EventType.Used:
          this.m_hasFocus = true;
          break;
      }
    }

    private void EnsureVisible(List<rdtGuiTree<T>.Node> nodes)
    {
      if (nodes.Count == 0)
        return;
      for (int index = 0; index < nodes.Count; ++index)
      {
        for (rdtGuiTree<T>.Node parent = nodes[index].Parent; parent != this.m_root; parent = parent.Parent)
          this.SetNodeExpanded(parent, true);
      }
      this.RefreshVisibleNodes();
      int num1 = this.m_visibleNodes.IndexOf(nodes[0]);
      float num2 = (float) num1 * 16f;
      float num3 = num2 + 16f;
      if ((double) num3 >= (double) this.m_scrollPosition.y + (double) this.m_scrollViewRect.height)
      {
        this.m_scrollPosition.y = num3 - this.m_scrollViewRect.height;
      }
      else
      {
        if ((double) num2 > (double) this.m_scrollPosition.y)
          return;
        this.m_scrollPosition.y = (float) num1 * 16f;
      }
    }

    private void DrawNode(rdtGuiTree<T>.Node node, int rowIndex, float maxWidth)
    {
      bool on = this.m_selectedNodes.Contains(node);
      bool expanded1 = node.Expanded;
      bool flag1 = expanded1;
      this.m_content.text = node.Name;
      Rect position1 = new Rect(0.0f, this.GetTopPixelForRow(rowIndex), maxWidth, 16f);
      if (!node.HasData)
      {
        Color color = GUI.color;
        GUI.color *= new Color(1f, 1f, 1f, 0.9f);
        GUI.Label(position1, GUIContent.none, this.m_noDataStyle);
        GUI.color = color;
      }
      Event current = Event.current;
      if (current.type == UnityEngine.EventType.Repaint)
      {
        if (on)
          this.m_selectionStyle.Draw(position1, false, false, true, this.m_hasFocus);
        float contentIndent = this.GetContentIndent(node);
        position1.x += contentIndent;
        position1.width -= contentIndent;
        GUIStyle guiStyle = node.IsBold ? this.m_boldLineStyle : (node.Enabled ? this.m_lineStyle : this.m_disabledLineStyle);
        guiStyle.padding.left = 2;
        if ((bool) (UnityEngine.Object) node.Icon)
          guiStyle.padding.left += 16;
        guiStyle.Draw(position1, node.Name, false, false, on, this.m_hasFocus);
        Texture2D icon = node.Icon;
        if ((bool) (UnityEngine.Object) icon)
        {
          Rect position2 = position1;
          position2.width = 16f;
          position2.height = 16f;
          GUI.DrawTexture(position2, (Texture) icon);
        }
      }
      if (node.Children.Count > 0 && string.IsNullOrEmpty(this.Filter))
      {
        bool expanded2 = GUI.Toggle(new Rect(this.GetFoldoutIndent(node), position1.y, 12f, position1.height), node.Expanded, GUIContent.none, this.m_foldoutStyle);
        if (expanded2 != node.Expanded)
          this.SetNodeExpanded(node, expanded2);
      }
      bool flag2 = current.isMouse && current.type == UnityEngine.EventType.MouseDown && current.button == 0;
      if ((!(position1.Contains(current.mousePosition) & flag2) ? 0 : (flag1 == expanded1 ? 1 : 0)) == 0)
        return;
      if ((current.modifiers & EventModifiers.Control) != EventModifiers.None)
      {
        if (this.SelectedNodes.Contains(node))
          this.SelectedNodes.Remove(node);
        else
          this.SelectedNodes.Add(node);
      }
      else if ((current.modifiers & EventModifiers.Shift) != EventModifiers.None)
      {
        int num1 = this.m_visibleNodes.IndexOf(node);
        int num2 = -1;
        int num3 = -1;
        for (int index = 0; index < this.SelectedNodes.Count; ++index)
        {
          int num4 = this.m_visibleNodes.IndexOf(this.SelectedNodes[index]);
          int num5 = Mathf.Abs(num1 - num4);
          if (num5 > num3)
          {
            num3 = num5;
            num2 = num4;
          }
        }
        if (num2 != -1)
        {
          int index = num2;
          int num4 = num1;
          if (num2 > num1)
          {
            index = num1;
            num4 = num2;
          }
          this.SelectedNodes.ReplaceAll((IEnumerable<rdtGuiTree<T>.Node>) this.m_visibleNodes.GetRange(index, num4 - index + 1));
        }
      }
      else
        this.SelectedNodes.ReplaceAll(node);
      current.Use();
    }

    private void InitStyles()
    {
      if (this.m_foldoutStyle != null && this.m_lineStyle != null && (this.m_boldLineStyle != null && this.m_selectionStyle != null))
        return;
      this.m_selectionStyle = new GUIStyle((GUIStyle) "PR Label");
      this.m_lineStyle = new GUIStyle((GUIStyle) "PR Label");
      Texture2D background = this.m_lineStyle.hover.background;
      this.m_lineStyle.onNormal.background = background;
      this.m_lineStyle.onActive.background = background;
      this.m_lineStyle.onFocused.background = background;
      this.m_lineStyle.alignment = TextAnchor.MiddleLeft;
      this.m_boldLineStyle = new GUIStyle(this.m_lineStyle);
      this.m_boldLineStyle.font = EditorStyles.boldLabel.font;
      this.m_boldLineStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
      this.m_disabledLineStyle = new GUIStyle((GUIStyle) "PR DisabledLabel");
      this.m_disabledLineStyle.alignment = TextAnchor.MiddleLeft;
      this.m_foldoutStyle = new GUIStyle((GUIStyle) "IN Foldout");
      this.m_noDataStyle = new GUIStyle((GUIStyle) "ProjectBrowserTopBarBg");
    }

    private void ProcessKeyboardInput()
    {
      Event current = Event.current;
      if (!current.isKey || this.m_selectedNodes.Count == 0 || (current.type != UnityEngine.EventType.KeyDown || GUIUtility.keyboardControl != 0))
        return;
      int num1 = this.m_visibleNodes.IndexOf(this.m_selectedNodes[this.m_selectedNodes.Count - 1]);
      if (num1 == -1 || this.m_visibleNodes.Count == 0)
      {
        this.SelectedNodes.ReplaceAll(this.m_visibleNodes.Count > 0 ? this.m_visibleNodes[0] : (rdtGuiTree<T>.Node) null);
        this.EnsureVisible((List<rdtGuiTree<T>.Node>) this.SelectedNodes);
      }
      else
      {
        bool flag = true;
        switch (current.keyCode)
        {
          case KeyCode.Delete:
            if (this.SelectedNodesDeleted != null)
              this.SelectedNodesDeleted();
            current.Use();
            break;
          case KeyCode.UpArrow:
            if (num1 > 0)
              this.SelectedNodes.ReplaceAll(this.m_visibleNodes[num1 - 1]);
            current.Use();
            break;
          case KeyCode.DownArrow:
            if (num1 < this.m_visibleNodes.Count - 1)
              this.SelectedNodes.ReplaceAll(this.m_visibleNodes[num1 + 1]);
            current.Use();
            break;
          case KeyCode.RightArrow:
            current.Use();
            this.ProcessRightArrow(this.m_visibleNodes);
            break;
          case KeyCode.LeftArrow:
            current.Use();
            this.ProcessLeftArrow(this.m_visibleNodes);
            break;
          case KeyCode.Home:
            this.SelectedNodes.ReplaceAll(this.m_visibleNodes[0]);
            current.Use();
            break;
          case KeyCode.End:
            this.SelectedNodes.ReplaceAll(this.m_visibleNodes[this.m_visibleNodes.Count - 1]);
            current.Use();
            break;
          case KeyCode.PageUp:
            int num2 = (int) ((double) this.m_scrollViewRect.height / 16.0);
            this.SelectedNodes.ReplaceAll(this.m_visibleNodes[Mathf.Max(num1 - num2, 0)]);
            current.Use();
            break;
          case KeyCode.PageDown:
            int num3 = (int) ((double) this.m_scrollViewRect.height / 16.0);
            this.SelectedNodes.ReplaceAll(this.m_visibleNodes[Mathf.Min(num1 + num3, this.m_visibleNodes.Count - 1)]);
            current.Use();
            break;
          default:
            flag = false;
            break;
        }
        if (!flag)
          return;
        this.EnsureVisible((List<rdtGuiTree<T>.Node>) this.SelectedNodes);
      }
    }

    private void ProcessLeftArrow(List<rdtGuiTree<T>.Node> flatList)
    {
      if (!string.IsNullOrEmpty(this.Filter))
        return;
      if (this.m_selectedNodes.Count == 1)
      {
        rdtGuiTree<T>.Node selectedNode = this.m_selectedNodes[0];
        if (selectedNode.Expanded)
          this.SetNodeExpanded(selectedNode, false);
        else if (selectedNode.Parent != this.m_root)
        {
          this.SelectedNodes.ReplaceAll(selectedNode.Parent);
        }
        else
        {
          for (int index = flatList.IndexOf(selectedNode) - 1; index >= 0; --index)
          {
            rdtGuiTree<T>.Node flat = flatList[index];
            if (flat.Children.Count > 0)
            {
              this.SelectedNodes.ReplaceAll(flat);
              break;
            }
          }
        }
      }
      else
      {
        for (int index = 0; index < this.m_selectedNodes.Count; ++index)
          this.SetNodeExpanded(this.m_selectedNodes[index], false);
      }
    }

    private void ProcessRightArrow(List<rdtGuiTree<T>.Node> flatList)
    {
      if (!string.IsNullOrEmpty(this.Filter))
        return;
      if (this.m_selectedNodes.Count == 1)
      {
        rdtGuiTree<T>.Node selectedNode = this.m_selectedNodes[0];
        if (selectedNode.Children.Count == 0 || selectedNode.Expanded)
        {
          for (int index = flatList.IndexOf(selectedNode) + 1; index < flatList.Count; ++index)
          {
            rdtGuiTree<T>.Node flat = flatList[index];
            if (flat.Children.Count > 0)
            {
              this.SelectedNodes.ReplaceAll(flat);
              break;
            }
          }
        }
        else
        {
          if (selectedNode.Children.Count <= 0)
            return;
          this.SetNodeExpanded(selectedNode, true);
        }
      }
      else
      {
        for (int index = 0; index < this.m_selectedNodes.Count; ++index)
          this.SetNodeExpanded(this.m_selectedNodes[index], true);
      }
    }

    private List<rdtGuiTree<T>.Node> BuildFlatList(
      List<rdtGuiTree<T>.Node> nodes,
      bool allNodes = false)
    {
      List<rdtGuiTree<T>.Node> nodeList = new List<rdtGuiTree<T>.Node>();
      for (int index = 0; index < nodes.Count; ++index)
      {
        rdtGuiTree<T>.Node node = nodes[index];
        bool flag = !string.IsNullOrEmpty(this.Filter);
        if ((allNodes || !flag ? 1 : (node.Name.IndexOf(this.Filter, StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 0)) != 0)
          nodeList.Add(node);
        if (((allNodes ? 1 : (node.Expanded ? 1 : 0)) | (flag ? 1 : 0)) != 0)
          nodeList.AddRange((IEnumerable<rdtGuiTree<T>.Node>) this.BuildFlatList(node.Children, false));
      }
      return nodeList;
    }

    private void RefreshVisibleNodes()
    {
      if (!this.m_visibleNodesDirty)
      {
        if (this.m_visibleNodes != null)
          return;
        this.m_visibleNodes = new List<rdtGuiTree<T>.Node>();
      }
      else
      {
        this.m_visibleNodesDirty = false;
        this.m_visibleNodes = this.BuildFlatList(this.m_root.Children, false);
      }
    }

    private float GetFoldoutIndent(rdtGuiTree<T>.Node node)
    {
      return !string.IsNullOrEmpty(this.Filter) ? 2f : (float) (2.0 + (double) node.Depth * 13.0);
    }

    private float GetContentIndent(rdtGuiTree<T>.Node node)
    {
      return this.GetFoldoutIndent(node) + 12f;
    }

    private Vector2 GetTotalSize()
    {
      return new Vector2(1f, (float) ((double) this.m_visibleNodes.Count * 16.0 + 2.0));
    }

    private void GetFirstLastRowVisible(out int first, out int last)
    {
      first = Mathf.Max(Mathf.FloorToInt(this.m_scrollPosition.y / 16f), 0);
      last = first + Mathf.CeilToInt(this.m_scrollViewRect.height / 16f);
      last = Mathf.Min(last, this.m_visibleNodes.Count - 1);
    }

    private float GetTopPixelForRow(int row)
    {
      return (float) row * 16f;
    }

    private GUIContent GetContent(string text)
    {
      this.m_tempContent.text = text;
      this.m_tempContent.tooltip = string.Empty;
      return this.m_tempContent;
    }

    private void SetNodeExpanded(rdtGuiTree<T>.Node node, bool expanded)
    {
      this.m_visibleNodesDirty |= node.Expanded != expanded;
      node.Expanded = expanded;
    }

    public class SelectedNodeCollection : ObservableList<rdtGuiTree<T>.Node>
    {
    }

    public class Node
    {
      private rdtExpandedCache m_expandedCache;

      public T Data { get; private set; }

      public string Name { get; private set; }

      public List<rdtGuiTree<T>.Node> Children { get; private set; }

      public rdtGuiTree<T>.Node Parent { get; private set; }

      public bool Enabled { get; set; }

      public bool HasData { get; private set; }

      public Texture2D Icon { get; set; }

      public bool IsBold { get; set; }

      public int Depth { get; set; }

      public bool Expanded
      {
        get
        {
          return this.m_expandedCache.IsExpanded(this.HasData ? this.Data.GetHashCode() : this.Name.GetHashCode(), (string) null);
        }
        set
        {
          int instanceId = this.HasData ? this.Data.GetHashCode() : this.Name.GetHashCode();
          this.m_expandedCache.SetExpanded(value, instanceId, (string) null);
        }
      }

      public Node(string name, rdtGuiTree<T>.Node parent, rdtExpandedCache expandedCache)
      {
        this.Depth = parent == null ? 0 : parent.Depth + 1;
        this.Parent = parent;
        this.HasData = false;
        this.Name = name;
        this.m_expandedCache = expandedCache;
        this.Enabled = true;
        this.Children = new List<rdtGuiTree<T>.Node>();
      }

      public Node(T data, rdtGuiTree<T>.Node parent, rdtExpandedCache expandedCache)
      {
        this.Depth = parent == null ? 0 : parent.Depth + 1;
        this.HasData = true;
        this.m_expandedCache = expandedCache;
        this.Enabled = true;
        this.Parent = parent;
        this.Data = data;
        this.Children = new List<rdtGuiTree<T>.Node>();
        this.Name = this.Data.ToString();
      }

      public rdtGuiTree<T>.Node AddNode(T data, bool enabled = true)
      {
        rdtGuiTree<T>.Node node = new rdtGuiTree<T>.Node(data, this, this.m_expandedCache);
        node.Enabled = enabled;
        this.Children.Add(node);
        return node;
      }
    }
  }
}
