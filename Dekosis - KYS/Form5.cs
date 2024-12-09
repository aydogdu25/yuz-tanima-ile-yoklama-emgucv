using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dekosis___KYS
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form5_FormClosed);
        }
        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form2 form2 = new Form2();
            string klasorYolu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arşiv");
            var dosyalar = new DirectoryInfo(klasorYolu).GetFiles();
            var enSonDosya = dosyalar.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            if (enSonDosya != null)
            {
                enSonDosya.Delete();
            }

            if (form2 == null || form2.IsDisposed)
            {
                form2 = new Form2();
            }
            form2.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "admin")
            {
                Form6 form6 = new Form6();
                form6.Show();
                this.Hide();
            }
            else if (textBox1.Text == "")
            {
                MessageBox.Show("Lütfen parola giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Parola yanlış", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "";
            }
        }
    }
}

