using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using iTextSharp;
using System.Xml.Linq;
using System.IO;
using System.Net;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Dekosis___KYS
{
    public partial class Form6 : Form
    {
        bool mailSend = false;
        Dictionary<string, string> hocaMail = new Dictionary<string, string>
        {
            { "hoca1", "hoca1@gmail.com" },
            { "hoca2", "hoca2@example.com" },
            { "hoca3", "hoca3@example.com" },
            { "hoca4", "hoca4@example.com" }
        };
        private string ConvertTurkishCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u")
                .Replace("Ç", "C")
                .Replace("Ğ", "G")
                .Replace("İ", "I")
                .Replace("Ö", "O")
                .Replace("Ş", "S")
                .Replace("Ü", "U");
        }

        public Form6()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form6_FormClosed);
            LoadComboBoxes();
        }
        private void Form6_FormClosed(object sender, FormClosedEventArgs e)
        {
            string klasorYolu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arşiv");
            var dosyalar = new DirectoryInfo(klasorYolu).GetFiles();
            var enSonDosya = dosyalar.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

            if (enSonDosya != null && !mailSend)
            {
                enSonDosya.Delete();
            }

            Form2 form2 = Application.OpenForms.OfType<Form2>().FirstOrDefault();
            if (form2 == null)
            {
                form2 = new Form2();
            }
            form2.Show();
        }
        private void LoadComboBoxes()
        {
            comboBox1.Items.AddRange(hocaMail.Keys.ToArray());
            comboBox2.Items.AddRange(new[] {
                "Görüntü  İşleme", "Web Programalama", "Gömülü Sistem Tasarımı", "Ayrık Matematik" 
            });
        }
        private void mailGonder_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir hoca ve ders seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string hocaAdi = comboBox1.SelectedItem.ToString();
            string dersAdi = comboBox2.SelectedItem.ToString();
            string hocaMailAdresi = hocaMail[hocaAdi];

            string arşivKlasoru = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arşiv");

            string xmlDosyasi = GetLatestXmlFile(arşivKlasoru);

            var xmlVerisi = XElement.Load(xmlDosyasi);

            string pdfDosyasi = CreatePdf(xmlVerisi, dersAdi, hocaAdi);

            SendEmail(hocaMailAdresi, pdfDosyasi, dersAdi, hocaAdi);
        }
        private string GetLatestXmlFile(string klasorYolu)
        {
            var dosyalar = new DirectoryInfo(klasorYolu).GetFiles("*.xml");

            if (dosyalar.Length == 0)
            {
                MessageBox.Show("XML dosyası bulunamadı.");
                return null; 
            }

            return dosyalar.OrderByDescending(f => f.LastWriteTime).First().FullName;
        }



        private string CreatePdf(XElement xmlVerisi, string dersAdi, string hocaAdi)
        {
            string projeKonu = AppDomain.CurrentDomain.BaseDirectory;

            string arşivKlasoru = Path.Combine(projeKonu, "Arşiv");

            string yoklamaListeleriKlasoru = Path.Combine(arşivKlasoru, "Yoklama Listeleri");

            Directory.CreateDirectory(yoklamaListeleriKlasoru);

            string tarih = DateTime.Now.ToString("dd.MM.yyyy");

            string pdfDosyaAdi = $"{dersAdi} - {tarih}.pdf";
            string pdfDosyaYolu = Path.Combine(yoklamaListeleriKlasoru, pdfDosyaAdi);

            using (Document doc = new Document())
            {
                PdfWriter.GetInstance(doc, new FileStream(pdfDosyaYolu, FileMode.Create));
                doc.Open();

                Paragraph title = new Paragraph($"Ders: {ConvertTurkishCharacters(dersAdi)} - Hoca: {ConvertTurkishCharacters(hocaAdi)} - Tarih: {tarih} (Mevcutlar)", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                };
                doc.Add(title);

                PdfPTable table = new PdfPTable(3) 
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f,
                    DefaultCell = { MinimumHeight = 25 }
                };

                string[] headers = { "Numara", "Adı", "Soyadı" };
                foreach (var header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD)))
                    {
                        BackgroundColor = new BaseColor(220, 220, 220),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        BorderWidth = 1
                    };
                    table.AddCell(headerCell);
                }

                foreach (var ogrenci in xmlVerisi.Elements("Ogrenci"))
                {
                    string numara = ConvertTurkishCharacters(ogrenci.Element("Numara")?.Value ?? string.Empty);
                    string adi = ConvertTurkishCharacters(ogrenci.Element("Ad")?.Value ?? string.Empty);
                    string soyadi = ConvertTurkishCharacters(ogrenci.Element("Soyad")?.Value ?? string.Empty);

                    table.AddCell(new PdfPCell(new Phrase(numara)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BorderWidth = 1 });
                    table.AddCell(new PdfPCell(new Phrase(adi)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BorderWidth = 1 });
                    table.AddCell(new PdfPCell(new Phrase(soyadi)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BorderWidth = 1 });
                }

                // Tabloyu PDF'e ekleyin
                doc.Add(table);
                doc.Close();
            }

            return pdfDosyaYolu;
        }

        private void SendEmail(string to, string pdfDosyasi, string dersAdi, string hocaAdi)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("gonderici_mailiniz@gmail.com"); 
                mail.To.Add(to);
                mail.Subject = $"{dersAdi} - {hocaAdi} ({DateTime.Now.ToString("dd.MM.yyyy")})"; 
                mail.Body = "İlgili dersin PDF'i ektedir.";
                mail.Attachments.Add(new Attachment(pdfDosyasi));

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) 
                {
                    smtp.Credentials = new NetworkCredential("kocaeliprojesi@gmail.com", "uygulama_sifreniz"); 
                    smtp.EnableSsl = true;

                    try
                    {
                        smtp.Send(mail);
                        MessageBox.Show("E-posta başarıyla gönderildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        mailSend = true;
                        RestartApplication();
                    }
                    catch (SmtpException ex)
                    {
                        MessageBox.Show($"SMTP Hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void RestartApplication()
        {
            string uygulamaYolu = Application.ExecutablePath;

            System.Diagnostics.Process.Start(uygulamaYolu);

            Application.Exit();
        }


    }



}

