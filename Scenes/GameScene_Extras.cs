using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Scenes
{
    partial class GameScene : Scene
    {
        float[] doorMinMaxVals = new float[] { 0, 0, 4.32f, 0.6f };//minPosY, minRotZ, maxPosY, maxRotZ. Min and Max values for the open and close door (acts as portal)
        int doorState = 3; //0 = opening, 1 = open, 2 = closing, 3 = closed.
        int pickupsCollected = 0;

        public int Score => pickupsCollected;

        private void HandleDoorPortal()
        {
            if (pickupsCollected >= 3)
            {
                doorState = 0;
            }
            Entity shipDoor = entityManager.Entities().Find(entity => entity.Name == "Landing_Ship_Door");
            if (shipDoor != null)
            {
                ComponentTransform compTransform = (ComponentTransform)shipDoor.Components.Find(component => component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM);
                Vector3 rotation = compTransform.Rotation;
                Vector3 position = compTransform.Position;
                ComponentAudio compAudio = (ComponentAudio)shipDoor.Components.Find(component => component.ComponentType == ComponentTypes.COMPONENT_AUDIO);
                //door close
                if (doorState == 2)
                {
                    if (position.Y < doorMinMaxVals[2] && rotation.Z < doorMinMaxVals[3])
                    {
                        rotation.Z += 0.00085f;
                        position.Y += 0.006f;
                        if (compAudio.State == ALSourceState.Stopped || compAudio.State == ALSourceState.Paused)
                            compAudio.Play();
                    }
                    else
                    {
                        doorState = 3;
                        compAudio.Stop();
                    }
                }
                if (doorState == 0)
                {
                    //door open
                    if (position.Y > doorMinMaxVals[0] && rotation.Z > doorMinMaxVals[1])
                    {
                        rotation.Z -= 0.00085f;
                        position.Y -= 0.006f;
                        if (compAudio.State == ALSourceState.Stopped || compAudio.State == ALSourceState.Paused)
                            compAudio.Play();
                    }
                    else
                    {
                        doorState = 1;
                        compAudio.Stop();
                    }
                }
                compTransform.Rotation = rotation;
                compTransform.Position = position;
            }
        }

        //private Entity[] pickupEntities = new Entity[3];
        private Vector3[] startPositions = new Vector3[3];
        bool firstRun = true;
        private void AnimatePickups()
        {
            List<Entity> pickupEntities = entityManager.Entities().Where(ent => ent.Name.ToLower().Contains("pickup")).ToList();
            for(int i = 0; i < pickupEntities.Count; i++)
            {
                ComponentTransform transform = (ComponentTransform)pickupEntities[i].Components.Find(component => component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM);
                Vector3 position = transform.Position;
                Vector3 rotation = transform.Rotation;
                if (firstRun) startPositions[i] = position;
                if (firstRun && i == pickupEntities.Count-1) firstRun = false;

                rotation.Y += MathHelper.DegreesToRadians(1);

                position.Y = startPositions[i].Y + 0.25f *  (float)Math.Sin(sceneManager.time);

                transform.Position = position;
                transform.Rotation = rotation;
            }
        }

        public void HandlePickup(Entity pickup)
        {
            pickupsCollected++;

            entityManager.RemoveEntity(pickup);
        }

        public void HandleShipDoorCollision()
        {
            if(doorState == 1)
            {
                sceneManager.ChangeScene(SceneType.SCENE_GAME_OVER);
            }
        }
    }
}
