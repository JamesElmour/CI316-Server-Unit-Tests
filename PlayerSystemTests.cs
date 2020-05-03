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
    public class PlayerSystemTests
    {
        [TestMethod]
        public void ValidPosition_AfterPlayerProcess()
        {
            Player player = CreatePlayer("Test_1_Player_1");
            PlayerSystem system = CreateSystem();
            player.Speed = 200;
            system.Add(player);

            // Move left test.
            player.Parent.Position.x = 500;
            player.Direction = 2;
            system.Update(0.5f);
            Assert.AreEqual(player.Parent.Position.x, 600, 0.001, "Player post-movement coordinates as predicted");

            // Move right test.
            player.Direction = 0;
            system.Update(0.5f);
            Assert.AreEqual(player.Parent.Position.x, 500, 0.001, "Player post-movement coordinates as predicted");

            // Limit right position to 1280 test.
            player.Parent.Position.x = 2000;
            system.Update(0.5f);
            Assert.AreEqual(player.Parent.Position.x, 1280, 0.001, "Player position not being moved within valid boundries.");

            // Limit left position to 0 test.
            player.Parent.Position.x = -500;
            system.Update(0.5f);
            Assert.AreEqual(player.Parent.Position.x, 0, 0.001, "Player position not being moved within valid boundries.");
        }

        [TestMethod]
        public void Player_DirectionIsCapped()
        {
            Player player = CreatePlayer("Test_2_Player_1");
            PlayerSystem system = CreateSystem();
            system.Add(player);

            player.Direction = 5;
            system.Update(0.5f);
            Assert.AreEqual(player.Direction, 2, 0.001, "Player direction upper bound not corrected to 2.");
        }

        [TestMethod]
        public void AddPlayer_SuccessfullyAdded()
        {
            Player playerOne = CreatePlayer("Test_3_Player_1");
            PlayerSystem system = CreateSystem();
            system.Add(playerOne);

            Assert.AreEqual(system.Get("Test_3_Player_1"), playerOne, "Single component not correctly inserted into the system.");
            Assert.AreEqual(system.Count(), 1, "Single component not correctly inserted into the system.");
        }

        [TestMethod]
        public void AddMultiplePlayers_SuccessfullyAdded()
        {
            Player playerOne = CreatePlayer("Test_4_Player_1");
            Player playerTwo = CreatePlayer("Test_4_Player_2");
            Player playerThree = CreatePlayer("Test_4_Player_3");

            PlayerSystem system = CreateSystem();
            List<Player> players = new List<Player>()
            {
                playerOne,
                playerTwo,
                playerThree
            };

            system.Add(players);

            Assert.AreEqual(system.Get("Test_4_Player_1"), playerOne, "Multiple components not correctly inserted into the system.");
            Assert.AreEqual(system.Get("Test_4_Player_2"), playerTwo, "Multiple components not correctly inserted into the system.");
            Assert.AreEqual(system.Get("Test_4_Player_3"), playerThree, "Multiple components not correctly inserted into the system.");
            Assert.AreEqual(system.Count(), 3, "Multiple components not correctly inserted into the system.");
        }

        private Player CreatePlayer(string name = "")
        {
            GameEntity entity = new GameEntity(name);
            return new Player(entity);
        }

        private PlayerSystem CreateSystem()
        {
            SubWorld world = new BreakoutWorld(0);
            return new PlayerSystem(world);
        }
    }
}
