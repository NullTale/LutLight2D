using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D
{
    internal static class Utils
    {
        public static int FloorToInt(this float f)
        {
            return Mathf.FloorToInt(f);
        }
        
        public static int Square(this Vector2Int vec)
        {
            return vec.x * vec.y;
        }
        
        public static RectInt GetRectInt(this Texture2D texture)
        {
            return new RectInt(0, 0, texture.width, texture.height);
        }
        public static Rect GetRect(this Texture2D texture)
        {
            return new Rect(0, 0, texture.width, texture.height);
        }
        public static Vector2Int GetSize(this Texture2D texture)
        {
            return new Vector2Int(texture.width, texture.height);
        }
        
        public static Texture2D Copy(this Texture2D texture)
        {
            return texture.Copy(texture.graphicsFormat);
        }
        public static Texture2D Clear(this Texture2D texture, Color color)
        {
            texture.SetPixels(Enumerable.Repeat(color, texture.GetSize().Square()).ToArray());
            texture.Apply();
            
            return texture;
        }

        public static Texture2D Copy(this Texture2D texture, GraphicsFormat format)
        {
            var dst = new Texture2D(texture.width, texture.height, texture.graphicsFormat, 0, TextureCreationFlags.None);
            dst.filterMode = texture.filterMode;
            dst.wrapMode   = texture.wrapMode;
            dst.SetPixelData(texture.GetRawTextureData<byte>(), 0);
            dst.Apply();
            
            return dst;
        }

        public static Texture2D Copy(this Texture2D source, int width, int height, bool getPixelBilinear = true, TextureFormat format = TextureFormat.RGBA32)
        {
            if (source.width == width && source.height == height)
                return source.Copy();

            var result = new Texture2D(width, height, format, false, false);
            var pixels = new Color[width * height];
            var incX   = (1f / width);
            var incY   = (1f / height);

            if (getPixelBilinear)
            {
                for (var px = 0; px < pixels.Length; px++)
                    pixels[px] = source.GetPixelBilinear(incX * (px % width), incY * (px / (float)width));
            }
            else
            {
                for (var px = 0; px < pixels.Length; px++)
                    pixels[px] = source.GetPixel((incX * (px % width)).FloorToInt(), (incY * (px / (float)width)).FloorToInt());
            }

            result.SetPixels(pixels, 0);
            result.Apply();
            return result;
        }

        public static Texture2D Copy(this Texture2D texture, RectInt rect, TextureFormat format)
        {
            var dst = new Texture2D(rect.width, rect.height, format, false, false);
            try
            {
                dst.filterMode = texture.filterMode;
                dst.wrapMode   = texture.wrapMode;
                dst.SetPixels(0, 0, rect.width, rect.height, texture.GetPixels(rect.x, rect.y, rect.width, rect.height), 0);
                dst.Apply();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Can't copy texture {e}");
            }

            return dst;
        }
        
        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator.MoveNext())
                return enumerator.Current;

            return default;
        }
        
        public static T[,] ToArray2D<T>(this IEnumerable<T> t, int width, int height) 
        {
            using var e = t.GetEnumerator();
            
            var result = new T[width, height];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                result[x, y] = e.Next();
            
            return result;
        }
        
        public static T[] ToArray<T>(this T[,] array)
        {
            var width  = array.GetLength(0);
            var height = array.GetLength(1);
            
            var result = new T[width * height];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                result[y * width + x] = array[x, y];

            return result;
        }
        
        public static IEnumerable<T> GetColumn<T>(this T[,] t, int column) 
        {
            var height = t.GetLength(1);
            for (var y = 0; y < height; y++)
                yield return t[column, y];
        }
        
        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }
        
        public static Vector3 Mul(this Vector3 thisVector, Vector3 otherVector)
        {
            return new Vector3(
                thisVector.x * otherVector.x, 
                thisVector.y * otherVector.y,
                thisVector.z * otherVector.z);
        }
        
        public static bool InRange(this int number, int min, int max)
        {
            return number >= min && number <= max;
        }
        
        public static float OneMinus(this float f)
        {
            return 1f - f;
        }
        
        public static float Clamp01(this float f)
        {
            return Mathf.Clamp01(f);
        }
        
        public static int CeilToInt(this float f)
        {
            return Mathf.CeilToInt(f);
        }
        
        public static bool InBounds<T>(this T[,] array, int x, int y)
        {
            return x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1);
        }
    }
}