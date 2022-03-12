using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethYakobiSk
{
    internal class CalcMatrix
    {
        public static double checkСonvergenceСondition(int n, double[,] a)
        {
            double[,] b = new double[3, 3];
            for (int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if(i != j)
                    {
                        b[i, j] = -a[i, j] / a[i, i];
                    }
                }
            }

            double b1 = 0;
            for (int i = 0; i < n; i++)
            {
                double b_tmp = 0;
                for (int j = 0; j < n; j++)
                {
                    b_tmp += Math.Abs(b[i, j]);
                }
                if (b_tmp > b1)
                {
                    b1 = b_tmp;
                }
            }

            double b2 = 0;
            for (int i = 0; i < n; i++)
            {
                double b_tmp = 0;
                for (int j = 0; j < n; j++)
                {
                    b_tmp += Math.Abs(b[j, i]);
                }
                if (b_tmp > b2)
                {
                    b2 = b_tmp;
                }
            }


            return b2 > b1 ? b2 : b1;
        }
        public static double getDet(int n, double[,] a_input) //метод гауса 
        {
            double[][] a = new double[n][];
            for (int i = 0; i < n; i++)
            {
                a[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    a[i][j] = a_input[i,j];
                }
            }


            double det = 1;
            //определяем переменную EPS
            const double EPS = 1E-9;
            
            double[][] b = new double[1][];
            b[0] = new double[n];

            //проходим по строкам
            for (int i = 0; i < n; ++i)
            {
                //присваиваем k номер строки
                int k = i;
                //идем по строке от i+1 до конца
                for (int j = i + 1; j < n; ++j)
                    //проверяем
                    if (Math.Abs(a[j][i]) > Math.Abs(a[k][i]))
                        //если равенство выполняется то k присваиваем j
                        k = j;
                //если равенство выполняется то определитель приравниваем 0 и выходим из программы
                if (Math.Abs(a[k][i]) < EPS)
                {
                    det = 0;
                    break;
                }
                //меняем местами a[i] и a[k]
                b[0] = a[i];
                a[i] = a[k];
                a[k] = b[0];
                //если i не равно k
                if (i != k)
                    //то меняем знак определителя
                    det = -det;
                //умножаем det на элемент a[i][i]
                det *= a[i][i];
                //идем по строке от i+1 до конца
                for (int j = i + 1; j < n; ++j)
                    //каждый элемент делим на a[i][i]
                    a[i][j] /= a[i][i];
                //идем по столбцам
                for (int j = 0; j < n; ++j)
                    //проверяем
                    if ((j != i) && (Math.Abs(a[j][i]) > EPS))
                        //если да, то идем по k от i+1
                        for (k = i + 1; k < n; ++k)
                            a[j][k] -= a[i][k] * a[j][i];
            }
            //выводим результат
            return det;
        }
    }
}
