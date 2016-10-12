using System;
using System.Collections.Generic;
using BoardGames.Resources;
namespace BoardGames.Games.Chess
{
    internal class ChessGame : BoardGame
    {
        public ChessGame()
            : base(new GamePiece[8, 8])
        {
        }

        public ChessGame(GamePiece[,] board)
            : base(board)
        {
        }

        public override String GameName
        {
            get { return "Chess"; }
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
            get { return 3; }
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
            return Strings.chess_command_instructions;
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
            GamePiece cp = ConvertCharacterToGamePiece(pUserCommand[0], pCurrentPlayer);
            Position from = ConvertNotationToPosition(pUserCommand[1]);
            Position to = ConvertNotationToPosition(pUserCommand[2]);

            Boolean isValid = true;

            if (cp == null || from == null || to == null)
            {
                UI.ShowMessage(Strings.game_invalid_table_position);
                isValid = false;
            }
            else if (Board[from.XAbsolute, from.YAbsolute] == null)
            {
                UI.ShowMessage(Strings.input_cell_from_empty);
                isValid = false;
            }
            else if (cp.GetType() != Board[from.XAbsolute, from.YAbsolute].GetType())
            {
                UI.ShowMessage(String.Format(
                    Strings.input_cell_wrong_piece_type, Board[from.XAbsolute, from.YAbsolute].PieceNameOneLetter));
                isValid = false;
            }
            else if (cp.Owner.ID != Board[from.XAbsolute, from.YAbsolute].Owner.ID)
            {
                UI.ShowMessage(Strings.input_cell_wrong_owner);
                isValid = false;
            }

            return isValid;
        }

        protected override GamePiece ConvertCharacterToGamePiece(string chessPieceType, Player currentPlayer)
        {
            ChessPiece cp = null;

            switch (chessPieceType.ToUpperInvariant())
            {
                case "P":
                    cp = new Pawn(currentPlayer, PlayerBoardAlignment.none);
                    break;
                case "N":
                    cp = new Knight(currentPlayer, PlayerBoardAlignment.none);
                    break;
                case "B":
                    cp = new Bishop(currentPlayer, PlayerBoardAlignment.none);
                    break;
                case "R":
                    cp = new Rook(currentPlayer, PlayerBoardAlignment.none);
                    break;
                case "Q":
                    cp = new Queen(currentPlayer, PlayerBoardAlignment.none);
                    break;
                case "K":
                    cp = new King(currentPlayer, PlayerBoardAlignment.none);
                    break;
                default:
                    cp = null;
                    UI.ShowMessage(String.Format(Strings.input_invalid_parameters, chessPieceType));
                    break;
            }
            return cp;
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

            ChessPiece cp = ConvertCharacterToGamePiece(userCommand[0], currentPlayer) as ChessPiece;
            Position from = ConvertNotationToPosition(userCommand[1]);
            Position to = ConvertNotationToPosition(userCommand[2]);

            if (!IsPositionExist(to))
            {
                UI.ShowMessage(Strings.wrong_position_does_not_exist);
                return continueTurn;
            }

            if (!cp.CanMove(Board, from, to))
            {
                UI.ShowMessage(Strings.wrong_position_invalid);
                return continueTurn;
            }
            else
            {
                //IMPROVE this should be in the king's canmove function.. this makes unnecessary checks
                #region king movement check inspection
                if (cp is King)
                {
                    //this is a castling
                    int horizontalMove = to.YAbsolute - from.YAbsolute;
                    bool checkTriggered = false;
                    if (Math.Abs(horizontalMove) == 2)
                    {
                        Position betweenPosition = new Position(to.XAbsolute, to.YAbsolute - horizontalMove / 2);
                        if (IsPositionTriggersCheck(betweenPosition, currentPlayer))
                        {
                            checkTriggered = true;
                        }
                    }
                    //final position
                    if (IsPositionTriggersCheck(to, currentPlayer))
                    {
                        checkTriggered = true;
                    }

                    if (checkTriggered)
                    {
                        UI.ShowMessage(Strings.chess_movement_invalid_check);
                        return continueTurn;
                    }
                }
                #endregion

                #region move piece AND inspect if our piece movement triggers our king fromPos be checked
                GamePiece sourcePosition = Board[from.XAbsolute, from.YAbsolute];
                GamePiece targetPosition = Board[to.XAbsolute, to.YAbsolute];

                MovePiece(from, to);
                Position ownKingPosition = FindKingPosition(currentPlayer, true);
                King ownKingPiece = Board[ownKingPosition.XAbsolute, ownKingPosition.YAbsolute] as King;
                if (IsPositionTriggersCheck(ownKingPosition, ownKingPiece.Owner))
                {
                    Board[from.XAbsolute, from.YAbsolute] = sourcePosition;
                    Board[to.XAbsolute, to.YAbsolute] = targetPosition;

                    UI.ShowMessage(Strings.chess_movement_invalid_check);
                    return continueTurn;
                }
                #endregion
                continueTurn = false;

                # region rook moved ~ castling
                Rook pieceRook = Board[to.XAbsolute, to.YAbsolute] as Rook;
                if (pieceRook != null) pieceRook.IsMoved = true;
                #endregion
                # region king moved ~ castling
                King pieceKing = Board[to.XAbsolute, to.YAbsolute] as King;
                if (pieceKing != null) pieceKing.CastlingAllowed = false;
                #endregion
                # region pawn final line check
                Pawn piecePawn = Board[to.XAbsolute, to.YAbsolute] as Pawn;
                if (piecePawn != null)
                {
                    if (piecePawn.IsFinalLine(Board, to))
                    {
                        UI.ShowMessage(Strings.chess_new_piece);
                        while (true)
                        {
                            String[] userCommandNewPiece = UI.ReadInput().Split(' ');
                            ChessPiece newPiece = ConvertCharacterToGamePiece(userCommandNewPiece[0], currentPlayer) as ChessPiece;
                            if (newPiece == null)
                            {
                                UI.ShowMessage(Strings.input_invalid_parameters);
                            }
                            else
                            {
                                Board[to.XAbsolute, to.YAbsolute] = null;
                                Board[to.XAbsolute, to.YAbsolute] = newPiece;
                                break;
                            }
                        }
                    }
                }
                #endregion
                #region check for check and checkmate
                //IMPROVE: could find a better approach
                //this is not good as if the king receives check and cannot move it is considered a checkmate. Albeit it is not sure...
                Position enemyKingPosition = FindKingPosition(currentPlayer, false);
                King enemyKingPiece = Board[enemyKingPosition.XAbsolute, enemyKingPosition.YAbsolute] as King;
                if (IsPositionTriggersCheck(enemyKingPosition, enemyKingPiece.Owner))
                {
                    enemyKingPiece.IsInCheck = true;
                    bool enemyKingCanMove = false;
                    List<Position> kingValidMovePositions = new List<Position>() { };
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            if (!(i == 0 && j == 0))
                                if (enemyKingPosition.XAbsolute + i >= 0 && enemyKingPosition.YAbsolute + j >= 0
                                    && enemyKingPosition.XAbsolute + i < TableHeight && enemyKingPosition.YAbsolute + j < TableWidth
                                    )
                                {
                                    Position toPosition = new Position(enemyKingPosition.XAbsolute + i, enemyKingPosition.YAbsolute + j);
                                    if (cp.CanMove(Board, enemyKingPosition, toPosition)) kingValidMovePositions.Add(toPosition);
                                }

                    foreach (Position toPosition in kingValidMovePositions)
                    {
                        enemyKingCanMove = !IsPositionTriggersCheck(enemyKingPosition, currentPlayer);
                        if (enemyKingCanMove) break;
                    }

                    if (!enemyKingCanMove)
                    {
                        isGameGoing = false;
                        UI.ShowMessage(Strings.chess_movement_checkmate);
                    }
                    else
                    {
                        UI.ShowMessage(Strings.chess_movement_check);
                    }
                }
                else
                {
                    enemyKingPiece.IsInCheck = false;
                }
                #endregion
                return continueTurn;
            }
        }



