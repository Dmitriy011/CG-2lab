using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using RedHeart.ObjImport;
using RedHeart.Shaders;

namespace RedHeart
{
    public class Window : GameWindow
    {
        private float[] verticesAndNormals_ball;
        private int[] elements_ball;

        private int VAO_ball;
        private int VBO_ball;
        private int _elementsBufferObject;

        private ShaderProgram shaderProgram;

        //Состояние камеры
        private Camera _camera;

        private float scale_ball = 0.05f;
        private Vector3 translate_ball = (-1f, 0f, 0f);
        private float scale_ring = 0.2f;
        private Vector3 translate_ring = (0.6f, 0f, 0f);
        private float rotate_ring = 45f;

        //Отвязанность прожектора от камеры
        private bool _spotlightFixed = false;

        //Начальное положение камеры в мире
        private Vector3 _startCameraPos = new Vector3(0.0f, 0.0f, 1f);
        //Текущее положение 
        private Vector3 CurSpotlightPos = new Vector3(0.0f, 0.0f, 1f);
        //Направление прожектора в данный момент
        private Vector3 CurSpotlightDir;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            //Импорт модели 
            ObjReader read_ball = new ObjReader();
            GL3DModel res_reading_obj_ball = read_ball.ReadObj("D:/CGOpenGL-master/Heart 3D Model/the_crowned_ring.obj");

                //ObjReader read_ring = new ObjReader();
                //GL3DModel res_reading_obj_ring = read_ball.ReadObj("D:/CGOpenGL-master/Heart 3D Model/the_crowned_ring.mtl.obj");

            //Получение массива вершин/нормалей и массива номеров вершин треугольников
            verticesAndNormals_ball = res_reading_obj_ball.GetVerticesWithNormals();
            elements_ball = res_reading_obj_ball.triangles;

            //Сборка шейдеров
            shaderProgram = new ShaderProgram(VertexShader.text, FragmentShader.text);

            VAO_ball = GL.GenVertexArray(); //Создание VAO 
            GL.BindVertexArray(VAO_ball); //привязка VAO

            //Создание объекта буфера вершин/нормалей, его привязка и заполнение
            VBO_ball = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_ball);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesAndNormals_ball.Length * sizeof(float),verticesAndNormals_ball, BufferUsageHint.StaticDraw);

            //Указание OpenGL, где искать вершины в буфере вершин/нормалей
            var posLoc = shaderProgram.GetAttribLocation("vPos");
            GL.EnableVertexAttribArray(posLoc);
            GL.VertexAttribPointer(posLoc, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            //Указание OpenGL, где искать нормали в буфере вершин/нормалей
            var normalLocation = shaderProgram.GetAttribLocation("vNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            //Создание, привязка и заполнение объекта-буфера элементов для треугольников
            _elementsBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementsBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements_ball.Length * sizeof(int), elements_ball, BufferUsageHint.StaticDraw);

            //Установка серого фона
            GL.ClearColor(0f, 1f, 1f, 0f);

            //Установка стартового положения камеры
            _camera = new Camera(_startCameraPos, Size.X / (float)Size.Y);

            //Захват курсора
            CursorGrabbed = true;
        }

        protected override void OnUnload()
        {
            //Отвязать буферы
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.UseProgram(0);

            //Очистить
            GL.DeleteVertexArray(VAO_ball);
            GL.DeleteBuffer(VBO_ball);
            GL.DeleteBuffer(_elementsBufferObject);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //Очистка буферов цвета и глубины
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Привязка буфера вершин
            GL.BindVertexArray(VAO_ball);
            //Использовать шейдеры
            shaderProgram.Use();

            //Привязка входных данных через uniform-переменные
            //(матрица модели - матрица для сердца в начале координат с заданным масштабированием и поворотом)
            Matrix4 ball = Matrix4.CreateScale(scale_ball) * Matrix4.CreateTranslation(translate_ball);

            Matrix4 ring = Matrix4.CreateScale(scale_ring) * Matrix4.CreateTranslation(translate_ring) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotate_ring));

            shaderProgram.SetMatrix4("model", ball);
            //(матрица перехода в пространство вида - "eye space")
            shaderProgram.SetMatrix4("view", _camera.GetViewMatrix());
            //(матрица проекции на систему координат от -1 до 1 по x и y)
            shaderProgram.SetMatrix4("projection", _camera.GetProjectionMatrix());
            //(позиция наблюдателя)
            shaderProgram.SetVector3("viewPos", _camera.Position);

            //(параметры света)
            if (_spotlightFixed)
            {
                shaderProgram.SetVector3("light.position", CurSpotlightPos);
                shaderProgram.SetVector3("light.direction", CurSpotlightDir);
            }
            else
            {
                shaderProgram.SetVector3("light.position", _camera.Position);
                shaderProgram.SetVector3("light.direction", _camera.Front);
            }

            shaderProgram.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            shaderProgram.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(32.5f)));
            shaderProgram.SetVector3("light.ambient", new Vector3(0.2f));
            shaderProgram.SetVector3("light.diffuse", new Vector3(0.7f));
            shaderProgram.SetVector3("light.specular", new Vector3(1.0f));
            shaderProgram.SetFloat("light.constant", 1.0f);
            shaderProgram.SetFloat("light.linear", 0.09f);
            shaderProgram.SetFloat("light.quadratic", 0.032f);

            GL.DrawElements( PrimitiveType.Triangles,elements_ball.Length,DrawElementsType.UnsignedInt, 0);
            shaderProgram.SetMatrix4("model", ring);
            GL.DrawElements(PrimitiveType.Triangles, elements_ball.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (!IsFocused)
                return;

            var pressure_Keyboard = KeyboardState;

            //Закрыть программу - Esc
            if (pressure_Keyboard.IsKeyDown(Keys.Escape))
            { 
                Close();
            }

            float component_y = -(translate_ball.X + (0.1f)) * (translate_ball.X + (0.1f)) + 1f;
            translate_ball = (translate_ball.X + 0.01f, component_y, 0f);

            if (translate_ball.X > 2)
            {
                translate_ball = (-1f, 0f, 0f);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}