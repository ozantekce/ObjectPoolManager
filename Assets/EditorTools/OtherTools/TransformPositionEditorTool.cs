#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TransformPositionEditorTool : EditorWindow
{
    private Transform parentTransform;
    private Vector3 newParentPosition;
    private Quaternion newParentRotation;

    [MenuItem("Tools/Arrange Parent Without Affecting Children")]
    public static void ShowWindow()
    {
        GetWindow<TransformPositionEditorTool>("Parent Arrangement Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Arrange Parent Without Affecting Children", EditorStyles.boldLabel);

        if(Selection.activeGameObject != null)
        {
            parentTransform = Selection.activeGameObject.transform;
        }
        else
        {
            parentTransform = null;
        }


        if (parentTransform == null)
        {
            EditorGUILayout.HelpBox("Please assign a parent transform.", MessageType.Warning);
            return;
        }

        // Load the parent transform's position and rotation values into the fields
        newParentPosition = EditorGUILayout.Vector3Field("Parent Position", newParentPosition);
        newParentRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Parent Rotation", newParentRotation.eulerAngles));

        // Apply the new values when user modifies them
        if (GUILayout.Button("Apply Changes"))
        {
            ArrangeParentWithoutAffectingChildren(parentTransform, newParentPosition, newParentRotation);
            SceneView.RepaintAll(); // Refresh the Scene view
        }
    }

    private void ArrangeParentWithoutAffectingChildren(Transform parent, Vector3 newPosition, Quaternion newRotation)
    {
        if (parent == null) return;

        // Store the world positions and rotations of all children
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>(true);
        Vector3[] childWorldPositions = new Vector3[childTransforms.Length];
        Quaternion[] childWorldRotations = new Quaternion[childTransforms.Length];

        for (int i = 0; i < childTransforms.Length; i++)
        {
            childWorldPositions[i] = childTransforms[i].position;
            childWorldRotations[i] = childTransforms[i].rotation;
        }

        // Apply the new parent position and rotation
        parent.SetPositionAndRotation(newPosition, newRotation);
        parent.hasChanged = true;

        // Reapply the saved world positions and rotations back to the children
        for (int i = 0; i < childTransforms.Length; i++)
        {
            if(childTransforms[i] == parent)
            {
                continue;
            }
            Debug.Log(childTransforms[i]);
            childTransforms[i].position = childWorldPositions[i]; // Keep the same world position
            childTransforms[i].rotation = childWorldRotations[i]; // Keep the same world rotation
            EditorUtility.SetDirty(childTransforms[i]);
        }

        EditorUtility.SetDirty(parent);
    }



}
#endif