        public void CreatePieces(Player pPlayer)
        {
            int row;
            var orientation = pPlayer.BoardAlignment;
            if (orientation == PlayerBoardAlignment.lowindex) row = 1;
            else row = TableHeight - 2;
            for (int i = 0; i < TableWidth; i++)
            {
                Board[row, i] = new Pawn(pPlayer, orientation);
            }
            if (orientation == PlayerBoardAlignment.lowindex) row = 0;
            else row = TableHeight - 1;
            PutPiece(new Position(row, 0), new Rook(pPlayer, orientation));
            Board[row, TableWidth - 1] = new Rook(pPlayer, orientation);
            Board[row, 1] = new Knight(pPlayer, orientation);
            Board[row, TableWidth - 2] = new Knight(pPlayer, orientation);
            Board[row, 2] = new Bishop(pPlayer, orientation);
            Board[row, TableWidth - 3] = new Bishop(pPlayer, orientation);
            Board[row, 3] = new Queen(pPlayer, orientation);
            Board[row, 4] = new King(pPlayer, orientation);
        }

        //IMPROVE store king position so it would be unnecessary to search for it
        protected Position FindKingPosition(Player currentPlayer, Boolean searchForOwn)
        {
            Position kingPosition = null;

            for (int x = 0; x < TableHeight; x++)
            {
                for (int y = 0; y < TableWidth; y++)
                {
                    var currentPiece = Board[x, y];
                    if (currentPiece != null && currentPiece is King)
                    {
                        //IMPROVE this is not quite elegant
                        if (searchForOwn)
                        {
                            if (currentPiece.Owner == currentPlayer)
                            {
                                kingPosition = new Position(x, y);
                            }
                        }
                        else
                        {
                            if (currentPiece.Owner != currentPlayer)
                            {
                                kingPosition = new Position(x, y);
                            }
                        }
                    }
                }
            }

            return kingPosition;
        }

        public bool IsPositionTriggersCheck(Position checkPosition, Player currentPlayer)
        {
            bool triggersCheck = false;

            for (int y = 0; y < TableHeight; y++)
            {
                for (int x = 0; x < TableWidth; x++)
                {
                    var currentPiece = Board[x, y];
                    if (currentPiece != null && currentPiece.Owner != currentPlayer)
                    {
                        Position piecePos = new Position(x, y);
                        triggersCheck = currentPiece.CanMove(Board, piecePos, checkPosition);
                        if (triggersCheck) return triggersCheck;
                    }
                }
            }
            return triggersCheck;
        }
    }
}
