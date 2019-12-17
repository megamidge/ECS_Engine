using OpenTK;

namespace OpenGL_Game.Components
{
    class ComponentTransform : IComponent
    {
        private Vector3 lastPosition;//Previous position of this transform.
        //Position, rotation and scale for all axis.
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale;
        public ComponentTransform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
        public ComponentTransform(Vector3 position)
        {
            this.position = position;
            this.rotation = new Vector3(0, 0, 0);
            this.scale = new Vector3(1, 1, 1);
        }
        public ComponentTransform(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
            this.rotation = new Vector3(0, 0, 0);
            this.scale = new Vector3(1, 1, 1);
        }
        public Vector3 LastPosition => lastPosition;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                lastPosition = position;
            }
        }
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public void Dispose()
        {

        }
        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TRANSFORM; }
        }
    }
}
