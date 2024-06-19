using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NicePaint
{
    public partial class Form1 : Form
    {
        int index = 0; //переменная инструментов

        int size = 1; //переменная размера пера

        int x1, y1; //координаты

        static Color color = Color.Black; //переменная цвета

        Pen p = new Pen(color); //перо
        
        
        
        

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height); 
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round; 
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedItem = 0;
        }
                  
        /// <summary>
        /// Изменять цвет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Dialog.ShowDialog() == DialogResult.OK && index == 1) 
            {
                comboBox1.SelectedIndex = 0;
            }           
            color = Dialog.Color;
            p.Color = color;
        }
        /// <summary>
        /// Изменяет размер карандаша
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            size = (int)numericUpDown1.Value;
            p.Width = size;
        }


        /// <summary>
        /// Обработчика события "движение мыши"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) //нужен для рисования карандашом и ластиком
        {
            try
            {
                if (index == 0 || index == 1)
                {
                    if (e.Button == MouseButtons.Left) //если зажата ЛКМ
                    {
                        Graphics gr = Graphics.FromImage(pictureBox1.Image);
                        gr.DrawLine(p, e.X, e.Y, x1, y1); //рисуем линию между ближайшими пикселями
                        x1 = e.X; //делаем конечные координаты линии начальными
                        y1 = e.Y;
                        pictureBox1.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                         
        }
    
        /// <summary>
        /// Метод заливки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rgb"></param>
        public void Fill(int x, int y, int rgb)
        {
            try
            {
                if (x > pictureBox1.Width - 1) return; //ограничения холста
                if (x < 1) return;
                if (y > pictureBox1.Height - 1) return;
                if (y < 1) return;
                Bitmap b = (Bitmap)pictureBox1.Image; //создаём биткарту
                int x1 = x; //дублируем переменную, чтобы не потерять её значение
                while (rgb == b.GetPixel(x1, y).ToArgb() && x1 > 1 && x1 < pictureBox1.Width - 1) //ищем правую границу области заливки
                {
                    x1++;
                }
                int x2 = x;
                while (rgb == b.GetPixel(x2, y).ToArgb() && x2 > 1 && x2 < pictureBox1.Width - 1) //ищем левую границу
                {
                    x2--;
                }
                x1--;
                x2++;
                Graphics g = Graphics.FromImage(pictureBox1.Image); //рисуем линию
                g.DrawLine(p, x1, y, x2, y);
                pictureBox1.Invalidate();
                b = (Bitmap)pictureBox1.Image;
                if (y + 1 < pictureBox1.Height - 1) //проверяем, возможно ли двигаться вниз
                {                                   //и запускаем рекурсию 
                    int x3 = x1;
                    x3--;
                    while (x3 > x2)
                    {
                        if (b.GetPixel(x3, y + 1).ToArgb() == rgb)
                        {
                            Fill(x3, y + 1, rgb);
                            b = (Bitmap)pictureBox1.Image;
                        }
                        x3--;
                    }

                }
                if (y - 1 > 0) //аналогично вверх 
                {
                    int x4 = x1;
                    x4--;
                    while (x4 > x2)
                    {
                        if (b.GetPixel(x4, y - 1).ToArgb() == rgb)
                        {
                            Fill(x4, y - 1, rgb);
                            b = (Bitmap)pictureBox1.Image;
                        }
                        x4--;
                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
          
        }
        /// <summary>
        /// Выбор инструмента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = comboBox1.SelectedIndex;
            if (index != 1 && index != 2) //для всех, кроме ластика и заливки
            {
                p.Color = color;
                p.Width = size;
            }
            if (index == 1) //ластик
            {               
                p.Color = Color.White;
                p.Width = size;
            }
            if (index == 2) //заливка
            {
                p.Color = color;
                p.Width = 1;
            }
            

        }
        /// <summary>
        /// Очистка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) 
        {
            Bitmap n = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = n;
        }
        /// <summary>
        /// Сохранить рисунок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.Cancel) //вызывает saveFileDialog
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        Bitmap pic = new Bitmap(pictureBox1.Image);
                        pic.Save(saveFileDialog1.FileName);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
            
            
            
        }
        /// <summary>
        /// Загрузить изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
                {
                    if (openFileDialog1.FileName != "")
                    {

                        Bitmap pic = (Bitmap)Image.FromFile(openFileDialog1.FileName);                       
                        pictureBox1.Image = new Bitmap(pic, new Size(pictureBox1.Width, pictureBox1.Height));
                        
                       

                        
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
           
            
        }
        /// <summary>
        /// Обработчик события отпускания кнопки мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (index == 3) //рисует линию
                {
                    Graphics g = Graphics.FromImage(pictureBox1.Image);
                    g.DrawLine(p, x1, y1, e.X, e.Y);
                    pictureBox1.Invalidate();
                }
                if (index == 4) //рисует прямоугольник
                {
                    Graphics g = Graphics.FromImage(pictureBox1.Image);
                    g.DrawRectangle(p, Math.Min(x1, e.X), Math.Min(y1, e.Y), Math.Abs(x1 - e.X), Math.Abs(y1 - e.Y));
                    pictureBox1.Invalidate();
                }
                if (index == 5) //рисует эллипс
                {
                    Graphics g = Graphics.FromImage(pictureBox1.Image);
                    g.DrawEllipse(p, Math.Min(x1, e.X), Math.Min(y1, e.Y), Math.Abs(x1 - e.X), Math.Abs(y1 - e.Y));
                    pictureBox1.Invalidate();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }
        /// <summary>
        /// Обработчик события нажания на кнопку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                x1 = e.X; //запаминаем координаты 
                y1 = e.Y;
                if (index == 2) //если выбрана заливка
                {
                    if (e.Button == MouseButtons.Left) //если нажата левая кнопка мыши
                    {
                        Bitmap c = (Bitmap)pictureBox1.Image;
                        if (c.GetPixel(e.X, e.Y).ToArgb() != color.ToArgb()) //и выбранный цвет отличается 
                        {
                            Fill(e.X, e.Y, c.GetPixel(e.X, e.Y).ToArgb()); //выполняем заливку
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }


    }
}
