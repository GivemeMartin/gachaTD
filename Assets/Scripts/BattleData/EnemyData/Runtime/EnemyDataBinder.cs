using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


public class EnemyDataBinder : EnemyDataHub.Binder, IPoolCallback {
  [SerializeField]
  private EnemyTemplateId dataTemplate;
  [SerializeField]
  private UnityEvent<EnemyData> onDataChange;

  private bool isRegistered; //�Ƿ�ע�������ݣ�һ�����������������ֻע��һ��
  private bool isDataLoaded; //�Ƿ��ȡ�����ݣ�������ظ���ʱ��Ҫ��ȡ�������

  protected override void OnDataUpdate(EnemyData data) {
    onDataChange.Invoke(Data);
  }

  protected override void Update() {
    base.Update();
    InitDataIfNot();
  }

  private void InitDataIfNot() {
    if (!isDataLoaded) {
      if (EnemyDataLoader.Instance == null || EnemyDataHub.Instance == null) {
        Debug.Log("[InitDataIfNot] null singleton");
        return;
      }

      var data = EnemyDataLoader.Instance.LoadData(dataTemplate);
      if (!isRegistered) {
        EnemyDataHub.Instance.RegisterData(DataPtr, data);
        isRegistered = true;
      } else {
        EnemyDataHub.Instance.SetData(DataPtr, data);
      }

      isDataLoaded = true;
    }

  }

  public void OnRelease() {
    isDataLoaded = false;
  }

}
