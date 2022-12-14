using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Buddy : TowerBase {
  [SerializeField]
  private RotationAiming rotationAiming;
  [SerializeField]
  private StraightLauncher launcher = new StraightLauncher();
  [SerializeField]
  private DDAnimator DDAnimator;
  [SerializeField]
  private float atkAnimationOffset;
  [SerializeField]
  private float idleAnimationLoopTime;

  private FixedClock attackClock = new FixedClock();
  private CircleDetector attackDetector = new CircleDetector();

  public override void OnDataChange(TowerData data) {
    base.OnDataChange(data);
    attackClock.freq = data.atkFreq;
    attackDetector.radius = data.atkRadius;
  }

  protected void FixedUpdate() {
    if (!cachedData.isInBattle) {
      return;
    }

    attackClock.Update(Time.fixedDeltaTime);

    var detectParam = new DetectParam {
      position = transform.position,
      layerMask = LayerConsts.EnemyMask,
      priorityRule = PriorityRule.Closet
    };

    var detetecResult = attackDetector.Detect(detectParam);

    if (detetecResult.singleResult != null) {
      rotationAiming.Aim(detetecResult.singleResult);
      DDAnimator.SetState("Slash", atkAnimationOffset, 0.25f);
      DDAnimator.SetUpdateMode(DDAnimator.UpdateMode.Manual);
      DDAnimator.Play(attackClock.normalizedTime);
    } else {
      DDAnimator.SetState("Idle", 0, 0.25f);
      DDAnimator.SetUpdateMode(DDAnimator.UpdateMode.Loop, idleAnimationLoopTime);
    }

    if (detetecResult.singleResult != null && attackClock.isReady) {
      attackClock.OnTrigger();
      var damageEvent = new TowerDamageEvent() {
        type = TowerDamageType.Attack,
        towerPtr = cachedDataPtr,
        enemyPtr = detetecResult.singleResult.FindDataPtr()
      };

      var launchParam = new LaunchParam {
        target = detetecResult.singleResult,
        projectilePlugin = new ProjectilePlugin.TowerLockDamage(damageEvent, detetecResult.singleResult.transform)
      };

      launcher.Launch(launchParam);
    }
  }

}
