using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using _2DChessCore.Models;

namespace _2DChessCore
{
    class Program
    {
        // Mouse
        static bool LeftMouseIsPressed = false;

        static void Main(string[] args)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string parentDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            const uint WindowWidth = 900;
            const uint WindowHeight = 900;

            RenderWindow window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), "Chess");
            window.SetActive();
            window.SetFramerateLimit(60);
            window.Closed += (_, __) => window.Close();
            window.KeyPressed += OnKeyPressed;
            window.MouseButtonPressed += OnMouseClick;
            window.MouseButtonReleased += OnMouseClick;

            // The square the mouse is currently in
            Vector2u ActiveSquare = new Vector2u();

            // Board setup
            ChessBoard ChessBoard = new ChessBoard();

            Image LightSquareImage = new Image($"{parentDirectory}/Assets/Board/square brown light_1x_ns.png");
            Image DarkSquareImage = new Image($"{parentDirectory}/Assets/Board/square brown dark_1x_ns.png");

            Console.WriteLine(LightSquareImage.Size);
            uint BoardWidth = LightSquareImage.Size.X * 8;
            uint BoardHeight = LightSquareImage.Size.Y * 8;

            Texture BoardTexture = new Texture(BoardWidth, BoardHeight);

            for (uint i = 0; i < 4 * 450; i += 450)
            {
                for (uint j = 0; j < 4 * 450; j += 450)
                {
                    BoardTexture.Update(LightSquareImage, j * 2, i * 2);
                    BoardTexture.Update(DarkSquareImage, 450 + j * 2, i * 2);
                    BoardTexture.Update(LightSquareImage, 450 + j * 2, 450 + i * 2);
                    BoardTexture.Update(DarkSquareImage, j * 2, 450 + i * 2);
                }
            }

            Sprite Board = new Sprite(BoardTexture)
            {
                Scale = new Vector2f(0.25f, 0.25f)
            };

            // Pieces setup
            bool PieceSelected = false;
            Vector2u MoveFrom = new Vector2u();
            //PieceType SelectedPiece = new PieceType();
            //SelectedPiece = PieceType.Empty;

            List<Sprite> Pieces = new List<Sprite>();

            // Pawns
            const uint SquareWidth = 451;
            const uint SquareHeight = 451;

            Image PawnImageWhite = new Image($"{parentDirectory}/Assets/Pieces/w_pawn_1x_ns.png");
            Image PawnImageBlack = new Image($"{parentDirectory}/Assets/Pieces/b_pawn_1x_ns.png");

            Vector2f PawnOffset = new Vector2f(SquareWidth - PawnImageWhite.Size.X, SquareWidth - PawnImageWhite.Size.Y);
            PawnOffset.X = PawnOffset.X / 2f;
            PawnOffset.Y = PawnOffset.Y / 2f;

            //
            Texture PawnTextureWhite = new Texture(SquareWidth, SquareHeight);
            PawnTextureWhite.Update(PawnImageWhite, (uint)PawnOffset.X, (uint)PawnOffset.Y);
            PawnTextureWhite.Smooth = true;

            // Black
            // double check this new Vector2f 
            PawnOffset = new Vector2f(SquareWidth - PawnImageBlack.Size.X, SquareWidth - PawnImageBlack.Size.Y);
            PawnOffset.X = PawnOffset.X / 2f;
            PawnOffset.Y = PawnOffset.Y / 2f;

            Texture PawnTextureBlack = new Texture(SquareWidth, SquareHeight);
            PawnTextureBlack.Update(PawnImageBlack, (uint)PawnOffset.X, (uint)PawnOffset.Y);
            PawnTextureBlack.Smooth = true;

            // Kings
            Image KingImageWhite = new Image($"{parentDirectory}/Assets/Pieces/w_king_1x_ns.png");
            Image KingImageBlack = new Image($"{parentDirectory}/Assets/Pieces/b_king_1x_ns.png");

            Vector2f KingOffset = new Vector2f(SquareWidth - KingImageWhite.Size.X, SquareWidth - KingImageWhite.Size.Y);
            KingOffset.X = KingOffset.X / 2f;
            KingOffset.Y = KingOffset.Y / 2f;

            Texture KingTextureWhite = new Texture(SquareWidth, SquareHeight);
            KingTextureWhite.Update(KingImageWhite, (uint)KingOffset.X, (uint)KingOffset.Y);
            KingTextureWhite.Smooth = true;

            Texture KingTextureBlack = new Texture(SquareWidth, SquareHeight);
            KingTextureBlack.Update(KingImageBlack, (uint)KingOffset.X, (uint)KingOffset.Y);
            KingTextureBlack.Smooth = true;

            // Avoid unessesary duplicate, right now we only need position to differ
            Sprite[] PieceArray = new Sprite[18];
            for (uint i = 0; i < 8; i++)
            {
                Sprite Pawn = new Sprite(PawnTextureWhite)
                {
                    Scale = new Vector2f(0.25f, 0.25f)
                };
                PieceArray[i] = Pawn;
            }
            for (uint i = 8; i < 16; i++)
            {
                Sprite Pawn = new Sprite(PawnTextureBlack)
                {
                    Scale = new Vector2f(0.25f, 0.25f)
                };
                PieceArray[i] = Pawn;
            }

            Sprite KingW = new Sprite(KingTextureWhite)
            {
                Scale = new Vector2f(0.25f, 0.25f)
            };
            PieceArray[16] = KingW;

            Sprite KingB = new Sprite(KingTextureBlack)
            {
                Scale = new Vector2f(0.25f, 0.25f)
            };
            PieceArray[17] = KingB;

            // Add all sprites
            Pieces.AddRange(PieceArray);
            
            Clock Clock = new Clock();
            // Make this only update frame each round
            // Remove nested IFs
            while (window.IsOpen)
            {
                float DeltaTime = Clock.Restart().AsSeconds();
                Console.WriteLine("FPS: " + (int)(1.0f / DeltaTime));
                // Events
                window.DispatchEvents();

                // Render pieces based on logical chess board - only needs to happen after a turn
                // Limit tick rate basically

                // Check order of checking for input and rendering based on that

                // Hotfix for removing pieces - could be repurposed to show captured pieces, scale them down etc
                foreach (var Piece in Pieces)
                {
                    Piece.Position = new Vector2f(-140.0f, -140.0f);
                }

                int PawnCounterW = 0;
                int PawnCounterB = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        switch (ChessBoard.Squares[i, j])
                        {
                            case PieceType.PawnWhite:
                                Pieces[PawnCounterW].Position = new Vector2f(j * 112.75f, i * 112.75f);
                                PawnCounterW++;
                                break;
                            case PieceType.PawnBlack:
                                // + 8 is the offset where those sprites start in the array
                                Pieces[PawnCounterB + 8].Position = new Vector2f(j * 112.75f, i * 112.75f);
                                PawnCounterB++;
                                break;
                            case PieceType.KingWhite:
                                Pieces[16].Position = new Vector2f(j * 112.75f, i * 112.75f);
                                break;
                            case PieceType.KingBlack:
                                Pieces[17].Position = new Vector2f(j * 112.75f, i * 112.75f);
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Capture and clamp mouse - could be moved inside function below
                Vector2i MousePosition = Mouse.GetPosition(window);
                Vector2u UnsignedMousePosition = HelperMethods.Clamp(MousePosition, WindowWidth, WindowHeight);
                //

                // Highlight selected piece - ADD
                if (LeftMouseIsPressed)
                {
                    // Make sure in bounds, right now if you click off screen it'll select a square along the 
                    // edges as its clamped - fix with outOfFocus or similar
                    // avoid making reduant valid checks?
                    ActiveSquare.X = (uint)(UnsignedMousePosition.X / 112.75f);
                    ActiveSquare.Y = (uint)(UnsignedMousePosition.Y / 112.75f);

                    if (PieceSelected)
                    {
                        ChessBoard.MovePiece(MoveFrom, new Vector2u(ActiveSquare.X, ActiveSquare.Y));
                        PieceSelected = false;
                        Console.WriteLine("PLACED");
                    }
                    else
                    {
                        // Check whether we are actually picking up a piece / a valid piece
                        if (ChessBoard.Squares[ActiveSquare.Y, ActiveSquare.X] != PieceType.Empty)
                        {
                            if ((ChessBoard.Turn == 'w' && ChessBoard.Squares[ActiveSquare.Y, ActiveSquare.X] < PieceType.PawnBlack) ||
                                (ChessBoard.Turn == 'b' && ChessBoard.Squares[ActiveSquare.Y, ActiveSquare.X] > PieceType.KingWhite))
                            {
                                MoveFrom = new Vector2u(ActiveSquare.X, ActiveSquare.Y);
                                PieceSelected = true;
                                Console.WriteLine("SELECTED");
                            }
                        }
                    }

                    LeftMouseIsPressed = false;

                    Console.WriteLine($"X: {ActiveSquare.X}");
                    Console.WriteLine($"Y: {ActiveSquare.Y}");
                    // used to be a foreach here
                }
                window.Clear(new Color(255, 255, 255, 255));
                window.Draw(Board);
                foreach (var Piece in Pieces)
                {
                    window.Draw(Piece);
                }
                window.Display();

                // Console 
                //Console.Clear();
                //Console.WriteLine($"Mouse X: {UnsignedMousePosition.X.ToString()}");
                //Console.WriteLine($"Mouse Y: {UnsignedMousePosition.Y.ToString()}");
                ////Console.WriteLine($"Pawn W: {Pawn.GetGlobalBounds().Width}");
                ////Console.WriteLine($"Pawn H: {Pawn.GetGlobalBounds().Height}");
                //Console.WriteLine($"Board W: {Board.GetGlobalBounds().Width}");
                //ChessBoard.DrawBoard();
                // Console 
            }
        }
        // Threads
        //private static void RotateSprite(Sprite sprite, float deltaTime)
        //{
        //    sprite.Rotation += 1 * deltaTime;
        //    Console.WriteLine("Called");
        //}

        // Events
        private static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
        }

        private static void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left && !LeftMouseIsPressed)
            {
                LeftMouseIsPressed = Mouse.IsButtonPressed(Mouse.Button.Left);
                //Console.WriteLine("PRESSED");
            }
        }
    }
}