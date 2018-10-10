﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;

namespace SpearSoft.Games.Dicey.GameEngine.Tests
{
    public class GameTests
    {
        [Test]
        public void GameInit_HasPlayer()
        {

            var game = new Game();

            Assert.IsTrue(game.Players!=null &&  game.Players.Count == 1,
                "Game constructor did not initilize a player.");

        }

        [Test]
        public void GameInit_HasPlayer_NamedDEFAULT()
        {

            var game = new Game();

            Assert.IsTrue(game.Players != null && game.Players[0].PlayerName == "DEFAULT",
                "Either there are no players or player is not named default");

        }

        [Test]
        public void GameInit_HasPlayer_GuidIDNotEmpty()
        {

            var game = new Game();

            Assert.IsTrue(game.Players != null && game.Players[0].Id != Guid.Empty,
                "Either there are no players or player is not named default");

        }

        [Test]
        public void GameInit_Player_HasGameCard()
        {

            var game = new Game();
            var player = game.Players[0];

            Assert.IsTrue(player.GameCard!=null, "Init did not create a game card for the player");
        }

        [Test]
        public void GameInit_Player_HasGameCardWithHands()
        {

            var game = new Game();
            var player = game.Players[0];

            Assert.IsTrue(player.GameCard != null && player.GameCard.Hands.Count == 14,"14 game cards should have been created on init.");
        }

        [Test]
        public void OnePlayer_NoHandApplied_CantIncrementRound()
        {
            var game = new Game();

            Assert.IsTrue(!game.IncrementCurrentRound() && game.CurrentRound==1);
        }

        [Test]
        public void TwoPlayer_2ndPlayerNoHandApplied_CantIncrementRound()
        {
            var players = new List<Player>();
            players.Add(new Player("PLAYER1", 1));
            players.Add(new Player("PLAYER2", 2));

            var game = new Game(players);

            var hand = game.CurrentPlayer.GameCard.Hands.SingleOrDefault(h => h.Name == GameCard.Aces);

            game.CurrentPlayer.GameCard.ApplyHand(hand);
            Assert.IsTrue(!game.IncrementCurrentRound() && game.CurrentRound == 1,"Not all players had a hand applied but you were still able to increment the round!");

        }

        [Test]
        public void OnePlayer_HandApplied_CanIncrementRound()
        {
            var game = new Game();

            var hand = game.CurrentPlayer.GameCard.Hands.SingleOrDefault(h => h.Name ==GameCard.Aces);
            game.CurrentPlayer.GameCard.ApplyHand(hand);

            Assert.IsTrue(game.IncrementCurrentRound() && game.CurrentRound==2,"IncrementCurrentRound should have return True and NewRound should be 2");
            
        }

        [Test]
        public void TwoPlayer_AllHaveHandApplied_CanIncrementRound()
        {
            var players = new List<Player>();
            players.Add(new Player("PLAYER1", 1));
            players.Add(new Player("PLAYER2", 2));

            var game = new Game(players);

            var hand = game.CurrentPlayer.GameCard.Hands.SingleOrDefault(h => h.Name == GameCard.Aces);
            var player2 = game.Players[1];
            var hand2 = player2.GameCard.Hands.SingleOrDefault(h => h.Name == GameCard.Twos);

            player2.GameCard.ApplyHand(hand2);

            game.CurrentPlayer.GameCard.ApplyHand(hand);
            Assert.IsTrue(game.IncrementCurrentRound() && game.CurrentRound == 2, "IncrementCurrentRound should have return True and NewRound should be 2");
        }

        [Test]
        public void OnePlayer_NoHandApplied_CannotGetNextPlayer()
        {
            var game = new Game();

            Assert.IsTrue(!game.GetNextPlayer() && game.CurrentRound == 1,"Next player should have returned false when no hand has been applied to current player.");
        }

        [Test]
        public void OnePlayer_HandApplied_CanGetNextPlayerAndRoundNotIncremented()
        {
            var game = new Game();

            var hand = game.CurrentPlayer.GameCard.Hands.SingleOrDefault(h => h.Name == GameCard.Aces);
            game.CurrentPlayer.GameCard.ApplyHand(hand);

            Assert.IsTrue(game.GetNextPlayer() && game.CurrentRound == 1,"Unable to get next player even with hand applied but current round should have stayed at 1");
        }


        [Test]
        public void TwoPlayer_1stPlayerNoHandApplied_CantGetNextPlayer()
        {
            var players = new List<Player>();
            players.Add(new Player("PLAYER1", 1));
            players.Add(new Player("PLAYER2", 2));

            var game = new Game(players);

            Assert.IsTrue(!game.GetNextPlayer() && game.CurrentPlayer.PlayerName=="PLAYER1","GetNextPlayer should have return false or current player should have been PLAYER1");
        }

        [Test]
        public void TwoPlayer_1stPlayerHandApplied_CanGetNextPlayer()
        {
            var players = new List<Player>();
            players.Add(new Player("PLAYER1", 1));
            players.Add(new Player("PLAYER2", 2));

            var game = new Game(players);
            var hand = game.CurrentPlayer.GameCard.Hands.SingleOrDefault(h => h.Name == GameCard.Aces);

            game.CurrentPlayer.GameCard.ApplyHand(hand);

            Assert.IsTrue(game.GetNextPlayer() && game.CurrentPlayer.PlayerName == "PLAYER2","Unable to get next player even though 1st player had hand applied");
        }

        [Test]
        public void OnePlayer_HandApplied_CanIncrementRoundButSamePlayer()
        {
            //arrange

            var players = new List<Player>();
            players.Add(new Player("PLAYER1", 1));
            var game = new Game(players);

            var hand = game.CurrentPlayer.GameCard.Hands.SingleOrDefault(h => h.Name == GameCard.Aces);
            game.CurrentPlayer.GameCard.ApplyHand(hand);

            //act
            game.IncrementCurrentRound();

            //assert
            Assert.IsTrue(game.CurrentRound == 2 && game.CurrentPlayer.PlayerName== "PLAYER1", "Player should have been (still PLAYER1) and the 2nd round");

        }

    }
}