using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LearnOpenTK.Common
{
    public class Shader
    {
        public readonly int ShaderProgram;

        private readonly Dictionary<string, int> _uniformLocations;
        public Shader(string vertPath, string fragPath)
        {
            /*Вершинный шейдер*/
            var text_from_shader = File.ReadAllText(vertPath);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader); //создаем вершинный шейдер //vertexShader - идентификатор (получили на выходе), по которому создался объект вершинного шейдера
            GL.ShaderSource(vertexShader, text_from_shader); //загрузили наши строчки в vertexShader из вершинного шейдера 
            CompileShader(vertexShader); //скомпилировали шейдер
            /*END*/

            /*Фрагментный шейдер*/
            text_from_shader = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, text_from_shader);
            CompileShader(fragmentShader);
            /*END*/

            /*Шейдерная программа*/
            //эти два шейдера должны быть объединены в шейдерную программу, которую может использовать OpenGL
            ShaderProgram = GL.CreateProgram(); //создаем объект шейдерной программы
            GL.AttachShader(ShaderProgram, vertexShader); //в шейдерную программу записываем вершинный шейдер
            GL.AttachShader(ShaderProgram, fragmentShader);//в шейдерную программу записываем фрагментный шейдер
            LinkProgram(ShaderProgram); //собираем шейдерную программу
            /*END*/

            /*Uniform-перменные*/
            //необходимо получить все uniform переменные (Запрос этого из шейдера очень медленный, поэтому мы делаем это один раз при инициализации и повторно используем эти значения)
            GL.GetProgram(ShaderProgram, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms); //получаем колличество активных uniform
            _uniformLocations = new Dictionary<string, int>();                                              //для хранения местоположения uniform перемнных
            for (var i = 0; i < numberOfUniforms; i++)                                                      // Проходим по всем uniform,
            {
                var key = GL.GetActiveUniform(ShaderProgram, i, out _, out _);                              //Получаем имя uniform
                var location = GL.GetUniformLocation(ShaderProgram, key);                                   //Получаем местоположение
                _uniformLocations.Add(key, location);                                                       //Записываем в _uniformLocations
            }
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
        }
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
        }
        public void Use()
        {
            GL.UseProgram(ShaderProgram);
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(ShaderProgram, attribName);
        }
        public void SetInt(string name, int data)
        {
            GL.UseProgram(ShaderProgram);
            GL.Uniform1(_uniformLocations[name], data);
        }
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(ShaderProgram);
            GL.Uniform1(_uniformLocations[name], data);
        }
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(ShaderProgram);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(ShaderProgram);
            GL.Uniform3(_uniformLocations[name], data);
        }
    }
}
