using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DChessCore.Models
{
    public enum PieceType
    {
        Empty,
        //
        PawnWhite,
        KnightWhite,
        BishopWhite,
        RookWhite,
        QueenWhite,
        KingWhite,
        //
        PawnBlack,
        KnightBlack,
        BishopBlack,
        RookBlack,
        QueenBlack,
        KingBlack,
    }
    public class ChessPiece
    {

    }
    public class ChessBoard
    {
        private readonly bool RespectTurns = true;
        private uint[] CapturedPiecesCount;
        public char Turn { get; private set; }
        public PieceType[,] Squares { get; private set; }
        public PieceType[,] CapturedPieces { get; private set; }
        // One or one for each? No, use bitflags 
        private int[,] CheckMap { get; set; }

        public ChessBoard()
        {
            Turn = 'w';
            Squares = new PieceType[8, 8];
            CapturedPieces = new PieceType[2, 8];
            CapturedPiecesCount = new uint[2];
            for (uint i = 0; i < 8; i++)
            {
                Squares[6, i] = PieceType.PawnWhite;
                Squares[1, i] = PieceType.PawnBlack;
            }
            Squares[0, 4] = PieceType.KingBlack;
            Squares[7, 4] = PieceType.KingWhite;
        }

        public void DrawBoard()
        {
            for (uint i = 0; i < 8; i++)
            {
                for (uint j = 0; j < 8; j++)
                {
                    char SymbolToPrint;
                    switch (Squares[i, j])
                    {
                        case PieceType.PawnWhite:
                            SymbolToPrint = 'P';
                            break;
                        case PieceType.PawnBlack:
                            SymbolToPrint = 'B';
                            break;
                        default:
                            SymbolToPrint = '-';
                            break;
                    }
                    Console.Write(SymbolToPrint);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        // Does it need to return anything? update gfx from state
        // use relative mouse position for from and to, clamp x/y somewhere
        public bool MovePiece(Vector2u from, Vector2u to)
        {
            // Check valid first, by rules and if empty etc. add visual for valid moves
            PieceType PieceToMove = Squares[from.Y, from.X];
            if (PieceToMove == PieceType.Empty) return false;

            // Rule book methods? 
            // Like length of move, direction etc 

            // En passant 
            // Promotion

            if (Turn == 'w')
            {
                switch (Squares[from.Y, from.X])
                {
                    case PieceType.PawnWhite:
                        // Careful with some of these type casts
                        // Some of these could be recycled for b/w pieces, some are just negative nrs 
                        if ((int)from.X - (int)to.X > 1 ||
                            (int)from.X - (int)to.X < -1 ||
                            (int)from.Y - (int)to.Y != 1 &&
                            (int)from.Y - (int)to.Y != 2) return false;

                        // Initial 2 square move, remember to track this for en passant somehow
                        if ((int)from.Y - (int)to.Y == 2 && from.Y != 6) return false;

                        // Own piece collision
                        if (Squares[to.Y, to.X] < PieceType.PawnBlack && Squares[to.Y, to.X] != PieceType.Empty) return false;

                        // Capture
                        if (from.X - to.X != 0)
                        {
                            if (Squares[to.Y, to.X] == PieceType.Empty) return false;
                        }
                        else if (Squares[to.Y, to.X] != PieceType.Empty) return false;

                        break;
                    case PieceType.KingWhite:
                        if ((int)from.X - (int)to.X > 1 ||
                            (int)from.X - (int)to.X < -1 ||
                            (int)from.Y - (int)to.Y > 1 ||
                            (int)from.Y - (int)to.Y < -1) return false;
                        // Own piece collision
                        if (Squares[to.Y, to.X] < PieceType.PawnBlack && Squares[to.Y, to.X] != PieceType.Empty) return false;

                        break;
                    default:
                        return false;
                }
            }
            else
            {
                switch (Squares[from.Y, from.X])
                {
                    case PieceType.PawnBlack:
                        if ((int)from.X - (int)to.X > 1 ||
                            (int)from.X - (int)to.X < -1 ||
                            (int)from.Y - (int)to.Y != -1 &&
                            (int)from.Y - (int)to.Y != -2) return false;

                        if ((int)from.Y - (int)to.Y == -2 && from.Y != 1) return false;

                        // Own piece collision
                        if (Squares[to.Y, to.X] > PieceType.KingWhite && Squares[to.Y, to.X] != PieceType.Empty) return false;

                        // Capture
                        if (from.X - to.X != 0)
                        {
                            if (Squares[to.Y, to.X] == PieceType.Empty) return false;
                        }
                        else if (Squares[to.Y, to.X] != PieceType.Empty) return false;

                        break;
                    case PieceType.KingBlack:
                        if ((int)from.X - (int)to.X > 1 ||
                            (int)from.X - (int)to.X < -1 ||
                            (int)from.Y - (int)to.Y > 1 ||
                            (int)from.Y - (int)to.Y < -1) return false;
                        // Own piece collision
                        if (Squares[to.Y, to.X] > PieceType.KingWhite && Squares[to.Y, to.X] != PieceType.Empty) return false;

                        break;
                    default:
                        return false;
                }
            }
            // Keep track of captured pieces, 0 = w, 1 = b
            if (Squares[to.Y, to.X] != PieceType.Empty)
            {
                // Uhhhhh
                int index = Turn == 'w' ? 0 : 1;
                CapturedPieces[index, CapturedPiecesCount[index]] = Squares[to.Y, to.X];
                CapturedPiecesCount[index]++;
            }
            Squares[from.Y, from.X] = PieceType.Empty;
            Squares[to.Y, to.X] = PieceToMove;

            // Switch turn
            if (RespectTurns)
            {
                if (Turn == 'w') Turn = 'b';
                else Turn = 'w';
            }

            return true;
        }
    }
}
