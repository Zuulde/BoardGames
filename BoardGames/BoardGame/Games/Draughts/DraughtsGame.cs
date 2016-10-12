using System;
using System.Collections.Generic;
using BoardGames.Resources;

namespace BoardGames.Games.Draughts
{
    internal class Draughts : BoardGame
    {
        protected int RowsToBeFilled = 4;

        public Draughts()
            : base(new GamePiece[10, 10])
        {
        }

        public Draughts(GamePiece[,] board)
            : base(board)
        {
        }

        public override string GameName
        {
            get { return "Draughts"; }
        }

        public override bool IsGamePlayable
        {
            get { return true; }
        }

        protected override int NumberOfPlayers
        {
            get { return 2; }
        }

        protected override int CommandParametersNo
        {
            get { return 2; }
        }

        protected override bool GameFinishesAfterWholeRound
        {
            get { return false; }
        }

        protected override bool IsSetupOk()
        {
            bool isOk = true;

            if (players.Count != 2)
            {
                isOk = false;
                UI.ShowMessage(String.Format(Strings.game_player_numbers, 2));
            }

            return isOk;
        }

        protected override string GetIntructions()
        {
            return Strings.draughts_instructions;
        }

        protected override void PrepareTable(List<Player> players)
        {
            players[0].BoardAlignment = PlayerBoardAlignment.lowindex;
            players[1].BoardAlignment = PlayerBoardAlignment.highindex;
            CreatePieces(players[0]);
            CreatePieces(players[1]);
        }

        protected override bool IsCommandValid(Player pCurrentPlayer, String[] pUserCommand)
        {
            Position from = ConvertNotationToPosition(pUserCommand[0]);
            Position to = ConvertNotationToPosition(pUserCommand[1]);

            Boolean isValid = true;

            if (from == null || to == null)
            {
                UI.ShowMessage(Strings.game_invalid_table_position);
                isValid = false;
            }
            else if (Board[from.XAbsolute, from.YAbsolute] == null)
            {
                UI.ShowMessage(Strings.input_cell_from_empty);
                isValid = false;
            }
            else if (pCurrentPlayer.ID != Board[from.XAbsolute, from.YAbsolute].Owner.ID)
            {
                UI.ShowMessage(Strings.input_cell_wrong_owner);
                isValid = false;
            }

            return isValid;
        }

        protected override GamePiece ConvertCharacterToGamePiece(string draughtsPieceType, Player currentPlayer)
        {
            DraughtsPiece dp = null;

            switch (draughtsPieceType.ToUpperInvariant())
            {
                case "M":
                    dp = new Man(currentPlayer, PlayerBoardAlignment.none);
                    break;
                case "K":
                    dp = new King(currentPlayer, PlayerBoardAlignment.none);
                    break;
                default:
                    dp = null;
                    UI.ShowMessage(String.Format(Strings.input_invalid_parameters, draughtsPieceType));
                    break;
            }
            return dp;
        }

        protected override Position ConvertNotationToPosition(string fromString)
        {
            if (fromString.Length != 2)
            {
                UI.ShowMessage(String.Format(Strings.input_invalid_parameters, fromString));
                return null;
            }

            fromString = fromString.ToUpperInvariant();
            char columnPos = fromString[0]; // letter
            char rowPos = fromString[1]; // number
            if (columnPos < 'A' || columnPos > (char)('A' + TableWidth) || rowPos < '1' || rowPos > (char)('1' + TableHeight))
                return null;

            Position pos = new Position(fromString[1] - '1', fromString[0] - 'A');

            return pos;
        }

