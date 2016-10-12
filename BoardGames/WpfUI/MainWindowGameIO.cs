using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BoardGames;

namespace WpfUI
{
    public partial class MainWindow : Window
    {
        #region handling user pInputMessage
        private void UserInputEntered()
        {
            ui.SetInputReady(TextBox_userInput.Text, true);
            TextBox_userInput.Text = String.Empty;
        }
        #endregion

        #region handling game message output
        public void UpdateInfoArea(String message)
        {
            TextBox_MessagaArea.Text += message + Environment.NewLine;
        }
        #endregion

        #region show board and related stuff section
        //IMRPOVE restructure the whole DrawBoard and use wpf xaml and templates and databinding
        public void DrawBoard(GamePiece[,] pTable)
        {
            int rows = pTable.GetLength(1);
            int columns = pTable.GetLength(0);
            System.Windows.Controls.Image[,] figures = new System.Windows.Controls.Image[columns, rows];

            var gameArea = BoardUniformGrid;
            //IMPROVE this is a quite resource wasting approach
            gameArea.Children.Clear();

            DrawColumnIdentifiers(gameArea, columns);
            for (int row = 0; row < rows; row++)
            {
                DrawRowIdentifier(gameArea, row + 1);
                for (int col = 0; col < columns; col++)
                {
                    #region figure creation section
                    String figureColor = String.Empty;
                    String cellColor = BoardGame.IsBlackCell(row, col) ? "Black" : "White";
                    String figure = String.Empty;
                    String extension = ".png";
                    String figurepath = String.Empty;
                    //IMPROVE handle null image file paths
                    if (pTable[row, col] != null)
                    {
                        figureColor = (pTable[row, col].BoardAlignment == PlayerBoardAlignment.lowindex) ?
                            "White" : "Black";

                        figure = GetFigure(currentGame.GameName, pTable[row, col].PieceNameOneLetter);
                        figurepath = currentSkinPath + figureColor + figure + extension;
                    }
                    String backgroundPath = Constants.RESOURCES_DIR + cellColor + "Square" + extension;
                    System.Drawing.Image backgroundImage = System.Drawing.Image.FromFile(backgroundPath);
                    System.Drawing.Image figureImage;
                    if (!String.IsNullOrEmpty(figurepath)) figureImage = System.Drawing.Image.FromFile(figurepath);
                    else figureImage = backgroundImage;

                    System.Windows.Controls.Image chessFigure = new System.Windows.Controls.Image();
                    chessFigure.Source = MergeImages(backgroundImage, figureImage);
                    chessFigure.Stretch = System.Windows.Media.Stretch.Fill;
                    #endregion
                    gameArea.Children.Add(chessFigure);
                }
                DrawRowIdentifier(gameArea, row + 1);
            }
            DrawColumnIdentifiers(gameArea, columns);
        }

        private void DrawColumnIdentifiers(Panel gameArea, int pColulmns)
        {
            Viewbox identifierArea = new Viewbox();
            identifierArea.Stretch = Stretch.Fill;
            Label identifierText = new Label();
            identifierText.Content = "";
            identifierArea.Child = identifierText;
            gameArea.Children.Add(identifierArea);

            for (int col = 0; col < pColulmns; col++)
            {
                Viewbox identifierAreaCol = new Viewbox();
                identifierAreaCol.Stretch = Stretch.Fill;
                Label identifierTextCol = new Label();
                identifierTextCol.Content = (char)('A' + col);
                identifierAreaCol.Child = identifierTextCol;
                gameArea.Children.Add(identifierAreaCol);
            }

            Viewbox identifierArea2 = new Viewbox();
            identifierArea2.Stretch = Stretch.Fill;
            Label identifierText2 = new Label();
            identifierText2.Content = "";
            identifierArea2.Child = identifierText2;
            gameArea.Children.Add(identifierArea2);
        }

        private void DrawRowIdentifier(Panel gameArea, int pRow)
        {
            Viewbox identifierArea = new Viewbox();
            identifierArea.Stretch = Stretch.Fill;
            Label identifierText = new Label();
            identifierText.Content = pRow.ToString();
            identifierArea.Child = identifierText;
            gameArea.Children.Add(identifierArea);
        }

        private String GetFigure(String pCurrentGameName, String pieceName)
        {
            String retFigure = String.Empty;

            if (pCurrentGameName == "Chess")
            {
                switch (pieceName)
                {
                    case "P":
                        retFigure = "Pawn";
                        break;
                    case "N":
                        retFigure = "Knight";
                        break;
                    case "B":
                        retFigure = "Bishop";
                        break;
                    case "R":
                        retFigure = "Rook";
                        break;
                    case "Q":
                        retFigure = "Queen";
                        break;
                    case "K":
                        retFigure = "King";
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            else if (pCurrentGameName == "Draughts" || pCurrentGameName == "DraughtsMini")
            {
                switch (pieceName)
                {
                    case "M":
                        retFigure = "Man";
                        break;
                    case "K":
                        retFigure = "King";
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            return retFigure;
        }

        #region create figure visual - messy section
        private ImageSource MergeImages(System.Drawing.Image image1, System.Drawing.Image image2)
        {
            using (Bitmap bitmap = new Bitmap(image1.Width, image1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    //this ensures that the backgroundcolor is transparent
                    canvas.Clear(System.Drawing.Color.Transparent);
                    canvas.DrawImage(image1, 0, 0);
                    canvas.DrawImage(image2, 0, 0);
                    canvas.Save();

                    return Convert(bitmap);
                }
            }
        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image; // do not use 'using' as we want the pictures to be persisted
        }
        #endregion
        #endregion
    }
}
