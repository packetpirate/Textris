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
            bool downKey = false;
            long score = 0L;

            while(!gameOver) {
                Console.Clear();
                bool landed = false;
                downKey = false;

                if(Console.KeyAvailable) {
                    ConsoleKeyInfo ck = Console.ReadKey();
                    switch(ck.Key) {
                        case ConsoleKey.UpArrow:
                            current.RotateCW(game.GameBoard);
                            break;
                        case ConsoleKey.LeftArrow:
                            current.Move(game.GameBoard, 0, -1);
                            break;
                        case ConsoleKey.RightArrow:
                            current.Move(game.GameBoard, 0, 1);
                            break;
                        case ConsoleKey.DownArrow:
                            downKey = true;
                            break;
                    }
                }

                if((ticks % ((downKey)?2:10)) == 0) {
                    landed = current.Move(game.GameBoard, 1, 0);
                }

                game.GameBoard.PrintBoard(current, landed, score);
                if(landed) {
                    current = Tetromino.Random();
                    game.GameBoard.CheckRows(ref score);
                }
                ticks++;

                Thread.Sleep(downKey?20:50);
            }
        }
    }
}
