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
    public class BallSystemTests
    {
        [TestMethod]
        public void Ball_WallBounceTest()
        {
            // Create ball and systems.
            BallSystem system = CreateSystem();
            Ball ball = CreatePlayer(system.World, "Test_1_Ball_1");

            // Set ball offscreen.
            ball.Parent.Position = new Vector2(-100, 300);
            ball.Direction = new Vector2(80, 120);
            system.Update(0.5f);                                                                                     // Check position.
            Assert.AreEqual(ball.Direction.x, 120, 0.001, "Ball left bounce not resorting to correct direction.");   // Check bouce occured.

            // Set ball offscreen.
            ball.Parent.Position = new Vector2(1300, 300);
            system.Update(0.5f);                                                                                     // Check position.
            Assert.AreEqual(ball.Direction.x, 80, 0.001, "Ball right bounce not resorting to correct direction.");   // Check bouce occured.

            // Set ball offscreen.
            ball.Parent.Position = new Vector2(400, -5);
            system.Update(0.5f);                                                                                     // Check position.
            Assert.AreEqual(ball.Direction.y, 80, 0.001, "Ball top bounce not resorting to correct direction.");     // Check bouce occured.

            // Set ball offscreen.
            ball.Parent.Position = new Vector2(400, 800);
            system.Update(0.5f);                                                                                      // Check position.
            Assert.AreEqual(ball.Direction.y, 120, 0.001, "Ball bottom bounce not resorting to correct direction.");  // Check bouce occured.
        }

        [TestMethod]
        public void Ball_PlayerBounceTest()
        {
            // Create ball and systems.
            BallSystem system = CreateSystem();
            Ball ball = CreatePlayer(system.World, "Test_2_Ball_1");

            // Set up ball-player bounce.
            system.World.GetSystem<ColliderSystem>().Get(ball.Parent).Colliding = true;
            system.World.GetSystem<ColliderSystem>().Get(ball.Parent).CollidingWith.Add("Player");
            system.World.GetSystem<ColliderSystem>().Get(ball.Parent).CollidedComponents.Add(new Collider(new GameEntity("Player", ball.Parent.Position), 128, 16, true));

            // Process bounce.
            ball.Direction = new Vector2(80, 120);
            system.Update(0.5f);

            // Assert bounce sucessfully occured.
            Assert.AreEqual(ball.Direction.y, 120, 0.001, "Ball not bouncing correctly off player.");
        }

        [TestMethod]
        public void Ball_BrickBounceTest()
        {
            // Create ball and systems.
            BallSystem system = CreateSystem();
            system.World.AddSystem(new BrickSystem(system.World));
            Ball ball = CreatePlayer(system.World, "Test_3_Ball_1");
            Brick brick = CreateBrick(system.World, "Test_3_Brick_1");
            Collider collider = system.World.GetSystem<ColliderSystem>().Get(ball.Parent);

            // Update position for left bounce.
            brick.Parent.Position.x = 100;
            ball.Parent.Position = brick.Parent.Position;
            brick.Parent.Position.x -= 32;

            // Add ball-brick collision.
            collider.Colliding = true;
            collider.CollidingWith.Add("Brick");
            collider.CollidedComponents.Add(system.World.GetSystem<ColliderSystem>().Get(brick.Parent));
            system.World.GetSystem<BrickSystem>().Add(brick);

            // Perform collision.
            ball.Direction = new Vector2(120, 80);
            system.Update(0.5f);
            Assert.AreEqual(ball.Direction.x, 80, 0.001, "Ball not left bouncing off brick correctly.");        // Assert bounce occured.

            // Update position for right bounce.
            system.World.GetSystem<BallSystem>().Get(ball.Parent).Parent.Position.x += 100;
            ball.WasCollidingWithBrick = false;

            // Perform collision.
            system.Update(0.5f);
            Assert.AreEqual(ball.Direction.x, 120, 0.001, "Ball not right bouncing off brick correctly.");        // Assert bounce occured.

            // Update position for top bounce.
            system.World.GetSystem<BallSystem>().Get(ball.Parent).Parent.Position.y -= 8;
            system.World.GetSystem<BallSystem>().Get(ball.Parent).Parent.Position.x = (short)(brick.Parent.Position.x + 24);
            ball.WasCollidingWithBrick = false;

            // Perform collision.
            system.Update(0.5f);
            Assert.AreEqual(ball.Direction.y, 80, 0.001, "Ball not top bouncing off brick correctly.");            // Assert bounce occured.

            // Update position for bottom bounce.
            system.World.GetSystem<BallSystem>().Get(ball.Parent).Parent.Position.y += 24;
            ball.WasCollidingWithBrick = false;

            // Perform collision.
            system.Update(0.5f);
            Assert.AreEqual(ball.Direction.y, 120, 0.001, "Ball not bottom bouncing off brick correctly.");        // Assert bounce occured.
        }

        // Create player and accompanying components.
        private Ball CreatePlayer(SubWorld world, string name = "")
        {
            GameEntity entity = new GameEntity(name);
            Ball ball = new Ball(entity);
            Collider collider = new Collider(entity, 64, 64, true);

            world.GetSystem<BallSystem>().Add(ball);
            world.GetSystem<ColliderSystem>().Add(collider);
            return ball;
        }

        // Create player system and accompanying components.
        private BallSystem CreateSystem()
        {
            SubWorld world = new BreakoutWorld(0);
            BallSystem ballSystem = new BallSystem(world);
            ColliderSystem colliderSystem = new ColliderSystem(world);

            world.AddSystem(new PowerUpSystem(world));
            world.AddSystem(colliderSystem);
            world.AddSystem(ballSystem);

            return ballSystem;
        }
        
        // Create brick and accompanying components.
        private Brick CreateBrick(SubWorld world, string name = "")
        {
            GameEntity entity = new GameEntity(name);
            Brick brick = new Brick(entity, 96, 16);
            Collider collider = new Collider(entity, 96, 16, false);

            world.GetSystem<ColliderSystem>().Add(collider);

            return brick;
        }
    }
}
