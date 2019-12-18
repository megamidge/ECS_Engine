using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Components;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenTK;

namespace OpenGL_Game.Systems
{
    class SystemCollisionCamera : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_COLLISION_SPHERE);
        const ComponentTypes MASK2 = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_COLLISION_LINE);
        public string Name => "SystemCollisionCameraSphere";

        CollisionManager collisionManager;
        Camera camera;
        public SystemCollisionCamera(CollisionManager collisionManager, Camera camera)
        {
            this.collisionManager = collisionManager;
            this.camera = camera;
        }

        public void OnAction(Entity entity)
        {
            if((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });

                IComponent collisionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_SPHERE;
                });

                SphereCollision(entity, (ComponentTransform)transformComponent, (ComponentCollisionSphere)collisionComponent);
            }
            if((entity.Mask & MASK2) == MASK2)
            {
                List<IComponent> components = entity.Components;

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });

                IComponent[] collisionComponents = components.FindAll(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_LINE;
                }).ToArray();

                foreach(ComponentCollisionLine collider in collisionComponents) //Loop through all line collider components.
                    LineCollision(entity, (ComponentTransform)transformComponent, collider);
            }
        }

        private void SphereCollision(Entity entity, ComponentTransform transformComponent, ComponentCollisionSphere collisionComponent)
        {
            if ((transformComponent.Position - camera.cameraPosition).Length < collisionComponent.Radius + camera.Radius)
            {
                collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.SPHERE_SPHERE);

            }
        }

        private void LineCollision(Entity entity, ComponentTransform transformComponent, ComponentCollisionLine collisionComponent)
        {
            for(int i = 0; i < collisionComponent.Points.Length-1; i++)
            {
                Vector2 lineStart = collisionComponent.Points[i];
                Vector2 lineEnd  = collisionComponent.Points[i+1];
                //To-do : take into account camera radius. Idea: Move the wall closer by the radius. Requires figuring out if the wall is +/- x,z, or both.
                Vector2 lineDirection = lineStart - lineEnd;
                Vector2 lineNormal = new Vector2(-lineDirection.Y, lineDirection.X).Normalized();

                Vector2 v1 = camera.cameraPosition.Xz - lineStart;
                Vector2 v2 = camera.previousPosition.Xz - lineStart;

                float v1DotNormal = Vector2.Dot(v1, lineNormal);
                float v2DotNormal = Vector2.Dot(v2, lineNormal);
                //Console.WriteLine(V1DotNormal * V2DotNormal);
                if(v1DotNormal * v2DotNormal < 0) //True = Movement crosses infinite line.
                {
                    Vector2 v3 = camera.previousPosition.Xz - lineStart;
                    Vector2 v4 = camera.previousPosition.Xz - lineEnd;

                    Vector2 moveDirection = camera.previousPosition.Xz - camera.cameraPosition.Xz;
                    Vector2 moveNormal = new Vector2(-moveDirection.Y, moveDirection.X).Normalized();

                    float v3DotNormal = Vector2.Dot(v3,moveNormal);
                    float v4DotNormal = Vector2.Dot(v4, moveNormal);

                    if(v3DotNormal * v4DotNormal < 0) //True = Collision has happened
                    {
                        collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.SPHERE_LINE);
                        return;
                    }
                }                
            }
        }
    }
}
