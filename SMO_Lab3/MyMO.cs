using System;
using System.Collections.Generic;
using System.Text;

namespace SMO_Lab3
{
    public static class MyMO
    {
        // кількість n систем масового обслуговування СМО1, СМО2, ..., СМОn, що складають мережу МО
        private static int n = 2;
        //кількість N вимог в мережі МО
        private static int N = 15;
        //кількість каналів обслуговування ri в СМОi;
        private static int[] ri = { 3, 3 };
        //інтенсивність µі обслуговування вимог кожним каналом СМОi.
        private static double[] mi = { (1 / 0.8), (1 / 0.6) };
        //матрицею ймовірностей ||рij||, елемент якої рij дування вимоги із СМОi у СМОj
        private static double[,] pij = { { 0.01, 0.99 }, { 0.55, 0.45 } };
        //коефіцієнти передачі
        private static double[] ei = { 1, 1.8 };

        private static double[] helpers = { (ei[0] / mi[0]), (ei[1] / mi[1]) };

        //Нормуючого множник для мережі МО
        private static double NormFactor;
        //середня кількість вимог у черзі СМОi
        private static double[] Li = new double[n];
        //середня кількість зайнятих пристроїв у СМОi
        private static double[] Ri = new double[n];
        //середня кількість вимог у СМОi
        private static double[] Mi = new double[n];
        //інтенсивність вихідного потоку вимог у СМОi
        private static double[] Intensities = new double[n];
        //середній час перебування вимоги в СМОi(друга формула Літтла).
        private static double[] Ti = new double[n];
        //середній час очікування в черзі СМОi(перша формула Літтла).
        private static double[] Qi = new double[n];

        /// <summary>
        /// Функція pi(k)
        /// </summary>
        /// <param name="i">індекс СМО</param>
        /// <param name="k">кількість вимог в СМОі</param>
        /// <returns>Обраховане значення за формулою 4.21</returns>
        public static double GetPi(int i, int k)
        {
            double firstPart = Math.Pow(helpers[i], k);
            double secondPart = k > ri[i] ? 1 / ((3 * 2) * Math.Pow(ri[i], k - ri[i])) : 1 / Factorial(k);

            return firstPart * secondPart;
        }

        /// <summary>
        /// Розрахунок нормуючого множника для мережі МО.
        /// </summary>
        /// <returns>Результат обчислення за формулою 4.24 (4.23 у загальному випадку)</returns>
        public static double GetNormalizingFactor()
        {
            double normalizingFactor = 0;

            for (int i = 0; i <= N; i++)
            {
                normalizingFactor += (GetPi(0, i) * GetPi(1, N - i));
            }

            NormFactor = Math.Pow(normalizingFactor, -1);
            return Math.Pow(normalizingFactor, -1);
        }

        /// <summary>
        /// Обчислює середню кількість вимог Li у черзі СМОi за формулою 4.32.
        /// </summary>
        private static void GetAverageNumberOfRequirements()
        {
            for (int i = 0; i < n; i++)
            {
                double L = 0;

                for (int j = ri[i] +1; j <= N; j++)
                {
                    L += (j - ri[i]) * GetPSMOi(i, j);
                }

                Li[i] = L;
            }
        }

        /// <summary>
        /// Обчислює середню кількість зайнятих пристроїв Ri у СМОi за формулою 4.33
        /// </summary>
        private static void GetRi()
        {
            for (int i = 0; i < n; i++)
            {
                double R = 0;

                for (int j = 0; j < ri[i] -1; j++)
                {
                    R += (ri[i] - j) * GetPSMOi(i, j);
                }

                Ri[i] = ri[i] - R;
            }
        }

        /// <summary>
        /// Обчислює середню кількість вимог Mi у СМОi за формулою 4.34.
        /// </summary>
        private static void GetMi()
        {
            for (int i = 0; i < n; i++)
            {
                Mi[i] = Ri[i] + Li[i];
            }
        }

        /// <summary>
        /// Обчислює ймовірність знаходження j вимог в СМОі за формулою 4.26.
        /// </summary>
        /// <param name="index">індекс СМО</param>
        /// <param name="j">кількість вимог в СМОі</param>
        /// <returns></returns>
        private static double GetPSMOi(int index, int j)
        {
            double first = index == 0 ? GetPi(0, j) : GetPi(0, N - j);
            double second = index == 0 ? GetPi(1, N-j) : GetPi(1, j);

            return NormFactor * first * second;
        }

        /// <summary>
        /// Обчислює інтенсивність вихідного потоку вимог у СМОi за формулою 4.35.
        /// </summary>
        private static void GetIntensities()
        {
            for (int i = 0; i < n; i++)
            {
                Intensities[i] = Ri[i] * mi[i];
            }
        }

        /// <summary>
        /// Обчислює середній час перебування вимоги в СМОi за формулою 4.36.
        /// </summary>
        private static void GetAverageTime()
        {
            for (int i = 0; i < n; i++)
            {
                Ti[i] = Mi[i] / Intensities[i];
            }
        }

        /// <summary>
        /// Обчислює середній час очікування в черзі СМОi за формулою 4.37.
        /// </summary>
        private static void GetAverageTimeInQueue()
        {
            for (int i = 0; i < n; i++)
            {
                Qi[i] = Li[i] / Intensities[i];
            }
        }

        /// <summary>
        /// Обчислює факторіал числа.
        /// </summary>
        /// <param name="number">Число</param>
        /// <returns></returns>
        public static int Factorial(int number)
        {
            int result = 1;
            while (number != 0)
            {
                result *= number;
                number--;
            }

            return result;
        }

        private static double TestResults(int index)
        {
            double sum = 0;
            for (int i = 0; i <= 15; i++)
            {
                sum += GetPSMOi(index, i);
            }

            return sum;
        }

        public static void GetResult()
        {
            Console.WriteLine("Normalizing factor C(N): " + GetNormalizingFactor());

            GetAverageNumberOfRequirements();
            GetRi();
            GetMi();
            GetIntensities();
            GetAverageTime();
            GetAverageTimeInQueue();

            Console.WriteLine("\nThe average number of requests in the CMOi queue:");
            Console.WriteLine("L1= " + Li[0] + "\nL2= " + Li[1]);

            Console.WriteLine("\nThe average number of busy devices in CMOi:");
            Console.WriteLine("R1= " + Ri[0] + "\nR2= " + Ri[1]);

            Console.WriteLine("\nThe average number of requirements in the CMOi queue:");
            Console.WriteLine("M1= " + Mi[0] + "\nM2= " + Mi[1]);

            Console.WriteLine("\nThe intensity of the output stream of requirements in the CMOi queue:");
            Console.WriteLine("λ1= " + Intensities[0] + "\nλ2= " + Intensities[1]);

            Console.WriteLine("\nThe the average residence time of the requirement in the CMOi queue:");
            Console.WriteLine("T1= " + Ti[0] + "\nT2= " + Ti[1]);

            Console.WriteLine("\nThe the average waiting time in the CMOi queue:");
            Console.WriteLine("Q1= " + Qi[0] + "\nQ2= " + Qi[1]);

            Console.WriteLine("\nSum of all probability CMOi\nCMO1= " + TestResults(0) + "\nCMO2= " + TestResults(1));
        }

    }
}
