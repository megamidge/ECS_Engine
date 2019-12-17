using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class Camera
    {
        public Vector3 previousPosition;
        public Matrix4 view, projection;
        public Vector3 cameraPosition, cameraDirection, cameraUp;
        private Vector3 targetPosition;
        private float radius;
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Camera()
        {
            cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
            previousPosition = cameraPosition;
            cameraDirection = new Vector3(0.0f, 0.0f, -1.0f);
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            radius = 2f;
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1.0f, 0.1f, 100f);
        }

        public Camera(Vector3 cameraPos, Vector3 targetPos, float ratio, float near, float far)
        {
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            cameraPosition = cameraPos;
            previousPosition = cameraPosition;
            cameraDirection = targetPos-cameraPos;
            cameraDirection.Normalize();
            radius = 2f;
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), ratio, near, far);
        }

        public void MoveForward(float move)
        {
            previousPosition = cameraPosition;
            cameraPosition += move*cameraDirection;
            UpdateView();
        }

        public void Translate(Vector3 move)
        {
            previousPosition = cameraPosition;
            cameraPosition += move;
            UpdateView();
        }
        public void SetPosition(Vector3 position)
        {
            //previousPosition = cameraPosition;
            cameraPosition = position;
            UpdateView();
        }
        public void RotateY(float angle)
        {
            cameraDirection = Matrix3.CreateRotationY(angle) * cameraDirection;
            UpdateView();
        }
        public void UpdateView()
        {
            targetPosition = cameraPosition + cameraDirection;
            view = Matrix4.LookAt(cameraPosition, targetPosition, cameraUp);
        }
    }
}
