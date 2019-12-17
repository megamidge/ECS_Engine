using OpenGL_Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Managers
{
    enum COLLISIONTYPE
    {
        SPHERE_SPHERE,
        LINE_LINE,
        SPHERE_LINE,
    }
    struct Collision
    {
        public Entity entity;
        public COLLISIONTYPE collisionType;
    }
    abstract class CollisionManager
    {
        protected List<Collision> collisionManifold = new List<Collision>();

        public CollisionManager()
        {
        }

        public void ClearManifold()
        {
            collisionManifold.Clear();
        }

        public void CollisionBetweenCamera(Entity entity, COLLISIONTYPE collisionType)
        {
            // If already in manifold, do not add..
            foreach (Collision coll in collisionManifold)
                if (coll.entity == entity) return;

            Collision collision;
            collision.entity = entity;
            collision.collisionType = collisionType;
            collisionManifold.Add(collision);
        }

        public abstract void ProcessCollisions();
    }
}
