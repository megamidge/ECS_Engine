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
    class SystemCollisionCameraSphere : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_COLLISION_SPHERE);
        const ComponentTypes MASK2 = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_COLLISION_LINE);
        public string Name => "SystemCollisionCameraSphere";

        CollisionManager collisionManager;
        Camera camera;
        public SystemCollisionCameraSphere(CollisionManager collisionManager, Camera camera)
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

                IComponent collisionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_LINE;
                });

                LineCollision(entity, (ComponentTransform)transformComponent, (ComponentCollisionLine)collisionComponent);
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
            Vector2 lineDirection = collisionComponent.StartPosition - collisionComponent.EndPosition;
            Vector2 lineNormal = new Vector2(-lineDirection.Y, lineDirection.X).Normalized();

            Vector2 v1 = camera.cameraPosition.Xz - collisionComponent.StartPosition;
            Vector2 v2 = camera.previousPosition.Xz - collisionComponent.StartPosition;

            float V1DotNormal = Vector2.Dot(v1, lineNormal);
            float V2DotNormal = Vector2.Dot(v2, lineNormal);
            //Console.WriteLine(V1DotNormal * V2DotNormal);
            if(V1DotNormal * V2DotNormal < 0)
            {
                collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.SPHERE_LINE);
            }
                
        }
    }
}
