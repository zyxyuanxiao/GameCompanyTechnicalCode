using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LogSystem
{
  public class rdtMessageGameObjectsHandler
  {
    private RemoteDebugServer m_server;
    private bool m_dontDestroyOnLoadBadObject;
    private List<GameObject> m_gameObjects;
    private List<rdtTcpMessageGameObjects.Gob> m_allGobs;
    private List<rdtTcpMessageComponents.Component> m_components;
    private List<UnityEngine.Component> m_unityComponents;

    public rdtMessageGameObjectsHandler(RemoteDebugServer server)
    {
      this.m_gameObjects = new List<GameObject>(2048);
      this.m_allGobs = new List<rdtTcpMessageGameObjects.Gob>(2048);
      this.m_components = new List<rdtTcpMessageComponents.Component>(16);
      this.m_unityComponents = new List<UnityEngine.Component>(16);
      this.m_server = server;
      this.m_server.AddCallback(typeof (rdtTcpMessageGetGameObjects), new Action<rdtTcpMessage>(this.OnRequestGameObjects));
      this.m_server.AddCallback(typeof (rdtTcpMessageGetComponents), new Action<rdtTcpMessage>(this.OnRequestGameObjectComponents));
      this.m_server.AddCallback(typeof (rdtTcpMessageUpdateComponentProperties), new Action<rdtTcpMessage>(this.OnUpdateComponentProperties));
      this.m_server.AddCallback(typeof (rdtTcpMessageUpdateGameObjectProperties), new Action<rdtTcpMessage>(this.OnUpdateGameObjectProperties));
      this.m_server.AddCallback(typeof (rdtTcpMessageSetArraySize), new Action<rdtTcpMessage>(this.OnSetArraySize));
      this.m_server.AddCallback(typeof (rdtTcpMessageDeleteGameObjects), new Action<rdtTcpMessage>(this.OnDeleteGameObjects));
    }

    private void OnUpdateGameObjectProperties(rdtTcpMessage message)
    {
      rdtTcpMessageUpdateGameObjectProperties objectProperties = (rdtTcpMessageUpdateGameObjectProperties) message;
      GameObject gameObject = this.FindGameObject(objectProperties.m_instanceId);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      if (objectProperties.HasFlag(rdtTcpMessageUpdateGameObjectProperties.Flags.UpdateEnabled))
        gameObject.SetActive(objectProperties.m_enabled);
      if (objectProperties.HasFlag(rdtTcpMessageUpdateGameObjectProperties.Flags.UpdateLayer))
        gameObject.layer = objectProperties.m_layer;
      if (!objectProperties.HasFlag(rdtTcpMessageUpdateGameObjectProperties.Flags.UpdateTag))
        return;
      gameObject.tag = objectProperties.m_tag;
    }

    private void OnUpdateComponentProperties(rdtTcpMessage message)
    {
      rdtDebug.Debug((object) this, nameof (OnUpdateComponentProperties));
      rdtTcpMessageUpdateComponentProperties componentProperties = (rdtTcpMessageUpdateComponentProperties) message;
      GameObject gameObject = this.FindGameObject(componentProperties.m_gameObjectInstanceId);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      UnityEngine.Component component = this.FindComponent(gameObject, componentProperties.m_componentInstanceId);
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        rdtDebug.Error((object) this, "Tried to update component with id {0} (name={1}) but couldn't find it!", (object) componentProperties.m_componentInstanceId, (object) componentProperties.m_componentName);
      }
      else
      {
        switch (component)
        {
          case Behaviour _:
            ((Behaviour) component).enabled = componentProperties.m_enabled;
            break;
          case Renderer _:
            ((Renderer) component).enabled = componentProperties.m_enabled;
            break;
          case Collider _:
            ((Collider) component).enabled = componentProperties.m_enabled;
            break;
        }
        if (componentProperties.m_properties == null)
          return;
        this.m_server.SerializerRegistry.WriteAllFields((object) component, componentProperties.m_properties, componentProperties.m_arrayIndex);
        Graphic graphic = component as Graphic;
        if (!(bool) (UnityEngine.Object) graphic)
          return;
        graphic.SetAllDirty();
      }
    }

    private void OnSetArraySize(rdtTcpMessage message)
    {
      rdtDebug.Debug((object) this, "rdtTcpMessageSetArraySize");
      rdtTcpMessageSetArraySize messageSetArraySize = (rdtTcpMessageSetArraySize) message;
      GameObject gameObject = this.FindGameObject(messageSetArraySize.m_gameObjectInstanceId);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null || messageSetArraySize.m_size < 0)
        return;
      UnityEngine.Component component = this.FindComponent(gameObject, messageSetArraySize.m_componentInstanceId);
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        rdtDebug.Error((object) this, "Tried to set array size on component with id {0} (name={1}) but couldn't find it!", (object) messageSetArraySize.m_componentInstanceId, (object) messageSetArraySize.m_componentName);
      else
        this.m_server.SerializerRegistry.SetArraySize((object) component, messageSetArraySize.m_properties, messageSetArraySize.m_size);
    }

    private void OnDeleteGameObjects(rdtTcpMessage message)
    {
      rdtTcpMessageDeleteGameObjects deleteGameObjects = (rdtTcpMessageDeleteGameObjects) message;
      for (int index = 0; index < deleteGameObjects.m_instanceIds.Count; ++index)
      {
        GameObject gameObject = this.FindGameObject(deleteGameObjects.m_instanceIds[index]);
        if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
          UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      }
    }

    private void OnRequestGameObjectComponents(rdtTcpMessage message)
    {
      rdtTcpMessageGetComponents messageGetComponents = (rdtTcpMessageGetComponents) message;
      GameObject gameObject = messageGetComponents.m_instanceId != 0 ? this.FindGameObject(messageGetComponents.m_instanceId) : (GameObject) null;
      rdtTcpMessageComponents messageComponents = new rdtTcpMessageComponents();
      messageComponents.m_instanceId = (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? messageGetComponents.m_instanceId : 0;
      messageComponents.m_components = new List<rdtTcpMessageComponents.Component>();
      messageComponents.m_layer = (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? gameObject.layer : 0;
      messageComponents.m_tag = (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? gameObject.tag : "";
      messageComponents.m_enabled = (UnityEngine.Object) gameObject != (UnityEngine.Object) null && gameObject.activeInHierarchy;
      if ((bool) (UnityEngine.Object) gameObject)
      {
        this.m_components.Clear();
        gameObject.GetComponents<UnityEngine.Component>(this.m_unityComponents);
        if (this.m_unityComponents.Count > this.m_components.Capacity)
          this.m_components.Capacity = this.m_unityComponents.Count;
        for (int index = 0; index < this.m_unityComponents.Count; ++index)
        {
          UnityEngine.Component unityComponent = this.m_unityComponents[index];
          if ((UnityEngine.Object) unityComponent == (UnityEngine.Object) null)
          {
            rdtDebug.Debug((object) this, "Component is null, skipping");
          }
          else
          {
            List<rdtTcpMessageComponents.Property> propertyList = this.m_server.SerializerRegistry.ReadAllFields((object) unityComponent);
            if (propertyList == null)
            {
              rdtDebug.Debug((object) this, "Properties are null, skipping");
            }
            else
            {
              rdtTcpMessageComponents.Component component = new rdtTcpMessageComponents.Component();
              switch (unityComponent)
              {
                case Behaviour _:
                  component.m_canBeDisabled = true;
                  component.m_enabled = ((Behaviour) unityComponent).enabled;
                  break;
                case Renderer _:
                  component.m_canBeDisabled = true;
                  component.m_enabled = ((Renderer) unityComponent).enabled;
                  break;
                case Collider _:
                  component.m_canBeDisabled = true;
                  component.m_enabled = ((Collider) unityComponent).enabled;
                  break;
                default:
                  component.m_canBeDisabled = false;
                  component.m_enabled = true;
                  break;
              }
              System.Type type = unityComponent.GetType();
              component.m_name = type.Name;
              component.m_assemblyName = type.AssemblyQualifiedName;
              component.m_instanceId = unityComponent.GetInstanceID();
              component.m_properties = propertyList;
              this.m_components.Add(component);
            }
          }
        }
      }
      messageComponents.m_components = this.m_components;
      this.m_unityComponents.Clear();
      this.m_server.EnqueueMessage((rdtTcpMessage) messageComponents);
    }

    private void OnRequestGameObjects(rdtTcpMessage message)
    {
      rdtTcpMessageGameObjects messageGameObjects = new rdtTcpMessageGameObjects();
      this.m_gameObjects.Clear();
      int num = 0;
      for (int index = 0; index < SceneManager.sceneCount; ++index)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index);
        if (sceneAt.isLoaded && sceneAt.IsValid())
        {
          num += sceneAt.rootCount;
          if (num > this.m_gameObjects.Capacity)
            this.m_gameObjects.Capacity = sceneAt.rootCount;
          this.m_gameObjects.AddRange((IEnumerable<GameObject>) sceneAt.GetRootGameObjects());
        }
      }
      List<GameObject> gameObjects = this.m_gameObjects;
      List<GameObject> destroyOnLoadObjects = this.m_server.DontDestroyOnLoadObjects;
      if (!this.m_dontDestroyOnLoadBadObject)
      {
        for (int index = 0; index < destroyOnLoadObjects.Count; ++index)
        {
          if ((UnityEngine.Object) destroyOnLoadObjects[index] == (UnityEngine.Object) null)
          {
            rdtDebug.Log(rdtDebug.LogLevel.Warning, "A null GameObject was found in the DontDestroyOnLoadObjects list! Please ensure only DontDestroyOnLoad objects are added to the server.");
            this.m_dontDestroyOnLoadBadObject = true;
            break;
          }
        }
      }
      for (int index = 0; index < destroyOnLoadObjects.Count; ++index)
      {
        GameObject gameObject = destroyOnLoadObjects[index];
        if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && !gameObjects.Contains(gameObject))
          gameObjects.Add(gameObject);
      }
      int count = gameObjects.Count;
      if (count > this.m_allGobs.Capacity)
        this.m_allGobs.Capacity = count;
      this.m_allGobs.Clear();
      for (int index = 0; index < count; ++index)
      {
        GameObject g = gameObjects[index];
        if (!((UnityEngine.Object) g == (UnityEngine.Object) null) && g.hideFlags == HideFlags.None && g.transform.hideFlags == HideFlags.None)
          this.AddGameObject(g, this.m_allGobs);
      }
      messageGameObjects.m_allGobs = this.m_allGobs;
      this.m_server.EnqueueMessage((rdtTcpMessage) messageGameObjects);
      this.m_gameObjects.Clear();
    }

    private void AddGameObject(GameObject g, List<rdtTcpMessageGameObjects.Gob> list)
    {
      rdtTcpMessageGameObjects.Gob gob = new rdtTcpMessageGameObjects.Gob();
      gob.m_scene = g.scene.IsValid() ? g.scene.name : "<no scene>";
      gob.m_name = g.name;
      gob.m_instanceId = g.GetInstanceID();
      Transform parent = g.transform.parent;
      gob.m_hasParent = (UnityEngine.Object) parent != (UnityEngine.Object) null;
      if (gob.m_hasParent)
        gob.m_parentInstanceId = parent.gameObject.GetInstanceID();
      gob.m_enabled = g.activeInHierarchy;
      list.Add(gob);
      for (int index = 0; index < g.transform.childCount; ++index)
        this.AddGameObject(g.transform.GetChild(index).gameObject, list);
    }

    public GameObject FindGameObject(int instanceId)
    {
      for (int index1 = 0; index1 < SceneManager.sceneCount; ++index1)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index1);
        if (sceneAt.isLoaded && sceneAt.IsValid())
        {
          if (sceneAt.rootCount > this.m_gameObjects.Capacity)
            this.m_gameObjects.Capacity = sceneAt.rootCount;
          this.m_gameObjects.Clear();
          sceneAt.GetRootGameObjects(this.m_gameObjects);
          int count = this.m_gameObjects.Count;
          for (int index2 = 0; index2 < count; ++index2)
          {
            GameObject gameObject1 = this.m_gameObjects[index2];
            GameObject gameObject2 = this.FindGameObject(instanceId, gameObject1);
            if ((UnityEngine.Object) gameObject2 != (UnityEngine.Object) null)
              return gameObject2;
          }
        }
      }
      List<GameObject> destroyOnLoadObjects = this.m_server.DontDestroyOnLoadObjects;
      for (int index = 0; index < destroyOnLoadObjects.Count; ++index)
      {
        GameObject parent = destroyOnLoadObjects[index];
        if (!((UnityEngine.Object) parent == (UnityEngine.Object) null))
        {
          GameObject gameObject = this.FindGameObject(instanceId, parent);
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
            return gameObject;
        }
      }
      return (GameObject) null;
    }

    private GameObject FindGameObject(int instanceId, GameObject parent)
    {
      if (parent.GetInstanceID() == instanceId)
        return parent;
      for (int index = 0; index < parent.transform.childCount; ++index)
      {
        GameObject gameObject1 = parent.transform.GetChild(index).gameObject;
        GameObject gameObject2 = this.FindGameObject(instanceId, gameObject1);
        if ((UnityEngine.Object) gameObject2 != (UnityEngine.Object) null)
          return gameObject2;
      }
      return (GameObject) null;
    }

    private UnityEngine.Component FindComponent(GameObject gob, int instanceId)
    {
      gob.GetComponents<UnityEngine.Component>(this.m_unityComponents);
      for (int index = 0; index < this.m_unityComponents.Count; ++index)
      {
        UnityEngine.Component unityComponent = this.m_unityComponents[index];
        if (unityComponent.GetInstanceID() == instanceId)
          return unityComponent;
      }
      return (UnityEngine.Component) null;
    }
  }
}
