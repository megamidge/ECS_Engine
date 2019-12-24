using OpenGL_Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Managers
{
    partial class GameSpecific_CollisionManager : CollisionManager
    {
        public override void ProcessCollisions()
        {
            foreach (Collision collision in collisionManifold)
            {
                if (collision.entity.Name.ToLower().Contains("pickup"))
                {
                    Scenes.GameScene.gameInstance.HandlePickup(collision.entity);
                }
                if (collision.entity.Name == "Landing_Ship_Door")
                {
                    Scenes.GameScene.gameInstance.HandleShipDoorCollision();
                }
                if (collision.entity.Name.ToLower().Contains("spikyball"))
                {
                    Scenes.GameScene.gameInstance.HandleSpikyBallCollision();
                }


                if(collision.collisionType == COLLISIONTYPE.SPHERE_LINE)
                {
                    Camera camera = Scenes.GameScene.gameInstance.camera;
                    camera.SetPosition(camera.previousPosition);
                    
                }
            }
            ClearManifold();
        }

    }
}
