using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Systems
{
    class SystemAudio : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_AUDIO);


        public SystemAudio()
        {
        }
        public string Name
        {
            get { return "SystemAudio"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent audioCommponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                });

                IComponent positionComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });

                IComponent velocityComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
                });

                DoAudio((ComponentTransform)positionComponent, (ComponentAudio)audioCommponent, (ComponentVelocity)velocityComponent);
            }
        }

        public void DoAudio(ComponentTransform componentPosition, ComponentAudio componentAudio, ComponentVelocity componentVelocity)
        {
            componentAudio.SourcePosition = componentPosition.Position;
            if (componentVelocity != null)
                componentAudio.SourceVelocity = componentVelocity.Velocity;
        }
    }
}
