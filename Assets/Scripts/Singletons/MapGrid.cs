using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapGrid : MonoSingleton<MapGrid> {
  #region Inner Classes
  private class Cell {
    public DraggablePositionHandler handlerHead;
  }

  private class CollectorPlugin : LifeCollector<DraggableObject>.Plugin {
    private MapGrid mapGrid;
    public CollectorPlugin(MapGrid mapGrid) : base(mapGrid) {
      this.mapGrid = mapGrid;
    }
    public override void OnAddObject(DraggableObject obj) {
      mapGrid.OnAddDraggable(obj);
    }
  }

  private class DraggablePositionHandler : DraggableObject.PositionHandler {
    private MapGrid mapGrid;
    public Vector2Int cell;
    private DraggablePositionHandler next;
    private DraggablePositionHandler prior;
    public DraggablePositionHandler Next => next;
    public DraggablePositionHandler Prior => prior;

    private DraggablePositionHandler() { }
    public DraggablePositionHandler(MapGrid mapGrid) {
      this.mapGrid = mapGrid;
    }

    public void Init(DraggableObject draggable) {
      cell = mapGrid.WorldToXZCell(draggable.transform.position);
    }

    public void SetNext(DraggablePositionHandler handler) {
      Debug.Assert(handler != null);
      if (handler == this) {
        Debug.LogError("Ilegal");
        return;
      }
      if (handler != null) {
        handler.transform.parent = transform;
        next = handler;
        handler.prior = this;
      }
    }

    public void DisconnectWithPrior() {
      if (prior != null) {
        prior.next = null;
        prior = null;
        transform.parent = null;
      }
    }

    public void DisconnectWithNext() {
      if (next != null) {
        next.prior = null;
        next.transform.parent = null;
        next = null;
      }
    }

    private void _RemoveFromCell(Cell cellData) {
      if (cellData.handlerHead == this) {
        cellData.handlerHead = null;
        return;
      }

      var next = cellData.handlerHead;
      while (next != null && next != this) {
        next = next.next;
      }
      if (next == this) {
        next.DisconnectWithPrior();
      } else {
        //?????????Cell??????????????????positionHandler?????????????????????
        //?????????????????????draggable????????????????????????????????????cellData
      }
    }

    private void _AddToCell(Cell cellData) {
      if (cellData.handlerHead == null) {
        cellData.handlerHead = this;
        return;
      }

      var next = cellData.handlerHead;
      while (next != null && next.next != null) {
        if (next == this) {
          Debug.LogError(111);
          break;
        }
        next = next.next;
      }

      next.SetNext(this);
    }

    private void _UpdateCell(Vector2Int newCell) {
      cell = newCell;
      var next = this.next;
      while (next != null) {
        next.cell = newCell;
        next = next.next;
      }
    }

    public void MoveDatToCell(Vector2Int newCell) {
      var curCellData = mapGrid.GetCellDataNotNull(cell);
      var newCellData = mapGrid.GetCellDataNotNull(newCell);
      _RemoveFromCell(curCellData);
      _AddToCell(newCellData);
      _UpdateCell(newCell);
    }

    public override void OnMouseDrag(Vector2 mousePosition) {
      if (mapGrid.MouseRaycastCell(mousePosition, out var newCell) && newCell != cell) {
        MoveDatToCell(newCell);
      }
    }

    public override void Recalculate() {
      // Debug.Log("Before:"+cell);
      var worldPlacePosition = transform.position;
      var newPos = mapGrid.WorldToXZCell(worldPlacePosition);
      MoveDatToCell(newPos);
      // Debug.Log("After:"+cell);
    }

    public override Vector3 GetPlacePosition() {
      if (prior == null) {
        var result = mapGrid.XZCellToWorld(cell);
        return result;
      } else {
        return Vector3.zero.SetY(prior.stackHeight);
      }
    }

    private Vector3 GetWorldPlacePosition() {
      var cellData = mapGrid.GetCellDataNotNull(cell);
      var height = 0f;
      DraggablePositionHandler next = cellData.handlerHead;

      while (next != null && next != this) {
        height += next.stackHeight; //todo: dynamic height
        next = next.next;
      }

      var result = mapGrid.XZCellToWorld(cell).SetY(height);


      return result;
    }

    public override void OnMouseStartDrag() {
    }

    public override void OnMouseStopDrag() {
    }
  }

  #endregion
  private static readonly string[] rayCastLayers = { "MapGrid", "DraggableObject" };

  [SerializeField]
  private Grid grid;

  private IGrid<Cell> gridData = new DictionaryGridImpl<Cell>();

  protected override void Awake() {
    base.Awake();
    LifeCollector<DraggableObject>.SetPlugin(new CollectorPlugin(this));
  }

  private bool MouseRaycastCell(Vector2 mousePosition, out Vector2Int cell) {
    cell = Vector2Int.zero;

    var layerMask = LayerMask.GetMask(rayCastLayers);
    var ray = Camera.main.ScreenPointToRay(mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hitInfo, 9999f, layerMask)) {
      var hitPosition = hitInfo.point;

      cell = WorldToXZCell(hitPosition);

      return true;
    }
    return false;
  }



  protected Vector3 XZCellToWorld(Vector2Int xzCell) {
    var offset = grid.CellToWorld(new Vector3Int(1, 0, 1)) * 0.5f; // offset to grid center, not considering Y
    var result = grid.CellToWorld(Vector3Int.zero.SetXZ(xzCell)) + offset;
    return result;
  }

  public Vector2Int WorldToXZCell(Vector3 world) {
    var result = grid.WorldToCell(world);
    return result.XZ();
  }

  protected void OnAddDraggable(DraggableObject draggable) {
    var positionHandler = new DraggablePositionHandler(this);
    positionHandler.Init(draggable);
    draggable.SetPositionHandler(positionHandler);

    positionHandler.MoveDatToCell(positionHandler.cell);

    draggable.transform.localPosition = positionHandler.GetPlacePosition();
  }

  private Cell GetCellDataNotNull(Vector2Int position) {
    if (!gridData.Contains(position)) {
      var cell = new Cell();
      gridData[position] = cell;
    }
    return gridData[position];
  }

  public bool GridNotNull(Vector2Int position) {
    return GetCellDataNotNull(position).handlerHead != null;
  }

}
