using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Microsoft.Kinect;

using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

using Newtonsoft.Json;

using System.Net.Http;
using System.Text;

namespace ArteCognitivoHackPuebla3
{
    public partial class Result
    {
        public IList<EmotionResult> Emotions { get; set; }
        public string Imagen { get; set; }
        public string Tiempo { get; set; }
    }

    public partial class EmotionResult
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }

    public partial class MainWindow : Window, Affdex.ImageListener
    {
        
        private KinectSensor _sensor = null;
        private ColorFrameReader _colorReader = null;
        private BodyFrameReader _bodyReader = null;
        private IList<Body> _bodies = null;

        private int _width = 0;
        private int _height = 0;
        private byte[] _pixels = null;
        private WriteableBitmap _bitmap = null;

        private KinectAudioStream convertStream = null;
        private SpeechRecognitionEngine speechEngine = null;

        private Affdex.Detector Detector { get; set; }
        Dictionary<int, Affdex.Face> found_faces = new Dictionary<int, Affdex.Face>();

        System.Media.SoundPlayer player;
        private IList<EmotionResult> emotions = new List<EmotionResult>();

        private int counter = 0;

        private int start_play, stop_play, start_seconds, stop_seconds;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void _bodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame()) {
                if (frame != null) {
                    frame.GetAndRefreshBodyData(_bodies);

                    Body body = _bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (body != null) {
                        Joint handRight = body.Joints[JointType.HandRight];
                        Joint spine_base = body.Joints[JointType.SpineBase];


                        if (handRight.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = handRight.Position;
                            ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                            float x = handRightPoint.X;
                            float y = handRightPoint.Y;

                            if (!float.IsInfinity(x) & !float.IsInfinity(y))
                            {
                                trail.Points.Add(new System.Windows.Point { X = x, Y = y });

                                Canvas.SetLeft(brush, x - brush.Width / 2.0);
                                Canvas.SetTop(brush, y - brush.Height);
                            }
                        }
                    }
                }
            }
        }

        private void _colorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.CopyConvertedFrameDataToArray(_pixels, ColorImageFormat.Bgra);

                    _bitmap.Lock();
                    Marshal.Copy(_pixels, 0, _bitmap.BackBuffer, _pixels.Length);
                    _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                    _bitmap.Unlock();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_colorReader != null) {
                _colorReader.Dispose();
            }

            if (_bodyReader != null) {
                _bodyReader.Dispose();
            }

            if (_sensor != null) {
                this._sensor.Close();
                this._sensor = null;
            }

            if (this.convertStream != null) {
                this.convertStream.SpeechActive = false;
            }

            if (this.speechEngine != null) {
                this.speechEngine.SpeechRecognized -= this.SpeechEngine_SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= this.SpeechEngine_SpeechRecognitionRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
            
            if ((Detector != null) && (Detector.isRunning()))
            {
                Detector.stop();
                Detector.Dispose();
                Detector = null;
            }
            
        }

        private RecognizerInfo tryGetKinectRecognizer() {
            IEnumerable<RecognizerInfo> recognizers;

            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException e) {
                return null;
            }
            
            foreach (RecognizerInfo recognizer in recognizers) {
                
                string data;

                recognizer.AdditionalInfo.TryGetValue("Kinect", out data);

                if ("en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase)) {
                    return recognizer;
                }
            }
            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();
            startWebCamProsessing();
            start_play = DateTime.Now.Minute;
            start_seconds = DateTime.Now.Second;

            if (_sensor != null)
            {
                _sensor.Open();

                _width = _sensor.ColorFrameSource.FrameDescription.Width;
                _height = _sensor.ColorFrameSource.FrameDescription.Height;

                _colorReader = _sensor.ColorFrameSource.OpenReader();
                _colorReader.FrameArrived += _colorReader_FrameArrived;

                _bodyReader = _sensor.BodyFrameSource.OpenReader();
                _bodyReader.FrameArrived += _bodyReader_FrameArrived;

                _pixels = new byte[_width * _height * 4];

                _bitmap = new WriteableBitmap(_width, _height, 96.0, 96.0, PixelFormats.Bgr32, null);

                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

                camera.Source = _bitmap;

                IReadOnlyList<AudioBeam> audioBeamList = this._sensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                this.convertStream = new KinectAudioStream(audioStream);
            }

            RecognizerInfo ri = tryGetKinectRecognizer();

            if (ri != null)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                var voice_commands = new Choices();
                voice_commands.Add(new SemanticResultValue("red", "RED"));
                voice_commands.Add(new SemanticResultValue("green", "GREEN"));
                voice_commands.Add(new SemanticResultValue("yellow", "YELLOW"));
                voice_commands.Add(new SemanticResultValue("blue", "BLUE"));
                voice_commands.Add(new SemanticResultValue("black", "BLACK"));
                voice_commands.Add(new SemanticResultValue("orange", "ORANGE"));

                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(voice_commands);

                var grammar = new Grammar(gb);
                this.speechEngine.LoadGrammar(grammar);

                this.speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected += SpeechEngine_SpeechRecognitionRejected;

                this.convertStream.SpeechActive = true;

                this.speechEngine.SetInputToAudioStream(this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void SpeechEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine(e.Result.ToString());
        }

        private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            const double confidenceThreshold = 0.3;

            if (e.Result.Confidence > confidenceThreshold) {
                switch (e.Result.Semantics.Value.ToString()) {
                    case "RED":
                        trail.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                        break;
                    case "GREEN":
                        trail.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                        break;
                    case "YELLOW":
                        trail.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 0));
                        break;
                    case "BLUE":
                        trail.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
                        break;
                    case "BLACK":
                        trail.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                        break;
                    case "ORANGE":
                        trail.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 165, 0));
                        break;
                }
            }
        }
        
        private void startWebCamProsessing() {
            try
            {
                const int cameraId = 1;
                const int numberOfFaces = 1;
                const int cameraFPS = 15;
                const int processFPS = 15;

                Detector = new Affdex.CameraDetector(cameraId, cameraFPS, processFPS, numberOfFaces, Affdex.FaceDetectorMode.LARGE_FACES);
                Detector.setClassifierPath("C:\\Program Files\\Affectiva\\AffdexSDK\\data");

                Detector.setDetectAllEmotions(true);
                Detector.setDetectGender(true);

                Detector.setImageListener(this);

                Detector.start();
            }
            catch (Affdex.AffdexException ex) {
                MessageBox.Show("Ocurrió un error al inicializar el analizador: " + ex.Message);
            }
        }

        public void onImageResults(Dictionary<int, Affdex.Face> faces, Affdex.Frame image)
        {
            
            foreach (KeyValuePair<int, Affdex.Face> pair in faces) {

                Affdex.Face face = pair.Value;
                float value = -1;

                float max_value = float.MinValue;
                string emotion_name = "";

                foreach (PropertyInfo info in typeof(Affdex.Emotions).GetProperties()) {
                    value = (float)info.GetValue(face.Emotions, null);
                    if (counter < 9) {
                        emotions.Add(new EmotionResult { Name = info.Name, Value = value });
                        counter++;
                    }

                    if (max_value < value) {
                        max_value = value;
                        emotion_name = info.Name;
                        Trace.WriteLine(info.Name);
                    }
                }
                playSound(emotion_name);
            }
            image.Dispose();
        }

        public void onImageCapture(Affdex.Frame image)
        {
            image.Dispose();
        }

        private void playSound(String emotion_name) {
           
            switch (emotion_name) {
                case "Anger":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\anger.wav");
                    break;
                case "Contempt":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\contempt.wav");
                    break;
                case "Disgust":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\disgust.wav");
                    break;
                case "Fear":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\fear.wav");
                    break;
                case "Joy":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\joy.wav");
                    break;
                case "Sadness":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\sadness.wav");
                    break;
                case "Surprice":
                    player = new System.Media.SoundPlayer(@"C:\Users\troja\OneDrive\Documentos\Visual Studio 2017\Projects\ArteCognitivoHackPuebla3\ArteCognitivoHackPuebla3\Sounds\surprise.wav");
                    break;
            }

            player.Play();
        }

        private void takeScreenShot(String fileName)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(this);
            double dpi = 96d;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height,
                                                                       dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderBitmap.Render(dv);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(fileName))
            {
                encoder.Save(file);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
       
            String picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            String fileName = String.Format("ScreenCapture {0:MMMM dd yyyy h mm ss}.png", DateTime.Now);
            fileName = System.IO.Path.Combine(picturesFolder, fileName);

            takeScreenShot(fileName);

            string base64StringImage = B64Encode(fileName);

            stop_play = DateTime.Now.Minute;
            stop_seconds = DateTime.Now.Second;

            var diff = stop_play - start_play;
            var second_diff = start_seconds - stop_seconds;
            
            string json_string = JsonConvert.SerializeObject(new Result { Emotions = emotions, Imagen = base64StringImage, Tiempo = diff + "." + second_diff });
            
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    response = client.PostAsync("http://10.50.123.39:3977/api/analisis", new StringContent(json_string, Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("OK");
                    }
                    else
                    {
                        MessageBox.Show("ERROR_1");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR_2");
            }

            counter = 0;
        }

        private string B64Encode(string path)
        {
            string image_encode = null;

            var a = System.Drawing.Image.FromFile(@path);

            using (MemoryStream ms = new MemoryStream())
            {
                a.Save(ms, a.RawFormat);
                byte[] imageBytes = ms.ToArray();

                image_encode = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
            }

            return image_encode;
        }
    }
}
