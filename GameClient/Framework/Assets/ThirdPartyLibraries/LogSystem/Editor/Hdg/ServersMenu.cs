using System;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace LogSystem
{
  internal class ServersMenu
  {
    private List<rdtServerAddress> m_servers;
    private Action<rdtServerAddress> m_onSelected;
    private ServerAddressWindow m_popup;
    private Rect m_buttonRect;
    private EditorWindow m_owner;
    private LinkedList<rdtServerAddress> m_customServers;
    private const int SERVER_MRU_COUNT = 5;

    public List<rdtServerAddress> Servers
    {
      set
      {
        this.m_servers = value;
        this.m_servers.Sort();
      }
    }

    public ServersMenu(Action<rdtServerAddress> onSelected, EditorWindow owner)
    {
      this.m_customServers = new LinkedList<rdtServerAddress>();
      for (int index = 4; index >= 0; --index)
      {
        string key = "Hdg.RemoteDebug.RecentServer" + (object) index;
        if (EditorPrefs.HasKey(key))
          this.AddServer(EditorPrefs.GetString(key));
      }
      this.m_servers = new List<rdtServerAddress>();
      this.m_onSelected = onSelected;
      this.m_owner = owner;
    }

    public void Show(rdtServerAddress currentServer)
    {
      GUIContent content = new GUIContent("Active Player");
      Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.toolbarDropDown);
      GUI.Label(rect, content, EditorStyles.toolbarDropDown);
      Event current = Event.current;
      if (current.type == UnityEngine.EventType.Repaint)
      {
        Rect position = this.m_owner.position;
        this.m_buttonRect = GUILayoutUtility.GetLastRect();
        this.m_buttonRect.x += position.x;
        this.m_buttonRect.y += position.y;
      }
      if (!current.isMouse || current.type != UnityEngine.EventType.MouseDown || !rect.Contains(current.mousePosition))
        return;
      current.Use();
      GenericMenu menu = new GenericMenu();
      menu.AddItem(new GUIContent("None"), currentServer == null, new GenericMenu.MenuFunction2(this.OnContextMenu), (object) null);
      this.AddServers((IEnumerable<rdtServerAddress>) this.m_servers, menu, currentServer);
      this.AddServers((IEnumerable<rdtServerAddress>) this.m_customServers, menu, currentServer);
      menu.AddItem(new GUIContent("<Enter IP>"), false, (GenericMenu.MenuFunction) (() => this.OnEnterIP(this.m_buttonRect)));
      menu.DropDown(rect);
    }

    public void Destroy()
    {
      if ((UnityEngine.Object) this.m_popup != (UnityEngine.Object) null)
        this.m_popup.Close();
      this.m_popup = (ServerAddressWindow) null;
    }

    private void AddServers(
      IEnumerable<rdtServerAddress> servers,
      GenericMenu menu,
      rdtServerAddress currentServer)
    {
      foreach (rdtServerAddress server in servers)
        menu.AddItem(new GUIContent(server.ToString()), server.Equals((object) currentServer), new GenericMenu.MenuFunction2(this.OnContextMenu), (object) server);
    }

    private void OnContextMenu(object userdata)
    {
      this.m_onSelected(userdata as rdtServerAddress);
    }

    private void OnEnterIP(Rect rect)
    {
      if ((UnityEngine.Object) this.m_popup != (UnityEngine.Object) null)
        return;
      this.m_popup = ScriptableObject.CreateInstance<ServerAddressWindow>();
      this.m_popup.Callback = new Action<string>(this.OnWindowDismissed);
      this.m_popup.ShowAsDropDown(rect, new Vector2(344f, 54f));
    }

    private void OnWindowDismissed(string server)
    {
      this.m_popup = (ServerAddressWindow) null;
      rdtServerAddress rdtServerAddress = this.AddServer(server);
      if (rdtServerAddress == null)
        return;
      int count = this.m_customServers.Count;
      if (count > 5)
      {
        int num = count - 5;
        for (int index = 0; index < num; ++index)
          this.m_customServers.RemoveLast();
      }
      int num1 = 0;
      foreach (rdtServerAddress customServer in this.m_customServers)
        EditorPrefs.SetString("Hdg.RemoteDebug.RecentServer" + (object) num1++, customServer.IPAddress.ToString() + ":" + (object) customServer.m_port);
      this.m_onSelected(rdtServerAddress);
    }

    private rdtServerAddress AddServer(string server)
    {
      if (string.IsNullOrEmpty(server))
        return (rdtServerAddress) null;
      int length = server.IndexOf(':');
      string ipString = length != -1 ? server.Substring(0, length) : server;
      int result = Settings.DEFAULT_SERVER_PORT;
      if (length != -1)
      {
        string s = server.Substring(length + 1);
        if (!string.IsNullOrEmpty(s) && !int.TryParse(s, out result))
        {
          Debug.LogError((object) string.Format("Invalid IP address '{0}'", (object) server));
          return (rdtServerAddress) null;
        }
      }
      IPAddress address = (IPAddress) null;
      if (!IPAddress.TryParse(ipString, out address))
      {
        Debug.LogError((object) string.Format("Invalid IP address '{0}'", (object) server));
        return (rdtServerAddress) null;
      }
      rdtServerAddress rdtServerAddress = new rdtServerAddress(address, result, server, "Custom", "Custom", "Unknown");
      this.m_customServers.AddFirst(rdtServerAddress);
      return rdtServerAddress;
    }
  }
}
