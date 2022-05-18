using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace LearnOpenTK.Common
{
    public class GL3DModel
    {
        //Вершины модели в виде списка
        public float[] vertices_x;
        public float[] vertices_y;
        public float[] vertices_z;

        public float[] norm_x;
        public float[] norm_y;
        public float[] norm_z;


        //Треугольники: (представленны списками вершин)
        //в виде v11, v12, v13, <- треугольник 1
        //       ...........
        //       vk1, vk2, vk3  <- треугольник k (последний)
        public int[] triangles;

        public GL3DModel(float[] _vertices_x, float[] _vertices_y, float[] _vertices_z, float[] _norm_x, float[] _norm_y, float[] _norm_z, int[] _triangles)
        {
            vertices_x = _vertices_x;
            vertices_y = _vertices_y;
            vertices_z = _vertices_z;

            norm_x = _norm_x;
            norm_y = _norm_y;
            norm_z = _norm_z;

            triangles = _triangles;
        }

        //Получение всех вершин с их нормалями в формате:
        //x1, y1, z1, nx1, ny1, nz1,
        //x2, y2, z2, nx2, ny2, nz2,
        //...........
        //xm, ym, zm, nxn, nyn, nzn
        //Первые 4 числа -  вершины, следующие 3 - нормали
        public float[] Get_vertices_and_normals()
        {
            float[] result = new float[vertices_x.Length * 6];
            for (int i = 0, j = 0; i < vertices_x.Length; i++, j += 6)
            {
                result[j] = vertices_x[i];
                result[j + 1] = vertices_y[i];
                result[j + 2] = vertices_z[i];

                result[j + 3] = norm_x[i];
                result[j + 4] = norm_y[i];
                result[j + 5] = norm_z[i];
            }
            return result;
        }
    }

    public class ObjReader
    {
        public GL3DModel ReadObj(string path)
        {
            List<float> objVertices_x = new List<float>();
            List<float> objVertices_y = new List<float>();
            List<float> objVertices_z = new List<float>();

            List<float> objNorm_x = new List<float>();
            List<float> objNorm_y = new List<float>();
            List<float> objNorm_z = new List<float>();

            string fileText = File.ReadAllText(path);
            string[] fileLines = fileText.Split('\n');

            for (int i = 0; i < fileLines.Length; i++)                                  //Проход по строкам obj
            {
                string line = fileLines[i];
                string[] lineParts = line.Split(' ');

                if (line.StartsWith("vn"))                                              //Считывание нормали
                {
                    float x = float.Parse(lineParts[1].Replace('.', ','));
                    float y = float.Parse(lineParts[2].Replace('.', ','));
                    float z = float.Parse(lineParts[3].Replace('.', ','));
                    objNorm_x.Add(x);
                    objNorm_y.Add(y);
                    objNorm_z.Add(z);
                }
                else
                {
                    if (line.StartsWith("v"))                                           //Считывание вершины
                    {
                        float x = float.Parse(lineParts[1].Replace('.', ','));
                        float y = float.Parse(lineParts[2].Replace('.', ','));
                        float z = float.Parse(lineParts[3].Replace('.', ','));
                        objVertices_x.Add(x);
                        objVertices_y.Add(y);
                        objVertices_z.Add(z);
                    }
                    else
                    {
                        if (line.StartsWith("f"))                                       //Начало треугольников в файле (конец перечисления вершин и нормалей)
                        {
                            break;
                        }
                    }
                }
            }

            //Массив нормалей к ним (уже отсортированный, каждой вершине нормаль)
            float[] normSorted_x = new float[objVertices_x.Count];
            float[] normSorted_y = new float[objVertices_y.Count];
            float[] normSorted_z = new float[objVertices_z.Count];
            List<int> objTriangles = new List<int>();

            for (int i = 0; i < fileLines.Length; i++)                                  //Считывание треугольников
            {
                string line = fileLines[i];
                string[] lineParts = line.Split(' ');

                if (line.StartsWith("f"))
                {
                    string[] s1 = lineParts[1].Split(new string[] { "//" }, StringSplitOptions.None);
                    string[] s2 = lineParts[2].Split(new string[] { "//" }, StringSplitOptions.None);
                    string[] s3 = lineParts[3].Split(new string[] { "//" }, StringSplitOptions.None);

                    int v1 = int.Parse(s1[0]) - 1;
                    int vn1 = int.Parse(s1[1]) - 1;

                    int v2 = int.Parse(s2[0]) - 1;
                    int vn2 = int.Parse(s2[1]) - 1;

                    int v3 = int.Parse(s3[0]) - 1;
                    int vn3 = int.Parse(s3[1]) - 1;

                    normSorted_x[v1] = objNorm_x[vn1];
                    normSorted_y[v2] = objNorm_y[vn2];
                    normSorted_z[v3] = objNorm_z[vn3];

                    objTriangles.Add(v1);
                    objTriangles.Add(v2);
                    objTriangles.Add(v3);
                }
            }
            return new GL3DModel(objVertices_x.ToArray(), objVertices_y.ToArray(), objVertices_z.ToArray(), normSorted_x, normSorted_y, normSorted_z, objTriangles.ToArray());
        }
    }
}