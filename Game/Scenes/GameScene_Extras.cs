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

        private void HandleDoorPortal(float dT)
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
                        rotation.Z += 0.085f * dT;
                        position.Y += 0.6f * dT;
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
                        rotation.Z -= 0.085f * dT;
                        position.Y -= 0.6f * dT;
                        if (compAudio.State == ALSourceState.Stopped || compAudio.State == ALSourceState.Paused)
                            compAudio.Play();
                        AlterShipSound();
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
        private void AnimatePickups(float dT)
        {
            List<Entity> pickupEntities = entityManager.Entities().Where(ent => ent.Name.ToLower().Contains("pickup")).ToList();
            for(int i = 0; i < pickupEntities.Count; i++)
            {
                ComponentTransform transform = (ComponentTransform)pickupEntities[i].Components.Find(component => component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM);
                Vector3 position = transform.Position;
                Vector3 rotation = transform.Rotation;
                if (firstRun) startPositions[i] = position;
                if (firstRun && i == pickupEntities.Count-1) firstRun = false;

                rotation.Y += MathHelper.DegreesToRadians(30) * dT;

                position.Y = startPositions[i].Y + 0.25f *  (float)Math.Sin(2 * sceneManager.time);

                transform.Position = position;
                transform.Rotation = rotation;
            }
        }

        public void HandlePickup(Entity pickup)
        {
            pickupsCollected++;
            ComponentAudio audioComp = (ComponentAudio)pickup.Components.Find(c => c.ComponentType == ComponentTypes.COMPONENT_AUDIO);
            audioComp.Play();
            entityManager.RemoveEntity(pickup);
        }

        public void HandleShipDoorCollision()
        {
            if(doorState == 1)
            {
                sceneManager.ChangeScene(SceneType.SCENE_GAME_OVER);
            }
        }

        public void HandleSpikyBallCollision()
        {
            KillPlayer();
        }
        public void HandleDroneCollision()
        {
            KillPlayer();
        }
        private void KillPlayer()
        {
            pickupsCollected = 0;

            lives -= 1;
            if (lives <= 0)
                sceneManager.ChangeScene(SceneType.SCENE_GAME_OVER);
            else
                Reset();
        }

        private void AnimateSpikyBalls(float dt)
        {
            AnimateSpiky1(dt);
            AnimateSpiky2(dt);
        }

        private void AnimateSpiky1(float dt)
        {
            Entity entity = entityManager.FindEntity("SpikyBall_1");
            ComponentTransform transforComponent = (ComponentTransform)entity.Components.Find(c => c.ComponentType == ComponentTypes.COMPONENT_TRANSFORM);

            //Animate;
            Vector3 rotation = transforComponent.Rotation;
            rotation.Y += 1 * dt;
            rotation.Z -= 4 * dt;
            transforComponent.Rotation = rotation;

            Vector3 position = transforComponent.Position;
            position.X = transforComponent.InitPosition.X + 5 * (float)Math.Sin(sceneManager.time);
            position.Z = transforComponent.InitPosition.Z + 5 * (float)Math.Cos(sceneManager.time);
            transforComponent.Position = position;

        }
        private void AnimateSpiky2(float dt)
        {
            Entity entity = entityManager.FindEntity("SpikyBall_2");
            ComponentTransform transforComponent = (ComponentTransform)entity.Components.Find(c => c.ComponentType == ComponentTypes.COMPONENT_TRANSFORM);

            //Animate;
            Vector3 rotation = transforComponent.Rotation;
            rotation.Y += 1 * dt;
            rotation.Z += 4 * dt;
            transforComponent.Rotation = rotation;

            float offset = 2f;
            Vector3 position = transforComponent.Position;
            position.X = transforComponent.InitPosition.X + 4 * (float)Math.Sin( 2 * sceneManager.time - offset);
            position.Z = transforComponent.InitPosition.Z + 4 * (float)Math.Cos(sceneManager.time - offset);
            position.Y = transforComponent.InitPosition.Y + Math.Abs(2 * (float)Math.Sin( 5 * sceneManager.time));
            transforComponent.Position = position;
        }
        private void AlterShipSound() {
            Entity entity = entityManager.FindEntity("Landing_Ship");
            ComponentAudio audio = (ComponentAudio)entity.Components.Find(c => c.ComponentType == ComponentTypes.COMPONENT_AUDIO);
            audio.SourcePitch = 2;
        }
    }
}
