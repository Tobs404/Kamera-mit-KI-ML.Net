using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using WinFormsApp1ML.Model;

namespace WinFormsApp1
{   /*
    Kurzes Vorab das hier ist 2019 ML.Net Version
    Das ist Version 1/2 mit Manuelem Caputre andere Version hat einen einfachen Timer und macht alle 3 sek ein Fotot und Predicted
    Dieses KI ist die auf dem neuesten Stand
    Andere Version wird nicht mitkomentiert da sehr ähnlich bis auf Timer
    Scource Code für beide auf Git Link dazu im offiziellen Doc.
    */
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // initialisiert componenten wie Butoon etc.
        }

        VideoCaptureDevice CAMERA;
        string saveDirectory = @"C:\Users\z004tyry\Downloads"; //wo Daten gespeichert werden sollen
        string fileName = "TmpPic.jpg"; //wie die Temp File gespeichert wird


        private void FmWebcamera_Load(object sender, EventArgs e)
        {
            // Vorbereiten der Kamera
        }

        private void Captured(object sender, NewFrameEventArgs eventArgs) // Umwandlung die Bitmap und weitergabe
        {
            Bitmap bmp;
            bmp = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = (eventArgs.Frame.Clone() as Bitmap);
        }

        private void FmWebcamera_FormClosing(object sender, FormClosingEventArgs e) // Stoppen der Kamera damit man rescourcen sparren kann
        {
            if (CAMERA != null && CAMERA.IsRunning)
            {
                CAMERA.Stop();
            }

            string filePath = Path.Combine(saveDirectory, fileName);

            if (File.Exists(filePath)) // Falls die File vorhanden ist wird sie hier gelöscht
            {
                try
                {
                    File.Delete(filePath);
                    MessageBox.Show("Temporary file deleted.");
                }
                catch (IOException ioExp)
                {
                    MessageBox.Show($"Error deleting temporary file: {ioExp.Message}");
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e) // eventhandler für button mit Namen Start der die Kamera startetd und das select Menu öffnet
        {
            VideoCaptureDeviceForm camera = new VideoCaptureDeviceForm();
            if (camera.ShowDialog() == DialogResult.OK)
            {
                CAMERA = camera.VideoDevice;
                CAMERA.NewFrame += new NewFrameEventHandler(Captured);
                CAMERA.Start();
            }
        }

        private void button2_Click_1(object sender, EventArgs e) // Caputre Button der die das Gewünschte Bild Cloned
        {
            pictureBox2.Image = pictureBox1.Image;
        }


        private void button4_Click(object sender, EventArgs e) // Eventhandler für Button Predict hier wird der KI bescheid gegeben das sie erraten soll was im Bild ist
        {

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string filePath = Path.Combine(saveDirectory, fileName);

            pictureBox2.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            if (File.Exists(filePath))
            {
                var input = new ModelInput { ImageSource = filePath }; // hier started die KI
                var result = ConsumeModel.Predict(input);               //output

                MessageBox.Show($"Prediction: {result.Prediction}");
            }
            else
            {
                MessageBox.Show("Image not found. Please capture and save an image first."); // error Handling
                
            }
        }
    }
}