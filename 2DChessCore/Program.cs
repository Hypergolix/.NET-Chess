using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // window.MouseMoved += OnMouseClick;

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

            Sprite Board = new Sprite(BoardTexture);
            Board.Scale = new Vector2f(0.25f, 0.25f);

            // Pieces setup
            List<Sprite> Pieces = new List<Sprite>();

            // Pawns
            Image PawnImage = new Image($"{parentDirectory}/Assets/Pieces/w_pawn_1x_ns.png");

            // PawnImage H: 293
            // PawnImage W: 239

            Texture PawnTexture = new Texture(451, 451);

            Vector2f PawnOffset = new Vector2f(PawnTexture.Size.X - PawnImage.Size.X, PawnTexture.Size.Y - PawnImage.Size.Y);
            PawnOffset.X = PawnOffset.X / 2f;
            PawnOffset.Y = PawnOffset.Y / 2f;

            // Remember to scale down sprite same as board

            PawnTexture.Update(PawnImage, (uint)PawnOffset.X, (uint)PawnOffset.Y);
            PawnTexture.Smooth = true;

            //Pawn.Origin = new Vector2f(Pawn.GetGlobalBounds().Width / 2, Pawn.GetGlobalBounds().Height / 2);
           
            // Make sprite bigger? take up entire square and THEN center texture 
            //Pawn.Position = new Vector2f(Pawn.Position.X + (Pawn.GetGlobalBounds().Width / 2), Pawn.Position.Y + (Pawn.GetGlobalBounds().Height / 2));

            // Avoid unessesary duplicate, right now we only need position to differ
            Sprite[] PawnArray = new Sprite[8];
            for (uint i = 0; i < 8; i++)
            {
                Sprite Pawn = new Sprite(PawnTexture);
                Pawn.Scale = new Vector2f(0.25f, 0.25f);
                Pawn.Position = new Vector2f(i * 112.5f, 0);
                PawnArray[i] = Pawn;
            }

            Pieces.AddRange(PawnArray);

            while (window.IsOpen)
            {
                // Render pieces based on logical chess board
                //for (uint i = 0; i < 8; i++)
                //{
                //    for (uint j = 0; j < 8; j++)
                //    {
                //        switch (ChessBoard.Squares[i, j])
                //        {
                //            case PieceType.PawnWhite:
                                
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //}

                // Capture and clamp mouse - could be moved inside function below
                Vector2i MousePosition = Mouse.GetPosition(window);
                Vector2u UnsignedMousePosition;

                if (MousePosition.X < 0) UnsignedMousePosition.X = 0;

                else if (MousePosition.X > WindowWidth) UnsignedMousePosition.X = WindowWidth;

                else UnsignedMousePosition.X = (uint)MousePosition.X;

                if (MousePosition.Y < 0) UnsignedMousePosition.Y = 0;

                else if (MousePosition.Y > WindowHeight) UnsignedMousePosition.Y = WindowHeight;

                else UnsignedMousePosition.Y = (uint)MousePosition.Y;
                //

                if (LeftMouseIsPressed)
                {
                    foreach (var Piece in Pieces)
                    {
                        if (Piece.GetGlobalBounds().Contains(UnsignedMousePosition.X, UnsignedMousePosition.Y))
                        {

                        }
                    }
                }

                window.Clear(new Color(255, 255, 255, 255));
                // Should this be at the top?
                window.DispatchEvents();
                window.Draw(Board);
                foreach (var Piece in Pieces)
                {
                    window.Draw(Piece);
                }
                window.Display();

                // Console 
                Console.Clear();
                Console.WriteLine($"Mouse X: {UnsignedMousePosition.X.ToString()}");
                Console.WriteLine($"Mouse Y: {UnsignedMousePosition.Y.ToString()}");
                //Console.WriteLine($"Pawn W: {Pawn.GetGlobalBounds().Width}");
                //Console.WriteLine($"Pawn H: {Pawn.GetGlobalBounds().Height}");
                Console.WriteLine($"Board W: {Board.GetGlobalBounds().Width}");
                ChessBoard.DrawBoard();
                // Console 
            }
        }
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
            if (e.Button == Mouse.Button.Left)
            {
                LeftMouseIsPressed = Mouse.IsButtonPressed(Mouse.Button.Left);
            }
        }
    }
}