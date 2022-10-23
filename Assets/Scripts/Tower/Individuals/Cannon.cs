using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {
  [SerializeField]
  private RotationAiming rotationAiming;
  [SerializeField]
  private StraightLauncher launcher = new StraightLauncher();

  private FixedClock attackClock = new FixedClock();
  private CircleDetector attackDetector = new CircleDetector();

  public void OnDataChange(TowerData data) {
    attackClock.interval = data.atkInterval;
    attackDetector.radius = data.atkRadius;
  }

  private void FixedUpdate() {
    attackClock.OnFixedUpdate();

    var detectParam = new DetectParam {
      position = transform.position,
      layerMask = LayerConsts.Enemy,
      priorityRule = PriorityRule.Closet
    };

    var detetecResult = attackDetector.Detect(detectParam);

    if (detetecResult.singleResult != null) {
      rotationAiming.Aim(detetecResult.singleResult);
    }

    if (detetecResult.singleResult != null && attackClock.IsReady()) {
      attackClock.Reset();

      var launchParam = new LaunchParam {
        target = detetecResult.singleResult
      };

      launcher.Launch(launchParam);
    }
  }
}
