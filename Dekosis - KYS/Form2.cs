using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Dekosis___KYS
{
    public partial class Form2 : Form
    {
        int Count, NumLabels, t;
        List<string> labels = new List<string>();
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        public Form2()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.FormClosed += OtherForm_FormClosed; 
            form3.Show();
            this.Hide(); 

        }
        private void OtherForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            this.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string xmlFilePath = Application.StartupPath + "/Faces/ogrenciler.xml";

                
                if (!System.IO.File.Exists(xmlFilePath))
                {
                    new XDocument(new XElement("Ogrenciler")).Save(xmlFilePath);
                }

                XDocument xmlDoc = XDocument.Load(xmlFilePath);
                var students = xmlDoc.Root.Elements("Ogrenci");
                NumLabels = students.Count();
                Count = NumLabels;

                foreach (var student in students)
                {
                    string studentNo = student.Element("Numara")?.Value;
                    string studentName = student.Element("Ad")?.Value;
                    string studentSurname = student.Element("Soyad")?.Value;
                    string studentFolder = Path.Combine(Application.StartupPath, "Faces", studentNo);

                    if (Directory.Exists(studentFolder))
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            string faceFile = Path.Combine(studentFolder, $"face_{i}.bmp");
                            if (File.Exists(faceFile))
                            {
                                trainingImages.Add(new Image<Gray, byte>(faceFile));
                                labels.Add($"{studentName} {studentSurname} - {studentNo}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (trainingImages.Count >= 20) 
            {
                Form4 form4 = new Form4();
                form4.FormClosed += OtherForm_FormClosed; 
                form4.Show();
                this.Hide(); 
            }
            else
            {
                MessageBox.Show("Birden fazla öğrenci kaydı olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
