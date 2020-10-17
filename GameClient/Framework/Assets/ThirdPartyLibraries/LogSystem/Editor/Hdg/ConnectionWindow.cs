using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace LogSystem
{
    public class ConnectionWindow : EditorWindow
    {
        private bool m_automaticRefresh = true;
        private rdtSerializerRegistry m_serializerRegistry = new rdtSerializerRegistry();
        private rdtExpandedCache m_expandedCache = new rdtExpandedCache();
        private bool m_debug;
        private bool m_forceRefresh;
        [NonSerialized]
        private bool m_waitingForGameObjects;
        private rdtTcpMessageComponents? m_components;
        private rdtClient m_client;
        private double m_lastTime;
        private rdtServerAddress m_currentServer;
        private Vector2 m_componentsScrollPos;
        private double m_gameObjectRefreshTimer;
        private double m_componentRefreshTimer;
        private rdtGuiSplit m_split;
        private rdtGuiTree<rdtTcpMessageGameObjects.Gob> m_tree;
        private bool m_updatingTree;
        private const float WIDE_MODE_SIZE_THRESHOLD = 330f;
        private const float LABEL_ADJUST_SIZE_THRESHOLD = 350f;
        private GUIStyle m_normalFoldoutStyle;
        private GUIStyle m_toggleStyle;
        private bool m_isProSkin;
        private rdtGuiProperty m_propertyGui;
        private rdtClientEnumerateServers m_serverEnum;
        private ServersMenu m_serversMenu;
        [NonSerialized]
        private rdtTcpMessageComponents.Component? m_pendingExpandComponent;
        [NonSerialized]
        private List<rdtTcpMessageGameObjects.Gob> m_gameObjects;
        [NonSerialized]
        private bool m_clearFocus;
        private static ConnectionWindow s_instance;
        private Texture2D m_sceneIcon;

        public static ConnectionWindow Instance
        {
            get
            {
                return ConnectionWindow.s_instance;
            }
        }

        public ConnectionWindow()
        {
            this.minSize = new Vector2(400f, 100f);
        }

        public void RestartServerEnumerator()
        {
            if (this.m_serverEnum != null)
            {
                rdtDebug.Debug("Stopping server enumerator");
                this.m_serverEnum.Stop();
            }
            this.m_serverEnum = new rdtClientEnumerateServers();
        }

        public void Connect(rdtServerAddress address)
        {
            this.Disconnect(true);
            this.m_expandedCache.Clear();
            this.m_pendingExpandComponent = new rdtTcpMessageComponents.Component?();
            this.m_components = new rdtTcpMessageComponents?();
            this.m_currentServer = address;
            this.m_client = new rdtClient();
            this.m_propertyGui = new rdtGuiProperty(new rdtGuiProperty.ComponentValueChangedHandler(this.OnComponentValueChanged));
            this.m_client.Connect(address.IPAddress, address.m_port);
            this.m_client.AddCallback(typeof(rdtTcpMessageGameObjects), new Action<rdtTcpMessage>(this.OnMessageGameObjects));
            this.m_client.AddCallback(typeof(rdtTcpMessageComponents), new Action<rdtTcpMessage>(this.OnMessageGameObjectComponents));
        }

        private void InitStyles()
        {
            if (this.m_isProSkin == EditorGUIUtility.isProSkin && this.m_toggleStyle != null && (this.m_normalFoldoutStyle != null && (UnityEngine.Object)this.m_toggleStyle.normal.background != (UnityEngine.Object)null))
                return;
            this.m_isProSkin = EditorGUIUtility.isProSkin;
            this.m_toggleStyle = new GUIStyle(EditorStyles.toggle);
            this.m_toggleStyle.overflow.top = -2;
            this.m_normalFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            this.m_normalFoldoutStyle.overflow.top = -2;
            this.m_normalFoldoutStyle.active.textColor = EditorStyles.foldout.normal.textColor;
            this.m_normalFoldoutStyle.onActive.textColor = EditorStyles.foldout.normal.textColor;
            this.m_normalFoldoutStyle.onFocused.textColor = EditorStyles.foldout.normal.textColor;
            this.m_normalFoldoutStyle.onFocused.background = EditorStyles.foldout.onNormal.background;
            this.m_normalFoldoutStyle.focused.textColor = EditorStyles.foldout.normal.textColor;
            this.m_normalFoldoutStyle.focused.background = EditorStyles.foldout.normal.background;
        }

        private void Disconnect(bool resetServer = true)
        {
            if (resetServer)
                this.m_currentServer = (rdtServerAddress)null;
            if (this.m_client != null)
                this.m_client.Stop();
            this.m_tree.Clear();
            this.m_components = new rdtTcpMessageComponents?();
            this.m_waitingForGameObjects = false;
        }

        private void OnEnable()
        {
            this.m_debug = EditorPrefs.GetBool("Hdg.RemoteDebug.Debug", false);
            rdtDebug.s_logLevel = this.m_debug ? rdtDebug.LogLevel.Debug : rdtDebug.LogLevel.Info;
            rdtDebug.Debug(nameof(OnEnable));
            ConnectionWindow.s_instance = this;
            this.m_split = new rdtGuiSplit(200f, 100f, (EditorWindow)this);
            this.m_tree = new rdtGuiTree<rdtTcpMessageGameObjects.Gob>();
            this.m_tree.SelectedNodesChanged += new Action(this.OnTreeSelectionChanged);
            this.m_tree.SelectedNodesDeleted += new Action(this.OnTreeSelectionDeleted);
            EditorApplication.playModeStateChanged += this.OnPlaymodeStateChanged;
            this.RestartServerEnumerator();
            this.m_automaticRefresh = EditorPrefs.GetBool("Hdg.RemoteDebug.AutomaticRefresh", false);
            this.m_serversMenu = new ServersMenu(new Action<rdtServerAddress>(this.OnServerSelected), (EditorWindow)this);
            this.m_isProSkin = EditorGUIUtility.isProSkin;
            this.m_expandedCache.Clear();
        }

        private void OnDisable()
        {
            rdtDebug.Debug(nameof(OnDisable));
            EditorApplication.playModeStateChanged -= this.OnPlaymodeStateChanged;
            ConnectionWindow.s_instance = (ConnectionWindow)null;
            this.m_serversMenu.Destroy();
            this.Disconnect(false);
            if (this.m_serverEnum == null)
                return;
            this.m_serverEnum.Stop();
        }

        private void OnMessageGameObjects(rdtTcpMessage message)
        {
            this.m_waitingForGameObjects = false;
            this.m_gameObjects = ((rdtTcpMessageGameObjects)message).m_allGobs;
            this.m_updatingTree = true;
            List<rdtTcpMessageGameObjects.Gob> list1 = this.m_tree.SelectedNodes.Where<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, bool>)(x => x.HasData)).Select<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, rdtTcpMessageGameObjects.Gob>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, rdtTcpMessageGameObjects.Gob>)(x => x.Data)).ToList<rdtTcpMessageGameObjects.Gob>();
            List<string> list2 = this.m_tree.SelectedNodes.Where<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, bool>)(x => !x.HasData)).Select<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, string>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, string>)(x => x.Name)).ToList<string>();
            this.BuildTree();
            if (list1.Count > 0 || list2.Count > 0)
            {
                List<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node> list3 = list1.Select<rdtTcpMessageGameObjects.Gob, rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtTcpMessageGameObjects.Gob, rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>)(x => this.m_tree.FindNode(x))).Where<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, bool>)(x => x != null)).ToList<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>();
                List<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node> list4 = list2.Select<string, rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<string, rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>)(x => this.m_tree.FindNode(x))).Where<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, bool>)(x => x != null)).ToList<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>();
                this.m_tree.SelectedNodes.AddRange((IEnumerable<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>)list3);
                this.m_tree.SelectedNodes.AddRange((IEnumerable<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>)list4);
            }
            this.m_updatingTree = false;
            this.Repaint();
        }

        private void BuildTree()
        {
            this.m_tree.Clear();
            List<rdtTcpMessageGameObjects.Gob> list1 = this.m_gameObjects.Where<rdtTcpMessageGameObjects.Gob>((Func<rdtTcpMessageGameObjects.Gob, bool>)(x => !x.m_hasParent)).ToList<rdtTcpMessageGameObjects.Gob>();
            Dictionary<int, rdtTcpMessageGameObjects.Gob> existing = new Dictionary<int, rdtTcpMessageGameObjects.Gob>();
            List<rdtTcpMessageGameObjects.Gob> list2 = this.m_gameObjects.Where<rdtTcpMessageGameObjects.Gob>((Func<rdtTcpMessageGameObjects.Gob, bool>)(x => x.m_hasParent)).Where<rdtTcpMessageGameObjects.Gob>((Func<rdtTcpMessageGameObjects.Gob, bool>)(x =>
          {
              if (existing.ContainsKey(x.m_instanceId))
                  return false;
              existing.Add(x.m_instanceId, x);
              return true;
          })).OrderBy<rdtTcpMessageGameObjects.Gob, int>((Func<rdtTcpMessageGameObjects.Gob, int>)(x => x.m_parentInstanceId)).ToList<rdtTcpMessageGameObjects.Gob>();
            if ((UnityEngine.Object)this.m_sceneIcon == (UnityEngine.Object)null)
                this.m_sceneIcon = EditorGUIUtility.FindTexture("BuildSettings.Editor.Small");
            List<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node> source = new List<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>();
            foreach (rdtTcpMessageGameObjects.Gob gob in list1)
            {
                rdtTcpMessageGameObjects.Gob r = gob;
                rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node node = source.FirstOrDefault<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, bool>)(x => x.Name.Equals(r.m_scene)));
                if (node == null)
                {
                    node = this.m_tree.AddNode(r.m_scene);
                    source.Add(node);
                    node.Icon = this.m_sceneIcon;
                    node.IsBold = true;
                }
                this.AddChildren(node.AddNode(r, r.m_enabled), list2);
            }
        }

        private void AddChildren(
          rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node parentNode,
          List<rdtTcpMessageGameObjects.Gob> nonRoots)
        {
            int index1 = 0;
            while (index1 < nonRoots.Count && nonRoots[index1].m_parentInstanceId != parentNode.Data.m_instanceId)
                ++index1;
            if (index1 >= nonRoots.Count || nonRoots.Count == 0)
                return;
            List<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node> nodeList = new List<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>();
            int count = 0;
            int index2 = index1;
            while (index2 < nonRoots.Count)
            {
                rdtTcpMessageGameObjects.Gob nonRoot = nonRoots[index2];
                if (nonRoot.m_parentInstanceId == parentNode.Data.m_instanceId)
                {
                    rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node node = parentNode.AddNode(nonRoot, nonRoot.m_enabled);
                    nodeList.Add(node);
                    ++index2;
                    ++count;
                }
                else
                    break;
            }
            nonRoots.RemoveRange(index1, count);
            for (int index3 = 0; index3 < nodeList.Count; ++index3)
                this.AddChildren(nodeList[index3], nonRoots);
        }

        private void Update()
        {
            if (EditorApplication.isCompiling && this.m_client != null && this.m_client.IsConnected)
                this.Disconnect(false);
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            double delta = timeSinceStartup - this.m_lastTime;
            this.m_lastTime = timeSinceStartup;
            this.UpdateServers(delta);
            if (this.m_client == null)
                return;
            bool isConnected = this.m_client.IsConnected;
            bool isConnecting = this.m_client.IsConnecting;
            this.m_client.Update(delta);
            if (this.m_client.IsConnected != isConnected || ((this.m_client.IsConnected ? 0 : (!this.m_client.IsConnecting ? 1 : 0)) & (isConnecting ? 1 : 0)) != 0)
                this.OnConnectionStatusChanged();
            if (!this.m_automaticRefresh && !this.m_forceRefresh)
                return;
            this.m_gameObjectRefreshTimer -= delta;
            if (this.m_gameObjectRefreshTimer <= 0.0)
            {
                this.m_forceRefresh = false;
                this.RefreshGameObjects();
                this.m_gameObjectRefreshTimer = (double)Settings.GAMEOBJECT_UPDATE_TIME;
            }
            if (this.m_tree.SelectedNodes.Count != 1)
                return;
            this.m_componentRefreshTimer -= delta;
            if (this.m_componentRefreshTimer > 0.0)
                return;
            this.RefreshComponents();
            this.m_componentRefreshTimer = (double)Settings.COMPONENT_UPDATE_TIME;
        }

        private void RefreshComponents()
        {
            if (this.m_tree.SelectedNodes.Count == 0)
                return;
            rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node selectedNode = this.m_tree.SelectedNodes[0];
            if (!selectedNode.HasData)
                return;
            this.m_client.EnqueueMessage((rdtTcpMessage)new rdtTcpMessageGetComponents()
            {
                m_instanceId = selectedNode.Data.m_instanceId
            });
        }

        private void RefreshGameObjects()
        {
            if (this.m_client == null || !this.m_client.IsConnected || this.m_waitingForGameObjects)
                return;
            rdtDebug.Debug((object)this, "Refreshing GameObject list from the server");
            this.m_client.EnqueueMessage((rdtTcpMessage)new rdtTcpMessageGetGameObjects());
            this.m_waitingForGameObjects = true;
        }

        private void OnGUI()
        {
            if (this.m_clearFocus)
            {
                this.m_clearFocus = false;
                UnityEngine.GUI.FocusControl((string)null);
            }
            this.InitStyles();
            this.DrawToolbar();
            this.Draw();
            this.ProcessInput();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            this.m_serversMenu.Show(this.m_currentServer);
            int num = this.m_client == null ? 0 : (this.m_client.IsConnecting ? 1 : 0);
            bool flag1 = this.m_client != null && this.m_client.IsConnected;
            string text = num != 0 ? "Connecting" : (!flag1 || this.m_currentServer == null ? "Not connected" : this.m_currentServer.ToString());
            if (flag1 && this.m_currentServer != null && this.m_debug)
                text = text + " - Server Version " + this.m_currentServer.m_serverVersion;
            GUILayout.Label(text, EditorStyles.toolbarButton);
            this.m_tree.Filter = UnityEditorInternals.GUI.ToolbarSearchField(this.m_tree.Filter, GUILayout.Width(250f));
            GUILayout.FlexibleSpace();
            bool enabled = UnityEngine.GUI.enabled;
            bool flag2 = GUILayout.Toggle(this.m_automaticRefresh, "Automatic Refresh", EditorStyles.toolbarButton);
            if (flag2 != this.m_automaticRefresh)
            {
                this.m_automaticRefresh = flag2;
                EditorPrefs.SetBool("Hdg.RemoteDebug.AutomaticRefresh", this.m_automaticRefresh);
            }
            UnityEngine.GUI.enabled = !this.m_waitingForGameObjects;
            if (!this.m_waitingForGameObjects)
                UnityEngine.GUI.enabled = !this.m_automaticRefresh;
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                this.RefreshGameObjects();
                this.RefreshComponents();
            }
            UnityEngine.GUI.enabled = enabled;
            GUILayout.EndHorizontal();
        }

        private void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            this.m_tree.Draw(EditorGUILayout.GetControlRect(false, 1f, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.Width(this.m_split.SeparatorPosition)), (UnityEngine.Object)EditorWindow.focusedWindow == (UnityEngine.Object)this);
            this.m_split.Draw();
            this.m_componentsScrollPos = EditorGUILayout.BeginScrollView(this.m_componentsScrollPos);
            float num = this.position.width - this.m_split.SeparatorPosition;
            EditorGUIUtility.wideMode = (double)num >= 330.0;
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
            if ((double)num > 350.0)
                EditorGUIUtility.labelWidth = (float)(((double)num - 350.0) * 0.5) + EditorGUIUtility.labelWidth;
            this.DrawComponents();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private void ProcessInput()
        {
            Event current = Event.current;
            if (!current.isMouse || current.type != UnityEngine.EventType.MouseUp || !this.m_pendingExpandComponent.HasValue)
                return;
            UnityEngine.GUI.FocusControl((string)null);
            this.m_expandedCache.SetExpanded(!this.m_expandedCache.IsExpanded(this.m_pendingExpandComponent.Value, (string)null), this.m_pendingExpandComponent.Value, (string)null);
            this.m_pendingExpandComponent = new rdtTcpMessageComponents.Component?();
            this.Repaint();
        }

        private void DrawComponents()
        {
            if (!this.m_components.HasValue)
                return;
            if (this.m_components.Value.m_instanceId == 0)
            {
                EditorGUILayout.LabelField("GameObject was not found.");
            }
            else
            {
                this.DrawGameObjectTitle();
                rdtGuiLine.DrawHorizontalLine();
                if (this.m_tree.SelectedNodes.Count > 1)
                    return;
                foreach (rdtTcpMessageComponents.Component component in this.m_components.Value.m_components)
                {
                    if (!this.DrawComponentTitle(component, (UnityEngine.Component)null))
                    {
                        rdtGuiLine.DrawHorizontalLine();
                    }
                    else
                    {
                        this.m_propertyGui.DrawComponent(this.m_components.Value.m_instanceId, component, component.m_properties);
                        EditorGUILayout.Space();
                        rdtGuiLine.DrawHorizontalLine();
                    }
                }
                EditorGUILayout.Space();
            }
        }

        private void DrawGameObjectTitle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            bool flag = EditorGUILayout.ToggleLeft("Enabled", this.m_components.Value.m_enabled);
            if (flag != this.m_components.Value.m_enabled)
            {
                rdtTcpMessageComponents messageComponents = this.m_components.Value;
                messageComponents.m_enabled = flag;
                this.m_components = new rdtTcpMessageComponents?(messageComponents);
                this.m_tree.SelectedNodes[0].Enabled = flag;
                this.OnGameObjectChanged();
            }
            if (this.m_tree.SelectedNodes.Count == 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tag", GUILayout.Width(30f));
                string str = EditorGUILayout.TagField(GUIContent.none, this.m_components.Value.m_tag, GUILayout.MinWidth(50f));
                if (str != this.m_components.Value.m_tag)
                {
                    rdtTcpMessageComponents messageComponents = this.m_components.Value;
                    messageComponents.m_tag = str;
                    this.m_components = new rdtTcpMessageComponents?(messageComponents);
                    this.OnGameObjectChanged();
                }
                EditorGUILayout.LabelField("Layer", GUILayout.Width(40f));
                int num = EditorGUILayout.LayerField(GUIContent.none, this.m_components.Value.m_layer, GUILayout.MinWidth(50f));
                if (num != this.m_components.Value.m_layer)
                {
                    rdtTcpMessageComponents messageComponents = this.m_components.Value;
                    messageComponents.m_layer = num;
                    this.m_components = new rdtTcpMessageComponents?(messageComponents);
                    this.OnGameObjectChanged();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void OnComponentValueChanged(rdtGuiProperty.ValueChangedEvent valueChangedEvent)
        {
            if (valueChangedEvent.UpdateProperty)
            {
                rdtTcpMessageComponents.Property property = valueChangedEvent.Hierarchy.Pop();
                property.m_value = valueChangedEvent.NewValue;
                valueChangedEvent.Hierarchy.Push(property);
            }
            this.OnPropertyChanged(valueChangedEvent, (UnityEngine.Component)null);
        }

        private bool DrawComponentTitle(
          rdtTcpMessageComponents.Component component,
          UnityEngine.Component unityComponent = null)
        {
            bool foldout = this.m_expandedCache.IsExpanded(component, (string)null);
            bool expanded = foldout;
            Rect rect1 = EditorGUILayout.BeginHorizontal(GUILayout.Height(18f));
            GUILayout.Space(4f);
            Rect rect2 = GUILayoutUtility.GetRect(13f, 16f, GUILayout.ExpandWidth(false));
            if (component.m_properties != null && component.m_properties.Count > 0)
            {
                expanded = EditorGUI.Foldout(rect2, foldout, GUIContent.none, this.m_normalFoldoutStyle);
                if (expanded != foldout)
                    this.m_expandedCache.SetExpanded(expanded, component, (string)null);
            }
            bool flag = true;
            int width = this.m_toggleStyle.normal.background.width;
            if (component.m_canBeDisabled)
                flag = EditorGUILayout.Toggle((component.m_enabled ? 1 : 0) != 0, this.m_toggleStyle, GUILayout.Width((float)width));
            else
                EditorGUILayout.LabelField("", GUILayout.Width((float)width));
            string label = ObjectNames.NicifyVariableName(component.m_name);
            if (this.m_debug)
                label = label + ":" + (object)component.m_instanceId;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            if (component.m_canBeDisabled && flag != component.m_enabled)
            {
                component.m_enabled = flag;
                this.OnPropertyChanged(new rdtGuiProperty.ValueChangedEvent()
                {
                    Component = component
                }, (UnityEngine.Component)null);
            }
            EditorGUILayout.EndHorizontal();
            if (component.m_properties != null && component.m_properties.Count > 0)
            {
                Event current = Event.current;
                if (rect1.Contains(current.mousePosition) && current.isMouse)
                {
                    if (current.type == UnityEngine.EventType.MouseDown)
                    {
                        this.m_pendingExpandComponent = new rdtTcpMessageComponents.Component?(component);
                        current.Use();
                    }
                    else if (current.type == UnityEngine.EventType.MouseUp && this.m_pendingExpandComponent.HasValue && this.m_pendingExpandComponent.Value.m_instanceId != component.m_instanceId)
                        this.m_pendingExpandComponent = new rdtTcpMessageComponents.Component?();
                }
            }
            return expanded;
        }

        private void OnMessageGameObjectComponents(rdtTcpMessage message)
        {
            this.m_components = new rdtTcpMessageComponents?((rdtTcpMessageComponents)message);
            List<rdtTcpMessageComponents.Component> components = this.m_components.Value.m_components;
            for (int index1 = 0; index1 < components.Count; ++index1)
            {
                rdtTcpMessageComponents.Component component = components[index1];
                if (component.m_properties == null)
                {
                    rdtDebug.Debug((object)this, "Component '{0}' has no properties", (object)component.m_name);
                }
                else
                {
                    for (int index2 = 0; index2 < component.m_properties.Count; ++index2)
                    {
                        rdtTcpMessageComponents.Property property = component.m_properties[index2];
                        property.Deserialise(this.m_serializerRegistry);
                        component.m_properties[index2] = property;
                    }
                    components[index1] = component;
                }
            }
            this.Repaint();
        }

        private void OnTreeSelectionChanged()
        {
            if (this.m_updatingTree)
                return;
            this.m_clearFocus = true;
            rdtGuiTree<rdtTcpMessageGameObjects.Gob>.SelectedNodeCollection selectedNodes = this.m_tree.SelectedNodes;
            if (selectedNodes.Count == 0 || selectedNodes.Any<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, bool>)(x => !x.HasData)))
            {
                this.m_pendingExpandComponent = new rdtTcpMessageComponents.Component?();
                this.m_components = new rdtTcpMessageComponents?();
            }
            else
                this.RefreshComponents();
            this.Repaint();
        }

        private void OnTreeSelectionDeleted()
        {
            rdtTcpMessageDeleteGameObjects deleteGameObjects = new rdtTcpMessageDeleteGameObjects();
            IEnumerable<int> source = this.m_tree.SelectedNodes.Select<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, int>((Func<rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node, int>)(x => x.Data.m_instanceId));
            deleteGameObjects.m_instanceIds = source.ToList<int>();
            this.m_client.EnqueueMessage((rdtTcpMessage)deleteGameObjects);
            this.m_tree.SelectedNodes.Clear();
            this.m_gameObjectRefreshTimer = 0.10000000149011612;
            this.m_forceRefresh = true;
        }

        private List<rdtTcpMessageComponents.Property> CloneAndSerialize(
          Stack<rdtTcpMessageComponents.Property> hierarchy,
          bool serialiseValues = true)
        {
            List<rdtTcpMessageComponents.Property> propertyList = new List<rdtTcpMessageComponents.Property>();
            rdtTcpMessageComponents.Property property1 = hierarchy.Peek();
            rdtTcpMessageComponents.Property property2 = property1.Clone();
            if (serialiseValues)
                property2.m_value = this.m_serializerRegistry.Serialize(property1.m_value);
            bool flag = true;
            foreach (rdtTcpMessageComponents.Property property3 in hierarchy)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    rdtTcpMessageComponents.Property property4 = property3.Clone();
                    property4.m_value = (object)new List<rdtTcpMessageComponents.Property>()
          {
            property2
          };
                    property2 = property4;
                }
            }
            propertyList.Add(property2);
            return propertyList;
        }

        private void OnPropertyChanged(
          rdtGuiProperty.ValueChangedEvent valueChangedEvent,
          UnityEngine.Component unityComponent = null)
        {
            if (this.m_client == null)
                return;
            rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node selectedNode = this.m_tree.SelectedNodes[0];
            rdtTcpMessage message;
            if (valueChangedEvent.NewArraySize != -1)
            {
                rdtTcpMessageSetArraySize messageSetArraySize = new rdtTcpMessageSetArraySize();
                messageSetArraySize.m_gameObjectInstanceId = selectedNode.Data.m_instanceId;
                messageSetArraySize.m_componentName = valueChangedEvent.Component.m_name;
                messageSetArraySize.m_componentInstanceId = valueChangedEvent.Component.m_instanceId;
                messageSetArraySize.m_size = valueChangedEvent.NewArraySize;
                Stack<rdtTcpMessageComponents.Property> hierarchy = valueChangedEvent.Hierarchy;
                if (hierarchy != null && hierarchy.Count > 0)
                    messageSetArraySize.m_properties = this.CloneAndSerialize(hierarchy, false);
                message = (rdtTcpMessage)messageSetArraySize;
            }
            else
            {
                rdtTcpMessageUpdateComponentProperties componentProperties = new rdtTcpMessageUpdateComponentProperties();
                componentProperties.m_arrayIndex = valueChangedEvent.ArrayIndex;
                componentProperties.m_gameObjectInstanceId = selectedNode.Data.m_instanceId;
                componentProperties.m_componentName = valueChangedEvent.Component.m_name;
                componentProperties.m_componentInstanceId = valueChangedEvent.Component.m_instanceId;
                componentProperties.m_enabled = valueChangedEvent.Component.m_enabled;
                Stack<rdtTcpMessageComponents.Property> hierarchy = valueChangedEvent.Hierarchy;
                if (hierarchy != null && hierarchy.Count > 0)
                    componentProperties.m_properties = this.CloneAndSerialize(hierarchy, true);
                message = (rdtTcpMessage)componentProperties;
            }
            this.m_client.EnqueueMessage(message);
            this.RefreshComponents();
        }

        private void OnGameObjectChanged()
        {
            if (this.m_client == null)
                return;
            rdtGuiTree<rdtTcpMessageGameObjects.Gob>.SelectedNodeCollection selectedNodes = this.m_tree.SelectedNodes;
            for (int index = 0; index < selectedNodes.Count; ++index)
            {
                rdtGuiTree<rdtTcpMessageGameObjects.Gob>.Node node = selectedNodes[index];
                rdtTcpMessageUpdateGameObjectProperties objectProperties = new rdtTcpMessageUpdateGameObjectProperties();
                objectProperties.m_instanceId = node.Data.m_instanceId;
                objectProperties.m_enabled = this.m_components.Value.m_enabled;
                objectProperties.SetFlag(rdtTcpMessageUpdateGameObjectProperties.Flags.UpdateEnabled, true);
                objectProperties.m_layer = this.m_components.Value.m_layer;
                objectProperties.SetFlag(rdtTcpMessageUpdateGameObjectProperties.Flags.UpdateLayer, selectedNodes.Count == 1);
                objectProperties.m_tag = this.m_components.Value.m_tag;
                objectProperties.SetFlag(rdtTcpMessageUpdateGameObjectProperties.Flags.UpdateTag, selectedNodes.Count == 1);
                this.m_client.EnqueueMessage((rdtTcpMessage)objectProperties);
            }
        }

        private void OnConnectionStatusChanged()
        {
            rdtDebug.Debug((object)this, nameof(OnConnectionStatusChanged));
            if (this.m_client == null || !this.m_client.IsConnected)
                this.m_currentServer = (rdtServerAddress)null;
            this.m_pendingExpandComponent = new rdtTcpMessageComponents.Component?();
            this.m_components = new rdtTcpMessageComponents?();
            this.m_expandedCache.Clear();
            this.m_tree.Clear();
            if (!this.m_automaticRefresh && this.m_client != null && this.m_client.IsConnected)
                this.RefreshGameObjects();
            this.Repaint();
        }

        private void UpdateServers(double delta)
        {
            this.m_serverEnum.Update(delta);
            this.m_serversMenu.Servers = this.m_serverEnum.Servers;
        }

        private void OnServerSelected(rdtServerAddress server)
        {
            if (server != null)
                this.Connect(server);
            else
                this.Disconnect(true);
        }

        [DidReloadScripts]
        private static void OnUnityReloadedAssemblies()
        {
            if ((UnityEngine.Object)ConnectionWindow.s_instance == (UnityEngine.Object)null)
                return;
            ConnectionWindow.s_instance.OnUnityReloadedAssembliesImp();
        }

        private void OnUnityReloadedAssembliesImp()
        {
            rdtDebug.Debug((object)this, "OnUnityReloadedAssemblies");
            rdtDebug.s_logLevel = this.m_debug ? rdtDebug.LogLevel.Debug : rdtDebug.LogLevel.Info;
            if (this.m_currentServer == null)
                return;
            if (this.m_currentServer.IPAddress == null)
                this.m_currentServer = (rdtServerAddress)null;
            else
                this.Connect(this.m_currentServer);
        }

        private void OnPlaymodeStateChanged(PlayModeStateChange state)
        {
            this.Disconnect(true);
        }
    }
}
