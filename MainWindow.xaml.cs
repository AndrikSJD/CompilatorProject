using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Antlr4.Runtime;
using Microsoft.Win32;
using SyntacticAnalysisGenerated;


namespace Proyecto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///
    /// Esta interfaz fue creada por Julio Cesar Castro Y Josue Orozco
    /// Se utilizo el lenguaje de programacion C# y el framework de WPF
    /// Tome en cuenta que esta interfaz es solo una propuesta, puede ser modificada
    /// Las funciones aqui implementadas son solo para mostrar como se puede implementar
    /// Las mismas estan vacias dentro de ellas esta el nombre de la funcion y un comentario que indica que es lo que hace la funcion
    /// 
    /// 
    /// </summary>
    public partial class MainWindow
    {
        
        public MainWindow() 
        {
            // El System.Diagnostics.Debug.WriteLine es para imprimir en la consola del debugger.
            System.Diagnostics.Debug.WriteLine("System Diagnostics Debug");
            
            InitializeComponent();
        }

        private void Add_Tab_Button_Click(object sender, EventArgs e)
        {
            TabItem nuevaTab = new TabItem();
            TextBox nuevoTextBox = new TextBox();
            nuevoTextBox.IsReadOnly = true;
            
            // Crea un cuadro de diálogo para abrir archivos
            var openFileDialog = new OpenFileDialog
            {
                Title = "Seleccione un archivo de texto",
                Filter = "Archivos de texto (*.txt)|*.txt"
            };

            // Muestra el cuadro de diálogo y si se seleccionó un archivo, lo carga en el TextBox
            if (openFileDialog.ShowDialog() == true)
            {
                // Establece el encabezado del TabItem como el nombre del archivo seleccionada
                nuevaTab.Header = openFileDialog.SafeFileName;
                
                // Guarda el nombre del archivo seleccionado
                string archivoTexto = openFileDialog.FileName;

                // Lee el contenido del archivo seleccionado y lo asigna al TextBox
                TextReader leer = new StreamReader(archivoTexto);
                nuevoTextBox.Text = leer.ReadToEnd();
                
                // Agrega el TextBox al TabItem
                nuevaTab.Content = nuevoTextBox;

                // Agrega el nuevo TabItem al TabControl
                Tab.Items.Add(nuevaTab);
            }
        }
        
        private void closeButton_Click(object sender, EventArgs e)
        {
            // Obtiene el TabItem seleccionado
            TabItem selectedTab = Tab.SelectedItem as TabItem;
            // Verifica que el TabItem seleccionado no sea nulo
            if (selectedTab != null)
            {
                //Elimina el contenido del tab item seleccionado
                selectedTab.Content = null;
                // Elimina el TabItem seleccionado
                Tab.Items.Remove(selectedTab);
            }
            // Eliminar la pestaña seleccionada TabItem
            // Elimina una pestaña del TabControl, tome en cuenta que al eliminar una pestaña 
            // tambien se debe eliminar el TextBox que esta dentro del TabControl
        }

        private void Pantalla_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }
        private void Pantalla_TextChanged(object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }   
        
        private void UpdateCursorPosition()
        {
            int index = Pantalla.SelectionStart;
            int line = Pantalla.GetLineIndexFromCharacterIndex(index) + 1;
            int column = index - Pantalla.GetCharacterIndexFromLineIndex(Pantalla.GetLineIndexFromCharacterIndex(index)) + 1;
         
            // Actualiza el texto de un label o de otro TextBox con el número de línea y columna.
            Output.Content = $"Línea: {line} \nColumna: {column}";
        }
        
        
        public void Upload_File_Button_Click(object? sender, RoutedEventArgs e) 
        {
            // Aqui va la logica para subir un archivo a la ventana principal
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Establecer el directorio inicial del diálogo en la carpeta de documentos del usuario
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.InitialDirectory = documentsFolder;
            // Añadimos un nombre para el cuadro de subir archivo
            openFileDialog.Title = "Seleccione un archivo de texto";
            // Abre un cuadro de dialogo para subir un archivo
            openFileDialog.ShowDialog();
            // Obtiene el nombre del archivo
            string archivoTexto = openFileDialog.FileName;
            try
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    TextReader leer = new StreamReader(archivoTexto);
                    Pantalla.Text = leer.ReadToEnd();
                    leer.Close();
                }
            }
            catch (Exception exception )
            {
                System.Diagnostics.Debug.WriteLine(exception);
                throw;
            }
            
        }
        
        private void Run_Button_Click(object? sender, RoutedEventArgs e) 
        {
            //Extraer texto de la pantalla principal
            ICharStream input = CharStreams.fromString(Pantalla.Text);
            
            //Escanea el texto que se le manda del input
            MiniCSharpScanner scanner = new MiniCSharpScanner(input);
            // Organiza los tokens streams
            CommonTokenStream tokens = new CommonTokenStream(scanner);
            //Le manda los tokens al parser
            MiniCSharpParser parser = new MiniCSharpParser(tokens);
            //Remueve los error listeners
            scanner.RemoveErrorListeners();
            parser.RemoveErrorListeners();
            //Crea un nuevo error listener
            MyParserErrorListener errorListener = new MyParserErrorListener();
            scanner.AddErrorListener(errorListener);
            parser.AddErrorListener(errorListener);
            //Obtiene el resultado
            MiniCSharpParser.ProgramContext tree = parser.program();
            
            
            
            //Mostrar texto de salida
            Consola salida = new Consola();
            // salida.SalidaConsola.Text = tree.ToStringTree(parser);
            if (errorListener.HasErrors())
            {
                salida.SalidaConsola.Text = errorListener.ToString();
            }
            else
            {
                salida.SalidaConsola.Text = "No hay errores";
            }
            salida.Show();
                
                
            
        }
        
        private void Exit_Button_Click(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Exit Button Clicked");
            Application.Current.Shutdown();
        }
    }
    
}
