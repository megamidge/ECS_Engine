using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    /// <summary>
    /// Describes a collider as a line with a start and end position. Is infinitely tall.
    /// </summary>
    class ComponentCollisionLine : IComponent
    {
        public ComponentTypes ComponentType => ComponentTypes.COMPONENT_COLLISION_LINE;

        Vector2 startPosition;
        Vector2 endPosition;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos">X and Z position to start the line</param>
        /// <param name="endPos">X and Z position to end the line</param>
        public ComponentCollisionLine(Vector2 startPos, Vector2 endPos)
        {
            this.startPosition = startPos;
            this.endPosition = endPos;
        }

        public Vector2 StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }
        public Vector2 EndPosition
        {
            get { return endPosition; }
            set { endPosition = value; }
        }

        public void Dispose()
        {
        }
    }
}
