using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;
using Font = SixLabors.Fonts.Font;
using Pen = SixLabors.ImageSharp.Drawing.Processing.Pen;
using PointF = SixLabors.ImageSharp.PointF;

using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using static System.Formats.Asn1.AsnWriter;
using System.IO.Ports;
using System.Text;

namespace AGV_YOLOV5
{
    public partial class Form1 : Form
    {
        private Font font;
        private Label label2;
        private YoloScorer<YoloCocoP5Model> scorer;
        public Form1()
        {
            InitializeComponent();
            font = new Font(new FontCollection().Add("C:/Windows/Fonts/consola.ttf"), 16);
            scorer = new YoloScorer<YoloCocoP5Model>("Assets/Weights/yolov5n.onnx");

            // ���� Label �ؼ�
            label2 = new Label();
            label2.Text = "--";
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.Transparent; // ���� Label ������ɫΪ͸��
            label2.ForeColor = System.Drawing.Color.Lime; // ���� Label �ı���ɫ
            label2.Location = new System.Drawing.Point(10, 10); // ���� Label �� PictureBox �ϵ�λ��
            pictureBox2.Controls.Add(label2); // �� Label ������ PictureBox ��

        }


        private Bitmap ImageToBitmap(Image<Rgba32> image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.SaveAsBmp(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new Bitmap(memoryStream);
            }
        }
        private async Task<Int64> ScanObjectAsync(Bitmap bmp)
        {
            var start = DateTime.Now.Ticks;
            // �� System.Drawing.Bitmap ת��Ϊ SixLabors.ImageSharp.Image
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Bmp); // ����ΪPNG��ʽ
                stream.Position = 0;
                var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream);
                {

                    var predictions = scorer.Predict(image);


                    foreach (var prediction in predictions) // draw predictions
                    {
                        var score = Math.Round(prediction.Score, 2);

                        var (x, y) = (prediction.Rectangle.Left - 3, prediction.Rectangle.Top - 23);
                        var pen = new Pen(prediction.Label.Color, 1);
                        image.Mutate(a => a.DrawPolygon(pen, new PointF(prediction.Rectangle.Left, prediction.Rectangle.Top),
                            new PointF(prediction.Rectangle.Right, prediction.Rectangle.Top),
                            new PointF(prediction.Rectangle.Right, prediction.Rectangle.Bottom),
                            new PointF(prediction.Rectangle.Left, prediction.Rectangle.Bottom)
                        ));
                        image.Mutate(a => a.DrawText($"{prediction.Label.Name} ({score})", font, prediction.Label.Color, new PointF(x, y)));
                        label2.Invoke((Action)(() =>
                        {
                            label2.Text = DateTime.Now.ToString() + " " + prediction.Label.Name + "\r\n" + label2.Text;
                            if (label2.Text.Length > 1000) label2.Text = "";
                        }));
                    }
                    // await image.SaveAsync("Assets/zt.jpg");
                    try
                    {
                        await Task.Run(() =>
                        {
                            pictureBox2.Invoke((Action)(() =>
                            {
                                //var start = DateTime.Now.Ticks;
                                pictureBox2.Image = ImageToBitmap(image);
                                //textBox1.Text = ((DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond).ToString();
                            }));

                        });

                    }
                    catch (Exception)
                    {

                        ;
                    }

                }

