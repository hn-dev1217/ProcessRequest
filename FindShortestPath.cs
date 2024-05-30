using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRequest
{

    public class FindShortestPath
    {
        private static readonly int[] kx = {2, 2, 1, 1, -1, -1, -2, -2 };
        private static readonly int[] ky = { 1, -1, 2, -2, 2, -2, 1, -1 };

        ////check to verify if the move is within the board.
        private static bool isWithinBoard(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }


        public static (List<string>, int) shortestPath(string start, string end)
        {
            var startIndex = Function1.KnightPositionsIndex(start);
            var endIndex = Function1.KnightPositionsIndex(end);

            if(startIndex == endIndex)
            {
                return (new List<string>() { start }, 0);
            }

                var queue = new Queue<(int, int, List<string>, int)>();
                var visited = new bool[8, 8];

                //adding the row index of the start position, column index of the start position, path taken so far which is initially start position,
                //no of moves made so far which is 0 at the start.
                queue.Enqueue((startIndex.Item1, startIndex.Item2, new List<string> { start }, 0));
                visited[startIndex.Item1, startIndex.Item2] = true;

                while (queue.Count > 0)
                {
                    var (currentRow, currentCol, path, noOfMoves) = queue.Dequeue();

                    //looping thru the board to calculate the new row and column for the path.
                    for (int i = 0; i < 8; i++)
                    {
                        int newRow = currentRow + kx[i];
                        int newCol = currentCol + ky[i];


                        //check if the new row and new column are within the board
                        if (isWithinBoard(newRow, newCol) && !visited[newRow, newCol])
                        {

                        var newPosition = Function1.IndexToKnightPositions(newCol, newRow);

                            var newPath = new List<string>(path) { newPosition.ToString() };


                        //check if knight new position is matching with the target position.
                        if (newRow == endIndex.Item1 && newCol == endIndex.Item2)
                            {
                                return (newPath, noOfMoves + 1);
                            }

                        //add the new calculated row, column, path, number of moves to the queue.
                        queue.Enqueue((newRow, newCol, newPath, noOfMoves + 1));
                            visited[newRow, newCol] = true;


                        }
                    }
                }
            

            return (null, -1);

        }

    }
}
