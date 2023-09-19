using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D.Editor
{
    public class LutAutoImport : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (didDomainReload)
                return;
            
            var textures = importedAssets.Select(n => AssetDatabase.LoadAssetAtPath<Texture2D>(n))
                                      .Where(n => AssetDatabase.GetLabels(n).Any(n => n == LutGenerator.k_GradsLabel));
            
            // use strange enumeration to prevent find asset operation for each imported texture
            var e = textures.GetEnumerator();
            while (e.MoveNext())
            {
                var gens = new List<LutGenerator>(AssetDatabase.FindAssets("t:" + nameof(LutGenerator)).Select(guid => AssetDatabase.LoadAssetAtPath<LutGenerator>(AssetDatabase.GUIDToAssetPath(guid))));
                do
                {
                    var gen = gens.FirstOrDefault(n => n._gradsPath == AssetDatabase.GetAssetPath(e.Current));
                    if (gen == null)
                        continue;
                    
                    _bake(gen);
                    
                } while(e.MoveNext());
            }
            e.Dispose();
        }

        private static async void _bake(LutGenerator gen)
        {
            await Task.Yield();
            gen.Bake();
        }
    }
}