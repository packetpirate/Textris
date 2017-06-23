using System;
using System.Linq;
using System.Collections.Generic;

namespace Tetris.Utils {
    public enum Shape {
        I, O, T, J, L, S, Z
    }

    class Tetromino {
        private Shape shape;
        public Shape GetShape => shape;

        private int rotation;
        public int Rotation {
            get => rotation;
            set => rotation = value;
        }
        private void Rotate(int dir) {
            rotation = (rotation + dir) % (Tetromino.Rotations[shape].Count);
        }
        public void RotateCW() { Rotate(1); }
        public void RotateCCW() { Rotate(-1); }

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

        public Tetromino(Shape shape_, int r, int c) {
            this.shape = shape_;
            this.rotation = 0;
            this.oR = r;
            this.oC = c;
        }

        public Tetromino(Tetromino other) {
            this.shape = other.GetShape;
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
         * @returns A bool representing whether or not this Tetromino should land.
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
                                // Check if Tetromino is touching bottom.
                                if(tr >= Board.HEIGHT) {
                                    Console.WriteLine("Out of bounds on bottom!");
                                    game.AddPiece(this);
                                    return true;
                                } else if(tc < 0) {
                                    // Make sure we don't move off the left of the board.
                                    return false;
                                } else if(tc >= Board.WIDTH) {
                                    // Make sure we don't move off the right of the board.
                                    return false;
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

        public static Tetromino Random() {
            Random r = new Random();
            Array values = Enum.GetValues(typeof(Shape));
            Shape randomShape = (Shape)values.GetValue(r.Next(values.Length));

            int width = (Tetromino.Rotations[randomShape])[0].GetLength(1);
            Tetromino t = new Tetromino(randomShape, 0, ((Board.WIDTH / 2) - (width / 2)));
            return t;
        }

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
