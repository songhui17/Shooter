using UnityEngine;
using System.Collections.Generic;

public class CellMap : MonoBehaviour {
    public bool ShowGrid = false;
    public Color CellColor = Color.yellow;

    [SerializeField]
    private List<Entity> _entityList;
    public List<Entity> EntityList {
        get { return _entityList ?? (_entityList = new List<Entity>());}
    }

    private static int _stride = 1;
    [SerializeField]
    private int _row = 100;
    private int _column = 100;
    private int[] _data;

    void Awake(){
        _data = new int[_row * _column];
        foreach (var entity_ in EntityList){
            AddEntity(entity_);
        }
    }

    private int GetIndex(int rowIndex_, int columnIndex_){
        return rowIndex_ * _column + columnIndex_;
    }

    public void AddEntity(Entity entity_){
        EntityList.Add(entity_);

//        for (int rowIndex = 0; rowIndex < entity_.Row; rowIndex++){
//            for (int columnIndex = 0; columnIndex < entity_.Column; columnIndex++){
//                _data[GetIndex(entity_.Y + rowIndex, entity_.X + columnIndex)] =
//                    entity_.Data[GetIndex(rowIndex, columnIndex)]; 
//            }
//        }
    }

    public static Vector3 GetCellPosition(int rowIndex_, int columnIndex_){
        var x = (columnIndex_ + 0.5f) * _stride;
        var y = (rowIndex_ + 0.5f) * _stride;
        return new Vector3(x, 0, y);
    }

    void OnDrawGizmos(){
        if (!Application.isPlaying) return;
        if (!ShowGrid) return;

//        Gizmos.color = CellColor;
//        var size = new Vector3(_stride, _stride, _stride);
//        for (int rowIndex = 0; rowIndex < _row; rowIndex++){
//            for (int columnIndex = 0; columnIndex < _column; columnIndex++){
//                if (_data[GetIndex(rowIndex, columnIndex)] == 0){
//                    Gizmos.DrawWireCube(GetCellPosition(rowIndex, columnIndex), size);
//                }
//            }
//        } 
    }
}
