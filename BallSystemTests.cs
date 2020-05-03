using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIGMServer.Game;
using PIGMServer.Game.Components;
using PIGMServer.Game.Systems;
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
            Ball player = CreatePlayer("Test_1_Ball_1");
            BallSystem system = CreateSystem();
            system.Add(player);

            player.Direction = 5;
            system.Update(0.5f);
            Assert.AreEqual(player.Direction, 2, 0.001, "Player direction upper bound not corrected to 2.");
        }

        private Ball CreatePlayer(string name = "")
        {
            GameEntity entity = new GameEntity(name);
            return new Ball(entity);
        }

        private BallSystem CreateSystem()
        {
            SubWorld world = new BreakoutWorld(0);
            return new BallSystem(world);
        }
    }
}
