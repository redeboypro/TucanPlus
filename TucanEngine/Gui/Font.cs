using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Common.Drawables;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Tools.Common.Bridges;

namespace TucanEngine.Gui
{
    public class Font
    {
        public const string CharSheet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-=_+[]{}\\|;:'\".,<>/?`~ ";
        public static readonly char[] Chars = CharSheet.ToCharArray();

        private readonly Texture2D textureData;
        private readonly List<GlArrayData> VAOs = new List<GlArrayData>();

        public Font(Texture2D textureData) {
            this.textureData = textureData;
            var textureCoords = (float[]) PrimitiveData.QuadPositions.Clone();
            
            foreach (var c in Chars)
            {
                const float charSize = 1 / 16f;
                var y = c >> 4;
                var x = c & 0b1111;

                var left = x * charSize;
                var right = left + charSize;
                var top = y * charSize;
                var bottom = top + charSize;

                textureCoords[0] = textureCoords[2] = left;
                textureCoords[4] = textureCoords[6] = right;
                textureCoords[1] = textureCoords[5] = top;
                textureCoords[3] = textureCoords[7] = bottom;

                var arrayData = new GlArrayData();
                arrayData.Push(0, 2, PrimitiveData.CharPositions, BufferTarget.ArrayBuffer);
                arrayData.Push(1, 2, textureCoords, BufferTarget.ArrayBuffer);
                arrayData.Create();
                
                VAOs.Add(arrayData);
            }
        }
        
        public GlArrayData GetCharArrayData(char character) {
            return VAOs[CharSheet.IndexOf(character)];
        }
        
        public Texture2D GetTexture() {
            return textureData;
        }

        public void Delete() {
            textureData.Delete();
            foreach (var charVao in VAOs) {
                charVao.Delete();
            }
        }
        
    }
}