                return ((DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond);
            }


        }
        private VideoCapture capture;
        private PictureBox pictureBox;

        private void btnOpen_Click(object sender, EventArgs e)
        {

            capture = new VideoCapture(0, VideoCapture.API.DShow); // ʹ��Ĭ�ϵ�����ͷ������Ϊ0��

            capture.Set(CapProp.FrameWidth, 1280);
            capture.Set(CapProp.FrameHeight, 720);
            StartVideoStreamAsync();
        }
        int delay = 30;
        private async void StartVideoStreamAsync()
        {
            while (true)
            {
                try
                {
                    Mat frame = capture.QueryFrame();

                    if (frame == null)
                    {
                        Console.WriteLine("�޷���ȡ����ͷ֡��");
                        return;
                    }
                    int cost = (int)(await ScanObjectAsync(frame.ToBitmap()));
                    // ��Matת��ΪBitmap������PictureBox����ʾ����UI�߳��ϣ�
                    //await Task.Run(() =>
                    //{
                    //    Bitmap bmp = frame.ToBitmap();
                    //    //pictureBox1.Invoke((Action)(() =>
                    //    //{
                    //    //    pictureBox1.Image = bmp;
                    //    //}));

                    //});
                    await Task.Delay(40); // Լ30֡ÿ��

                    // ��Ӷ����ӳ��Կ���֡��
                    //if (cost<30)
                    //{
                    //    await Task.Delay(30-cost); // Լ30֡ÿ��
                    //}


                }
                catch (Exception ex)
                {

                    ;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (capture != null) capture.Stop();
                isListening = false;
                if (serialPort != null) serialPort.Close();
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }
        SerialPort serialPort;
        static bool isListening = true;
        private void btnTemp_Click(object sender, EventArgs e)
        {
            serialPort = new SerialPort(txtCOM.Text, 9600); // �滻Ϊʵ�ʵĴ��ںźͲ�����
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                serialPort.Open();
                Console.WriteLine("�����Ѵ򿪡�");

                // ���������߳�
                Thread sendThread = new Thread(SendData);
                sendThread.Start();

                //Console.ReadLine(); // ���ֳ�������

                //// ֹͣ����
                //isListening = false;
                //sendThread.Join(); // �ȴ������߳����

                //Console.WriteLine("������ֹͣ��");
            }
            catch (Exception ex)
            {
                Console.WriteLine("���ڴ�ʧ�ܣ�" + ex.Message);
            }
            finally
            {
                //serialPort.Close();
            }
        }
        private void SendData()
        {
            while (isListening)
            {
                // ����Ҫ���͵ı���
                byte[] sendMessage = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0A };

                // ���ͱ��ĵ�����
                serialPort.Write(sendMessage, 0, sendMessage.Length);

                Thread.Sleep(1000); // ÿ��1�뷢��һ��
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;

            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);

            // ��������Ƿ�Ϊ�̶����ȣ��Լ�ȷ�����㹻�����ݿɹ�����
            if (buffer.Length == 7)
            {
                // �������ĺ͵����ֽڵ��¶�����
                byte byte4 = buffer[3];
                byte byte5 = buffer[4];

                int temperature = (byte4 << 8) | byte5; // �������ֽ����Ϊһ������



                // ���� Label �ؼ�
                label2.Invoke((Action)(() =>
                {
                    label2.Text = DateTime.Now.ToString() + " ��ǰ�¶ȣ�" + temperature / 10 + "��\r\n" + label2.Text;
                    if (label2.Text.Length > 1000) label2.Text = "";
                }));
                //Console.WriteLine("�¶����ݣ� " + temperature + " ��");
            }
            else
            {
                Console.WriteLine("��Ч�����ݳ��ȡ�");
            }
        }
        private void SerialPort_DataReceivedLight(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            int bytesToRead = sp.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            sp.Read(buffer, 0, bytesToRead);
            string asciiString = Encoding.ASCII.GetString(buffer);

            // ���� Label �ؼ�
            label2.Invoke((Action)(() =>
            {
                label2.Text = DateTime.Now.ToString() + " ��ǰ����ǿ�ȣ�" + asciiString + "\r\n" + label2.Text;
                if (label2.Text.Length > 1000) label2.Text = "";
            }));


        }
        SerialPort serialPortLight;
        static bool LightListening = true;
        private void button1_Click(object sender, EventArgs e)
        {
            serialPortLight = new SerialPort(txtCOML.Text, 9600); // �滻Ϊʵ�ʵĴ��ںźͲ�����
            serialPortLight.DataReceived += SerialPort_DataReceivedLight;
            try
            {
                serialPortLight.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("���ڴ�ʧ�ܣ�" + ex.Message);
            }
        }
    }
}