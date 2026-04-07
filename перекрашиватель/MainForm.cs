using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace перекрашиватель
{
	public partial class MainForm : Form
	{
		Bitmap b;
		Graphics gr;
		Bitmap b2;
		Graphics gr2;
		double x;
		double y;
		double k;
		int Lx;
		int Ly;
		double a;
		double c;

		public MainForm()
		{
			InitializeComponent();
			this.MouseWheel += new MouseEventHandler(this.Form1_MouseWhel);
			b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			gr = Graphics.FromImage(b);
			b2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			gr2 = Graphics.FromImage(b2);
			k = 1;
			//gr.Clear(Color.White);
			pictureBox1.Image = b2;
			timer1.Start();
			Draw();
		}

		void Form1_MouseWhel(object sender, MouseEventArgs e) //вращение колёсика мыши
		{
			if (e.Delta < 0) k *= 1.35;
			else if (k > 0.0001) k /= 1.35;
		}

		void pictureBox1_MouseDown(object sender, MouseEventArgs e) { Lx = e.X; Ly = e.Y; }
		void pictureBox1_MouseMove(object sender, MouseEventArgs e) //перетаскивание мышью
		{
			if (e.Button == MouseButtons.Left)
			{
				x -= (e.X - Lx) * k;
				y -= (e.Y - Ly) * k;
				Lx = e.X;
				Ly = e.Y;
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Draw();
		}

		void Draw() //отрисовка
		{
			gr2.Clear(Color.FromArgb(0, 0, 0));
			gr2.DrawImage
			(
				b,
				Convert.ToInt32((0 - b.Width / 2 - x) / k + pictureBox1.Width / 2),
				Convert.ToInt32((0 - b.Height / 2 - y) / k + pictureBox1.Height / 2),
				Convert.ToInt32(b.Width / k),
				Convert.ToInt32(b.Height / k)
			);
			pictureBox1.Image = b2;
		}

		void MainFormSizeChanged(object sender, EventArgs e) //изменение размеров окна
		{
			pictureBox1.Width = this.Width - 150;
			pictureBox1.Height = this.Height;
			if (pictureBox1.Width != 0 && pictureBox1.Height != 0)
			{
				b2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
				gr2 = Graphics.FromImage(b2);
			}
			Draw();
		}

		void Button1Click(object sender, EventArgs e) //загрузить
		{
			if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
			{
				b = new Bitmap(Image.FromFile(openFileDialog1.FileName).Width, Image.FromFile(openFileDialog1.FileName).Height);
				gr = Graphics.FromImage(b);
				gr.DrawImage(Image.FromFile(openFileDialog1.FileName),0 ,0 , Image.FromFile(openFileDialog1.FileName).Width, Image.FromFile(openFileDialog1.FileName).Height);
			}
			pictureBox1.Image = b;
		}

		void Button2Click(object sender, EventArgs e) //сохранить
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string st = "";
				for (int i = 0; i < saveFileDialog1.FileName.Length && saveFileDialog1.FileName[i] != '.'; i++) st += saveFileDialog1.FileName[i];
				saveFileDialog1.FileName = st;
				b.Save((saveFileDialog1.FileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
				//try
				//{
				//	b.Save((saveFileDialog1.FileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
				//}
				//catch
				//{
				//	MessageBox.Show("Impossible to save image", "FATAL ERROR",
				//	MessageBoxButtons.OK, MessageBoxIcon.Error);
				//}
			}
		}

		void Button3Click(object sender, EventArgs e) //перекрасить
		{
			timer1.Stop();
			a = 0;
			c = b.Height * b.Width;
			progressBar1.Value = 0;
			label5.Text = "0%";
			progressBar1.Visible = true;
			label5.Visible = true;
			for (int j = 0; j < b.Height; j++)
			for(int i = 0; i < b.Width; i++)
			{
				Color cl = b.GetPixel(i,j);
				int A = 0;
				if(textBox4.Enabled) A = Calculation(textBox4.Text, cl);
				else A = cl.A;
				int R = 0;
				if(textBox1.Enabled) R = Calculation(textBox1.Text, cl);
				else R = cl.R;
				int G = 0;
				if(textBox2.Enabled) G = Calculation(textBox2.Text, cl);
				else G = cl.G;
				int B = 0;
				if(textBox3.Enabled) B = Calculation(textBox3.Text, cl);
				else B = cl.B;
				b.SetPixel(i,j,Color.FromArgb(A, R, G, B));
				progressBar1.Value = Convert.ToInt32(++a / c * 100);
				//label5.Text = (a / c * 100) + "%";
				//b.SetPixel(i,j,Color.FromArgb((cl.R + cl.G)/2, (cl.R + cl.G) / 2, cl.B));
				//b.SetPixel(i,j,Color.FromArgb(Convert.ToInt32(Math.Sqrt((double)cl.R * (double)cl.G)), Convert.ToInt32(Math.Sqrt((double)cl.G * (double)cl.B)), Convert.ToInt32(Math.Sqrt((double)cl.B * (double)cl.R))));
				//b.SetPixel(i,j,Color.FromArgb(cl.G, cl.B, cl.R));
				}
			pictureBox1.Image = b;
			progressBar1.Visible = false;
			label5.Visible = false;
			Draw();
			timer1.Start();
		}

		int Calculation(string formula, Color color) //вычисление по формуле
        {
			string f = "";
			for (int i = 0; i < formula.Length; i++) if (formula[i] != ' ') f += formula[i]; //удаление прабелов
			if (true) //замена букв на числа
			{
				string f1 = "";
				for (int i = 0; i < f.Length; i++) if (f[i] != 'A' && f[i] != 'R' && f[i] != 'G' && f[i] != 'B') f1 += f[i];
					else
					{
						if (f[i] == 'A') f1 += color.A;
						if (f[i] == 'R') f1 += color.R;
						if (f[i] == 'G') f1 += color.G;
						if (f[i] == 'B') f1 += color.B;
					}
				f = f1;
			}
			string namSt = "";
			double nam1;
			double nam2;
			int n = 0; int n1 = 0; int n2 = 0;
			while(true) //вычесление
			{
				n = 0; //проверка на наличие действий
				while (n < f.Length && f[n] != '(' && f[n] != '+' && f[n] != '*' && f[n] != '/' && f[n] != '^' && (n == 0 || f[n] != '-')) n++;
				if (n == f.Length) break; //выход из цикла
				n = 0;
				n1 = 0;
				n2 = 0;
				while (n < f.Length-1 && f[n] != ')' ) n++; //поиск скобок
				while (n > -1 && f[n] != '(') n--;
				int n3 = n; //кордината скобки
				while (++n < f.Length-1 && f[n] != ')') //поиск действий
				{
					if (n2 < 1 && f[n] == '-') { n1 = n; }
					if (n2 < 1 && f[n] == '+') { n2 = 1; n1 = n; }
					if (n2 < 2 && (f[n] == '*' || f[n] == '/')) { n2 = 2; n1 = n; }
					if (n2 < 3 && f[n] == '^') { n2 = 3; n1 = n; }
				}
				if (n1 > n3+1 && n2 < 1) n2 = 1; //исключение на отрицательные числа
				if (n2 < 1) //раскрытие скобок
				{
					string f1 = "";
					for (int i = 0; i < f.Length; i++) if (i != n) f1 += f[i];
					f = f1;
					f1 = "";
					n--;
					while (n > -1 && f[n] != '(') n--;
					for (int i = 0; i < f.Length && n > -1; i++) if (i != n) f1 += f[i];
					f = f1;
					n = 0;
				}
				else //выполнение операции
				{
					char act = f[n1]; //знак операции
					n = n1 - 1;
					while (n >= 0 && f[n] != '(' && f[n] != '+' && f[n] != '*' && f[n] != '/' && f[n] != '^' && (n2 < 3 || f[n] != '-')) n--;
					n1 = n + 1; //нахождение первого числа
					while (++n < f.Length-1 && f[n] != ')' && f[n] != '+' && f[n] != '*' && f[n] != '/' && f[n] != '^' && (n1 == n || f[n] != '-'))
					{ //замена точек на запятые
						if (f[n] != '.') namSt += f[n];
						else namSt += ',';
					}
					nam1 = Convert.ToDouble(namSt);
					namSt = "";
					n2 = n + 1; //нахождение второго числа
					while (++n < f.Length && f[n] != ')' && f[n] != '+' && f[n] != '*' && f[n] != '/' && f[n] != '^' && (n2 == n || f[n] != '-'))
					{
						if (f[n] != '.') namSt += f[n];
						else namSt += ',';
					}
					n2 = n - 1;
					nam2 = Convert.ToDouble(namSt);
					namSt = "";
					if (act == '+') nam1 += nam2; //выполнение соответствующего действия
					if (act == '-') nam1 -= nam2;
					if (act == '*') nam1 *= nam2;
					if (act == '/') nam1 /= nam2;
					if (act == '^') nam1 = Math.Pow(nam1, nam2);
					string f1 = "";
					for (int i = 0; i < f.Length; i++) //замена записи действия между 2-я числами на результат
					{
						if (i < n1 || i > n2) f1 += f[i];
						else
						{
							f1 += nam1;
							i = n2;
						}
					}
					f = f1; 
					n = 0; //обнуление переменных
					n1 = 0;
					n2 = 0;
				}
			}
			string f2 = "";
			for (int i = 0; i < f.Length; i++) //замена точек на запятые
			{
				if (f[i] != '.') f2 += f[i];
				else f2 += ',';
			}
			int value = Convert.ToInt32(Convert.ToDouble(f2)); //конвертация записи числа в число
			if (value < 0) value = 0;
			if (value > 255) value = 255;
			return value; 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
			textBox4.Enabled = !textBox4.Enabled;
			if (!textBox4.Enabled) textBox4.Text = "A";
		}

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
			textBox1.Enabled = !textBox1.Enabled;
			if (!textBox1.Enabled) textBox1.Text = "R";
		}

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
			textBox2.Enabled = !textBox2.Enabled;
			if (!textBox2.Enabled) textBox2.Text = "G";
		}

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
			textBox3.Enabled = !textBox3.Enabled;
			if (!textBox3.Enabled) textBox3.Text = "B";
		}
    }
}
