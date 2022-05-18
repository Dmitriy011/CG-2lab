using System;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using CG_lab2;
using System.Collections.Generic;

namespace LearnOpenTK
{
    public class Window : GameWindow
    {
        private int VBO_ball;
        private int VAO_ball;
        private int EBO_ball;
        private float scale_ball = 0.5f;
        private Vector3 translation_ball = (-1f, 0f, 0f);
        private Shader ShaderProgr_ball_ligting;
        float[] vertices_ball;
        uint[]  indices_ball;
        private int lenght_indices_ball;
        Texture texture_ball;

        private int VAO_ring;
        private int VBO_ring;
        private float scale_ring = 0.5f;
        private Vector3 translate_ring = (-1.1f, -0.5f, 0f);
        private float rotate_ring_x = 90f;
        private Shader ShaderProgram_ring;
        private float[] vertices_and_normals_ring;
        private int[] elements_ring;    
        private int elementsBuffer_ring;

        private Vector3 start_сamera_position = new Vector3(2.0f, 1.0f, 2.5f);       //Начальное положение камеры
        private Vector3 light_position = new Vector3(2.0f, 1.0f, 2.5f);              //положение прожектора
        private Vector3 light_direction;                                             //Направление прожектора

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings): base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() //вызывается один раз при создании окошка
        {

        /*Кольцо*/
            /*Создали шейдерную программу*/
            ShaderProgram_ring = new Shader("C:/Users/Rozanov/Desktop/CG_lab2-master/Chapter2/5-LightCasters-Spotlight/Shaders_ring/shader_ring.vert", "C:/Users/Rozanov/Desktop/CG_lab2-master/Chapter2/5-LightCasters-Spotlight/Shaders_ring/shader_ring.frag");
            /*Считали данные*/
            ObjReader text_about_object_ring = new ObjReader();
            GL3DModel res_ring = text_about_object_ring.ReadObj("C:/Users/Rozanov/Desktop/CGOpenGL-master/Heart 3D Model/the_crowned_ring.obj");
            vertices_and_normals_ring = res_ring.Get_vertices_and_normals();
            elements_ring = res_ring.triangles;
            /*Создаем VBO и записываем в него (много точек)*/
            VBO_ring = GL.GenBuffer();                                                                                  //создаем буфер (как бы массив) - создаем идентификатор, в который можно записывать массив вершин 
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_ring);                                                          //"делаем текущим" наш вершинный буфер, и связываем наш буфер-идентификатор с Target(со слотом) //Есть слот - Target, 
                                                                                                                        //и оно  обладает структурой, можем вызывать разные ф-ии для Target //Далее обращение не к объекту, а Target, с которым 
                                                                                                                        //соединил наш объект  
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_and_normals_ring.Length * sizeof(float), vertices_and_normals_ring, BufferUsageHint.StaticDraw); //в наш буферный оюъект загружаем массив точек //1ое: в какое Target грузим, 2ое: 
                                                                                                                                                              //размер, 3ее: ссылка на массив, 4ое:как будем использовать буфер (будет ли 
                                                                                                                                                              //мзеняться, ...)
            var positionLocation2 = ShaderProgram_ring.GetAttribLocation("position");                                   //возвращает идентификатор, по которому наш шейдер будет брать параметр position  //Указание OpenGL, где искать вершины 
                                                                                                                        //в буфере вершин     
            /*Создаем VAO, чтобы расскзать, как будут использоваться точки из VAO*/
            VAO_ring = GL.GenVertexArray();                                                                             //создаем еще "массив" //берем его идентификатор
            GL.BindVertexArray(VAO_ring);                                                                               //делаем текущим
            GL.VertexAttribPointer(positionLocation2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);   //"говорим, как будем использовать" наш буфер для вершин - позиция //1ое: данные вершинного объекта будем использовать в
                                                                                                                        //перменной position, //2ое: 3 числа нужно, 3ое: какой тип данных нужен, 4ое: нормальизация, ... 
            GL.EnableVertexAttribArray(positionLocation2);
            var normalLocation2 = ShaderProgram_ring.GetAttribLocation("vNormal");                                      //Указание OpenGL, где искать нормали в буфере вершин 
            GL.EnableVertexAttribArray(normalLocation2);
            GL.VertexAttribPointer(normalLocation2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            /*Создаем EBO - буфер объектов (заполняется треугольниками))*/
            elementsBuffer_ring = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementsBuffer_ring);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements_ring.Length * sizeof(int), elements_ring, BufferUsageHint.StaticDraw);

            //к одному VBO может быть несколько VAO (разные VAO исп разные части VBO)
        /*END*/

         /*Мяч*/
            Sphere ball = new Sphere(0.4f, 0.0f, 0.0f, 0.0f);
            /*Создали шейдерную программу*/
            ShaderProgr_ball_ligting = new Shader("../../../Shaders/shader_ball.vert", "../../../Shaders/lighting_ball.frag");
            /*Считали данные*/
            vertices_ball = ball.GetAll();
            indices_ball = ball.GetIndices();
            lenght_indices_ball = indices_ball.Length;
            /*Создаем VBO и записываем в него (много точек)*/
            VBO_ball = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_ball);
            GL.NamedBufferStorage(VBO_ball, vertices_ball.Length * sizeof(float), vertices_ball, BufferStorageFlags.MapWriteBit);
            /*VAO*/
            VAO_ball = GL.GenVertexArray();
            GL.BindVertexArray(VAO_ball);
            GL.EnableVertexArrayAttrib(VAO_ball, 0);
            GL.VertexArrayVertexBuffer(VAO_ball, 0, VBO_ball, IntPtr.Zero, 8 * sizeof(float));
            var positionLocation = ShaderProgr_ball_ligting.GetAttribLocation("aPos​");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            var normalLocation = ShaderProgr_ball_ligting.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            var texCoordLocation = ShaderProgr_ball_ligting.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            /*EBO*/
            EBO_ball = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO_ball);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices_ball.Length * sizeof(uint), indices_ball, BufferUsageHint.DynamicDraw);
            /*Текстура*/
            texture_ball = Texture.LoadFromFile("../../../Textures/ball.jpg");
        /*END*/

            _camera = new Camera(start_сamera_position, Size.X / (float)Size.Y);

            GL.ClearColor(0.15f, 0.15f, 0.15f, 0.0f);        //цвет, которым чистим буфер кадров
           
            GL.Enable(EnableCap.DepthTest);
            base.OnLoad();                                  //метод базового класса

            CursorGrabbed = true;
        }

        //
        protected override void OnRenderFrame(FrameEventArgs e)                                                     //отрисовка
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);                              //чистим буферы: ColorBufferBit - цветной буфер

            /*Кольцо*/
            GL.BindVertexArray(VAO_ring);                                                                           //активируем VAO
            ShaderProgram_ring.Use();                                                                               //активируем Шейдерную программу
            Matrix4 ring = Matrix4.CreateScale(scale_ring) * Matrix4.CreateTranslation(translate_ring) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotate_ring_x)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(180));
            ShaderProgram_ring.SetMatrix4("model", ring);
            ShaderProgram_ring.SetMatrix4("view", _camera.GetViewMatrix());
            ShaderProgram_ring.SetMatrix4("projection", _camera.GetProjectionMatrix());
            ShaderProgram_ring.SetVector3("viewPos", _camera.camera_position);                                             //позиция камеры
            /*Настройка прожектора*/
            ShaderProgram_ring.SetVector3("light.position", _camera.camera_position);                               //передаем данные о позиции прожектора 
            ShaderProgram_ring.SetVector3("light.direction", _camera.Front);                                        //передаем данные о направлении прожектора 
            ShaderProgram_ring.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));             // передаем данные об угле отсечения (который нжуен, чтобы знать радиус освещения)
            ShaderProgram_ring.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(32.5f)));        //угол между направлением прожектора и направлением из прожектора до фрагмента
            /*Коэф для ослабление интенсивности освещения с расстоянием - эффект затухания*/
            ShaderProgram_ring.SetFloat("light.constant", 1.0f);
            ShaderProgram_ring.SetFloat("light.linear", 0.09f);
            ShaderProgram_ring.SetFloat("light.quadratic", 0.032f);
            /*Свойства материала*/
            ShaderProgram_ring.SetVector3("material.ambient", new Vector3(1.0f, 0.2f, 0.4f));
            ShaderProgram_ring.SetVector3("material.diffuse", new Vector3(1.0f, 0.2f, 0.4f));
            ShaderProgram_ring.SetVector3("material.specular", new Vector3(0.8f));
            ShaderProgram_ring.SetFloat("material.shininess", 48.0f);
            /*Свойства света*/
            ShaderProgram_ring.SetVector3("light.ambient", new Vector3(0.2f));
            ShaderProgram_ring.SetVector3("light.diffuse", new Vector3(0.7f));
            ShaderProgram_ring.SetVector3("light.specular", new Vector3(1.0f));

            GL.DrawElements(PrimitiveType.Triangles, elements_ring.Length, DrawElementsType.UnsignedInt, 0);
            /*END*/

            /*Мяч*/
            Matrix4 ball = Matrix4.CreateScale(scale_ball) * Matrix4.CreateTranslation(translation_ball) 
                    * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(45));
            GL.BindVertexArray(VAO_ball);
            ShaderProgr_ball_ligting.Use();
            ShaderProgr_ball_ligting.SetMatrix4("view", _camera.GetViewMatrix());
            ShaderProgr_ball_ligting.SetMatrix4("projection", _camera.GetProjectionMatrix());
            ShaderProgr_ball_ligting.SetVector3("viewPos", _camera.camera_position);
            /*Настройка прожектора*/
            ShaderProgr_ball_ligting.SetVector3("light.position", _camera.camera_position);                         //передаем данные о позиции прожектора 
            ShaderProgr_ball_ligting.SetVector3("light.direction", _camera.Front);                                  //передаем данные о направлении прожектора 
            ShaderProgr_ball_ligting.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));       // передаем данные об угле отсечения (который нжуен, чтобы знать радиус освещения)
            ShaderProgr_ball_ligting.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));  //угол между направлением прожектора и направлением из прожектора до фрагмента     
            /*Коэф для ослабление интенсивности освещения с расстоянием - эффект затухания*/
            ShaderProgr_ball_ligting.SetFloat("light.constant", 1.0f);
            ShaderProgr_ball_ligting.SetFloat("light.linear", 0.09f);
            ShaderProgr_ball_ligting.SetFloat("light.quadratic", 0.032f);
            /*Свойства материала*/
            ShaderProgr_ball_ligting.SetInt("material.diffuse", 0);
            ShaderProgr_ball_ligting.SetInt("material.specular", 1);
            ShaderProgr_ball_ligting.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            ShaderProgr_ball_ligting.SetFloat("material.shininess", 32.0f);
            /*Свойства света*/
            ShaderProgr_ball_ligting.SetVector3("light.ambient", new Vector3(0.2f));
            ShaderProgr_ball_ligting.SetVector3("light.diffuse", new Vector3(0.5f));
            ShaderProgr_ball_ligting.SetVector3("light.specular", new Vector3(1.0f));

            texture_ball.Use(TextureUnit.Texture0);
            ShaderProgr_ball_ligting.SetMatrix4("model", ball);
            GL.DrawElements(PrimitiveType.Triangles, lenght_indices_ball, DrawElementsType.UnsignedInt, 0);
            /*END*/

            SwapBuffers(); //поменять буфер, который отрисовывали, покзаать, а 2ой отвести, чтобы на нем нарисовать 
            base.OnRenderFrame(e); //базовая отрисовка
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            /*Перемещение мяча*/
            float delta_x = 0.0005f;
            float component_y = -(translation_ball.X + (delta_x)) * (translation_ball.X + (delta_x)) + 1f;
            translation_ball = (translation_ball.X + delta_x, component_y, 0f);
            if (translation_ball.X > 2)
            {
                translation_ball = (-1f, 0f, 0f);
            }
            /*END*/

            const float sensitivity = 0.1f;
            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y); //Если изменился размер картинки (передаем количество пикселей)
            _camera.AspectRatio = Size.X / (float)Size.Y;

            base.OnResize(e);
        }
    }
}
