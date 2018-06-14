using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var reader = new StreamReader("in.txt");
            var readLine = reader.ReadLine().Split(' ');
            var itemN = Int32.Parse(readLine[0]);
            var userN = Int32.Parse(readLine[1]);

            var itemGrade = new int[itemN][]; //[i][u]
            var userGrade = new int[userN][]; // [u][i]

            var itemAverage = new double[itemN][];
            var userAverage = new double[userN][];

            var ps = new double[itemN][]; //[i][i]
            var userPS = new double[userN][]; //[u][u] 

            for (var i = 0; i < itemN; i++)
            {
                if (itemGrade[i] == null)
                    itemGrade[i] = new int[userN];

                readLine = reader.ReadLine().Split(' ');
                for (var j = 0; j < userN; j++)
                {
                    if (readLine[j] == "X")
                    {
                        itemGrade[i][j] = 0;
                    }
                    else
                    {
                        itemGrade[i][j] = int.Parse(readLine[j]);
                    }
                }
            }

            for (var i = 0; i < userN; i++)
            {
                if (userGrade[i] == null)
                    userGrade[i] = new int[itemN];

                for (var j = 0; j < itemN; j++)
                {
                    userGrade[i][j] = itemGrade[j][i];
                }
            }

            for (var i = 0; i < itemN; i++)
            {
                itemAverage[i] = reduce(itemGrade[i]);
            }

            for (var i = 0; i < userN; i++)
            {
                userAverage[i] = reduce(userGrade[i]);
            }

            for (var i = 0; i < itemN; i++)
            {
                ps[i] = new double[itemN];
            }

            for (var i = 0; i < userN; i++)
            {
                userPS[i] = new double[userN];
            }

            for (var i = 0; i < itemN; i++)
            {
                for (var j = i; j < itemN; j++)
                {
                    double pParameter = cosineSim(itemAverage[i], itemAverage[j]);
                    ps[i][j] = pParameter;
                    ps[j][i] = pParameter;
                }
            }

            for (var i = 0; i < userN; i++)
            {
                for (var j = i; j < userN; j++)
                {
                    double pearsonCoef = cosineSim(userAverage[i], userAverage[j]);
                    userPS[i][j] = pearsonCoef;
                    userPS[j][i] = pearsonCoef;
                }
            }

            var qParameter = int.Parse(reader.ReadLine());
            for (var i = 0; i < qParameter; i++)
            {
                readLine = reader.ReadLine().Split(' ');

                var item = Int32.Parse(readLine[0]);
                var user = Int32.Parse(readLine[1]);

                var tParameter = Int32.Parse(readLine[2]);
                var kParameter = Int32.Parse(readLine[3]);

                if (tParameter == 0)
                {
                    rate(user - 1, item - 1, ps, kParameter, itemGrade);
                }
                else if (tParameter == 1)
                {
                    rate(item - 1, user - 1, userPS, kParameter, userGrade);
                }
            }
        }

        private static void rate(int user, int item, double[][] ps, int kParameter, int[][] grade)
        {
            var simDuplicate = new List<double>();
            for (var i = 0; i < ps.Length; i++)
            {
                if (i == item)
                {
                    simDuplicate.Add(-5.0);
                }
                else
                {
                    simDuplicate.Add(ps[item][i]);
                }
            }

            var simForI = new List<double>(simDuplicate);
            simDuplicate.Sort();
            simDuplicate.Reverse();

            var simItems = new List<int>();
            var cnt = 0;

            for (var i = 0; i < simDuplicate.Count; i++)
            {
                if (simDuplicate[i] > 0 && grade[simForI.IndexOf(simDuplicate[i])][user] > 0)
                {
                    simItems.Add(simForI.IndexOf(simDuplicate[i]));
                    cnt++;

                    if (cnt == kParameter)
                        break;
                }
            }

            var brojnik = 0.0;
            var nazivnik = 0.0;

            for (var i = 0; i < simItems.Count; i++)
            {
                brojnik += grade[simItems[i]][user] * ps[simItems[i]][item];
            }

            for (var i = 0; i < simItems.Count; i++)
            {
                nazivnik += ps[simItems[i]][item];
            }

            var result = brojnik / nazivnik;

            var d = new decimal(result);
            d = Math.Round(d, 3, MidpointRounding.AwayFromZero);

            Console.WriteLine(d.ToString());
        }

        private static double cosineSim(double[] a, double[] b)
        {
            var multiplication = multiplyCalculation(a, b);

            var magnA = Math.Sqrt(multiplyCalculation(a, a));
            var magnB = Math.Sqrt(multiplyCalculation(b, b));

            return multiplication / (magnA * magnB);
        }

        private static double multiplyCalculation(double[] a, double[] b)
        {
            var calculation = 0.0;
            for (var i = 0; i < a.Length; i++)
            {
                calculation += a[i] * b[i];
            }

            return calculation;
        }

        private static double[] reduce(int[] v)
        {
            var addition = 0.0;
            var cnt = 0;
            var newV = new double[v.Length];

            for (var i = 0; i < v.Length; i++)
            {
                if (v[i] == 0)
                {
                    newV[i] = 0;
                }
                else
                {
                    addition += v[i];
                    cnt++;
                }
            }

            double average = addition / cnt;

            for (var i = 0; i < v.Length; i++)
            {
                if (v[i] == 0)
                {
                    newV[i] = 0;
                }
                else
                {
                    newV[i] = v[i] - average;
                }
            }
            return newV;
        }
    }
}
