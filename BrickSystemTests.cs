using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIGMServer.Game;
using PIGMServer.Game.Components;
using PIGMServer.Game.Systems;
using PIGMServer.Game.Types;
using PIGMServer.Game.Worlds;
using PIGMServer.Game.Worlds.Levels;
using System.Collections.Generic;

namespace PIGMServerTesting
{
    [TestClass]
    public class BrickSystemTests
    {
        [TestMethod]
        public void Brick_BeingHitByBall()
        {
            // Create brick and systems.
            BrickSystem system = CreateBrickSystem();
            Brick brick = CreateBrick(system.World, "Test_1_Brick_1");

            // Add ball collision to Brick collider.
            system.World.GetSystem<ColliderSystem>().Get(brick.Parent).Colliding = true;
            system.World.GetSystem<ColliderSystem>().Get(brick.Parent).CollidingWith.Add("Ball");

            // Process collision.
            system.Update(0.5f);

            // Check new health.
            Assert.AreEqual(brick.Health, 2, 0.001, "Brick not being hit by ball.");
        }

        [TestMethod]
        public void Brick_BeingDestroyedByBall()
        {
            // Create brick and systems.
            BrickSystem system = CreateBrickSystem();
            Brick brick = CreateBrick(system.World, "Test_2_Brick_1");

            // Add ball collision to Brick collider.
            system.World.GetSystem<ColliderSystem>().Get(brick.Parent).Colliding = true;
            system.World.GetSystem<ColliderSystem>().Get(brick.Parent).CollidingWith.Add("Ball");

            // 'Hit' brick with the ball 3 times.
            system.Update(0.5f);
            system.Update(0.5f);
            system.Update(0.5f);

            // Check health is 0 and parent entity is set to be destroyed.
            Assert.AreEqual(brick.Health, 0, 0.001, "Brick not being destroyed by ball.");
            Assert.AreEqual(brick.Parent.Destroy, true, "Brick entity not being destroyed by ball.");
        }

        // Create brick system and accompanying components.
        public BrickSystem CreateBrickSystem()
        {
            BreakoutWorld world = new BreakoutWorld(0);
            world.AddSystem(new ColliderSystem(world));

            BrickSystem brickSystem = new BrickSystem(world);
            world.AddSystem(brickSystem);

            return brickSystem;
        }

        // Create brick and accompanying components.
        private Brick CreateBrick(SubWorld world, string name = "")
        {
            GameEntity entity = new GameEntity(name);
            Brick brick = new Brick(entity, 96, 16);
            Collider collider = new Collider(entity, 96, 16, false);

            world.GetSystem<ColliderSystem>().Add(collider);
            world.GetSystem<BrickSystem>().Add(brick);

            return brick;
        }
    }
}
