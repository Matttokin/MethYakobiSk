using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MethYakobiSk
{
    public partial class Form1 : Form
    {
        int countCol;
        double eps; /// желаемая точность
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int[] cmbValues = new int[] { 3, 4, 5, 6, 7, 8 };
            foreach (var item in cmbValues)
            {
                comboBox1.Items.Add(item);
            }
            comboBox1.SelectedIndex = 0;

            drawMatrix();
        }
        #region drawMatrix
        private void drawMatrix()
        {
            dataGridView1.Columns.Clear();
            dataGridView3.Columns.Clear();

            countCol = int.Parse(comboBox1.SelectedItem.ToString());
            if (countCol > 8 || countCol < 3)
            {
                MessageBox.Show("Произошла ошибка");
                return; 
            }

            int widthMatrixValue = dataGridView1.Width;
            int widthColMV = (widthMatrixValue - 1) / (countCol + 1);
            
            DataGridViewTextBoxColumn[] columnGr1 = new DataGridViewTextBoxColumn[countCol + 1];
            DataGridViewTextBoxColumn[] columnGr3 = new DataGridViewTextBoxColumn[countCol];

            for (int i = 0; i <= countCol; i++)
            {
                columnGr1[i] = new DataGridViewTextBoxColumn();
                
                if (i == countCol)
                {
                    columnGr1[i].HeaderText = "B";
                    columnGr1[i].Name = "B" + i;
                    columnGr1[i].Width = widthMatrixValue - countCol * widthColMV - 3;

                }
                else
                {
                    columnGr1[i].HeaderText = "X" + (i + 1);
                    columnGr1[i].Name = "X" + (i + 1);
                    columnGr1[i].Width = widthColMV;
                }

                if (i < countCol)
                {
                    columnGr3[i] = new DataGridViewTextBoxColumn();
                    columnGr3[i].HeaderText = "X" + (i + 1);
                    columnGr3[i].Name = "X" + (i + 1);
                    if (i == countCol - 1)
                    {
                        columnGr3[i].Width = widthMatrixValue - (countCol - 1) * widthColMV - 3;
                    }
                    else
                    {
                        columnGr3[i].Width = widthColMV;
                    }
                }
            }

            widthMatrixValue = dataGridView3.Width;
            widthColMV = (widthMatrixValue - 1) / countCol;

            for (int i = 0; i < countCol; i++)
            {
                columnGr3[i] = new DataGridViewTextBoxColumn();
                columnGr3[i].HeaderText = "X" + (i + 1);
                columnGr3[i].Name = "X" + (i + 1);
                if (i == countCol - 1)
                {
                    columnGr3[i].Width = widthMatrixValue - (countCol - 1) * widthColMV - 3;
                }
                else
                {
                    columnGr3[i].Width = widthColMV;
                }
            }

            dataGridView1.Columns.AddRange(columnGr1);
            dataGridView3.Columns.AddRange(columnGr3);

            for (int i = 0; i < countCol; i++)
            {
                dataGridView1.Rows.Add();
            }
            dataGridView3.Rows.Add();
        }
        #endregion drawMatrix
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawMatrix();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region parseForm
            double.TryParse(textBox1.Text, out eps);
            if(eps <= 0)
            {
                eps = 0.001;
            }

            int[,] arr = new int[countCol, countCol];
            double[,] A1 = new double[countCol, countCol];
            
            for(int i = 0; i < countCol; i++)
            {
                for (int j = 0; j < countCol; j++)
                {
                    double valueCells;
                    double.TryParse(dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? "0", out valueCells);
                    A1[i,j] = valueCells;
                }
            }

            double[] F1 = new double[countCol];
            for (int j = 0; j < countCol; j++)
            {
                double valueCells;
                double.TryParse(dataGridView1.Rows[j].Cells[countCol].Value?.ToString() ?? "0", out valueCells);
                F1[j] = valueCells;
            }



            #endregion parseForm

            var a = CalcMatrix.checkСonvergenceСondition(countCol, A1);
            if(a >= 1)
            {
                MessageBox.Show("Условие сходимости не выполнено");
                return;
            }

            var det = CalcMatrix.getDet(countCol, A1);
            if(det == 0)
            {
                MessageBox.Show("Определитель равен 0");
                return;
            }
            var result = Jacobi(countCol, A1, F1);

            //вывод в таблицу
            for (int i = 0; i < result.Count(); i++)
            {
                dataGridView3.Rows[0].Cells[i].Value = Math.Round(result[i],3);
            }
        }
        

        /// N - размерность матрицы; 
        /// A[N][N] - матрица коэффициентов
        /// F[N] - столбец свободных членов
        /// X[N] - начальное приближение, ответ записывается также в X[N]
        double[] Jacobi(int N, double[,] A, double[] F)
        {
            double[] X = getStartX(N,A,F);
            double[] TempX = new double[N];
            double norm; // норма, определяемая как наибольшая разность компонент столбца иксов соседних итераций.

            do
            {
                // Процедура нахождения решения
                // https://ru.wikipedia.org/wiki/%D0%9C%D0%B5%D1%82%D0%BE%D0%B4_%D0%AF%D0%BA%D0%BE%D0%B1%D0%B8#%D0%A3%D1%81%D0%BB%D0%BE%D0%B2%D0%B8%D0%B5_%D0%BE%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B8
                // Блок "Описание метода"
                for (int i = 0; i < N; i++)
                {
                    TempX[i] = F[i];
                    for (int g = 0; g < N; g++)
                    {
                        if (i != g)
                        {
                            TempX[i] -= A[i,g] * X[g];
                        }
                    }
                    TempX[i] /= A[i,i];
                }

                norm = Math.Abs(Math.Abs(X[0]) - Math.Abs(TempX[0])); //отклонение = разность между текущим значение и его потомком
                for (int h = 0; h < N; h++)
                {
                    if (Math.Abs(Math.Abs(X[h]) - Math.Abs(TempX[h])) > norm) //определяем наибольшее отклонение
                    {
                        norm = Math.Abs(Math.Abs(X[h]) - Math.Abs(TempX[h]));
                    }
                    X[h] = TempX[h];
                }
            } 
            while (norm > eps); //пока отклонение не сравняется с эталанной погрешностью
            
            return X;
        }

        double[] getStartX(int N, double[,] A, double[] F)
        {
            double[] X = new double[N];
            for (int i = 0; i < N; ++i)
            {
                X[i] = F[i] / A[i,i];
            }
            return X;
        }
    }
}
