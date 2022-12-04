using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace TucanEngine.Rendering.Tools.Common
{
    public class ArrayData
    {
        public const int NoneId = -1;

        private int id;
        public int Id => id;

        private List<int> vbos = new List<int>();
        private List<Action> bindings = new List<Action>();

        public int GetVboId(int dataLocation) => vbos[dataLocation];

        public void Push<T>(int location, int dim, T[] data, BufferTarget target) where T : struct {
            bindings.Add(() =>
            {
                var vbo = new BufferData<T>(location, dim, data, target);
                vbos.Add(vbo.Id);
            });
        }

        public void Create() {
            GL.GenVertexArrays(1, out id);
            GL.BindVertexArray(id);
            foreach (var binding in bindings) binding.Invoke();
            GL.BindVertexArray(0);
        }

        public void Delete()
        {
            GL.BindVertexArray(id);
            foreach (var vbo in vbos) GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(id);
            GL.BindVertexArray(0);
        }
    }
}