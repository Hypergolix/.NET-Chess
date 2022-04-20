using System;
using System.Collections.Generic;
using System.Text;

namespace _2DChessCore.Models
{
    public enum PieceType
    {
        Empty,
        PawnWhite,
        KnightWhite,
        BishopWhite,
        RookWhite,
        QueenWhite,
        KingWhite
    }
    public class ChessPiece
    {

    }
    public class ChessBoard
    {
        public PieceType[,] Squares { get; set; }

        public ChessBoard()
        {
            Squares = new PieceType[8, 8];
            for(uint i = 0; i < 8; i++)
            {
                Squares[6, i] = PieceType.PawnWhite;
            }
        }

        public void DrawBoard()
        {
            for (uint i = 0; i < 8; i++)
            {
                for (uint j = 0; j < 8; j++)
                {
                    char SymbolToPrint;
                    switch(Squares[i, j])
                    {
                        case PieceType.PawnWhite:
                            SymbolToPrint = 'P';
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
        public bool MovePiece(uint from, uint to)
        {


            return true;
        }
    }
}
