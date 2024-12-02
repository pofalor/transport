using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport
{
    public static class CreatePotentialsAndCheck
    {

        public static void CreatePotentialsOnRowsAndCols(int N, int M, Element[][] transportPlan, List<(int, int)> ValuesIndexes, int?[] rowPotentials, int?[] colPotentials)
        {

            rowPotentials[0] = 0;
            var ColIndexes = new List<int>();
            List<int> RowIndexes = new List<int>();
            List<(int, int)> toRem = new List<(int, int)>();

            foreach (var pair in ValuesIndexes)
            {
                if (pair.Item1 == 0)
                {
                    colPotentials[pair.Item2] = transportPlan[pair.Item1][pair.Item2].Value - rowPotentials[0];
                    ColIndexes.Add(pair.Item2);
                    toRem.Add(pair);
                }
            }
            foreach (var el in toRem)
            {
                ValuesIndexes.Remove(el);
            }
            toRem.Clear();
            while (ValuesIndexes.Count > 0)
            {
                foreach (int j in ColIndexes)
                {
                    //RowIndexes = new List<int>();

                    foreach (var pair in ValuesIndexes)
                    {
                        if (pair.Item2 == j)
                        {
                            rowPotentials[pair.Item1] = transportPlan[pair.Item1][pair.Item2].Value - colPotentials[j];
                            RowIndexes.Add(pair.Item1);
                            toRem.Add(pair);

                        }
                    }
                }
                foreach (var el in toRem)
                {
                    ValuesIndexes.Remove(el);
                }
                toRem.Clear();
                ColIndexes.Clear();
                foreach (int i in RowIndexes)
                {
                    //ColIndexes = new List<int>();

                    foreach (var pair in ValuesIndexes)
                    {
                        if (pair.Item1 == i)
                        {
                            colPotentials[pair.Item2] = transportPlan[pair.Item1][pair.Item2].Value - rowPotentials[i];
                            ColIndexes.Add(pair.Item2);
                            toRem.Add(pair);

                        }
                    }
                }
                foreach (var el in toRem)
                {
                    ValuesIndexes.Remove(el);
                }
                toRem.Clear();
                RowIndexes.Clear();
            }
        }
    
        public static bool CreatePotentialsOnMatrix(int?[] rowPotentials, int?[] colPotentials, Element[][] transportPlan, List<(int, int)> ValuesIndexes)
        {
            bool isNegative = false;
            for (int i = 0; i < transportPlan.Length; i++)
            {
                for (int j = 0; j < transportPlan[i].Length; j++)
                {
                    if (transportPlan[i][j].Weight == 0)
                    {
                        transportPlan[i][j].Potential = transportPlan[i][j].Value - rowPotentials[i] - colPotentials[j];
                        transportPlan[i][j].IsPotentialNegative = (transportPlan[i][j].Potential < 0);
                        isNegative = isNegative || transportPlan[i][j].IsPotentialNegative;
                    }
                    else
                    {
                        ValuesIndexes.Add((i, j));
                    }
                        
                }
            }
            return isNegative;                
        }
    }
}
