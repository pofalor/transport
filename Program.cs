using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Transport;

//using OptimalRoute;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static void Main(string[] args)
    {
        Solution();

        //       int[][] array = new int[][]
        //{
        //           new int[] { 5, 8, 1, 7 },
        //           new int[] { 6, 2, 4, 9 },
        //           new int[] { 5, 3, 8, 6 }
        //};
        //       distributeByVogel(3, 4, array);

    }


    static void Solution()
    {
        // Чтение входных данных
        int N, M;
        //потребности потребителей
        int[] supplies, demands;
        int[][] costs;
        ReadAndFillData(out N, out M, out supplies, out demands, out costs);

        CheckSumsResources(supplies, demands);

        Element[][] transportPlan = new Element[N][];
        HashSet<(int, int)> notDefined = new HashSet<(int, int)>();
        for (int i = 0; i < N; i++)
        {
            transportPlan[i] = new Element[M];
            for (int j = 0; j < M; j++)
            {
                transportPlan[i][j].Weight = -1;
                notDefined.Add((i, j));
            }
        }

        distributeByVogel(N, M, costs, transportPlan, supplies, demands, notDefined);

        Console.WriteLine(JsonSerializer.Serialize(transportPlan));

        //int totalCost = SolveSync(N, M, supplies, demands, costs, transportPlan);

        // Запись результатов
        //using (var writer = new StreamWriter("out.txt"))
        //{
        //    writer.WriteLine(totalCost);
        //    for (int i = 0; i < N; i++)
        //    {
        //        writer.WriteLine(string.Join(" ", transportPlan.GetRow(i)));
        //    }
        //}
    }
    static void CheckSumsResources(int[] supplies, int[] demands)
    {
        int s = supplies.Sum(), s1 = demands.Sum();
        if (s > s1)
        {
            Console.WriteLine("Запасы поставщиков превышают запросы потребителей, для решения задачи должны быть равны");
            Environment.Exit(0);
        }
        else if (s < s1)
        {
            Console.WriteLine("Запросы потребителей превышают запасы поставщиков, для решения задачи должны быть равны");
            Environment.Exit(0);
        }

    }
    /// <summary>
    /// Считать и записать данные
    /// </summary>
    /// <param name="N"></param>
    /// <param name="M"></param>
    /// <param name="supplies">Запасы поставщиков</param>
    /// <param name="demands">Потребности потребителей</param>
    /// <param name="costs"></param>
    static void ReadAndFillData(out int N, out int M, out int[] supplies, out int[] demands, out int[][] costs)
    {
        var input = File.ReadAllLines("in.txt");
        var dimensions = input[0].Trim().Split().Select(int.Parse).ToArray();
        N = dimensions[0];
        M = dimensions[1];
        supplies = input[1].Trim().Split().Select(int.Parse).ToArray();
        demands = input[2].Trim().Split().Select(int.Parse).ToArray();
        costs = input.Skip(3).Select(line => line.Trim().Split().Select(int.Parse).ToArray()).ToArray();
    }

    static void distributeByVogel(int N, int M, int[][] costs, Element[][] transportPlan, int[] supplies, int[] demands, HashSet<(int, int)> notDefined)
    {
        while (notDefined.Count > 1)
        {
            List<(int, int)> indexes = new List<(int, int)>();
            //transportPlan = new int[N, M];
            getIndexesToFillNeeds(N, M, costs, indexes, transportPlan);
            foreach (var (row, col) in indexes)
            {
                transportPlan[row][col].Weight = Math.Min(supplies[row], demands[col]);
                notDefined.Remove((row, col));
                supplies[row] -= transportPlan[row][col].Weight.Value;
                demands[col] -= transportPlan[row][col].Weight.Value;

                if (supplies[row] == 0)
                {

                    for (int i = 0; i < M; i++)
                    {
                        if (transportPlan[row][i].Weight == -1)
                        {
                            transportPlan[row][i].Weight = 0;
                            notDefined.Remove((row, i));
                        }
                    }
                }
                if (demands[col] == 0)
                {
                    for (int i = 0; i < N; i++)
                    {
                        if (transportPlan[i][col].Weight == -1)
                        {
                            transportPlan[i][col].Weight = 0;
                            notDefined.Remove((i, col));
                        }
                    }
                }

            }
        }
        if (notDefined.Count == 1)
        {
            var coords = notDefined.First();
            if (supplies[coords.Item1] != demands[coords.Item2])
            {
                Console.WriteLine();
            }
            else
            {
                transportPlan[coords.Item1][coords.Item2].Weight = supplies[coords.Item1];
                supplies[coords.Item1] = demands[coords.Item2] = 0;
            }
        }
        Console.WriteLine(transportPlan);
    }

    static void getIndexesToFillNeeds(int N, int M, int[][] costs, List<(int, int)> indexes, Element[][] transportPlan)
    {
        //int[] rowColDiff = new int[N + M];
        var rowColDiff = new PriorityQueue<(int, (int, int)), int>();
        int lim = Math.Min(N, M);
        //Parallel.For(0, lim, i =>
        //{
        //var twoMinRow = costs[i].OrderBy(x => x).Take(2).ToArray();
        //rowColDiff.Enqueue((twoMinRow[1] - twoMinRow[0], (i, 'r')), twoMinRow[0] - twoMinRow[1]);
        //var twoMinCol = costs.Select(row => row[i]).OrderBy(x => x).Take(2).ToArray();
        //rowColDiff.Enqueue((twoMinCol[1] - twoMinCol[0], (i, 'c')), twoMinCol[0] - twoMinCol[1]);
        //});
        for (int i = 0; i < lim; i++)
        {
            findMinValsRowOrCol(i, -1, costs, transportPlan, rowColDiff);
            findMinValsRowOrCol(-1, i, costs, transportPlan, rowColDiff);
        }
        if (M == N) { }
        else if (lim == N)
        {
            //Parallel.For(lim, M, i =>
            //{
            //    var twoMinCol = costs.Select(row => row[i]).OrderBy(x => x).Take(2).ToArray();
            //    rowColDiff.Enqueue((twoMinCol[1] - twoMinCol[0], (i, 'c')), twoMinCol[0] - twoMinCol[1]);
            //});
            for (int i = lim; i < M; i++)
            {
                findMinValsRowOrCol(-1, i, costs, transportPlan, rowColDiff);
            }
        }
        else
        {
            //Parallel.For(lim, N, i =>
            //{
            //    var twoMinRow = costs[i].OrderBy(x => x).Take(2).ToArray();
            //    rowColDiff.Enqueue((twoMinRow[1] - twoMinRow[0], (i, 'r')), twoMinRow[0] - twoMinRow[1]);
            //});
            for (int i = lim; i < N; i++)
            {
                findMinValsRowOrCol(i, -1, costs, transportPlan, rowColDiff);
            }
        }
        //if (rowColDiff.Count == 5)
        //{
        //    Console.WriteLine();
        //}
        var el = rowColDiff.Dequeue();
        var diff = el.Item1;
        //indexes.Add(el.Item2);
        //el = rowColDiff.Dequeue();
        if (!indexes.Contains(el.Item2))
        {
            indexes.Add(el.Item2);
        }
        while ((el.Item1 == diff) && (rowColDiff.Count > 0))
        {
            if (!indexes.Contains(el.Item2))
            {
                indexes.Add(el.Item2);
            }
            el = rowColDiff.Dequeue();
        }

        if (indexes.Count > 1)
        {
            //indexes = indexes.OrderBy(x => costs[x.Item1][x.Item2]).ToList();
            indexes.Sort((a, b) => costs[a.Item1][a.Item2].CompareTo(costs[b.Item1][b.Item2]));
            //indexes.Sort()
        }
        Console.WriteLine();


    }



    static void findMinValsRowOrCol(int i, int j, int[][] costs, Element[][] transportPlan, PriorityQueue<(int, (int, int)), int> rowColDiff)
    {
        if (i == -1)
        {
            var twoMin = costs
                .Select(row => row[j])
                .Select((value, index) => (value, index))
                .Where(x => transportPlan[x.index][j].Weight == -1)
                .OrderBy(x => x.ToTuple().Item1)
                .Take(2)
                .ToArray();
            if (twoMin.Length == 2)
            {
                rowColDiff.Enqueue((twoMin[1].value - twoMin[0].value, (twoMin[0].index, j)), twoMin[0].value - twoMin[1].value);
            }
            else
            {
                Console.WriteLine();
            }
        }
        else
        {
            var twoMin = costs[i]
                .Select((value, index) => (value, index))
                .Where(x => transportPlan[i][x.index].Weight == -1)
                .OrderBy(x => x.ToTuple().Item1)
                .Take(2)
                .ToArray();
            if (twoMin.Length == 2)
            {
                rowColDiff.Enqueue((twoMin[1].value - twoMin[0].value, (i, twoMin[0].index)), twoMin[0].value - twoMin[1].value);
            }
            else
            {
                Console.WriteLine();
            }
        }
    }
}

//static class Extensions
//{
//    public static T[] GetRow<T>(this T[,] array, int row)
//    {
//        return Enumerable.Range(0, array.GetLength(1))
//                         .Select(col => array[row, col])
//                         .ToArray();
//    }
//}