        protected override Boolean DoTurnGameSpecific(Player currentPlayer, string[] userCommand)
        {
            Boolean continueTurn = true;

            Position from = ConvertNotationToPosition(userCommand[0]);
            Position to = ConvertNotationToPosition(userCommand[1]);

            if (!IsPositionExist(to))
            {
                UI.ShowMessage(Strings.wrong_position_does_not_exist);
                return continueTurn;
            }

            DraughtsPiece dp = Board[from.XAbsolute, from.YAbsolute] as DraughtsPiece;

            if (!dp.CanMove(Board, from, to))
            {
                UI.ShowMessage(Strings.wrong_position_invalid);
                return continueTurn;
            }
            else
            {
                bool isMoveIsAttack = (Math.Abs(from.XAbsolute - to.XAbsolute) == 2) ? true : false;

                #region check if any piece can do a mandatory capture
                if (!isMoveIsAttack && CanPlayerAttack(currentPlayer))
                {
                    UI.ShowMessage(Strings.draughts_do_turn_jump);
                    return continueTurn;
                }
                #endregion
                #region move piece and check if further captures are possible
                MovePiece(from, to);
                continueTurn = false;
                if (isMoveIsAttack)
                {
                    var capturedPiecePos = new Position((from.XAbsolute + to.XAbsolute) / 2, (from.YAbsolute + to.YAbsolute) / 2);
                    RemovePiece(capturedPiecePos);
                    //IMPROVE it bothers me a lot that the code is duplicatet with CanPlayerAttack
                    List<Position> checkPositions = createDraughtPieceMovementList(dp, to, currentPlayer);
                    foreach (var checkPos in checkPositions)
                    {
                        if (dp.CanAttack(Board, to, checkPos))
                        {
                            UI.ShowMessage(Strings.draughts_do_turn_jump);
                            continueTurn = true;
                        }
                    }
                }
                #endregion
                # region man final line check
                Man pieceMan = dp as Man;
                if (pieceMan != null)
                {
                    if (pieceMan.IsFinalLine(Board, to))
                    {
                        UI.ShowMessage(Strings.draughts_piece_promote);
                        King newPiece = new King(currentPlayer, PlayerBoardAlignment.none);
                        Board[to.XAbsolute, to.YAbsolute] = null;
                        Board[to.XAbsolute, to.YAbsolute] = newPiece;
                    }
                }
                #endregion
                #region check if game ended: all enemy pieces has been captured
                //IMPROVE use a variable for each player to store value and do not calculate it
                int enemyPiecesNo = GetNumberOfEnemyPieces(currentPlayer);
                if (enemyPiecesNo == 0)
                {
                    continueTurn = false;
                    isGameGoing = false;
                    UI.ShowMessage(Strings.draughts_end_enemy_killed);
                }
                #endregion

                return continueTurn;
            }
        }

        protected void CreatePieces(Player pPlayer, bool createKings = false)
        {
            var orientation = pPlayer.BoardAlignment;
            int direction = +1;
            int startRow = 0;
            if (orientation == PlayerBoardAlignment.highindex)
            {
                direction = -1;
                startRow = TableHeight - 1;
            }
            for (int i = 0; i < TableWidth; i++)
            {
                int rowCounter = 1;
                for (int j = startRow; rowCounter <= RowsToBeFilled; j += direction, rowCounter++)
                {
                    if (BoardGame.IsBlackCell(i, j))
                    {
                        if (createKings)
                            Board[j, i] = new King(pPlayer, orientation);
                        else
                            Board[j, i] = new Man(pPlayer, orientation);
                    }
                }
            }
        }

        protected int GetNumberOfEnemyPieces(Player currentPlayer)
        {
            int noOfPieces = 0;

            for (int i = 0; i < TableWidth; i++)
            {
                for (int j = 0; j < TableHeight; j++)
                {
                    if (Board[j, i] != null && Board[j, i].Owner != currentPlayer)
                    {
                        noOfPieces++;
                    }
                }
            }

            return noOfPieces;
        }

        protected bool CanPlayerAttack(Player currentPlayer)
        {
            bool canAttack = false;

            for (int y = 0; y < TableWidth; y++)
            {
                for (int x = 0; x < TableHeight; x++)
                {
                    var dp = Board[x, y] as DraughtsPiece;
                    if (dp != null && dp.Owner == currentPlayer)
                    {
                        Position fromPos = new Position(x, y);
                        List<Position> toPoses = createDraughtPieceMovementList(dp, fromPos, currentPlayer);
                        foreach (var checkPos in toPoses)
                        {
                            if (dp.CanAttack(Board, fromPos, checkPos))
                            {
                                canAttack = true;
                                break;
                            }
                        }
                    }
                }
            }

            return canAttack;
        }

        protected List<Position> createDraughtPieceMovementList(DraughtsPiece dp, Position fromPos, Player player)
        {
            List<Position> positions = new List<Position>();
            if (dp is King)
            {
                positions.Add(new Position(fromPos.XAbsolute - 2, fromPos.YAbsolute - 2));
                positions.Add(new Position(fromPos.XAbsolute - 2, fromPos.YAbsolute + 2));
                positions.Add(new Position(fromPos.XAbsolute + 2, fromPos.YAbsolute - 2));
                positions.Add(new Position(fromPos.XAbsolute + 2, fromPos.YAbsolute + 2));
            }
            else
            {
                int direction = 1;
                if (player.BoardAlignment == PlayerBoardAlignment.highindex)
                {
                    direction = -1;
                }
                positions.Add(new Position(fromPos.XAbsolute + direction * 2, fromPos.YAbsolute - 2));
                positions.Add(new Position(fromPos.XAbsolute + direction * 2, fromPos.YAbsolute + 2));
            }

            for (int index = positions.Count - 1; index >= 0; index--)
            {
                if (!IsPositionExist(positions[index]))
                {
                    positions.RemoveAt(index);
                }
            }

            return positions;
        }
    }
}
