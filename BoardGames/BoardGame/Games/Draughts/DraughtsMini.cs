using System;
using System.Collections.Generic;

namespace BoardGames.Games.Draughts
{
    internal class DraughtsMini : Draughts
    {
        public DraughtsMini()
            : base(new GamePiece[6, 6])
        {
            RowsToBeFilled = 2;
        }

        public override string GameName
        {
            get { return "DraughtsMini"; }
        }

        protected override void PrepareTable(List<Player> players)
        {
            players[0].BoardAlignment = PlayerBoardAlignment.lowindex;
            players[1].BoardAlignment = PlayerBoardAlignment.highindex;
            CreatePieces(players[0], true);
            CreatePieces(players[1], true);
        }
    }
}
