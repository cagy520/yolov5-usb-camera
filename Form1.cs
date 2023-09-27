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

            // 创建 Label 控件
            label2 = new Label();
            label2.Text = "--";
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.Transparent; // 设置 Label 背景颜色为透明
            label2.ForeColor = System.Drawing.Color.Lime; // 设置 Label 文本颜色
            label2.Location = new System.Drawing.Point(10, 10); // 设置 Label 在 PictureBox 上的位置
            pictureBox2.Controls.Add(label2); // 将 Label 放置在 PictureBox 上

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
            // 将 System.Drawing.Bitmap 转换为 SixLabors.ImageSharp.Image
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Bmp); // 保存为PNG格式
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

            capture = new VideoCapture(0, VideoCapture.API.DShow); // 使用默认的摄像头（索引为0）

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
                        Console.WriteLine("无法读取摄像头帧。");
                        return;
                    }
                    int cost = (int)(await ScanObjectAsync(frame.ToBitmap()));
                    // 将Mat转换为Bitmap，并在PictureBox中显示（在UI线程上）
                    //await Task.Run(() =>
                    //{
                    //    Bitmap bmp = frame.ToBitmap();
                    //    //pictureBox1.Invoke((Action)(() =>
                    //    //{
                    //    //    pictureBox1.Image = bmp;
                    //    //}));

                    //});
                    await Task.Delay(40); // 约30帧每秒

                    // 添加短暂延迟以控制帧率
                    //if (cost<30)
                    //{
                    //    await Task.Delay(30-cost); // 约30帧每秒
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
            serialPort = new SerialPort(txtCOM.Text, 9600); // 替换为实际的串口号和波特率
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                serialPort.Open();
                Console.WriteLine("串口已打开。");

                // 启动发送线程
                Thread sendThread = new Thread(SendData);
                sendThread.Start();

                //Console.ReadLine(); // 保持程序运行

                //// 停止监听
                //isListening = false;
                //sendThread.Join(); // 等待发送线程完成

                //Console.WriteLine("程序已停止。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口打开失败：" + ex.Message);
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
                // 构造要发送的报文
                byte[] sendMessage = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0A };

                // 发送报文到串口
                serialPort.Write(sendMessage, 0, sendMessage.Length);

                Thread.Sleep(1000); // 每隔1秒发送一次
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;

            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);

            // 检查数据是否为固定长度，以及确保有足够的数据可供解析
            if (buffer.Length == 7)
            {
                // 解析第四和第五字节的温度数据
                byte byte4 = buffer[3];
                byte byte5 = buffer[4];

                int temperature = (byte4 << 8) | byte5; // 将两个字节组合为一个整数



                // 创建 Label 控件
                label2.Invoke((Action)(() =>
                {
                    label2.Text = DateTime.Now.ToString() + " 当前温度：" + temperature / 10 + "℃\r\n" + label2.Text;
                    if (label2.Text.Length > 1000) label2.Text = "";
                }));
                //Console.WriteLine("温度数据： " + temperature + " 度");
            }
            else
            {
                Console.WriteLine("无效的数据长度。");
            }
        }
        private void SerialPort_DataReceivedLight(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            int bytesToRead = sp.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            sp.Read(buffer, 0, bytesToRead);
            string asciiString = Encoding.ASCII.GetString(buffer);

            // 创建 Label 控件
            label2.Invoke((Action)(() =>
            {
                label2.Text = DateTime.Now.ToString() + " 当前光照强度：" + asciiString + "\r\n" + label2.Text;
                if (label2.Text.Length > 1000) label2.Text = "";
            }));


        }
        SerialPort serialPortLight;
        static bool LightListening = true;
        private void button1_Click(object sender, EventArgs e)
        {
            serialPortLight = new SerialPort(txtCOML.Text, 9600); // 替换为实际的串口号和波特率
            serialPortLight.DataReceived += SerialPort_DataReceivedLight;
            try
            {
                serialPortLight.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口打开失败：" + ex.Message);
            }
        }
    }
}