using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class EnemyDeathSystem : BattleEngine.System {
  public override void Handle(BattleEngine.Event eve) {
    if (eve is EnemyDeathEvent ede) {
      if (!enemyDataHub.TryGetData(ede.enemyPtr, out var data)) {
        Debug.LogError("cant get enemyData");
      }

      Debug.Log($"Kill Enemy");
    }
  }
}