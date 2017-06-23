using System;
using System.Linq;
using System.Collections.Generic;

namespace Tetris.Utils {
    public enum Shape {
        I, O, T, J, L, S, Z
    }

    class Tetromino {
        public static ConsoleColor[] Colors = new ConsoleColor[8] { ConsoleColor.Cyan, ConsoleColor.Red,
                                                                    ConsoleColor.Green, ConsoleColor.Yellow,
                                                                    ConsoleColor.DarkBlue, ConsoleColor.DarkRed,
                                                                    ConsoleColor.DarkGreen, ConsoleColor.DarkYellow };

        private Shape shape;
        public Shape GetShape => shape;

        private int color;
        public int Color => (color + 1);
        public static ConsoleColor GetColor(int c) {
            return Tetromino.Colors[c - 1];
        }

        private int rotation;
        public int Rotation {
            get => rotation;
            set => rotation = value;
        }
        private void Rotate(Board game, int dir) {
            int old = rotation;
            rotation = (rotation + dir) % (Tetromino.Rotations[shape].Count);
            bool collision = Move(game, 0, 0); // to make sure this rotation doesn't collide with anything
            if(collision) rotation = old;
        }
        public void RotateCW(Board game) { Rotate(game, 1); }
        public void RotateCCW(Board game) { Rotate(game, -1); }

        public int[,] GetState() {
            return (Tetromino.Rotations[shape])[rotation];
        }

        private int oR, oC;
        public int OriginRow {
            get => oR;
            set => oR = value;
        }
        public int OriginCol {
            get => oC;
            set => oC = value;
        }
        public void GetTransformed(ref int r, ref int c) {
            r += oR;
            c += oC;
        }
        public bool IsBlock(int r, int c) {
            int[,] state = GetState();
            for(int cr = 0; cr < state.GetLength(0); cr++) {
                for(int cc = 0; cc < state.GetLength(1); cc++) {
                    if(state[cr, cc] != 0) {
                        int tr = cr, tc = cc;
                        GetTransformed(ref tr, ref tc);
                        if((tr == r) && (tc == c)) return true;
                    }
                }
            }

            return false;
        }

        public Tetromino(Shape shape_, int color_, int r, int c) {
            this.shape = shape_;
            this.color = color_;
            this.rotation = 0;
            this.oR = r;
            this.oC = c;
        }

        public Tetromino(Tetromino other) {
            this.shape = other.GetShape;
            this.color = other.Color;
            this.rotation = other.Rotation;
            this.oR = other.OriginRow;
            this.oC = other.OriginCol;
        }

        /**
         * Use to transform current Tetromino into its next state.
         * @param next The Tetromino to transform this Tetromino's values into.
         **/
        private void Transform(Tetromino next) {
            this.rotation = next.Rotation;
            this.oR = next.OriginRow;
            this.oC = next.OriginCol;
        }

