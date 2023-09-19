using UnityEditor;
using UnityEngine;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D.Editor
{
    [CustomEditor(typeof(LutGenerator))]
    public class LutGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (GUILayout.Button("Bake"))
                ((LutGenerator)target).Bake();
        }

        public override bool HasPreviewGUI()
        {
            return ((LutGenerator)target)._lutGenerated != null;
        }

        public override void DrawPreview(Rect previewArea)
        {
            EditorGUI.DrawPreviewTexture(previewArea, ((LutGenerator)target)._lutGenerated, null, ScaleMode.ScaleToFit);
        }
    }
}