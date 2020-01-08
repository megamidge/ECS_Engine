using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentSkybox : IComponent
    {
        public ComponentTypes ComponentType => ComponentTypes.COMPONENT_SKYBOX;

        public ComponentSkybox(string cubemap)
        {

        }

        public void Dispose()
        {
        }
    }
}
