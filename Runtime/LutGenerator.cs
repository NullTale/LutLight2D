using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D
{
    public class LutGenerator : ScriptableObject
    {
        public const string k_ShaderName = "Shader Graphs/LutLight";
        public const string k_GradsLabel = "Grads";

        // =======================================================================
        [Tooltip("Texture with gradient ramps")]
        public Texture2D       _ramps;
        [Tooltip("Be default first color in gradient ramps used as key for lut color replacement." +
                 " In indexed mode, the first color of gradient ramps will be used as a key and from the second one his ramp will be taken." +
                 " It may be useful, for example, to make color ramps look the same in high light, but would have a different shading behaviour.")]
        public bool            _indexed;
        [Tooltip("Material for auto update.")]
        public Optional<Material> _material = new Optional<Material>(true, null);
        [Tooltip("Height in pixels of each gradient ramp." +
                 " Used if the texture does not match the gradient size or need to experiment with the size of the ramp." +
                 " In indexed mode, the first color is used as color key, and the gradient ramp is taken starting from the second. (height) ")]
        public Optional<int>   _rampsDepth = new Optional<int>(false, 7);
        [Tooltip("Number of gradient ramps, if disabled will be taken from the texture width." +
                 " It might be helpful to edit gradient ramps in one file with art from which they are taken. (width) ")]
        public Optional<int>   _rampsCount = new Optional<int>(false, 21);
        
        [Header("Advanced")]
        [Tooltip("Size of the lut table, can be increased if have color indexing problems." +
                 " In case the palette has colors that are close to each other.")]
        public  LutSize        _lutSize;
        [Tooltip("Blending between default lut and formed with gradient ramps. Basically weight of lut changes.")]
        [HideInInspector] [Range(0, 1)]
        public float           _weight = 1f;
        [Tooltip("Gamma type within which the colors are indexed in the gradient texture." +
                 " Rec601 is the standard gamma for most images, but it can be useful in some rare cases if indexing doesn't work correctly.")]
        public Gamma           _gamma = Gamma.rec601;
        [HideInInspector] [Tooltip("Blur radius in pixels.")]
        public Optional<float> _blur = new Optional<float>(false, 1);
        
        [Tooltip("Save indexed lut texture for debugging purposes, which is used as a mask to overwrite colors from the lut table.")]
        public bool           _saveIndexedLut;
        [HideInInspector]
        public string         _gradsPath;
        [HideInInspector]
        public Texture2D      _lut;
        [HideInInspector]
        public Texture2D      _lutIndexed;
        [HideInInspector]
        public Texture2D      _lutGenerated;
        [HideInInspector]
        public Texture2D      _gradsLink;

        // =======================================================================
        public class Grade
        {
            public Color[]      grade;
            public Vector2Int[] shape;

            // =======================================================================
            public Grade(Color[] grade, Vector2Int[] shape)
            {
                this.grade = grade;
                this.shape = shape;
            }
        }

        [Serializable]
        public enum LutSize
        {
            x16,
            x32,
            x64
        }

        [Serializable]
        public enum Gamma
        {
            rec601,
            rec709,
            rec2100,
            average,
        }
        
        // =======================================================================
        public void GenerateIndex()
        {
            _lutIndexed = _getLut().Copy();
            
            var lut       = _lutIndexed.GetPixels();
            var colors    = new List<Color>();
            var gradsSize = new Vector2Int(_rampsCount.GetValue(_ramps.width), _rampsDepth.GetValue(_ramps.height));
            
            var gradient = _ramps.GetPixels(0, _ramps.height - gradsSize.y, gradsSize.x, gradsSize.y, 0).ToArray2D(gradsSize.x, gradsSize.y);
            for (var x = 0; x < gradient.GetLength(0); x++)
            {
                var grade = gradient.GetColumn(x).Reverse().ToArray();
                var color = grade.First();
                
                colors.Add(color);
            }

            // grade colors from lut to palette by rgb 
            var pixels = lut.Select(lutColor => colors.Select(gradeColor => (grade: compare(lutColor, gradeColor), color: gradeColor)).OrderBy(n => n.grade).First())
                            .Select(n => n.color)
                            .ToArray();
            
            _lutIndexed.SetPixels(pixels);
            _lutIndexed.Apply();
            
#if UNITY_EDITOR
            if (_saveIndexedLut)
            {
                var path = $"{Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this))}\\{name} Indexed.png";
                File.WriteAllBytes(path, _lutIndexed.EncodeToPNG());
                UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
                _setImportOptions(UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path), true);
            }
#endif

            // -----------------------------------------------------------------------
            float compare(Color a, Color b)
            {
                // compare colors by grayscale distance
                var weight = _gamma switch
                {
                    Gamma.rec601  => new Vector3(0.299f, 0.587f, 0.114f),
                    Gamma.rec709  => new Vector3(0.2126f, 0.7152f, 0.0722f),
                    Gamma.rec2100 => new Vector3(0.2627f, 0.6780f, 0.0593f),
                    Gamma.average => new Vector3(0.33333f, 0.33333f, 0.33333f),
                    _             => throw new ArgumentOutOfRangeException()
                };

                var c = a.ToVector3().Mul(weight) - b.ToVector3().Mul(weight);
                
                return c.magnitude;
            }
        }

        private void OnValidate()
        {
            _setupMaterial();
            
#if UNITY_EDITOR
            // take grads path for auto import, set the label of a gradient texture
            if (_ramps != null)
            {
                _gradsPath = UnityEditor.AssetDatabase.GetAssetPath(_ramps);
                
                _addLabel(_ramps, k_GradsLabel);
                
                _gradsLink = _ramps;
            }
            
            // clear path, remove label
            if (_ramps == null)
            {
                _gradsPath = string.Empty;
                
                _removeLabel(_gradsLink, k_GradsLabel);
                
                _gradsLink = null;
            }

            // -----------------------------------------------------------------------
            async void _addLabel(Object obj, string lable)
            {
                await Task.Yield();
                
                if (obj == null)
                    return;
                
                var labels = UnityEditor.AssetDatabase.GetLabels(obj);
                if (labels.Contains(lable))
                    return;
                
                UnityEditor.AssetDatabase.SetLabels(_ramps, labels.Append(lable).ToArray());
            }
            
            async void _removeLabel(Object obj, string label)
            {
                await Task.Yield();
                
                if (obj == null)
                    return;
                
                var labels = UnityEditor.AssetDatabase.GetLabels(obj);
                if (labels.Contains(label) == false)
                    return; 
                
                UnityEditor.AssetDatabase.SetLabels(obj, labels.Except(new []{label}).ToArray());
            }
#endif
        }
        
        public void Bake()
        {
            if (_ramps == null)
            {
                Debug.LogError($"{name} Can't bake lut, gradients is not set", this);
                return;
            }
                
            GenerateIndex();
            
            var lutSize   = _getLutSize();
            var width     = lutSize * lutSize;
            var height    = lutSize;
            var lut       = _lutIndexed.GetPixels().ToArray2D(width, height);
            var grades    = new List<Grade>();
            var gradsSize = new Vector2Int(_rampsCount.GetValue(_ramps.width), _rampsDepth.GetValue(_ramps.height - (_indexed ? 1 : 0)) + (_indexed ? 1 : 0));
            var tex       = new Texture2D(width, height * _rampsDepth.GetValue(_ramps.height - (_indexed ? 1 : 0)), TextureFormat.RGBA32, false, false);
            
            // collect shapes
            var gradient = _ramps.GetPixels(0, _ramps.height - gradsSize.y, gradsSize.x, gradsSize.y, 0).ToArray2D(gradsSize.x, gradsSize.y);
            for (var x = 0; x < gradient.GetLength(0); x++)
            {
                var grade = gradient.GetColumn(x).Reverse().ToArray();
                var shape = _colorShape(grade.First()).ToArray();
                
                grades.Add(new Grade(grade, shape));
            }
            
            // draw gradient luts
            var pixels = new Color[tex.width, tex.height];
            for (var row = 0; row < gradsSize.y + (_indexed ? -1 : 0); row++)
            {
                var offset = new Vector2Int(0, (gradsSize.y - (row + 1 + (_indexed ? 1 : 0))) * lutSize);

                foreach (var grade in grades)
                {
                    var gradeColor = grade.grade[row + (_indexed ? 1 : 0)];
                    foreach (var pos in grade.shape)
                    {
                        var colorPos = pos + offset;
                        pixels[colorPos.x, colorPos.y] = gradeColor;
                    }
                }
            }
            
            // blur pixels
            if (_blur.enabled)
            {
                var sampled = new Color[tex.width, tex.height];
                for (var x = 0; x < tex.width; x++)
                for (var y = 0; y < tex.height; y++)
                {
                    sampled[x, y] = _softSample(x, y);
                }
                
                pixels = sampled;

                // -----------------------------------------------------------------------
                Color ImpactAt(int x, int y, int z, Color graded, Color fallback)
                {
                    if (x.InRange(0, lutSize - 1) == false || y.InRange(0, lutSize - 1) == false || z.InRange(0, lutSize - 1) == false)
                        return fallback;
                    
                    return graded - _lutAt(x, y, z);
                }
                
                Color _softSample(int x, int y)
                {
                    var radius = _blur.value.CeilToInt();
                    var (xBase, yBase, zBase) = _to3D(x, y % lutSize);
                    
                    var impact = Color.clear;
                    var totalWeight = 0f;
                    
                    // 3d sphere sample
                    for (var x3d = -radius + xBase; x3d <= radius + xBase; x3d++)
                    for (var y3d = -radius + yBase; y3d <= radius + yBase; y3d++)
                    for (var z3d = -radius + zBase; z3d <= radius + zBase; z3d++)
                    {
                        var weight   = (Vector3.Distance(new Vector3(x3d, y3d, z3d), new Vector3(xBase, yBase, zBase)) / _blur.value).OneMinus().Clamp01();
                        if (weight <= 0f)
                            continue;
                        totalWeight += weight;
                        
                        var (x2d, y2d) =  _to2D(x3d, y3d, z3d);
                        y2d            += (y / (float)lutSize).FloorToInt() * lutSize;
                        if (pixels.InBounds(x2d, y2d) == false)
                        {
                            impact += (pixels[x, y] - _lutAt(x, y % lutSize)) * weight;
                            continue;
                        }
                        
                        impact += ImpactAt(x3d, y3d, z3d, pixels[x2d, y2d], pixels[x, y] - _lutAt(x, y % lutSize)) * weight;
                    }
                    
                    return _lutAt(x, y % lutSize) + impact / totalWeight;
                }
            }
            
            // lerp against initial lut
            if (_weight != 1f)
            {
                var lerp = _weight;
                
                for (var x = 0; x < tex.width; x++)
                for (var y = 0; y < tex.height; y++)
                    pixels[x, y] = Color.Lerp(_lutAt(x, y % lutSize), pixels[x,y ], lerp); 
            }
            
            
            tex.SetPixels(pixels.ToArray());
            tex.Apply();
            
#if UNITY_EDITOR
            var path = $"{Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this))}\\{name}.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());

            UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
            _lutGenerated = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path); 
            _setImportOptions(_lutGenerated, false);
#endif
            _setupMaterial();

            // -----------------------------------------------------------------------
            IEnumerable<Vector2Int> _colorShape(Color color)
            {
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    if (lut[x, y] == color)
                        yield return new Vector2Int(x, y);
                }
            }
        }
        
        // =======================================================================
        internal async void _setupMaterial()
        {
            if (_material.TryGetValue(out var mat) == false || mat == null)
                return;
            
            mat.SetTexture("_Lut", _lutGenerated);
            mat.SetFloat("_Grades", _rampsDepth.GetValue(_ramps.height - (_indexed ? 1 : 0)));
            mat.SetInt("_LUT_SIZE", (int)_lutSize);
            
#if UNITY_EDITOR
            await Task.Yield();
            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(_material.Value));
