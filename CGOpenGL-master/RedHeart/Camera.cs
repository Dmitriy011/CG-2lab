using OpenTK.Mathematics;
using System;

namespace RedHeart
{
   public class Camera
    {
        //�������, �������� ����������� ���� ��������� ��� ������
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        //���� �������� ������ ������ ��� X (� ��������)
        private float _pitch;
        //���� �������� ������ ������ ��� Y (� ��������)
        //(��� -2pi/2 ������ ��� ������� ���� �� �������� ������ �� 90 ��������)
        private float _yaw = -MathHelper.PiOver2;

        //���� ������ ������ (� ��������)
        private float _fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        //������� ������ (�����)
        public Vector3 Position { get; set; }

        //����������� ������ ����, ������������ ��� ������� projection-�������
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;
     
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        //��������� ������� ��������
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }
    }
}