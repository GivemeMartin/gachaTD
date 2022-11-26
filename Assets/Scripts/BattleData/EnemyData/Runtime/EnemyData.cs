using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataHub;
using System;

[System.Serializable]
public struct EnemyData : IData<EnemyData> {
  private int version;

  public EnemyName enemyName;
  public int hp;
  [NonSerialized]
  public bool isDead;

  public bool HasDiff(EnemyData data) {
    return version != data.version;
  }

  public void UpdateVersion() {
    version++;
  }
}


/* TemplateId: ����ģ�壬ͬһ�����ֵĵ��˿����ж���ģ�壨���羫Ӣ/�Ǿ�Ӣ�����߱�Ķ�����
 * Name�����˵����֣�������Ӧ������Դ����Ϣ
 */

public enum EnemyTemplateId {
  PapaWorm
}

public enum EnemyName {
  PapaWorm
}
