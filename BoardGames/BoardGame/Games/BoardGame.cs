using System;
using System.Collections.Generic;
using BoardGames.Resources;

namespace BoardGames
{
    public abstract class BoardGame
    {
        /// <summary>
        /// Initialize a table game with the given board
        /// </summary>
        /// <param name="pTable">Size of the game board</param>
        protected BoardGame(GamePiece[,] pTable)
        {
            board = pTable;
        }

        private IUserInterface ui; //current user interface
        /// <summary>
        /// User interface that shows visually the game to the user.
        /// </summary>
        /// <remarks>
        /// Already defined ui may not be overwritten.
        /// </remarks>
        public IUserInterface UI
        {
            get { return ui; }
            set
            {
                if (ui == null)
                {
                    ui = value;
                }
            }
        }

        //IMPROVE create a list for each player that holds a list for all the killed pieces
        //IMRPOVE create a list that contains all the current game's figures. Interfaces do not know what to implement.
        //still, hide all the internal details as much as possible

        /// <summary>
        /// The game's name. Should be unique.
        /// </summary>
        public abstract String GameName { get; }

        /// <summary>
        /// Indicates if the game is ready to be played.
        /// </summary>
        public abstract bool IsGamePlayable { get; }


        protected GamePiece[,] board; //UNSURE create abstract set property to force declaration
        public GamePiece[,] Board
        {
            get { return board; }
        }

        protected bool isGameGoing; //UNSURE: create property to force declaration
        /// <summary>
        /// Indicates if the game should be aborted immediately
        /// </summary>
        private bool doExitGame = false;

        protected abstract int CommandParametersNo { get; }
        /// <summary>
        /// Indicates if the game is over at the moment of first 'ending' move.
        /// </summary>
        /// <example>
        /// Some games ends if the whole turn is over and every player played its round.
        /// </example>
        protected abstract bool GameFinishesAfterWholeRound { get; }
        protected abstract int NumberOfPlayers { get; }
        protected List<Player> players = new List<Player>();

        /// <summary>
        /// columns
        /// </summary>
        public int TableWidth
        {
            get { return board.GetLength(0); }
        }
        /// <summary>
        /// rows
        /// </summary>
        public int TableHeight
        {
            get { return board.GetLength(1); }
        }

        private void Reset()
        {
            doExitGame = false;
            isGameGoing = false;
            board = new GamePiece[0, 0];
            players.Clear();
            UI.ShowBoard(Board);
        }

        //IMPROVE make these abstract as the board is not necessarely has pattern
        //however this makes it non static and introduces some problems...
        public static bool IsCellIsBlack(Position cellPosition)
        {
            return IsBlackCell(cellPosition.XAbsolute, cellPosition.YAbsolute);
        }

        public static bool IsBlackCell(int x, int y)
        {
            bool isBlack = true;

            if (x % 2 == 0)
            {
                if (y % 2 == 0) isBlack = true;
                else isBlack = false;
            }
            else
            {
                if (y % 2 == 1) isBlack = true;
                else isBlack = false;
            }

            return isBlack;
        }
#if false
        public bool AddPlayer(Player pPlayer)
        {
            bool isSuccess = false;
            //IMPROVE make some conditions and checking
            players.Add(pPlayer);
            return isSuccess;
        }

        public bool RemovePlayer(Player pPlayer)
        {
            bool isSuccess = false;
            //IMPROVE make some conditions and checking
            players.Remove(pPlayer);
            return isSuccess;
        }
#endif
        /// <summary>
        /// Core method of the game. This runs the actual game.
        /// </summary>
        public void Play()
        {
            int playerNo = 1;
            while (playerNo <= NumberOfPlayers)
            {
                UI.ShowMessage(String.Format(Strings.player_give_name, (NumbersOrders)playerNo));
                players.Add(new Player(UI.ReadInput()));
                playerNo++;
            }

            if (!IsSetupOk())
            {
                UI.ShowMessage(Strings.game_cannot_start);
                return;
            }
            UI.ShowMessage(Strings.game_is_starting);
            PrepareTable(players);
            UI.ShowMessage(Strings.game_has_started);
            UI.ShowMessage(GetIntructions());
            UI.ShowMessage(Strings.game_commands);
            isGameGoing = true;
            //this is one round (everyone played his/her/its turn)
            while (isGameGoing)
            {
                foreach (var player in players)
                {
                    UI.ShowBoard(Board); //if necessary
                    UI.ShowMessage(String.Format(Strings.player_turn, player.Name));
                    DoTurnGeneral(player);
                    if (doExitGame)
                    {
                        Reset();
                        UI.ShowMessage(Strings.game_exit);
                        break;
                    }
                    if (!isGameGoing && !GameFinishesAfterWholeRound)
                    {
                        UI.ShowMessage(String.Format(Strings.player_game_win, player.Name));
                        break;
                    }
                }
            }
            UI.ShowMessage(Strings.game_has_ended);
        }

        protected void DoTurnGeneral(Player currentPlayer)
        {
            do
            {
                String userInput = UI.ReadInput();
                if (userInput == "quit" || userInput == "exit")
                {
                    doExitGame = true;
                    break;
                }
                String[] userCommand = userInput.Split(' ');
                if (userCommand.Length < CommandParametersNo)
                {
                    UI.ShowMessage(Strings.input_missing_parameters);
                    continue;
                }
                if (!IsCommandValid(currentPlayer, userCommand))
                {
                    //errors should be defined in overriden method
                    continue;
                }
                // until the player is finished with its turn
                if (DoTurnGameSpecific(currentPlayer, userCommand)) UI.ShowBoard(Board);
                else break;


            } while (true);
        }

        protected bool IsPositionExist(Position pPosition)
        {
            bool isExist = true;

            if (pPosition.XAbsolute >= TableHeight ||
                pPosition.YAbsolute >= TableWidth ||
                pPosition.XAbsolute < 0 ||
                pPosition.YAbsolute < 0
                )
            {
                isExist = false;
            }

            return isExist;
        }

        protected void MovePiece(Position from, Position to)
        {
            board[to.XAbsolute, to.YAbsolute] = board[from.XAbsolute, from.YAbsolute];
            board[from.XAbsolute, from.YAbsolute] = null;
        }

        protected void PutPiece(Position to, GamePiece piece)
        {
            board[to.XAbsolute, to.YAbsolute] = piece;
        }

        protected void RemovePiece(Position from)
        {
            board[from.XAbsolute, from.YAbsolute] = null;
        }

        protected abstract bool IsSetupOk();

        protected abstract string GetIntructions();

        protected abstract void PrepareTable(List<Player> players);

        protected abstract Boolean IsCommandValid(Player currentPlayer, String[] userCommand);

        protected abstract GamePiece ConvertCharacterToGamePiece(string chessPieceType, Player currentPlayer);

        protected abstract Position ConvertNotationToPosition(string fromString);

        protected abstract Boolean DoTurnGameSpecific(Player purrentPlayer, string[] userCommand);




    }
}
