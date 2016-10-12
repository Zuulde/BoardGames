using System;
using BoardGames;

namespace ConsoleUI
{
    public class ConsoleInterface : IUserInterface
    {
        //unnecessary as data is directly read and processed from console
        public void SetInputReady(string input, bool inputStatus)
        {
           ;
        }


        public void ShowMessage(String msg)
        {
            Console.WriteLine(msg);
        }

        public string ReadInput()
        {
            String input;
            do
            {
                input = Console.ReadLine();
            } while (String.IsNullOrWhiteSpace(input)); 
            input = input.Substring(0, Math.Min(input.Length, Consts.PLAYER_NAME_MAX_LENGTH)); //IMPROVE not for all inputs!
            return input;
        }

        public void ShowBoard(GamePiece[,] pTable)
        {
            int rows = pTable.GetLength(1);
            int columns = pTable.GetLength(0);
            //IMPROVE cellwidth calculation - content is not correct
            int cellWidth = 4;
            int cellHeight = 2;

#if false            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (pTable[x, y] == null) Console.Write("__ ");
                    else Console.Write("{0}{1} ", pTable[x, y].OwnerNameOneLetter, pTable[x, y].PieceNameOneLetter);
                }
                Console.WriteLine();
            }
#endif            

            //IMPROVE: optimize code a bit
            DrawColumnIdentifiers(columns, cellWidth);
            for (int drawRow = 0; drawRow <= rows * cellHeight; drawRow++)
            {
                int row = (drawRow - 1) / cellHeight;
                if (drawRow % cellHeight == 0)
                {
                    DrawRowIdentifier(false, row + 1);
                    DrawHorizontalSectionA(columns, cellWidth);
                }
#if false
                if (row % cellHeight == 1 || row % cellHeight == 3)
                {
                    DrawHorizontalSectionB(columns, cellWidth);
                }
#endif
                if (drawRow % cellHeight == cellHeight / 2)
                {
                    DrawRowIdentifier(true, row + 1);
                    DrawHorizontalSectionFigures(columns, cellWidth, drawRow, cellHeight, pTable);
                    DrawRowIdentifier(true, row + 1);
                    Console.WriteLine();
                }
            }
            DrawColumnIdentifiers(columns, cellWidth);

        }

        private void DrawHorizontalSectionA(int columns, int cellWidth)
        {
            for (int column = 0; column <= columns * cellWidth; column++)
            {
                if (column % cellWidth == 0) Console.Write(Constants.LINE_INTERSECTION);
                else Console.Write(Constants.LINE_HORIZONTAL);
            }
            Console.WriteLine();
        }

        private void DrawHorizontalSectionB(int columns, int cellWidth)
        {
            for (int column = 0; column <= columns * cellWidth; column++)
            {
                if (column % cellWidth == 0) Console.Write(Constants.LINE_VERTICAL);
                else if (column % cellWidth == 1) Console.Write("    ");
            }
            Console.WriteLine();
        }

        private void DrawHorizontalSectionFigures(int columns, int cellWidth, int row, int cellHeight, GamePiece[,] pTable)
        {
            //IMPROVE bla bla bla. do not hardcode colors
            ConsoleColor player1 = ConsoleColor.DarkBlue;
            ConsoleColor player2 = ConsoleColor.DarkRed;

            for (int column = 0; column <= columns * cellWidth; column++)
            {
                if (column % cellWidth == 0) Console.Write(Constants.LINE_VERTICAL);
                else if (column % cellWidth == 1)
                {
                    int xPos = (row - 1) / cellHeight;
                    int yPos = (column - 1) / cellWidth;
                    ConsoleColor backgroundColor = ConsoleColor.Black;
                    if (pTable[xPos, yPos] != null)
                    {
                        backgroundColor = player1;
                        if (pTable[xPos, yPos].BoardAlignment == PlayerBoardAlignment.highindex) backgroundColor = player2;
                        Console.BackgroundColor = backgroundColor;
                        Console.Write("{0, " + (cellWidth - 2) + "} ", pTable[xPos, yPos].PieceNameOneLetter);
                        Console.ResetColor();
                    }
                    else
                    {
                        if (!BoardGame.IsBlackCell(xPos, yPos)) backgroundColor = ConsoleColor.White;
                        Console.BackgroundColor = backgroundColor;
                        Console.Write("{0, " + (cellWidth - 1) + "}", "");
                        Console.ResetColor();
                    }
                }
            }
        }

        private void DrawColumnIdentifiers(int columns, int cellWidth)
        {
            Console.Write(" ");
            for (int column = 0; column < columns * cellWidth; column++)
            {
                if (column % cellWidth == 0)
                {
                    int yPos = column / cellWidth;
                    Console.Write(" {0, " + (cellWidth - 1) + "}", (char)((int)'A' + yPos));
                }
            }
            Console.WriteLine();
        }

        private void DrawRowIdentifier(bool drawIdentifier, int row)
        {
            if (drawIdentifier) Console.Write(row.ToString().PadLeft(2));
            else Console.Write("  ");

        }
    }
}
