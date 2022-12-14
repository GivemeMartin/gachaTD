using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(TriggerCallback))]
public class ProjectilePoolObject : PoolObject {
  #region Inner Classes
  public abstract class Plugin {
    public TriggerCallback triggerCallback;
    public ProjectilePoolObject poolObject;

    public abstract void OnSetPlugin();

    public virtual void OnUpdate() { }
  }
  #endregion

  private Plugin plugin;


  protected override void Awake() {
    base.Awake();
  }

  private void Update() {
    if (plugin != null) {
      plugin.OnUpdate();
    }
  }

  public void SetPlugin(Plugin plugin) {
    this.plugin = plugin;
    plugin.poolObject = this;
    plugin.triggerCallback = GetComponent<TriggerCallback>();
    plugin.OnSetPlugin();
  }

}
