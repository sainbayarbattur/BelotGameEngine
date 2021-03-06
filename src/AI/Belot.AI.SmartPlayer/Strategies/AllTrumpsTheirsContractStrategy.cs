namespace Belot.AI.SmartPlayer.Strategies
{
    using System.Linq;

    using Belot.Engine.Cards;
    using Belot.Engine.Players;

    public class AllTrumpsTheirsContractStrategy : IPlayStrategy
    {
        public PlayCardAction PlayFirst(PlayerPlayCardContext context, CardCollection playedCards)
        {
            // Play card if it will surely win the trick
            var card = CardHelpers.GetCardThatSurelyWinsATrickInAllTrumps(
                context.AvailableCardsToPlay,
                context.MyCards,
                playedCards,
                1);
            if (card != null)
            {
                return new PlayCardAction(card);
            }

            // Play card of the same suit as one of my teammate's bids
            var teammate = context.MyPosition.GetTeammate();
            for (var i = 0; i < Card.AllSuits.Length; i++)
            {
                var cardSuit = Card.AllSuits[i];
                if (context.Bids.Any(x => x.Player == teammate && x.Type == cardSuit.ToBidType())
                    && context.AvailableCardsToPlay.HasAnyOfSuit(cardSuit))
                {
                    return new PlayCardAction(
                        context.AvailableCardsToPlay.Where(x => x.Suit == cardSuit).Lowest(x => x.TrumpOrder));
                }
            }

            for (var i = 0; i < Card.AllSuits.Length; i++)
            {
                var cardSuit = Card.AllSuits[i];
                if (context.AvailableCardsToPlay.Contains(Card.GetCard(cardSuit, CardType.Queen))
                    && context.AvailableCardsToPlay.Contains(Card.GetCard(cardSuit, CardType.King)))
                {
                    return new PlayCardAction(Card.GetCard(cardSuit, CardType.Queen), true);
                }
            }

            return new PlayCardAction(context.AvailableCardsToPlay.Lowest(x => x.TrumpOrder));
        }

        public PlayCardAction PlaySecond(PlayerPlayCardContext context, CardCollection playedCards)
        {
            var firstCardSuit = context.CurrentTrickActions[0].Card.Suit;
            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.Jack)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.Jack));
            }

            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.Nine))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Jack)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.Nine));
            }

            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.Ace))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Nine))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Jack)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.Ace));
            }

            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.Ten))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Ace))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Nine))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Jack)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.Ten));
            }

            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.King))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Ten))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Ace))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Nine))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Jack)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.King));
            }

            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.Queen))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.King))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Ten))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Ace))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Nine))
                && playedCards.Contains(Card.GetCard(firstCardSuit, CardType.Jack)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.Queen));
            }

            if (context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.Queen))
                && context.AvailableCardsToPlay.Contains(Card.GetCard(firstCardSuit, CardType.King)))
            {
                return new PlayCardAction(Card.GetCard(firstCardSuit, CardType.Queen), true);
            }

            return new PlayCardAction(context.AvailableCardsToPlay.Lowest(x => x.TrumpOrder));
        }

        public PlayCardAction PlayThird(PlayerPlayCardContext context, CardCollection playedCards, PlayerPosition trickWinner)
        {
            return this.PlaySecond(context, playedCards);
        }

        public PlayCardAction PlayFourth(PlayerPlayCardContext context, CardCollection playedCards, PlayerPosition trickWinner)
        {
            return this.PlaySecond(context, playedCards);
        }
    }
}