#endif
        }
        
        internal int _getLutSize()
        {
            return _lutSize switch
            {
                LutSize.x16 => 16,
                LutSize.x32 => 32,
                LutSize.x64 => 64,
                _           => throw new ArgumentOutOfRangeException()
            };
        }
        
        internal Texture2D _getLut()
        {
            var lutSize = _getLutSize();
            if (_lut != null && _lut.height == lutSize)
                return _lut;
            
            _lut            = new Texture2D(lutSize * lutSize, lutSize, TextureFormat.RGBA32, 0, false);
            _lut.filterMode = FilterMode.Point;

            for (var y = 0; y < lutSize; y++)
                for (var x = 0; x < lutSize * lutSize; x++)
                {
                    _lut.SetPixel(x, y, _lutAt(x, y));
                }
            
            _lut.Apply();
            
            return _lut;
        }
        
        private Color _lutAt(int x, int y)
        {
            var lutSize = _getLutSize();
            return new Color((x % lutSize) / (lutSize - 1f), y / (lutSize - 1f), (x / (float)lutSize).FloorToInt() * (1f / (lutSize - 1f)), 1f);
        }
        
        private (int x, int y, int z) _to3D(int x, int y)
        {
            var lutSize = _getLutSize();
            return (x % lutSize, y, (x / (float)lutSize).FloorToInt());
        }
        
        private (int x, int y) _to2D(int x, int y, int z)
        {
            var lutSize = _getLutSize();
            return (x + z * lutSize, y);
        }
        
        private Color _lutAt(int x, int y, int z)
        {
            var lutSize = _getLutSize();
            return new Color(x / (lutSize - 1f), y / (lutSize - 1f), z / (lutSize - 1f), 1f);
        }
        
#if UNITY_EDITOR
        public static void _setImportOptions(Texture2D tex, bool readable, bool import = true)
        {
            var path     = UnityEditor.AssetDatabase.GetAssetPath(tex);
            var importer = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
            importer.alphaSource         = UnityEditor.TextureImporterAlphaSource.FromInput;
            importer.anisoLevel          = 0;
            importer.textureType         = UnityEditor.TextureImporterType.Default;
            importer.textureCompression  = UnityEditor.TextureImporterCompression.Uncompressed;
            importer.filterMode          = FilterMode.Point;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture         = false;
            importer.isReadable          = readable;
            importer.mipmapEnabled       = false;
            importer.npotScale           = UnityEditor.TextureImporterNPOTScale.None;
            
            var texset = importer.GetDefaultPlatformTextureSettings();
            texset.format              = UnityEditor.TextureImporterFormat.RGBA32;
            texset.crunchedCompression = false;
            importer.SetPlatformTextureSettings(texset);
            
            if (import)
                UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
        }
#endif
    }
}