using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;

namespace OpenGL_Game.Systems
{
    class SystemAI : ISystem
    {

        const ComponentTypes MASK = (ComponentTypes.COMPONENT_AI | ComponentTypes.COMPONENT_TRANSFORM);

        public string Name => "SystemAI";

        private NavMesh navMesh;
        private Camera camera; //If player was more than just a camera, this would be an entity instead. Used to get the player position.

        public SystemAI(NavMesh navMesh, Camera camera)
        {
            this.navMesh = navMesh;
            this.camera = camera;
        }
        private static bool enabled = true;
        public static void Toggle()
        {
            enabled = !enabled;
        }
        public void OnAction(Entity entity)
        {
            if((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent transformComponent = components.Find(comp => comp.ComponentType == ComponentTypes.COMPONENT_TRANSFORM);

                IComponent aiComponent = components.Find(comp => comp.ComponentType == ComponentTypes.COMPONENT_AI);

                if(enabled)
                Locomote((ComponentTransform)transformComponent, (ComponentAI)aiComponent);
            }
        }

        private void Locomote(ComponentTransform transformComponent, ComponentAI aiComponent)
        {
            List<NavNode> nearestNodes = navMesh.FindNearestNodes(transformComponent.Position.Xz);
            List<NavNode> nearestNodesToPlayer = navMesh.FindNearestNodes(camera.cameraPosition.Xz);
            List<NavNode> nodes = navMesh.Nodes;
            foreach(NavNode node in nodes)
            {
                node.distanceToTarget = Vector2.Distance(node.position, camera.cameraPosition.Xz);
            }

            List<Route> possibleRoutes = new List<Route>();
            Route shortestRoute = new Route{ nodes = null, length = 0};
            foreach(NavNode nearNode in nearestNodes)
            {
                float length = Vector2.Distance(transformComponent.Position.Xz, nearNode.position) + nearNode.distanceToTarget;
                Route route = new Route();
                route.nodes = new List<NavNode>() { nearNode };
                route.length = length;
                possibleRoutes.Add(route);
                shortestRoute = possibleRoutes.OrderBy(r => r.length).First();
            }

            bool routeFound = false;

            if (nearestNodesToPlayer.Contains(shortestRoute.nodes.Last()))
                routeFound = true;
            int depth = 0;
            while (!routeFound)
            {
                List<Route> routesToAdd = new List<Route>();
                foreach(Route route in possibleRoutes)
                {
                    Route copyRoute = new Route(route);
                    for(int i = 0; i < route.nodes[depth].ConnectedNodes.Count(); i++)
                    {
                        if (i == 0)
                        {
                            KeyValuePair<NavNode, float> connectedNode = route.nodes[depth].ConnectedNodes.ElementAt(i);
                            route.nodes.Add(connectedNode.Key);
                            route.length += connectedNode.Key.distanceToTarget;
                        }
                        else
                        {
                            KeyValuePair<NavNode, float> connectedNode = route.nodes[depth].ConnectedNodes.ElementAt(i);
                            Route newRoute = new Route(copyRoute);
                            newRoute.nodes.Add(connectedNode.Key);
                            newRoute.length += connectedNode.Key.distanceToTarget;
                            routesToAdd.Add(newRoute);
                        }
                    }
                }
                possibleRoutes.AddRange(routesToAdd);
                routesToAdd.Clear();
                shortestRoute = possibleRoutes.OrderBy(pR => pR.length).First();
                if (nearestNodesToPlayer.Contains(shortestRoute.nodes.Last()))
                    routeFound = true;
                if (depth > 5) //Limit the depth of the search so that if the player can't be found in a reasonable amount of time, the AI simply stops. This mostly just stops an inifnite loop happening should the player leave the navmesh.
                    routeFound = true;
                depth++;
            }

            Vector3 rotation = transformComponent.Rotation;
            Vector3 position = transformComponent.Position;
            
            Vector3 targetPos = new Vector3(shortestRoute.nodes[0].position.X, position.Y, shortestRoute.nodes[0].position.Y);
            List<Cell> currentCells = navMesh.FindCellsForPosition(position.Xz);
            List<Cell> playerCells = navMesh.FindCellsForPosition(camera.cameraPosition.Xz);
            if(currentCells.Intersect(playerCells).Count() > 0)
            {
                targetPos = new Vector3(camera.cameraPosition.X, position.Y, camera.cameraPosition.Z);
            }
            Vector3 moveDirection = (targetPos - position).Normalized();
            //todo: fix angle
            float targetAngle = Vector3.CalculateAngle(new Vector3(0, 0, 1), moveDirection);
            if (moveDirection.X < 0)
                targetAngle = -targetAngle;
            if (rotation.Y < targetAngle)
                rotation.Y += 0.02f;
            else
                rotation.Y = targetAngle;
            position += 3 * moveDirection * Managers.SceneManager.deltaTime;

            transformComponent.Position = position;
            transformComponent.Rotation = rotation;
        }
        private class Route
        {
            public List<NavNode> nodes;
            public float length;


            public Route()
            {

            }
            //Copy ctor
            public Route(Route route)
            {
                this.nodes = new List<NavNode>(route.nodes);
                this.length = route.length;
            }
        }
    }
}
