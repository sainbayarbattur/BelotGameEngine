﻿namespace Belot.Engine.GameMechanics
{
    using System.Collections.Generic;

    using Belot.Engine.Cards;
    using Belot.Engine.Game;
    using Belot.Engine.Players;

    public class ValidAnnouncesService
    {
        public bool IsBeloteAllowed(CardCollection playerCards, BidType contract, IList<PlayCardAction> currentTrickActions, Card playedCard)
        {
            if (playedCard.Type != CardType.Queen && playedCard.Type != CardType.King)
            {
                return false;
            }

            if (contract.HasFlag(BidType.NoTrumps))
            {
                return false;
            }

            if (contract.HasFlag(BidType.AllTrumps))
            {
                if (currentTrickActions.Count > 0 && currentTrickActions[0].Card.Suit != playedCard.Suit)
                {
                    // Belote is only allowed when playing card from the same suit as the first card played
                    return false;
                }
            }
            else
            {
                // Clubs, Diamonds, Hearts or Spades
                if (playedCard.Suit != contract.ToCardSuit())
                {
                    // Belote is only allowed when playing card from the trump suit
                    return false;
                }
            }

            if (playedCard.Type == CardType.Queen)
            {
                return playerCards.Contains(Card.GetCard(playedCard.Suit, CardType.King));
            }

            if (playedCard.Type == CardType.King)
            {
                return playerCards.Contains(Card.GetCard(playedCard.Suit, CardType.Queen));
            }

            return false;
        }

        public ICollection<Announce> GetAvailableAnnounces(CardCollection playerCards)
        {
            var cards = new CardCollection(playerCards);

            var combinations = new List<Announce>(2);
            FindFourOfAKindAnnounces(cards, combinations);
            FindSequentialAnnounces(cards, combinations);
            return combinations;
        }

        private static void FindFourOfAKindAnnounces(CardCollection cards, ICollection<Announce> combinations)
        {
            // Group by type
            var countOfCardTypes = new int[8];
            foreach (var card in cards)
            {
                countOfCardTypes[(int)card.Type]++;
            }

            // Check each type
            for (var i = 0; i < 8; i++)
            {
                var cardType = (CardType)i;
                if (countOfCardTypes[i] != 4 || cardType == CardType.Seven || cardType == CardType.Eight)
                {
                    continue;
                }

                switch (cardType)
                {
                    case CardType.Jack:
                        combinations.Add(
                            new Announce(AnnounceType.FourJacks, Card.GetCard(CardSuit.Spade, cardType)));
                        break;
                    case CardType.Nine:
                        combinations.Add(
                            new Announce(AnnounceType.FourNines, Card.GetCard(CardSuit.Spade, cardType)));
                        break;
                    case CardType.Ace:
                    case CardType.King:
                    case CardType.Queen:
                    case CardType.Ten:
                        combinations.Add(
                            new Announce(AnnounceType.FourOfAKind, Card.GetCard(CardSuit.Spade, cardType)));
                        break;
                }

                // Remove these cards from the available combination cards
                foreach (var card in cards)
                {
                    if (card.Type == cardType)
                    {
                        cards.Remove(card);
                    }
                }
            }
        }

        private static void FindSequentialAnnounces(CardCollection cards, ICollection<Announce> combinations)
        {
            // Group by suit
            var cardsBySuit = new[] { new List<Card>(8), new List<Card>(8), new List<Card>(8), new List<Card>(8) };
            foreach (var card in cards)
            {
                cardsBySuit[(int)card.Suit].Add(card);
            }

            // Check each suit
            for (var suitIndex = 0; suitIndex < 4; suitIndex++)
            {
                var suitedCards = cardsBySuit[suitIndex];
                if (suitedCards.Count < 3)
                {
                    continue;
                }

                suitedCards.Sort((card, card1) => card.Type.CompareTo(card1.Type));
                var previousCardValue = (int)suitedCards[0].Type;
                var count = 1;
                for (var i = 1; i < suitedCards.Count; i++)
                {
                    if ((int)suitedCards[i].Type == previousCardValue + 1)
                    {
                        count++;
                    }
                    else
                    {
                        if (count == 3)
                        {
                            combinations.Add(new Announce(AnnounceType.Tierce, suitedCards[i - 1]));
                        }

                        if (count == 4)
                        {
                            combinations.Add(new Announce(AnnounceType.Quarte, suitedCards[i - 1]));
                        }

                        if (count >= 5)
                        {
                            combinations.Add(new Announce(AnnounceType.Quinte, suitedCards[i - 1]));
                        }

                        count = 1;
                    }

                    previousCardValue = (int)suitedCards[i].Type;
                }

                if (count == 3)
                {
                    combinations.Add(new Announce(AnnounceType.Tierce, suitedCards[suitedCards.Count - 1]));
                }

                if (count == 4)
                {
                    combinations.Add(new Announce(AnnounceType.Quarte, suitedCards[suitedCards.Count - 1]));
                }

                if (count >= 5)
                {
                    combinations.Add(new Announce(AnnounceType.Quinte, suitedCards[suitedCards.Count - 1]));
                }
            }
        }
    }
}