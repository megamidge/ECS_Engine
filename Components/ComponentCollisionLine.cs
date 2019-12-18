using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    /// <summary>
    /// Describes a collider as a line with a start and end position. Is infinitely tall.
    /// Multiple line collision components can be stacked to describe seperate line strips.
    /// </summary>
    class ComponentCollisionLine : IComponent
    {
        public ComponentTypes ComponentType => ComponentTypes.COMPONENT_COLLISION_LINE;

        Vector2[] points;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos">X and Z position to start the line</param>
        /// <param name="endPos">X and Z position to end the line</param>
        public ComponentCollisionLine(Vector2 startPos, Vector2 endPos)
        {
            points = new Vector2[2];
            points[0] = startPos;
            points[1] = endPos;
        }
        public ComponentCollisionLine(Vector2[] points)
        {
            this.points = points;
        }
        public Vector2[] Points
        {
            get { return points; }
            set { points = value; }
        }

        public void Dispose()
        {
        }
    }
}
