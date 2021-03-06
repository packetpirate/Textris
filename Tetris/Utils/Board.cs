﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tetris.Utils {
    class Board {
        public const int WIDTH = 10;
        public const int HEIGHT = 20;

        private int[,] still;
        public int[,] Still => still;
        public int GetPlace(int row, int col) {
            if((row < 0) || (col < 0) || (row >= Board.HEIGHT) || (col >= Board.WIDTH)) return -1;
            return still[row, col];
        }
        public void SetPlace(int row, int col, int val) {
            if((row >= 0) || (col >= 0) || (row < Board.HEIGHT) || (col < Board.WIDTH)) {
                still[row, col] = val;
            }
        }

        public Board() {
            still = new int[Board.HEIGHT, Board.WIDTH];
        }

        /**
         * Adds the given Tetromino to the grid of "landed" Tetromino blocks.
         * @param t The Tetromino to "land" on the grid.
         **/
        public void AddPiece(Tetromino t) {
            int[,] state = t.GetState();

            for(int r = 0; r < state.GetLength(0); r++) {
                for(int c = 0; c < state.GetLength(1); c++) {
                    int tr = r, tc = c;
                    t.GetTransformed(ref tr, ref tc);
                    if(state[r,c] != 0) {
                        still[tr, tc] = t.Color;
                    }
                }
            }
        }

        /**
            * Checks each row to see if they are full, and if so, deletes the row and adds to score.
            * @param score A long representing the current score, passed as reference, so that it can be modified.
            **/
        public void CheckRows(ref long score) {
            int rowsCleared = 0;
            for(int r = 0; r < Board.HEIGHT; r++) {
                bool zero = false;
                for(int c = 0; c < Board.WIDTH; c++) {
                    if(still[r, c] == 0) {
                        zero = true;
                        break;
                    }
                }

                if(!zero) {
                    rowsCleared++;
                    DeleteRow(r);
                }
            }

            if(rowsCleared > 0) score += (new int[] { 40, 100, 300, 1200 })[rowsCleared - 1];
        }

        /**
         * Deletes row r from the array of landed Tetromino blocks.
         * @param r The row to delete from the landed array.
         **/
        private void DeleteRow(int row) {
            for(int r = row; r > 0; r--) {
                for(int c = 0; c < Board.WIDTH; c++) {
                    // Overwrite the value of this column from the row above.
                    still[r, c] = still[(r - 1), c];
                }
            }
        }

        /**
         * Prints the game board showing Tetromino blocks as # characters.
         * @param t The current Tetromino. This needs to be drawn as well.
         **/
        public void PrintBoard(Tetromino t, bool landed, long score) {
            Console.WriteLine("Score: {0}", score);
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", (Board.WIDTH + 2))));
            for(int r = 0; r < Board.HEIGHT; r++) {
                Console.Write("|");
                for(int c = 0; c < Board.WIDTH; c++) {
                    int val = GetPlace(r, c);
                    bool currentTetromino = t.IsBlock(r, c);
                    if(currentTetromino || (val != 0)) {
                        Console.ForegroundColor = Tetromino.GetColor((currentTetromino)?t.Color:val);
                    }
                    Console.Write("{0}", (((val == 0) && !currentTetromino) ? ' ' : '#'));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("|");
                Console.WriteLine();
            }
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", (Board.WIDTH + 2))));
        }
    }
}
