using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Objects
{
    class NavMesh
    {
        List<NavNode> nodes;
        List<Cell> cells;
        public NavMesh(List<NavNode> nodes, List<Cell> cells)
        {
            foreach (NavNode node in nodes)
            {
                foreach (string nodeName in node.connectedNodeNames)
                {
                    NavNode nodeToAdd = nodes.Find(n => n.name == nodeName);
                    node.AddConnectedNode(nodeToAdd);
                }
            }
            foreach(Cell cell in cells)
            {
                foreach(string nodeName in cell.containedNodeNames)
                {
                    NavNode nodeToAdd = nodes.Find(n => n.name == nodeName);
                    cell.AddContainedNode(nodeToAdd);
                }
            }
            this.nodes = nodes;
            this.cells = cells;
        }

        public List<NavNode> Nodes => nodes;
        /// <summary>
        /// Find the <paramref name="count"/> nearest nodes to <paramref name="position"/>
        /// Known issue: might not work for all NavMeshes as it does not take cells into account (yet). It should for the maze though.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <returns><paramref name="count"/> nearest nodes to <paramref name="position"/></returns>
        public List<NavNode> FindNearestNodes(Vector2 position)
        {
            List<Cell> currentCells = FindCellsForPosition(position);
            List<NavNode> navNodes = new List<NavNode>();
            foreach(Cell cell in currentCells)
            {
                foreach(NavNode navNode in cell.ContainedNodes)
                {
                    if (!navNodes.Contains(navNode))
                        navNodes.Add(navNode);
                }
            }

            return navNodes;
        }
        public List<Cell> FindCellsForPosition(Vector2 position)
        {
            float margin = 0.3f; //Margin of error for edge of cell (position is likely to not be exactly on the edge, meaning the ai would get stuck very close to a node)
            List<Cell> foundCells = new List<Cell>();
            foreach(Cell cell in cells)
            {
                if(position.X - margin <= cell.boundsPositive.X && position.X + margin >= cell.boundsNegative.X && position.Y - margin <= cell.boundsPositive.Y && position.Y + margin >= cell.boundsNegative.Y)
                {
                    foundCells.Add(cell);
                }
            }
            return foundCells;
        }
    }

    /// <summary>
    /// Only supports axis-aligned square cells.
    /// </summary>
    class Cell
    {
        public readonly Vector2 boundsPositive;
        public readonly Vector2 boundsNegative;
        public readonly string name;
        List<NavNode> containedNodes;

        public readonly string[] containedNodeNames;

        public Cell(string name, Vector2 boundsPostive, Vector2 boundsNegative, string[] containedNodeNames)
        {
            this.name = name;
            this.boundsPositive = boundsPostive;
            this.boundsNegative = boundsNegative;
            this.containedNodeNames = containedNodeNames;
            containedNodes = new List<NavNode>();
        }

        public void AddContainedNode(NavNode node)
        {
            containedNodes.Add(node);
        }

        public List<NavNode> ContainedNodes => containedNodes;
    }
    class NavNode
    {
        public readonly string name;
        public readonly Vector2 position;
        Dictionary<NavNode, float> connectedNodes;
        public float distanceToTarget = 0;

        public readonly string[] connectedNodeNames; //Names of nodes this node is connected to. Use to help populate connectedNodes dictionary.

        public NavNode (string name, Vector2 position, string[] connectedNodeNames)
        {
            this.name = name;
            this.position = position;
            this.connectedNodeNames = connectedNodeNames;
            connectedNodes = new Dictionary<NavNode, float>();
        }

        public void AddConnectedNode(NavNode node)
        {
            float dist = Vector2.Distance(this.position, node.position);
            connectedNodes.Add(node, dist);
        }

        public Dictionary<NavNode, float> ConnectedNodes => connectedNodes;
    }
}
