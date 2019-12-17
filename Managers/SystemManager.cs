using System.Collections.Generic;
using OpenGL_Game.Systems;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Managers
{
    class SystemManager
    {
        List<ISystem> systemList = new List<ISystem>();

        public SystemManager()
        {
        }

        bool firstRun = true;
        public void ActionSystems(EntityManager entityManager)
        {
            List<Entity> entityList = entityManager.Entities();
            foreach(ISystem system in systemList)
            {
                if (firstRun)
                    System.Console.WriteLine(system.Name);
                if (system.Name == "SystemRender") continue;
                foreach(Entity entity in entityList)
                {
                    system.OnAction(entity);
                }
            }
            firstRun = false;
        }
        public void ActionRenderSystems(EntityManager entityManager)
        {
            List<Entity> entityList = entityManager.Entities();
            ISystem system = systemList.Find(s => s.Name == "SystemRender");
            foreach (Entity entity in entityList)
            {
                system.OnAction(entity);
            }
        }
        public void AddSystem(ISystem system)
        {
            ISystem result = FindSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            systemList.Add(system);
        }

        private ISystem FindSystem(string name)
        {
            return systemList.Find(delegate(ISystem system)
            {
                return system.Name == name;
            }
            );
        }
        
    }
}
