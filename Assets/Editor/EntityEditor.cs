using UnityEditor;
using Assets.Scripts.Entities;
using UnityEngine;

[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Entity entity = (Entity)target;

        GUI.enabled = Application.isPlaying && entity.Health > 0f;

        if (GUILayout.Button("Kill Entity"))
        {
            entity.Kill();
        }
    }
}
