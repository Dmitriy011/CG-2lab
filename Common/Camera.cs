using OpenTK.Mathematics;
using System;

namespace LearnOpenTK.Common
{
    public class Camera
    {
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;        //??????, ???????????? ?????
        private Vector3 _right = Vector3.UnitX;
        
        private float _pitch;                       // ??????? ?????? ? (? ????????)
        private float _yaw = -MathHelper.PiOver2;   // ??????? ?????? ? (? ????????)                                                             //= 90 ????????
        private float _fov = MathHelper.PiOver2;    // ???????? ?????? Z -  ???? ?????? ?????? (? ????????) - ???????????? ???? ?????? ??????    //= 90 ????????

        public Camera(Vector3 position, float aspectRatio)
        {
            camera_position = position;
            AspectRatio = aspectRatio;
        }

        public Vector3 camera_position { get; set; }                   //??????? ??????
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f); //???? ????? -89 ? 89 ????????
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);

            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }
        public Matrix4 GetViewMatrix()                                              //???????? ??????? view, ????????? ??????? LookAt
        {
            return Matrix4.LookAt(camera_position, camera_position + _front, _up);  //camera_position + _front - ?????????? ????
        }
        public Matrix4 GetProjectionMatrix()                                        //???????? ??????? ????????
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        private void UpdateVectors()
        {   
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);                         //direction.x - Cos(_pitch) 
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}