using System;
using System.Collections.Generic;

namespace LogSystem
{
  [Serializable]
  public class rdtExpandedCache
  {
    private Dictionary<string, bool> m_expandedState;

    public rdtExpandedCache()
    {
      this.m_expandedState = new Dictionary<string, bool>();
    }

    public void Clear()
    {
      this.m_expandedState.Clear();
    }

    public bool IsExpanded(int instanceId, string suffix = null)
    {
      string key = instanceId.ToString();
      if (!string.IsNullOrEmpty(suffix))
        key = key + "." + suffix;
      return this.m_expandedState.ContainsKey(key) && this.m_expandedState[key];
    }

    public bool IsExpanded(rdtTcpMessageComponents.Component component, string key = null)
    {
      return this.IsExpanded(component.m_instanceId, key);
    }

    public bool IsExpanded(
      rdtTcpMessageComponents.Component component,
      rdtTcpMessageComponents.Property property,
      string suffix = null)
    {
      return this.IsExpanded(component.m_instanceId, property.m_name + suffix);
    }

    public void SetExpanded(bool expanded, rdtTcpMessageComponents.Component component, string key = null)
    {
      this.SetExpanded(expanded, component.m_instanceId, key);
    }

    public void SetExpanded(
      bool expanded,
      rdtTcpMessageComponents.Component component,
      rdtTcpMessageComponents.Property property)
    {
      this.SetExpanded(expanded, component.m_instanceId, property.m_name);
    }

    public void SetExpanded(bool expanded, int instanceId, string suffix = null)
    {
      string index = instanceId.ToString();
      if (!string.IsNullOrEmpty(suffix))
        index = index + "." + suffix;
      this.m_expandedState[index] = expanded;
    }
  }
}
