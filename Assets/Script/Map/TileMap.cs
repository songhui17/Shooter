using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class TileMapSetting {
    public int Stride = 1;
    public float Height = 1;
    public int ColumnCount = 100;
    public int RowCount = 100;

    public int[] Tiles;
}

[Serializable]
public class Waypoint {
    public int[] Index = new int[2];
}

[Serializable]
public class Path {
    public Waypoint[] Waypoints;
}

public class TileMap : MonoBehaviour {
    public int Stride = 1;
    public float Height = 1;
    public float MinY = 0.5f;
    public int ColumnCount = 100;
    public int RowCount = 100;
    
    public static TileMap Instance { get; private set; }
    void Awake() {
        // if (Instance != null) {
        //     throw new NotImplementedException("Only a single TileMap is allowed now");
        // }
        Instance = this;
    }

    public int[] Tiles;

    private MeshFilter _filter;
    public MeshFilter Filter {
        get {
            if (_filter == null) {
                _filter = GetComponent<MeshFilter>();
                if (_filter == null) {
                    _filter = gameObject.AddComponent<MeshFilter>();
                }
            }
            return _filter;
        }
    }

    public Mesh mesh;
    [ContextMenu("Create Mesh")]
    void CreateMesh(){
        if (mesh == null) {
            mesh = new Mesh();
        }
        var vertCount = (ColumnCount + 1) * (RowCount + 1);
        var verts = new Vector3[vertCount];
        var triangles = new int[3 * 2 * ColumnCount * RowCount];

        int triangleIndex = 0;
        for (int row = 0; row <= RowCount; row++) {
            for (int column = 0; column <= ColumnCount; column++) {
                var vertIndex = row * (ColumnCount + 1) + column;
                verts[vertIndex] = new Vector3(
                        column * Stride, Height, row * Stride);

                /*
                 *      1 ---- 2        
                 *      |      |         
                 *      0 ---- 3       
                 * */
                if (column < ColumnCount && row < RowCount) {
                    var index0 = vertIndex;
                    var index1 = vertIndex + ColumnCount + 1;
                    var index2 = vertIndex + ColumnCount + 2;
                    var index3 = vertIndex + 1;

                    triangles[triangleIndex++] = index0;
                    triangles[triangleIndex++] = index1;
                    triangles[triangleIndex++] = index3;

                    triangles[triangleIndex++] = index1;
                    triangles[triangleIndex++] = index2;
                    triangles[triangleIndex++] = index3;
                }
            }
        }

        mesh.vertices = verts;
        mesh.triangles = triangles;
        Filter.sharedMesh = mesh;
    }

    [ContextMenu("Cast to Ground")]
    void CastToGround() {
        if (mesh == null) {
            Debug.LogWarning("Create mesh first");
            return;
        }

        var verts = mesh.vertices;
        var newVerts = new Vector3[verts.Length];
        var newTriangles = new List<int>();
        Tiles = new int[RowCount * ColumnCount];

        Array.Copy(verts, 0, newVerts, 0, verts.Length);

        for (int row = 0; row < RowCount; row++) {
            for (int column = 0; column < ColumnCount; column++) {
                var vertIndex = row * (ColumnCount + 1) + column;
                /*
                 *      1 ---- 2        
                 *      |      |         
                 *      0 ---- 3       
                 * */
                var index0 = vertIndex;
                var index1 = vertIndex + ColumnCount + 1;
                var index2 = vertIndex + ColumnCount + 2;
                var index3 = vertIndex + 1;
                var vert = Vector3.zero;
                vert += verts[index0];
                vert += verts[index1];
                vert += verts[index2];
                vert += verts[index3];
                vert /= 4;
                
                RaycastHit hitInfo;
                var tileIndex = row * (ColumnCount) + column;
                if(Physics.Raycast(vert, Vector3.down, out hitInfo)) {
                    var y = hitInfo.point.y;
                    if (y <= MinY) {
                        newVerts[index0].y = y;
                        newVerts[index1].y = y;
                        newVerts[index2].y = y;
                        newVerts[index3].y = y;

                        newTriangles.Add(index0);
                        newTriangles.Add(index1);
                        newTriangles.Add(index3);

                        newTriangles.Add(index1);
                        newTriangles.Add(index2);
                        newTriangles.Add(index3);

                        Tiles[tileIndex] = 0;
                    }else{
                        Tiles[tileIndex] = 1;  //int.MaxValue;
                    }
                }else{
                    // verts[vertIndex].y = -1;
                    Tiles[tileIndex] = -1;  // int.MinValue;
                }
            }
        }

        mesh.vertices = newVerts;
        mesh.triangles = newTriangles.ToArray();
    }

    [ContextMenu("Clear Mesh")]
    void ClearMesh() {
        if (mesh != null) {
            DestroyImmediate(mesh);
            mesh = null;
        }
    }

