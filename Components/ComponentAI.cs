using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentAI : IComponent
    {
        public ComponentTypes ComponentType => ComponentTypes.COMPONENT_AI;

        public ComponentAI()
        {

        }

        public void Dispose()
        {
        }
    }
}
