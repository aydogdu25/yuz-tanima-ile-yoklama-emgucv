using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.Structure;


namespace Dekosis___KYS
{
    
    public partial class Form4 : Form
    {
        MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 0.6, 0.6);
        HaarCascade faceDetacted;
        Image<Bgr, Byte> Frame;
        Capture camera;
        Image<Gray, byte> result;
        Image<Gray, byte> TrainedFace = null;
        Image<Gray, byte> grayFace = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> Users = new List<string>();
        int Count, NumLabels, t;
        string name, names = null;
        private XDocument yoklamaDoc;
        private XElement rootElement;
        private string dosyaYolu;
        bool bitir = false;
        Form2 form2;

        public Form4()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form4_FormClosed);
            faceDetacted = new HaarCascade("haarcascade_frontalface_default.xml");
            label1.Hide();
            KlasorVeDosyaOlustur();
            bitirButon.Enabled = false;
            okButton.Enabled = false;
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
        }

        private void FrameProcedure(object sender, EventArgs e)
        {
            Frame = camera.QueryFrame().Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            grayFace = Frame.Convert<Gray, Byte>();

            MCvAvgComp[][] faceDetectedNow = grayFace.DetectHaarCascade(faceDetacted, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

            foreach (MCvAvgComp item in faceDetectedNow[0])
            {
                result = Frame.Copy(item.rect).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                Frame.Draw(item.rect, new Bgr(Color.Green), 3);

                string name = "Tanimsiz"; 

                if (trainingImages.Count > 0)
                {
                    MCvTermCriteria termCriterias = new MCvTermCriteria(Count, 0.001);
                    EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), 1500, ref termCriterias);

                    name = recognizer.Recognize(result);

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "Tanimsiz";
                    }
                    else
                    {
                        float[] distances = recognizer.GetEigenDistances(result);
                        float minDistance = distances.Min();

                        if (minDistance > 2000)
                        {
                            name = "Tanimsiz";
                        }
                    }
                }
                if (name == "Tanimsiz")
                {
                    label1.BackColor = Color.FromArgb(192, 0, 0);
                    label1.ForeColor = Color.Transparent;
                    label1.Text = "Öğrenci Kayıtlı Değil!";
                    okButton.Enabled = false;
                }
                else
                {
                    label1.BackColor = Color.ForestGreen;
                    label1.ForeColor = Color.Transparent;
                    label1.Text = name;

                    if (bitir)
                    {
                        okButton.Enabled = false;
                    }
                    else
                    {
                        okButton.Enabled = true;
                    }
                }

                Frame.Draw(name, ref font, new Point(item.rect.X - 2, item.rect.Y - 2), new Bgr(Color.Red));
            }

            cameraBox.Image = Frame;
        }




        private void KlasorVeDosyaOlustur()
        {
            string klasorYolu = Path.Combine(Environment.CurrentDirectory, "Arşiv");
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }

            dosyaYolu = Path.Combine(klasorYolu, DateTime.Now.ToString("dd.MM.yyyy_HHmmss") + ".xml");

            rootElement = new XElement("Yoklama");
            yoklamaDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rootElement);
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            if (label1.Text != "Kayıtlı Değil!")
            {
                var labelText = label1.Text.Split(new[] { " - " }, StringSplitOptions.None);

                if (labelText.Length == 2)
                {
                    var adSoyad = labelText[0].Split(' ');
                    var numara = labelText[1];

                    if (adSoyad.Length >= 2)
                    {
                        var ad = adSoyad[0];
                        var soyad = adSoyad[1];

                        bool mevcutKayitVar = rootElement.Elements("Ogrenci").Any(o =>
                            (string)o.Element("Ad") == ad &&
                            (string)o.Element("Soyad") == soyad &&
                            (string)o.Element("Numara") == numara
                        );

                        if (mevcutKayitVar)
                        {
                            MessageBox.Show("Yoklama zaten alınmış!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            XElement ogrenciElement = new XElement("Ogrenci",
                                new XElement("Ad", ad),
                                new XElement("Soyad", soyad),
                                new XElement("Numara", numara)
                            );

                            rootElement.Add(ogrenciElement);
                            MessageBox.Show("Başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            bitirButon.Enabled = true;
                        }
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            bitir = true;
            yoklamaDoc.Save(dosyaYolu);
            MessageBox.Show("Yoklama başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            okButton.Enabled = false;
            bitirButon.Enabled = false;
            Form5 form5 = new Form5();
            form5.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StopCamera();
            Form1.form2.Show();
            this.Close();
        }

      

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopCamera();
            Form1.form2.Show();
        }

        private void camStart_Click(object sender, EventArgs e)
        {
            if (camera == null)
            {
                camStart.Enabled = false;
                camera = new Capture();
                camera.QueryFrame();
                Application.Idle += new EventHandler(FrameProcedure);
                label1.Show();
                okButton.Enabled = true;
            }
        }
        private void StopCamera()
        {
            if (camera != null)
            {
                Application.Idle -= FrameProcedure; 
                camera.Dispose(); 
                camera = null;
                camStart.Enabled = true;
                label1.Hide();
                okButton.Enabled = false;
            }
        }

    }
}
