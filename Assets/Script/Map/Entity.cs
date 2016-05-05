using UnityEngine;
using System;

[ExecuteInEditMode]
public class Entity : MonoBehaviour {
    public bool ShowGrid = false;
    public Color CellColor = Color.yellow;
    public bool LocalSpace = true;
    public float Radius = 0.1f;

//    public int X;
//    public int Y;
//
//    public int Row;
//    public int Column;
//    public int [] Data;
//
//    private int GetIndex(int rowIndex_, int columnIndex_){
//        return rowIndex_ * Column + columnIndex_;
//    }
//
//    void OnDrawGizmos(){
//        if (!ShowGrid) return;
//
//        Gizmos.color = CellColor;
//        var _stride = 1.0f;
//        var size = new Vector3(_stride, _stride, _stride);
//        for (int rowIndex = 0; rowIndex < Row; rowIndex++){
//            for (int columnIndex = 0; columnIndex < Column; columnIndex++){
//                if (Data[GetIndex(rowIndex, columnIndex)] == 1){
//                    Gizmos.DrawWireCube(CellMap.GetCellPosition(Y + rowIndex, X + columnIndex), size);
//                }
//            }
//        } 
//    }
//

    private int _minX;
    private int _maxX;
    private int _minZ;
    private int _maxZ;

    void OnEnable(){
        AABB(out _minX, out _maxX, out _minZ, out _maxZ);
    }

    void OnDrawGizmos(){
        if (!ShowGrid) return;

        var center = new Vector3((_minX + _maxX) / 2, 0, (_minZ + _maxZ) / 2);
        var leftBottom = new Vector3(_minX, 0, _minZ);
        var rightBottom = new Vector3(_maxX, 0, _minZ);
        var rightTop = new Vector3(_maxX, 0, _maxZ);
        var leftTop = new Vector3(_minX, 0, _maxZ);

        leftBottom = transform.TransformPoint(leftBottom);
        rightBottom = transform.TransformPoint(rightBottom);
        rightTop = transform.TransformPoint(rightTop);
        leftTop = transform.TransformPoint(leftTop);

        Gizmos.color = CellColor;
        Gizmos.DrawLine(leftBottom, rightBottom);
        Gizmos.DrawLine(rightBottom, rightTop);
        Gizmos.DrawLine(rightTop, leftTop);
        Gizmos.DrawLine(leftTop, leftBottom);
    }

    void AABB(out int minX_, out int maxX_,
              out int minZ_, out int maxZ_){
        var meshFilter = GetComponentInChildren<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        var vertices = mesh.vertices;
        var minX = float.MaxValue;
        var maxX = float.MinValue;
        var minZ = float.MaxValue;
        var maxZ = float.MinValue;

        var meshTransform = meshFilter.transform;
        var localMatrix = Matrix4x4.TRS(meshTransform.localPosition,
                meshTransform.localRotation, meshTransform.localScale);
        foreach (var vertex in vertices){
            var v = vertex;

            if(LocalSpace){
                v = localMatrix.MultiplyPoint(vertex);
            }
            minX = Mathf.Min(v.x, minX);
            maxX = Mathf.Max(v.x, maxX);
            minZ = Mathf.Min(v.z, minZ);
            maxZ = Mathf.Max(v.z, maxZ);
        }

        var info = "AABB:\n";
        info += "minX: " + minX + "\n";
        info += "maxX: " + maxX + "\n";
        info += "minZ: " + minZ + "\n";
        info += "maxZ: " + maxZ + "\n";
        info += "\n";

        minX += Radius;
        maxX -= Radius;
        minZ += Radius;
        maxZ -= Radius;

        info += "minX: " + minX + "\n";
        info += "maxX: " + maxX + "\n";
        info += "minZ: " + minZ + "\n";
        info += "maxZ: " + maxZ + "\n";
        // Debug.Log(info);

        minX = Mathf.Floor(minX);
        maxX = Mathf.Ceil(maxX);
        minZ = Mathf.Floor(minZ);
        maxZ = Mathf.Ceil(maxZ);

        minX_ = (int)minX;
        maxX_ = (int)maxX;
        minZ_ = (int)minZ;
        maxZ_ = (int)maxZ;
    }
}
