# yolov5-usb-camera
yolov5 usb camera object detect

from:
https://github.com/techwingslab/yolov5-net

Changed to desktop winform program and supports real-time detection with USB camera


.net6.0  vs2022

![image](https://github.com/cagy520/yolov5-usb-camera/assets/9970419/17a90b9e-305f-4b07-a277-47e2cb24f845)



And it supports data acquisition from temperature sensors and light sensors

------------------------------------------------------------------------------------------------------

If you want to use the CUDA core, please use the following code
1.CUDA版本11.2

2.cuDNN用cudnn-windows-x86_64-8.9.3.28_cuda11-archive，记得把压缩包的三个文件夹放到cuda根目录下覆盖

3.Microsoft.ML.OnnxRuntime.Gpu要使用1.13.1,如果版本太新，SessionOptions会报错。

private SessionOptions GetSessionOptions()
 {
     bool supportGPU = (Environment.GetEnvironmentVariable("CPAI_MODULE_SUPPORT_GPU") ?? "true").ToLower() == "true";
 
     SessionOptions sessionOpts = new SessionOptions();
     string[]? providers = null;
     try
     {
         providers = OrtEnv.Instance().GetAvailableProviders();
     }
     catch
     {
     }
     sessionOpts.AppendExecutionProvider_CUDA();//显卡用这个
     //sessionOpts.AppendExecutionProvider_CPU();
     return sessionOpts;
 }
 
 
//初始化onnx模型的代码
 
 SessionOptions sessionOpts = GetSessionOptions();
 
 _scorer = new YoloScorer<YoloCocoP5Model>("Assets/Weights/yolov5n.onnx", sessionOpts);
 
 _capture = new VideoCapture("http://192.168.151.130:8080/video"); // 手机摄像头流媒体

