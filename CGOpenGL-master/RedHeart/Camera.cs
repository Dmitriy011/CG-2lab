using OpenTK.Mathematics;
using System;

namespace RedHeart
{
   public class Camera
    {
        //Векторы, задающие направления осей координат для камеры
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        //Угол поворота камеры вокруг оси X (в радианах)
        private float _pitch;
        //Угол поворота камеры вокруг оси Y (в радианах)
        //(без -2pi/2 камера при запуске была бы повёрнута вправо на 90 градусов)
        private float _yaw = -MathHelper.PiOver2;

        //Угол обзора камеры (в радианах)
        private float _fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        //Позиция камеры (точка)
        public Vector3 Position { get; set; }

        //Соотношение сторон окна, используемое для расчёта projection-матрицы
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;
     
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        //Получение матрицы проекции
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }
    }
}