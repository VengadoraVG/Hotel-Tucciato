using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Building {
    // GUILayout.Button("label")
    // Handles.Button(pos, rot, size1, size2, Handles.RectangleHandleCap)
    // Handles.PositionHandle(pos, rot)
    // EditorUtility.SetDirty(gameObject);
    [CustomEditor(typeof(FloorTile))]
    public class FloorTileEditor : Editor {
        FloorTile _parsedTarget;
        FloorTile Target {
            get {
                if (_parsedTarget == null) _parsedTarget = (FloorTile) target;
                return _parsedTarget;
            }
        }

        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            if (GUILayout.Button("Generate")) Target.Generate();
            if (GUILayout.Button("Reset")) {
                Target.ClearTilesInfo();
                Target.Generate();
            }

            if (GUI.changed) {
                EditorUtility.SetDirty(Target);
                EditorSceneManager.MarkSceneDirty(Target.gameObject.scene);
            }
        }

        bool DrawWallButton (BuildingTile tile, CardinalPoint orientation) {
            float size = FloorTile.tileSize * 0.1f;
            Vector3 pos = tile.transform.localPosition +
                (tile.transform.forward - tile.transform.right) *
                (FloorTile.tileSize/2f);
            pos += tile.transform.TransformDirection(Util.UnitVector(orientation)) *
                (FloorTile.tileSize/2f - size * 2);

            return Handles.Button(pos, tile.transform.rotation * Quaternion.Euler(90, 0, 0),
                                  size, size, Handles.RectangleHandleCap);
        }

        void EndAndStart () {
            Handles.matrix = Target.transform.localToWorldMatrix;

            Vector3 forward = Target.start - Target.end;
            Quaternion rot = forward == Vector3.zero? Quaternion.identity:
                Quaternion.LookRotation(forward);
            Vector3 start = Handles.PositionHandle(Target.start, rot);
            Vector3 end = Handles.
                PositionHandle(Target.end,  rot * Quaternion.Euler(0, 180, 0));
            bool shouldGenerate = start != Target.start || end != Target.end;

            if (start == end && forward != Vector3.zero) {
                start = Target.start; end = Target.end;
            } else if (forward == Vector3.zero) {
                Target.end = Target.start + new Vector3(1, 0, 0);
            }

            Target.start = start; Target.end = end;


            if (shouldGenerate) {
                Target.Generate();
            }
        }

        bool BuildingTileEdition () {
            bool clicked = false;

            if (Target.tileInstances == null) return false;

            foreach (BuildingTile tile in Target.tileInstances) {
                foreach (CardinalPoint orientation in
                         System.Enum.GetValues(typeof(CardinalPoint))) {
                    if (DrawWallButton(tile, orientation)) {
                        tile.ShuffleWall(orientation);
                        clicked = true;
                    }
                }
            }

            return clicked;
        }

        void OnSceneGUI () {
            Tools.current = Tool.None;

            EndAndStart();
            bool edited = BuildingTileEdition();
            if (edited) {
                Target.Generate();
            }

            if (GUI.changed || edited) {
                EditorUtility.SetDirty(Target);
                EditorSceneManager.MarkSceneDirty(Target.gameObject.scene);
            }
        }
    }
}