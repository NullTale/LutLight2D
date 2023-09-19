using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D.Editor
{
    internal static class MenuItems
    {
        private static Texture2D s_ObjectIcon = (EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D);
        
        // =======================================================================
        private class DoCreateFile : EndNameEditAction
        {
            // =======================================================================
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                _create(pathName);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                //_create(pathName);
            }

            // =======================================================================
            private void _create(string pathName)
            {
                var soName = Path.GetFileName(pathName);
                var so     = CreateInstance<LutGenerator>();
                
                // create placeholder texture
                var gradsPath = Path.ChangeExtension(pathName + "_Ramps", ".png");
                
                var gradsClean = new Texture2D(7, 7);
                for (var x = 0; x < gradsClean.width; x++)
                for (var y = 0; y < gradsClean.height; y++)
                    gradsClean.SetPixel(x, y, Color.HSVToRGB(x / (float)gradsClean.width, 0f, (y + 3 - x) / (float)(gradsClean.height + 2)));

                gradsClean.Apply();
                
                File.WriteAllBytes(gradsPath, gradsClean.EncodeToPNG());
                AssetDatabase.ImportAsset(gradsPath, ImportAssetOptions.ForceUpdate);
                
                gradsClean = AssetDatabase.LoadAssetAtPath<Texture2D>(gradsPath);
                LutGenerator._setImportOptions(gradsClean, true);
                
                so._ramps = gradsClean;
                
                // create material
                var mat = new Material(Shader.Find(LutGenerator.k_ShaderName));
                AssetDatabase.CreateAsset(mat, Path.ChangeExtension(pathName, ".mat"));
                
                so._material.Value = mat;
                
                // create asset
                AssetDatabase.CreateAsset(so, Path.ChangeExtension(pathName, ".asset"));
                ProjectWindowUtil.ShowCreatedAsset(so);

                so.Bake();
            }
        }
        
        private class DoCreateFileGrads : EndNameEditAction
        {
            // =======================================================================
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                _create(pathName);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                //_create(pathName);
            }

            // =======================================================================
            private void _create(string pathName)
            {
                var soName = Path.GetFileName(pathName);
                
                // create placeholder texture
                var gradsPath = Path.ChangeExtension(pathName + "_Grads", ".png");
                
                var steps = 16;
                var grads = 7;
                var gradsClean = new Texture2D(steps * grads, grads, TextureFormat.RGBA32, false);
                
                for (var h = 0; h < steps; h++)
                for (var s = 1; s <= grads; s++)
                for (var v = 0; v < grads; v++)
                {
                    gradsClean.SetPixel(h * grads + s - 1, v, Color.HSVToRGB(h / (float)steps, s / (float)grads, (s / (float)grads)* (v / (float)(grads - 1))) );
                }
                gradsClean.Apply();

                File.WriteAllBytes(gradsPath, gradsClean.EncodeToPNG());
                AssetDatabase.ImportAsset(gradsPath, ImportAssetOptions.ForceUpdate);
                
                gradsClean = AssetDatabase.LoadAssetAtPath<Texture2D>(gradsPath);
            }
        }
        
        [MenuItem("Assets/Create/2D/LutLight2D Asset", false, 10)]
        public static void CreateGrads(MenuCommand menuCommand)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateFile>(),
                "LutLight2D",
                s_ObjectIcon,
                string.Empty);
        }
        
        //[MenuItem("Assets/Create/2D/Clean Lut Gradient", false, 10)]
        public static void CreateGradsTable(MenuCommand menuCommand)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateFileGrads>(),
                "Grads",
                s_ObjectIcon,
                string.Empty);
        }
        
    }
}