    private void PositionToTile(Vector3 position_, out int row_, out int column_) {
        //TODO
        var row = (int)Mathf.Floor(position_.z);
        row = Mathf.Max(0, Mathf.Min(RowCount - 1, row));
        row_ = row;

        //TODO
        var column = (int)Mathf.Floor(position_.x);
        column = Mathf.Max(0, Mathf.Min(ColumnCount - 1, column));
        column_ = column;
    }

    public void Move(Transform transform_, Vector3 movement_, bool isJumping_) {
        var startPosition = transform_.position;

        var moveX = movement_;
        moveX.y = 0;
        moveX.z = 0;

        // float mag = movement_.magnitude;
        float magX = moveX.magnitude;
        bool collideX = false;
        for (float distance = 0; distance <= magX; ) {
            if (distance != 0) {
                int row, column;
                // var position = startPosition + movement_.normalized * distance;
                var position = startPosition + moveX.normalized * distance;
                PositionToTile(position, out row, out column);
                var tileIndex = row * (ColumnCount) + column;
                if (Tiles[tileIndex] != 0) {
                    collideX = true;
                    break;
                }
            }

            if (distance == magX) {
                break;
            }else{
                distance += Stride;
                if (distance > magX) {
                    distance = magX;
                }
            }
        }
        if (!collideX) {
            transform_.position += moveX;
        }

        var moveZ = movement_;
        moveZ.x = 0;
        moveZ.y = 0;
        float magZ = moveZ.magnitude;
        bool collideZ = false;
        for (float distance = 0; distance <= magZ;) {
            if (distance != 0) {
                int row, column;
                var position = startPosition + moveZ.normalized * distance;
                PositionToTile(position, out row, out column);
                var tileIndex = row * (ColumnCount) + column;
                if (Tiles[tileIndex] != 0) {
                    collideZ = true;
                    break;
                }
            }

            if (distance == magZ) {
                break;
            }else{
                distance += Stride;
                if (distance > magZ) {
                    distance = magZ;
                }
            }
        }
        if (!collideZ) {
            transform_.position += moveZ;
        }

        if (isJumping_) {
            var moveY = movement_;
            moveY.x = 0;
            moveY.z = 0;
            var newPosition = transform_.position + moveY;
            if (newPosition.y < 0) {
                newPosition.y = 0;
            }
            transform_.position = newPosition;
        }

        // transform_.position += movement_;
    }

    [ContextMenu("Export to JSON")]
    void Export() {
        var map = new TileMapSetting();

        map.Stride = Stride;
        map.Height = Height;
        map.ColumnCount = ColumnCount;
        map.RowCount = RowCount;
        map.Tiles = Tiles;

        var json = JsonUtility.ToJson(map);
        TextWriter writer = null;
        try {
            writer = new StreamWriter("map");
            writer.Write(json);
        }catch(Exception ex){
            Debug.LogError(ex);
        }
        writer.Close();
    }

    // TODO:???
    // public void ClearRegion(Rect rect) {
    // }

    [SerializeField]
    private Path _path;
    [ContextMenu("Load Path")]
    void LoadPath() {
        string fname="find_path_result";
        TextReader reader = null;
        try{
            reader = new StreamReader(fname);
            var text = reader.ReadToEnd();
            Debug.Log(text);
            _path = JsonUtility.FromJson<Path>(text) as Path;
        }catch(Exception ex){
            Debug.Log("LoadPath failed, ex:" + ex);
        }
        if (reader != null) {
            reader.Close();
        }
    }

    void OnDrawGizmosSelected() {
        if (_path != null) {
            foreach (var tile in _path.Waypoints) {
                var row = tile.Index[0];
                var column = tile.Index[1];
                var position = new Vector3() {
                    x = (column + 0.5f) * Stride,
                    y = 0.0f,
                    z = (row + 0.5f) * Stride,
                };

                Gizmos.DrawWireCube(position, new Vector3(1,1,1));
            }

            var savedColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            for(int i = 0; i < _path.Waypoints.Length; i++) {
                var tile = _path.Waypoints[i];
                var row = tile.Index[0];
                var column = tile.Index[1];
                var position = new Vector3() {
                    x = (column + 0.5f) * Stride,
                    y = 0.0f,
                    z = (row + 0.5f) * Stride,
                };

                if (i < _path.Waypoints.Length - 1) {
                    var nextTile = _path.Waypoints[i + 1];
                    var nextPosition = new Vector3() {
                        x = (nextTile.Index[1] + 0.5f) * Stride,
                        y = 0.0f,
                        z = (nextTile.Index[0] + 0.5f) * Stride,
                    };
                    Gizmos.DrawLine(position, nextPosition);
                }
            }
            Gizmos.color = savedColor;
        }
    }
}