        /**
         * Checks to see if the Tetromino can be moved, and if so, moves it.
         * @param game The board to check the Tetromino against.
         * @param dr The row offset to add to the Tetromino to simulate downward "movement".
         * @param dc The column offset to add to the Tetromino to simulate left and right "movement".
         * @returns A bool representing whether or not this Tetromino should land, or if checking a rotated piece,
         *          returns true if a collision is detected from the rotated piece.
         **/
        public bool Move(Board game, int dr, int dc) {
            Tetromino next = new Tetromino(this);
            next.OriginRow += dr;
            next.OriginCol += dc;
            int[,] state = next.GetState();

            // Check to see if this Tetromino has collided with any of the already landed blocks.
            for(int r = 0; r < Board.HEIGHT; r++) {
                for(int c = 0; c < Board.WIDTH; c++) {
                    // Iterate through positions of the current shape's blocks.
                    for(int ir = 0; ir < state.GetLength(0); ir++) {
                        for(int ic = 0; ic < state.GetLength(1); ic++) {
                            if(state[ir, ic] != 0) {
                                int tr = ir, tc = ic;
                                next.GetTransformed(ref tr, ref tc);
                                // Various conditions that determine if the Tetromino should "land".
                                // TODO: add case for checking collisions with rotations
                                if((dc != 0) && (game.GetPlace(r, c) != 0) && (tr == r) && (tc == c)) {
                                    // Do we have a horizontal collision between pieces?
                                    return false;
                                } else if(tr >= Board.HEIGHT) {
                                    // Check if the Tetromino is touching the bottom.
                                    game.AddPiece(this);
                                    return true;
                                } else if(tc < 0) {
                                    // Make sure we don't move off the left of the board.
                                    return ((dr == 0) && (dc == 0)); // checks to see if this is a rotation check
                                } else if(tc >= Board.WIDTH) {
                                    // Make sure we don't move off the right of the board.
                                    return ((dr == 0) && (dc == 0)); // checks to see if this is a rotation check
                                } else if((game.GetPlace(r, c) != 0) && (tr == r) && (tc == c)) {
                                    // Is there already a block here? If so, land.
                                    game.AddPiece(this);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            
            Transform(next);
            return false;
        }

        /**
         * Generate a random Tetromino piece and position it in the center of the top of the board.
         * @returns A Tetromino object representing the randomly generated Tetromino block.
         **/ 
        public static Tetromino Random() {
            Random r = new Random();
            Array values = Enum.GetValues(typeof(Shape));
            Shape randomShape = (Shape)values.GetValue(r.Next(values.Length));
            int color = r.Next(Tetromino.Colors.Length);

            int width = (Tetromino.Rotations[randomShape])[0].GetLength(1);
            Tetromino t = new Tetromino(randomShape, color, 0, ((Board.WIDTH / 2) - (width / 2)));
            return t;
        }

        // Define the multi-dimensional arrays representing the different rotations of each Tetromino shape.
        public static Dictionary<Shape, List<int[,]>> Rotations = new Dictionary<Shape, List<int[,]>>() {
            { Shape.I, new List<int[,]>(){ new int[,]{ {1},{1},{1},{1} },
                                           new int[,]{ {1,1,1,1} } } },
            { Shape.O, new List<int[,]>(){ new int[,]{ {1,1}, {1,1} } } },
            { Shape.T, new List<int[,]>(){ new int[,]{ {0,1,0},
                                                       {1,1,1} },
                                           new int[,]{ {1,0},
                                                       {1,1},
                                                       {1,0} },
                                           new int[,]{ {1,1,1},
                                                       {0,1,0} },
                                           new int[,]{ {0,1},
                                                       {1,1},
                                                       {0,1} } } },
            { Shape.J, new List<int[,]>(){ new int[,]{ {0,1},
                                                       {0,1},
                                                       {1,1} },
                                           new int[,]{ {1,0,0},
                                                       {1,1,1} },
                                           new int[,]{ {1,1},
                                                       {1,0},
                                                       {1,0} },
                                           new int[,]{ {1,1,1},
                                                       {0,0,1} } } },
            { Shape.L, new List<int[,]>(){ new int[,]{ {1,0},
                                                       {1,0},
                                                       {1,1} },
                                           new int[,]{ {1,1,1},
                                                       {1,0,0} },
                                           new int[,]{ {1,1},
                                                       {0,1},
                                                       {0,1} },
                                           new int[,]{ {0,0,1},
                                                       {1,1,1} } } },
            { Shape.S, new List<int[,]>(){ new int[,]{ {0,1,1},
                                                       {1,1,0} },
                                           new int[,]{ {1,0},
                                                       {1,1},
                                                       {0,1} } } },
            { Shape.Z, new List<int[,]>(){ new int[,]{ {1,1,0},
                                                       {0,1,1} },
                                           new int[,]{ {0,1},
                                                       {1,1},
                                                       {1,0} } } }
        };
    }
}
