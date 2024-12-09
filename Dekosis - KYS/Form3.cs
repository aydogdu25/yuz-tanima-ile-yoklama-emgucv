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
using Emgu.CV;
using Emgu.CV.Structure;
using System.Xml;
using System.Xml.Linq;
using System.Threading;

namespace Dekosis___KYS
{
    public partial class Form3 : Form
    {
        List<MCvHistogram> savedFaceDescriptors = new List<MCvHistogram>();

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
        Form2 form2;


        private bool ContainsInvalidCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return true; 
            }

            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return true; 
                }
            }

            return false;
        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxIsım.Text) || string.IsNullOrWhiteSpace(txtNo.Text) || string.IsNullOrWhiteSpace(txtSoyisim.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (btnStart.Enabled)
            {
                MessageBox.Show("Lütfen önce kamerayı başlatınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string studentNo = txtNo.Text.Trim();
            string studentName = textBoxIsım.Text.Trim();
            string studentSurname = txtSoyisim.Text.Trim();

            if (ContainsInvalidCharacters(studentNo))
            {
                MessageBox.Show("Öğrenci numarası geçersiz karakterler içeriyor!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string xmlFilePath = Path.Combine(Application.StartupPath, "Faces", "ogrenciler.xml");

            XDocument xmlDoc = File.Exists(xmlFilePath) ? XDocument.Load(xmlFilePath) : new XDocument(new XElement("Ogrenciler"));
            bool studentExists = xmlDoc.Root.Elements("Ogrenci").Any(x => x.Element("Numara")?.Value == studentNo);

            if (studentExists)
            {
                MessageBox.Show("Bu öğrenci numarası zaten kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            grayFace = camera.QueryGrayFrame().Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            MCvAvgComp[][] detectedFaces = grayFace.DetectHaarCascade(faceDetacted, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

            Image<Gray, byte> referenceFace = null;
            foreach (MCvAvgComp item in detectedFaces[0])
            {
                referenceFace = grayFace.Copy(item.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                break;
            }

            if (referenceFace == null)
            {
                MessageBox.Show("Yüz algılanamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string trainingMatrixPath = Path.Combine(Application.StartupPath, "Faces", "TrainingMatrix.xml");
            if (File.Exists(trainingMatrixPath))
            {
                XDocument trainingDoc = XDocument.Load(trainingMatrixPath);
                foreach (var existingMatrixElement in trainingDoc.Root.Elements("FaceMatrix"))
                {
                    string[] matrixValues = existingMatrixElement.Element("Matrix").Value.Split(',');
                    double[] existingMatrix = Array.ConvertAll(matrixValues, Double.Parse);

                    double similarity = CalculateSimilarity(referenceFace, existingMatrix);

                    if (similarity < 3600) 
                    {
                        MessageBox.Show("Bu yüz başka bir öğrenciye ait!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            XElement studentElement = new XElement("Ogrenci",
                new XElement("Numara", studentNo),
                new XElement("Ad", studentName),
                new XElement("Soyad", studentSurname)
            );
            xmlDoc.Root.Add(studentElement);
            xmlDoc.Save(xmlFilePath);

            double[] referenceMatrix = GetMatrix(referenceFace);
            string referenceMatrixString = string.Join(",", referenceMatrix); 
            XDocument trainingMatrixDoc = File.Exists(trainingMatrixPath) ? XDocument.Load(trainingMatrixPath) : new XDocument(new XElement("TrainingMatrices"));
            XElement matrixElement = new XElement("FaceMatrix",
                new XElement("Numara", studentNo),
                new XElement("Matrix", referenceMatrixString)
            );
            trainingMatrixDoc.Root.Add(matrixElement);
            trainingMatrixDoc.Save(trainingMatrixPath);

            string studentFolder = Path.Combine(Application.StartupPath, "Faces", studentNo);
            if (!Directory.Exists(studentFolder))
            {
                Directory.CreateDirectory(studentFolder);
            }

            for (int i = 1; i <= 10; i++)
            {
                string faceImagePath = Path.Combine(studentFolder, $"face_{i}.bmp");
                referenceFace.Save(faceImagePath);
                Thread.Sleep(100);
            }

            MessageBox.Show("Öğrenci bilgileri ve yüz görüntüleri başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            textBoxIsım.Text = "";
            txtNo.Text = "";
            txtSoyisim.Text = "";
            
        }

        private double[] GetMatrix(Image<Gray, byte> faceImage, int sampleRate = 6)
        {
            List<double> matrixValues = new List<double>();

            for (int y = 0; y < faceImage.Height; y += sampleRate)
            {
                for (int x = 0; x < faceImage.Width; x += sampleRate)
                {
                    matrixValues.Add(faceImage.Data[y, x, 0]);
                }
            }

            return matrixValues.ToArray();
        }


        private double CalculateSimilarity(Image<Gray, byte> newFace, double[] existingMatrix)
        {
            double[] newMatrixValues = GetMatrix(newFace);

            if (newMatrixValues.Length != existingMatrix.Length)
            {
                return double.MaxValue; 
            }

            double mse = 0; 
            for (int i = 0; i < newMatrixValues.Length; i++)
            {
                mse += Math.Pow(newMatrixValues[i] - existingMatrix[i], 2);
            }

            mse /= newMatrixValues.Length; 

            return mse;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (camera == null)
            {
                btnStart.Enabled = false;
                camera = new Capture();
                camera.QueryFrame();
                Application.Idle += new EventHandler(FrameProcedure);
            }
            
        }
        private void StopCamera()
        {
            if (camera != null)
            {
                Application.Idle -= FrameProcedure; 
                camera.Dispose(); 
                camera = null;    
                btnStart.Enabled = true; 
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
                Frame.Draw(item.rect, new Bgr(Color.Red), 3);
            }
            cameraBox.Image = Frame;
        }

        public Form3()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form3_FormClosed);
            faceDetacted = new HaarCascade("haarcascade_frontalface_default.xml");
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopCamera();
            Form1.form2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StopCamera();
            Form1.form2.Show();
            this.Close();
        }

    }
}