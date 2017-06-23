using System;
using System.Threading;
using Tetris.Utils;

namespace Tetris {
    class Program {
        private Board gameBoard;
        public Board GameBoard {
            get => gameBoard;
            set => gameBoard = value;
        }

        static void Main(string[] args) {
            Program game = new Program();
            game.GameBoard = new Board();

            Tetromino current = Tetromino.Random();
            bool gameOver = false;
            int ticks = 0;

            while(!gameOver) {
                Console.Clear();
                bool landed = false;

                if(ticks == 10) {
                    landed = current.Move(game.GameBoard, 1, 0);
                    ticks = 0;
                }

                game.GameBoard.PrintBoard(current, landed);
                if(landed) current = Tetromino.Random();
                ticks++;
                Thread.Sleep(50);
            }
        }
    }
}
