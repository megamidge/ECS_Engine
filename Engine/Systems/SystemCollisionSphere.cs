using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Components;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Systems
{
    class SystemCollisionSphere : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_COLLISION_SPHERE);
        public string Name => "SystemCollisionSphere";

        public void OnAction(Entity entity)
        {
            if((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });

                IComponent collisionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_SPHERE;
                });

                Collision(entity, (ComponentTransform)positionComponent, (ComponentCollisionSphere)collisionComponent);
            }
        }
        private void Collision(Entity entity, ComponentTransform positionComponent, ComponentCollisionSphere collisionComponent)
        {
            //if ((positionComponent.Position - camera.cameraPosition).Length < collisionComponent.Radius + camera.Radius)
            //{
            //    collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.SPHERE_SPHERE);

            //}
        }
    }
}
