using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool click3 = false; //создаем переменную и присваиваем ей ложь, с помощью которой будем делать прерывание

        public Form1()
        {
            InitializeComponent(); //инициализация
            label2.Text = char.ConvertFromUtf32(945) + char.ConvertFromUtf32(8321) + label2.Text; //добавляет к тексту "альфу" и подстрочную "1" из unicode
            label3.Text = char.ConvertFromUtf32(956) + label3.Text; //добавляет к тексту "мю" из unicode
            label5.Text = char.ConvertFromUtf32(945) + char.ConvertFromUtf32(8320) + label5.Text; //добавляет к тексту "альфу" и подстрочную "0" из unicode
            label6.Text = char.ConvertFromUtf32(969) + char.ConvertFromUtf32(8320) + label6.Text; //добавляет к тексту "амегу" и подстрочную "0" из unicode
            label8.Text = label8.Text + char.ConvertFromUtf32(8322) + " (длина подвеса после препятствия):"; //добавляет к имеющему тексту подстрочную "1" из unicode и дополнительный текст
            chart1.Series[0].Name += " " +char.ConvertFromUtf32(945);
            chart1.Series[1].Name += " " + char.ConvertFromUtf32(969);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        public void clearform () //функция очишающая изображения и текст расчетов
        {
            chart1.Series[0].Points.Clear(); //очищает синусоидальное изображение с помощью уравнения y[0] = y[0] + h * y[1]
            chart1.Series[1].Points.Clear(); //очищает синусоидальное изображение с помощью уравнения y[1] = y[1] + h * ((-g * Math.Sin(y[0])) / L - mu * y[0]) 
            richTextBox1.Text = ""; //очищает поле вывода результатов
        }

        //кнопка построения маятника в свободном движении
        private void button1_Click(object sender, EventArgs e)
        {
            double L = Convert.ToDouble(numericUpDown1.Text); //длина подвеса, число 1 означает полную высоту
            double mu = Convert.ToDouble(textBox3.Text); //коэффициент трения (сопротивления)
            double g = Convert.ToDouble(textBox4.Text); //ускорение свободного падения
            int n = Convert.ToInt32(textBox9.Text); //количество секций
            double om0 = Convert.ToDouble(textBox6.Text); //начальная угловая скорость
            double h = Convert.ToDouble(textBox10.Text); //шаг секции
            double al0 = Convert.ToDouble(textBox5.Text) * Math.PI / 180; //начальное отклонение, преобразуем из градусов в радианы
            double x0, y0; //точки х и у в графике
            double[] time = new double[n];
            double[] y = new double[2];
            time[0] = 0; //начальное время
            y[0] = om0; //начальное значение скорости
            y[1] = al0; //начальный угол отклонения
            click3 = false; //операция отмены неактивна
            button3.Enabled = true; //после нажатия кнопка отмены активна
            button2.Enabled = false; //после нажатия кнопка маятника с ограничением неактивна
            button1.Enabled = false; //после нажатия кнопка маятника при свободном движении неактивна
            clearform(); //выполняем очистку формы
            chart1.Series[0].Points.AddXY(0, y[0]); //рисуем первую точку y[0]
            chart1.Series[1].Points.AddXY(0, y[1]); //рисуем первую точку у[1]
            richTextBox1.Text += "t[" + 0 + "] = " + time[0] + ";\n"; //выводим в текст значение времени
            richTextBox1.Text += char.ConvertFromUtf32(945) + "[" + 0 + "] = " + y[0]  + ";\n"; //выводим в текст значение у[0] в градусах
            richTextBox1.Text += char.ConvertFromUtf32(969) + "[" + 0 + "] = " + y[1]  + ";\n"; //выводим в текст значение у[1] в градусах
            richTextBox1.Text += "---------------------------------\n"; //линия прерывания вывода начального
            for (int i = 1; i<n; i++) //цикл расчета
            {
                time[i] = h * i; //время, умножаем шаг секции на шаг цикла
                y[0] = y[0] + h * y[1]; //решение вектора y[0]
                y[1] = y[1] + h * ((-g * Math.Sin(y[0])) / L - mu * y[0]); //решение вектора y[1]
                richTextBox1.Text += "t[" + i + "] = " + time[i] + ";\n"; //выводим в текст значение времени
                richTextBox1.Text += char.ConvertFromUtf32(945) + "[" + i + "] = " + y[0]  + ";\n"; //выводим в текст значение у[0] в градусах
                richTextBox1.Text += char.ConvertFromUtf32(969) + "[" + i + "] = " + y[1]  + ";\n"; //выводим в текст значение у[1] в градусах
                richTextBox1.Text += "---------------------------------\n"; //линия прерывания вывода текущего
                chart1.Series[0].Points.AddXY(i * h, y[0]); //рисуем точку y[0]
                chart1.Series[1].Points.AddXY(i * h, y[1]); //рисуем точку у[1]
                button3.Text = "Отмена\nОсталось секций " + (n - i); //вывести количества секций до окончания на кнопку отмены
                Application.DoEvents(); //выполнение в реальном времени
                button3.Focus(); //возвращает true при клике на кнопку отмены
                System.Threading.Thread.Sleep((int)(h)); //установим время на выполнение расчета
                if (click3 == true) //проверяем была ли использована переменная
                    break; //если переменная вернула положительный результат, прерываем расчет
            }
            button1.Enabled = true; //после окончания построения кнопка маятника при свободном движении активна
            button2.Enabled = true; //после окончания построения кнопка маятника при ограничении активна
            button3.Text = "Отмена"; //восстановить описание кнопки отмены после окончания
            button3.Enabled = false; //после окончания построения кнопка отмены неактивна
           
        }

        //кнопка прерывания построения
        private void button3_Click(object sender, EventArgs e)
        {
           click3 = true; //устанавливаем переменной положительный результат
           button3.Text = "Отмена"; //заменить/восстановить описание кнопки отмены
           button3.Enabled = false; //после нажатия кнопка отмены неактивна
        }

        //кнопка построения маятника при ограничении
        private void button2_Click_1(object sender, EventArgs e)
        {
            double L = Convert.ToDouble(numericUpDown1.Text); //длина подвеса
            double al1 = Convert.ToDouble(textBox2.Text) * Math.PI / 180; //угол до ограничения, преобразуем из градусов в радианы
            double mu = Convert.ToDouble(textBox3.Text); //коэффициент трения (сопротивления)
            double g = Convert.ToDouble(textBox4.Text); //ускорение свободного падения
            double al0 = Convert.ToDouble(textBox5.Text) * Math.PI / 180; //начальное отклонение, преобразуем из градусов в радианы
            double om0 = Convert.ToDouble(textBox6.Text); //начальная угловая скорость
            double L2 = Convert.ToDouble(numericUpDown2.Text); //длина подвеса после препятствия
            int n = Convert.ToInt32(textBox9.Text); //количество секций
            double h = Convert.ToDouble(textBox10.Text); //шаг секции
            double L1 = L - L2; //длина подвеса до препятствия
            double al2 = 0; //отклонение после препятствия
            double x0, y0; //точки х и у в графике
            double[] time = new double[n];
            double[] y = new double[2];
            click3 = false; //операция отмены неактивна
            button3.Enabled = true; //после нажатия кнопка отмены активна
            button2.Enabled = false; //после нажатия кнопка маятника с ограничением неактивна
            button1.Enabled = false; //после нажатия кнопка маятника при свободном движении неактивна
            clearform(); //выполняем очистку формы
            time[0] = 0; //начальное время
            y[0] = om0; //начальное значение скорости
            y[1] = al0; //начальный уголотклонения
            chart1.Series[0].Points.AddXY(0, y[0]); //рисуем первую точку y[0]
            chart1.Series[1].Points.AddXY(0, y[1]); //рисуем первую точку у[1]
            richTextBox1.Text += "t[" + 0 + "] = " + time[0] + ";\n"; //выводим в текст значение времени
            richTextBox1.Text += char.ConvertFromUtf32(945) + "[" + 0 + "] = " + y[0] + ";\n"; //выводим в текст значение у[0] в градусах
            richTextBox1.Text += char.ConvertFromUtf32(969) + "[" + 0 + "] = " + y[1] + ";\n"; //выводим в текст значение у[1] в градусах
            richTextBox1.Text += "---------------------------------\n"; //линия прерывания вывода начального
            for (int i = 1; i < n; i++) //цикл расчета
            {
                time[i] = h * i; //время, умножаем шаг секции на шаг цикла
                y[0] = y[0] + h * y[1]; //решение вектора y[0]
                if (al1 < y[1]) //проверяем не превышает ли вектор y[1] линию угла до ограничения
                {
                    richTextBox1.Text += "Угол больше заданного " + al1 + " < " + y[1] + "\n"; //выводим в сообщении сравнение двух градусов
                    al2 = y[1] - al1;
                    richTextBox1.Text += "Прерывание составляет " + al2 + "\n"; //выводим количество прерывания в градусах
                    y[1] = y[1] + h * ((-g * Math.Sin(y[0])) / L2 - mu * y[0]); //решение вектора y[1] при прерывании
                }
                else //иначе 
                    y[1] = y[1] + h * ((-g * Math.Sin(y[0])) / L - mu * y[0]); //решение вектора y[1] при свободном движении
                richTextBox1.Text += "t[" + i + "] = " + time[i] + ";\n"; //выводим в текст значение времени
                richTextBox1.Text += char.ConvertFromUtf32(945) + "[" + i + "] = " + y[0] + ";\n"; //выводим в текст значение у[0] в градусах
                richTextBox1.Text += char.ConvertFromUtf32(969) + "[" + i + "] = " + y[1] + ";\n"; //выводим в текст значение у[1] в градусах
                richTextBox1.Text += "---------------------------------\n"; //линия прерывания вывода текущего
                chart1.Series[0].Points.AddXY(i * h, y[0]); //рисуем точку y[0]
                chart1.Series[1].Points.AddXY(i * h, y[1]); //рисуем точку у[1]
                button3.Text = "Отмена\nОсталось секций " + (n - i); //вывести количества секций до окончания на кнопку отмены
                Application.DoEvents(); //выполнение в реальном времени
                button3.Focus(); //возвращает true при клике на кнопку отмены
                System.Threading.Thread.Sleep((int)(h)); //установим время на выполнение расчета
                if (click3 == true) //проверяем была ли использована переменная
                    break; //если переменная вернула положительный результат, прерываем расчет
            }
            button1.Enabled = true; //после окончания построения кнопка маятника при свободном движении активна
            button2.Enabled = true; //после окончания построения кнопка маятника при ограничении активна
            button3.Text = "Отмена"; //восстановить описание кнопки отмены после окончания
            button3.Enabled = false; //после окончания построения кнопка отмены неактивна
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = Convert.ToDecimal(numericUpDown1.Text) - (decimal)0.1; //После кликанья утстановим для L2 - максимальная длина на 0.1 меньше чем L1
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = Convert.ToDecimal(numericUpDown1.Text) - (decimal)0.1; //После кликанья утстановим для L2 - максимальная длина на 0.1 меньше чем L1
        }


    }
}
