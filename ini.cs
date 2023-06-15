using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.Odbc;
using Microsoft.Win32;
using FirebirdSql.Data.FirebirdClient;
using System.Linq.Expressions;
using System.Diagnostics;
using Shapes = System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace TelaMain
{
    public partial class MainWindow : Window
    {
        private int AutoAtualizador;
        private int StatusCheckBox2;
        private int uniBanco = 0;
        public MainWindow()
        {
            InitializeComponent();
            SetWindowSize();
            MeuComponentes();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("Imagens/fundo.jpg", UriKind.Relative));
            this.Background = imageBrush;
        }

        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            MainWindow mainWindow = new MainWindow();
            app.Run(mainWindow);
        }

        private void SetWindowSize()
        {
            ResizeMode = ResizeMode.NoResize;
            MaxWidth = 1200;
            MaxHeight = 650;
        }

        private void MeuComponentes()
        {
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string DiretorioRaiz = Path.Combine(DiretorioDeExecução, "..");
            string CaminhoDaDados = Path.Combine(DiretorioRaiz, "dados");
            string[] subpastas = Directory.GetDirectories(CaminhoDaDados);
            string versaoDP = PegaVersaoDP();
            Canvas canvas = FindName("canvas2") as Canvas;
            Canvas canvas3 = FindName("canvas3") as Canvas;
            Shapes.Rectangle VersaoDispo = new Shapes.Rectangle();
            VersaoDispo.Fill = Brushes.White;
            VersaoDispo.Width = 150;
            VersaoDispo.Height = 20;

            TextBlock textVersaoDispo = new TextBlock();
            textVersaoDispo.Text = "Versão do Dispositivo: " + versaoDP;
            textVersaoDispo.Foreground = Brushes.Black;
            textVersaoDispo.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(textVersaoDispo, 31);
            Canvas.SetTop(textVersaoDispo, 70);

            Canvas.SetLeft(VersaoDispo, 31);
            Canvas.SetTop(VersaoDispo, 70);

            canvas3.Children.Add(VersaoDispo);
            canvas3.Children.Add(textVersaoDispo);
            Shapes.Rectangle desc1 = new Shapes.Rectangle();
            desc1.Fill = Brushes.Green;
            desc1.Width = 150;
            desc1.Height = 20;

            TextBlock desc1text = new TextBlock();
            desc1text.Text = "Atualizado";
            desc1text.Foreground = Brushes.White;
            desc1text.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(desc1text, 800);
            Canvas.SetTop(desc1text, 70);

            Canvas.SetLeft(desc1, 800);
            Canvas.SetTop(desc1, 70);

            canvas3.Children.Add(desc1);
            canvas3.Children.Add(desc1text);
            CheckBox checkboxatualizador = new CheckBox();
            checkboxatualizador.Background = Brushes.White;
            checkboxatualizador.Content = "Atualizador de Banco Automático";
            checkboxatualizador.Width = 230;
            checkboxatualizador.Foreground = Brushes.Black;
            checkboxatualizador.FontWeight = FontWeights.Bold;
            checkboxatualizador.Checked += Atualizador_CheckedChanged;
            checkboxatualizador.Unchecked += Atualizador_CheckedChanged;
            checkboxatualizador.IsChecked = true;
            Canvas.SetLeft(checkboxatualizador, 500);
            Canvas.SetTop(checkboxatualizador, 72);

            Shapes.Rectangle descAt = new Shapes.Rectangle();
            descAt.Fill = Brushes.White;
            descAt.Width = 220;//150
            descAt.Height = 20;
            Canvas.SetLeft(descAt, 500);
            Canvas.SetTop(descAt, 70);
            canvas3.Children.Add(descAt);
            canvas3.Children.Add(checkboxatualizador);
            Shapes.Rectangle desc2 = new Shapes.Rectangle();
            desc2.Fill = Brushes.Red;
            desc2.Width = 150;
            desc2.Height = 20;

            TextBlock desc2text = new TextBlock();
            desc2text.Text = "Desatualizado";
            desc2text.Foreground = Brushes.White;
            desc2text.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(desc2text, 970);
            Canvas.SetTop(desc2text, 70);

            Canvas.SetLeft(desc2, 970);
            Canvas.SetTop(desc2, 70);

            canvas3.Children.Add(desc2);
            canvas3.Children.Add(desc2text);

            BancosTelaInicial();
            paginas();
            foreach (UIElement elemento in canvas4parte2.Children)
            {
                elemento.Visibility = Visibility.Collapsed;
            }
        }

        private void VerificarTexto(object sender, RoutedEventArgs e)
        {
            string texto = canvas1.Children.OfType<TextBox>().FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(texto))
            {
                SearchTextBox_TextChanged(sender, new RoutedEventArgs());
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchTextBox_TextChanged(sender, new RoutedEventArgs());
            }
        }


        private void paginas()
        {
            Button ButtonProxima = new Button();
            ButtonProxima.Background = Brushes.White;
            ButtonProxima.Content = "Próxima";
            ButtonProxima.Width = 70;
            ButtonProxima.Foreground = Brushes.Black;
            ButtonProxima.FontWeight = FontWeights.Bold;
            ButtonProxima.Click += CarregarButtonProxima;
            Canvas.SetLeft(ButtonProxima, 580);
            Canvas.SetTop(ButtonProxima, 530);

            canvas3.Children.Add(ButtonProxima);
            Button ButtonAnterior = new Button();
            ButtonAnterior.Background = Brushes.White;
            ButtonAnterior.Content = "Anterior";
            ButtonAnterior.Width = 70;
            ButtonAnterior.Foreground = Brushes.Black;
            ButtonAnterior.FontWeight = FontWeights.Bold;
            ButtonAnterior.Click += CarregarButtonAnterior;
            Canvas.SetLeft(ButtonAnterior, 500);
            Canvas.SetTop(ButtonAnterior, 530);

            canvas3.Children.Add(ButtonAnterior);
        }

        private void CarregarButtonProxima(object sender, RoutedEventArgs e)
        {
            foreach (UIElement elemento in canvas4.Children)
            {
                elemento.Visibility = Visibility.Collapsed;
            }

            foreach (UIElement elemento in canvas4parte2.Children)
            {
                elemento.Visibility = Visibility.Visible;
            }
        }
        private void CarregarButtonAnterior(object sender, RoutedEventArgs e)
        {
            foreach (UIElement elemento in canvas4parte2.Children)
            {
                elemento.Visibility = Visibility.Collapsed;
            }

            foreach (UIElement elemento in canvas4.Children)
            {
                elemento.Visibility = Visibility.Visible;
            }
        }

        private void BancosTelaInicial()
        {
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string DiretorioRaiz = Path.Combine(DiretorioDeExecução, "..");
            string CaminhoDaDados = Path.Combine(DiretorioRaiz, "dados");
            string[] subpastas = Directory.GetDirectories(CaminhoDaDados);
            string versaoDP = PegaVersaoDP();

            Canvas canvas4 = FindName("canvas4") as Canvas;
            Canvas canvas4parte2 = FindName("canvas4parte2") as Canvas;
            int largura = 40;
            int altura = 155;
            int rest = 0;
            int stop = 0;
            int Qdado = 0;
            for (int i = 0; i < subpastas.Length; i++)
            {
                stop++;
                rest++;
                string Caminho = subpastas[i];
                string NameDoBanco = Path.GetFileName(Caminho);

                RadioButton radioButtonBancos = new RadioButton();
                radioButtonBancos.Background = Brushes.White;
                radioButtonBancos.Content = NameDoBanco;
                radioButtonBancos.Width = 210;
                radioButtonBancos.Foreground = Brushes.Black;
                radioButtonBancos.FontWeight = FontWeights.Bold;
                radioButtonBancos.Checked += RadioButtonBancos_Checked;

                Border border = new Border();
                border.Background = Brushes.White;
                border.BorderThickness = new Thickness(3);
                border.Child = radioButtonBancos;

                Shapes.Rectangle statusBanco = new Shapes.Rectangle();
                statusBanco.Fill = Brushes.Green;
                statusBanco.Width = 210;
                statusBanco.Height = 20;

                TextBlock textBlock = new TextBlock();
                string VersaoBanco = PegaVersao(NameDoBanco);
                textBlock.Text = "Versão do Banco: " + VersaoBanco;
                textBlock.Foreground = Brushes.White;
                textBlock.FontWeight = FontWeights.Bold;

                if (VersaoBanco != versaoDP) { statusBanco.Fill = Brushes.Red; }
                else { statusBanco.Fill = Brushes.Green; }
                if (VersaoBanco == "Pasta Vazia!") { statusBanco.Fill = Brushes.Black; }

                Canvas.SetLeft(border, largura - 3);
                Canvas.SetTop(border, altura);

                Canvas.SetLeft(statusBanco, largura);
                Canvas.SetTop(statusBanco, altura + 18);

                Canvas.SetLeft(textBlock, largura);
                Canvas.SetTop(textBlock, altura + 18);

                RadioButton radioButtonBancosCopy = new RadioButton();
                radioButtonBancosCopy.Background = Brushes.White;
                radioButtonBancosCopy.Content = NameDoBanco;
                radioButtonBancosCopy.Width = 210;
                radioButtonBancosCopy.Foreground = Brushes.Black;
                radioButtonBancosCopy.FontWeight = FontWeights.Bold;
                radioButtonBancosCopy.Checked += RadioButtonBancos_Checked;

                Border borderCopy = new Border();
                borderCopy.Background = border.Background;
                borderCopy.BorderThickness = border.BorderThickness;
                borderCopy.Child = radioButtonBancosCopy;
                borderCopy.Visibility = border.Visibility;

                Shapes.Rectangle statusBancoCopy = new Shapes.Rectangle();
                statusBancoCopy.Fill = statusBanco.Fill;
                statusBancoCopy.Width = statusBanco.Width;
                statusBancoCopy.Height = statusBanco.Height;
                statusBancoCopy.Visibility = statusBanco.Visibility;

                TextBlock textBlockCopy = new TextBlock();
                textBlockCopy.Text = textBlock.Text;
                textBlockCopy.Foreground = textBlock.Foreground;
                textBlockCopy.FontWeight = textBlock.FontWeight;
                textBlockCopy.Visibility = textBlock.Visibility;

                Canvas.SetLeft(borderCopy, largura - 3);
                Canvas.SetTop(borderCopy, altura);

                Canvas.SetLeft(statusBancoCopy, largura);
                Canvas.SetTop(statusBancoCopy, altura + 18);

                Canvas.SetLeft(textBlockCopy, largura);
                Canvas.SetTop(textBlockCopy, altura + 18);

                largura += 220;
                if (rest == 5)
                {
                    rest = 0;
                    altura += 65;
                    largura = 40;
                }

                if (stop <= 30)
                {
                    canvas4.Children.Add(border);
                    canvas4.Children.Add(statusBanco);
                    canvas4.Children.Add(textBlock);
                }
                if (stop == 30) { altura = 155; }
                if (stop <= 60)
                {
                    borderCopy.Visibility = Visibility.Collapsed;
                    statusBancoCopy.Visibility = Visibility.Collapsed;
                    textBlockCopy.Visibility = Visibility.Collapsed;

                    canvas4parte2.Children.Add(borderCopy);
                    canvas4parte2.Children.Add(statusBancoCopy);
                    canvas4parte2.Children.Add(textBlockCopy);
                }
                Qdado++;
                var statusBar = new StatusBar();
                statusBar.Height = 30;
                statusBar.Width = 1200;

                var statusBarItem = new StatusBarItem();
                statusBarItem.VerticalAlignment = VerticalAlignment.Bottom;
                statusBarItem.HorizontalAlignment = HorizontalAlignment.Stretch;

                var DescRodape = new TextBlock();
                DescRodape.Text = "Total de bancos de Dados: " + Qdado;
                DescRodape.Foreground = Brushes.Black;
                DescRodape.FontWeight = FontWeights.Bold;
                DescRodape.FontSize = 20;

                statusBarItem.Content = DescRodape;
                statusBar.Items.Add(statusBarItem);
                Rodape.Children.Add(statusBar);


                Canvas.SetLeft(statusBar, 0);
                Canvas.SetTop(statusBar, 580);
                if (stop == 60) { break; }

            }
        }

        private void Atualizador_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked == true)
            {
                AutoAtualizador = 1;
            }
            else
            {
                AutoAtualizador = 0;
            }
        }

        private void CheckAtualizador()
        {
            try
            {
                string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                string diretorioAtualizador = Path.Combine(diretorioPaiDados, "Atualizador.exe");
                string caminhoExe = diretorioAtualizador;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = caminhoExe;
                using (Process processo = new Process())
                {
                    processo.StartInfo = startInfo;
                    processo.Start();
                    processo.WaitForExit();
                }
                MessageBox.Show("Banco Configurado com Sucesso!");
                StatusCheckBox2 = 0;
                canvas2.Children.Clear();
                BancosTelaInicial();
            }
            catch
            {
                MessageBox.Show("Erro! Atualizador.exe não encontrado!\nNão foi possível iniciar o atualizador, mas o banco foi configurado com Sucesso!.");
            }
        }

        private string PegaVersaoDP()
        {
            string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
            string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
            string diretorioAtualizador = Path.Combine(diretorioPaiDados, "Acesso.exe");
            string caminhoDoArquivo = diretorioAtualizador;
            string resultVersao = "";
            if (System.IO.File.Exists(caminhoDoArquivo))
            {
                FileVersionInfo informacoesVersao = FileVersionInfo.GetVersionInfo(caminhoDoArquivo);
                string versao = informacoesVersao.FileVersion;
                int index = versao.IndexOf('.', versao.IndexOf('.') + 1);
                resultVersao = versao.Substring(0, index);
            }
            else
            {
                MessageBox.Show("Acesso.exe não encontrado!");
                Environment.Exit(0);
            }
            return resultVersao;

        }

        private int CheckBancoUni(string NameDoBanco)
        {
            string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
            string DiretorioRaizRenovar = Path.Combine(DiretorioDeExecuçãoDados, "..");
            string PatasDados = Path.Combine(DiretorioRaizRenovar, "dados");
            string CaminhoDoArquivoDoBanco = Path.Combine(PatasDados, NameDoBanco);

            string connectionString = "User=SYSDBA;Password=masterkey;Database=" + CaminhoDoArquivoDoBanco + "\\DADOS.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;";
            string query = "SELECT COUNT(distinct codemp) FROM produtos;";
            string query2 = "SELECT COUNT(distinct codemp) FROM pessoa;";
            int Result = 0;
            int Result2 = 0;
            int ResultFinal = 0;
            try
            {
                using (FbConnection connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query, connection))
                    {
                        using (FbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Result = reader.GetInt32(0);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch { MessageBox.Show("Erro: Falha na consulta ao banco de dados durante a verificação do banco unificado."); }
            try
            {
                using (FbConnection connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query2, connection))
                    {
                        using (FbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Result2 = reader.GetInt32(0);
                            }
                        }
                    }
                }
            }
            catch { MessageBox.Show("Erro: Falha na consulta ao banco de dados durante a verificação do banco unificado."); }
            if (Result > 1) { ResultFinal = 2; }
            else if (Result2 > 1) { ResultFinal = 2; }
            else { ResultFinal = 1; }

            return ResultFinal;
        }

        private string PegaVersao(string NameDoBanco)
        {
            string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
            string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
            string pastaDados = Path.Combine(diretorioPaiDados, "dados");
            string pastaComBanco = Path.Combine(pastaDados, NameDoBanco);
            string connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOSEMP.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;";
            string query1 = "select VERSAOPRINCIPAL from modulos_versao WHERE modulo = 'GERENCIAL'";
            string query2 = "select VERSAOMENOR from modulos_versao WHERE modulo = 'GERENCIAL'";
            string VersaoResult1 = "";
            string VersaoResult2 = "";
            try
            {
                using (FbConnection connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query1, connection))
                    {
                        using (FbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VersaoResult1 = reader.GetString(0);
                            }
                        }
                    }
                    connection.Close();
                }
                using (FbConnection connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query2, connection))
                    {
                        using (FbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VersaoResult2 = reader.GetString(0);
                            }
                        }
                    }
                    connection.Close();
                }
                string VersaoTotal = VersaoResult1 + "." + VersaoResult2;
                return VersaoTotal;
            }
            catch
            {
                string ErroMSG = "Pasta Vazia!";
                return ErroMSG;
            }

        }

        private List<string> PegaCNPJ(object sender, EventArgs e)
        {
            RadioButton RadioButtonBanco = (RadioButton)sender;
            string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
            string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
            string pastaDados = Path.Combine(diretorioPaiDados, "dados");
            string pastaComBanco = Path.Combine(pastaDados, (string)RadioButtonBanco.Content);
            string connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOSEMP.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;";
            string query = "SELECT cnpjemp FROM empresa";
            List<string> cnpjResult = new List<string>();
            using (FbConnection connection = new FbConnection(connectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand(query, connection))
                {
                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string cnpj = reader.GetString(0);
                            cnpjResult.Add(cnpj);
                        }
                    }
                }
                connection.Close();
            }
            return cnpjResult;
        }

        private List<string> PegaDadosUniBanco(object sender, EventArgs e)
        {
            List<string> ListaUniDados = new List<string>();
            try
            {
                RadioButton radioButtonBancos = (RadioButton)sender;
                string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                string pastaComBanco = Path.Combine(pastaDados, (string)radioButtonBancos.Content);
                string[] arquivos = Directory.GetFiles(pastaComBanco, "*DADOS*").Where(arquivo => !Path.GetFileName(arquivo).Contains("DADOSEMP")).ToArray();
                int rest = 1;
                int ResultFinal;
                string connectionString = "";
                foreach (string arquivo in arquivos)
                {
                    if (rest == 1) { connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOS.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;"; }
                    else { connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOS0" + rest + ".fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;"; }
                    string query = "SELECT COUNT(distinct codemp) FROM pessoa;";
                    int Result = 0;
                    try
                    {
                        using (FbConnection connection = new FbConnection(connectionString))
                        {
                            connection.Open();
                            using (FbCommand command = new FbCommand(query, connection))
                            {
                                using (FbDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Result = reader.GetInt32(0);
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: Falha na consulta ao banco de dados durante a verificação do banco unificado. :01: " + ex);
                        return ListaUniDados;
                    }
                    if (Result > 1) { ResultFinal = 2; }
                    else { ResultFinal = 1; }

                    if (ResultFinal == 1)
                    {
                        if (rest != 1)
                        {
                            ListaUniDados.Add("0" + rest);
                        }
                        else
                        {
                            for (int i = 0; i < Result; i++)
                            {
                                ListaUniDados.Add("");
                            }
                        }
                    }
                    else
                    {
                        uniBanco = 1;
                        for (int i = 0; i < Result; i++)
                        {
                            if (rest == 1) { ListaUniDados.Add(""); }
                            else { ListaUniDados.Add("0" + rest); }
                        }
                    }
                    rest++;

                }
            }
            catch
            {
                MessageBox.Show("Erro: Falha na consulta ao banco de dados durante a verificação do banco unificado. :03");
                return ListaUniDados;
            }
            return ListaUniDados;
        }

        private void Criarini(object sender, EventArgs e)
        {
            RadioButton radioButtonBancos = (RadioButton)sender;

            int uniBancoCheck = CheckBancoUni((string)radioButtonBancos.Content);

            if (uniBancoCheck > 1) { uniBanco = 1; }
            else { uniBanco = 0; }

            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
            string pastaProcurada = Path.Combine(diretorioPai, "dados");
            string[] diretorio = Directory.GetDirectories(pastaProcurada);
            string pastaDestino = @"\renovar";
            string NomeDoArquivo = "renovar.ini";
            string caminhoCompleto = Path.Combine(pastaDestino, NomeDoArquivo);
            List<string> cnpjResult = PegaCNPJ(sender, e);
            List<string> DadosUni = PegaDadosUniBanco(sender, e);

            for (int i = 0; i < 10; i++)
            {
                if (i >= DadosUni.Count)
                {
                    DadosUni.Add("sem Dados1");
                }
                else
                {
                    DadosUni.Add("");
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (i >= cnpjResult.Count)
                {
                    cnpjResult.Add("sem CNPJ");
                }
                else if (string.IsNullOrEmpty(cnpjResult[i]))
                {
                    cnpjResult[i] = "sem CNPJ";
                }
                else if (cnpjResult[i] == "00000000000000")
                {
                    for (int j = 0; j < cnpjResult.Count; j++)
                    {
                        if (cnpjResult[j] != "00000000000000") { cnpjResult[i] = cnpjResult[j]; }
                    }

                }
            }
            string DadosDoArquivo = @"
[SISTEMA]
DADOS01=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados.fdb
DADOS02=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[1] + ".fdb" + @"
DADOS03=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[2] + ".fdb" + @"
DADOS04=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[3] + ".fdb" + @"
DADOS05=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[4] + ".fdb" + @"
DADOS06=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[5] + ".fdb" + @"
DADOS07=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[6] + ".fdb" + @"
DADOS08=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[7] + ".fdb" + @"
DADOS09=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[8] + ".fdb" + @"
DADOSEMP=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\DadosEmp.fdb
DADOSREDE01=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados.fdb
DADOSREDE02=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[1] + ".fdb" + @"
DADOSREDE03=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[2] + ".fdb" + @"
DADOSREDE04=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[3] + ".fdb" + @"
DADOSREDE05=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[4] + ".fdb" + @"
DADOSREDE06=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[5] + ".fdb" + @"
DADOSREDE07=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[6] + ".fdb" + @"
DADOSREDE08=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[7] + ".fdb" + @"
DADOSREDE09=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[8] + ".fdb" + @"
DADOSREDEEMP=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\DadosEmp.fdb
DADOSLOG=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Log.fdb
DADOSREDELOG=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Log.fdb
CNPJ01=" + cnpjResult[0] + "\n" +
    "CNPJ02=" + cnpjResult[1] + "\n" +
    "CNPJ03=" + cnpjResult[2] + "\n" +
    "CNPJ04=" + cnpjResult[3] + "\n" +
    "CNPJ05=" + cnpjResult[4] + "\n" +
    "CNPJ06=" + cnpjResult[5] + "\n" +
    "CNPJ07=" + cnpjResult[6] + "\n" +
    "CNPJ08=" + cnpjResult[7] + "\n" +
    "CNPJ09=" + cnpjResult[8] + @"
SERVIDOR=LOCALHOST

[REDE]
DADOSREDE01=C:\RENOVAR\DADOS\" + radioButtonBancos.Content + @"\Dados.fdb
DADOSREDEEMP=C:\RENOVAR\DADOS\" + radioButtonBancos.Content + @"\DadosEmp.fdb
DADOSREDELOG=C:\RENOVAR\DADOS\" + radioButtonBancos.Content + @"\Log.fdb
SERVIDORREDE=LOCALHOST

[SQL]
DADOS_SQL01=Dados01
DADOS_SQLEMP=DadosEmp
DADOS_SQLLOG=DadosLog" + "\n" +

    @"[HOST]
HOST01=DESENV01\SQL2008
HOST02=DESENV01\SQL2008
HOST03=DESENV01\SQL2008
HOST04=DESENV01\SQL2008
HOST05=DESENV01\SQL2008
HOST06=DESENV01\SQL2008
HOST07=DESENV01\SQL2008
HOST08=DESENV01\SQL2008
HOST09=DESENV01\SQL2008
HOST10=DESENV01\SQL2008
HOSTEMP=DESENV01\SQL2008
HOSTLOG=DESENV01\SQL2008

[DATABASE]
SGBD=01
UNIFICADA=" + uniBanco + "\n" + @"

[DATABASE VERSION]
VERSION=2008

[Tipo SGDB]
Firebird = 01
SQLServer = 02

[ECF]
IMPRESSORA=2
MFD=S

[Tipo Impressora]
BEMATECH = 1
SWEDA = 2
DARUMA = 3

[Atualizacao]
Repositorio=C:\RENOVAR\
[LAYOUT]
SIZE=6
[TEMA]
NOME=Office2010Blue
[GESTORONLINE]
HOSTNAME=
USERNAME=
PASSWORD=
DATABASE=
PORT=
EMPRESA=1";
            using (StreamWriter writer = new StreamWriter(caminhoCompleto)) { writer.WriteLine(DadosDoArquivo); }
        }
        private IEnumerable<string> BuscarArquivosDados(object sender, EventArgs e)
        {
            IEnumerable<string> returne = Enumerable.Empty<string>();
            try
            {
                RadioButton radioButtonBancos = (RadioButton)sender;
                string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                string pastaComBanco = Path.Combine(pastaDados, (string)radioButtonBancos.Content);
                string[] arquivos = Directory.GetFiles(pastaComBanco, "*DADOS*").Where(arquivo => !Path.GetFileName(arquivo).Contains("DADOSEMP")).ToArray();
                return arquivos;
            }
            catch { return returne; }
        }

        private void RadioButtonBancos_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton RadioButtonBanco = (RadioButton)sender;
            if (RadioButtonBanco.IsChecked == true)
            {
                int i = 0;
                string pastaComBanco;
                IEnumerable<string> arquivosFiltrados = BuscarArquivosDados(sender, e);
                if (arquivosFiltrados.Any()) { }
                else
                {
                    MessageBox.Show("Nenhum Arquivo DADOS.FDB encontrado!");
                    return;
                }
                Criarini(sender, e);

                string versaoDP = PegaVersaoDP();
                string VersaoBanco = PegaVersao((string)RadioButtonBanco.Content);
                if (AutoAtualizador == 1) { if (versaoDP != VersaoBanco) { StatusCheckBox2 = 1; } }

                try
                {
                    foreach (string arquivos in arquivosFiltrados)
                    {
                        i++;
                        string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                        string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                        string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                        if (i > 1) { pastaComBanco = Path.Combine(pastaDados, (string)RadioButtonBanco.Content, "dados0" + i + ".fdb"); }
                        else { pastaComBanco = Path.Combine(pastaDados, (string)RadioButtonBanco.Content, "dados.fdb"); }
                        string dsnName = "RENOVARFB0" + i;
                        string driverName = "Firebird/InterBase(r) driver";
                        string databasePath = pastaComBanco;
                        string username = "SYSDBA";
                        string password = "masterkey";
                        string Descrição = "Dados0" + i;
                        string Client = "C:\\Program Files (x86)\\Firebird\\Firebird_3_0\\fbclient.dll";
                        string dsnConnectionString = $"DRIVER={{{driverName}}};DBNAME={databasePath};UID={username};PWD={password};";
                        if (Registry.CurrentUser.OpenSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources") == null)
                        {
                            Registry.CurrentUser.CreateSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources");
                        }
                        RegistryKey odbcKey = Registry.CurrentUser.OpenSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources", true);
                        odbcKey.SetValue(dsnName, driverName);
                        if (odbcKey != null) { odbcKey.Close(); }
                        RegistryKey dsnKey = Registry.CurrentUser.CreateSubKey("Software\\ODBC\\ODBC.INI\\" + dsnName);
                        if (dsnKey != null)
                        {
                            dsnKey.SetValue("Driver", driverName);
                            dsnKey.SetValue("Dbname", databasePath);
                            dsnKey.SetValue("User", username);
                            dsnKey.SetValue("Password", password);
                            dsnKey.SetValue("Client", Client);
                            dsnKey.SetValue("Description", Descrição);
                            dsnKey.Close();
                        }
                    }
                    if (StatusCheckBox2 == 1) { CheckAtualizador(); }
                    else
                    {
                        MessageBox.Show("Banco Configurado com Sucesso!");
                        canvas2.Children.Clear();
                        BancosTelaInicial();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocorreu um erro: " + ex.Message);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            Canvas canvas4 = FindName("canvas4") as Canvas;
            Canvas canvas4parte2 = FindName("canvas4parte2") as Canvas;
            string texto = canvas1.Children.OfType<TextBox>().FirstOrDefault()?.Text;
            string versaoDP = PegaVersaoDP();
            string searchText = texto;
            string VersaoBanco;
            int largura = 40;
            int altura = 155;
            int rest = 0;
            int stop = 0;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string termoDeBusca = searchText;
                if (canvas4 != null) { canvas4.Children.Clear(); }
                if (canvas4parte2 != null) { canvas4parte2.Children.Clear(); }

                string DiretorioDeExecução = Directory.GetCurrentDirectory();
                string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
                string pastaProcurada = Path.Combine(diretorioPai, "dados");
                string[] diretorio = Directory.GetDirectories(pastaProcurada);
                IEnumerable<string> pastasFiltradas = diretorio.Where(pasta => pasta.IndexOf(termoDeBusca, StringComparison.OrdinalIgnoreCase) >= 0);
                foreach (string pasta in pastasFiltradas)
                {
                    string folderPath = pasta.ToString();
                    string folderName = Path.GetFileName(folderPath);
                    RadioButton radioButtonExistente = canvas1.Children.OfType<RadioButton>()
                    .FirstOrDefault(radioButton => string.Equals(radioButton.Content.ToString(), termoDeBusca, StringComparison.OrdinalIgnoreCase)) as RadioButton;

                    if (radioButtonExistente == null)
                    {
                        stop++;
                        rest++;
                        string Caminho = pasta;
                        string NameDoBanco = Path.GetFileName(Caminho);

                        RadioButton radioButtonBancos = new RadioButton();
                        radioButtonBancos.Background = Brushes.White;
                        radioButtonBancos.Content = NameDoBanco;
                        radioButtonBancos.Width = 210;
                        radioButtonBancos.Foreground = Brushes.Black;
                        radioButtonBancos.FontWeight = FontWeights.Bold;
                        radioButtonBancos.Checked += RadioButtonBancos_Checked;

                        Border border = new Border();
                        border.Background = Brushes.White;
                        border.BorderThickness = new Thickness(3);
                        border.Child = radioButtonBancos;

                        Shapes.Rectangle statusBanco = new Shapes.Rectangle();
                        statusBanco.Fill = Brushes.Green;
                        statusBanco.Width = 210;
                        statusBanco.Height = 20;

                        TextBlock textBlock = new TextBlock();
                        VersaoBanco = PegaVersao(NameDoBanco);
                        textBlock.Text = "Versão do Banco: " + VersaoBanco;
                        textBlock.Foreground = Brushes.White;
                        textBlock.FontWeight = FontWeights.Bold;

                        if (VersaoBanco != versaoDP) { statusBanco.Fill = Brushes.Red; }
                        else { statusBanco.Fill = Brushes.Green; }
                        if (VersaoBanco == "Pasta Vazia!") { statusBanco.Fill = Brushes.Black; }

                        Canvas.SetLeft(border, largura - 3);
                        Canvas.SetTop(border, altura);

                        Canvas.SetLeft(statusBanco, largura);
                        Canvas.SetTop(statusBanco, altura + 18);

                        Canvas.SetLeft(textBlock, largura);
                        Canvas.SetTop(textBlock, altura + 18);

                        RadioButton radioButtonBancosCopy = new RadioButton();
                        radioButtonBancosCopy.Background = Brushes.White;
                        radioButtonBancosCopy.Content = NameDoBanco;
                        radioButtonBancosCopy.Width = 210;
                        radioButtonBancosCopy.Foreground = Brushes.Black;
                        radioButtonBancosCopy.FontWeight = FontWeights.Bold;
                        radioButtonBancosCopy.Checked += RadioButtonBancos_Checked;

                        Border borderCopy = new Border();
                        borderCopy.Background = border.Background;
                        borderCopy.BorderThickness = border.BorderThickness;
                        borderCopy.Child = radioButtonBancosCopy;
                        borderCopy.Visibility = border.Visibility;

                        Shapes.Rectangle statusBancoCopy = new Shapes.Rectangle();
                        statusBancoCopy.Fill = statusBanco.Fill;
                        statusBancoCopy.Width = statusBanco.Width;
                        statusBancoCopy.Height = statusBanco.Height;
                        statusBancoCopy.Visibility = statusBanco.Visibility;

                        TextBlock textBlockCopy = new TextBlock();
                        textBlockCopy.Text = textBlock.Text;
                        textBlockCopy.Foreground = textBlock.Foreground;
                        textBlockCopy.FontWeight = textBlock.FontWeight;
                        textBlockCopy.Visibility = textBlock.Visibility;

                        Canvas.SetLeft(borderCopy, largura - 3);
                        Canvas.SetTop(borderCopy, altura);

                        Canvas.SetLeft(statusBancoCopy, largura);
                        Canvas.SetTop(statusBancoCopy, altura + 18);

                        Canvas.SetLeft(textBlockCopy, largura);
                        Canvas.SetTop(textBlockCopy, altura + 18);

                        largura += 220;
                        if (rest == 5)
                        {
                            rest = 0;
                            altura += 65;
                            largura = 40;
                        }

                        if (stop <= 30)
                        {
                            canvas4.Children.Add(border);
                            canvas4.Children.Add(statusBanco);
                            canvas4.Children.Add(textBlock);
                        }
                        if (stop == 30) { altura = 155; }
                        if (stop <= 60)
                        {
                            borderCopy.Visibility = Visibility.Collapsed;
                            statusBancoCopy.Visibility = Visibility.Collapsed;
                            textBlockCopy.Visibility = Visibility.Collapsed;

                            canvas4parte2.Children.Add(borderCopy);
                            canvas4parte2.Children.Add(statusBancoCopy);
                            canvas4parte2.Children.Add(textBlockCopy);
                        }
                        if (stop == 60) { break; }
                    }
                }
            }
            else
            {
                canvas4.Children.Clear();
                canvas4parte2.Children.Clear();

                string DiretorioDeExecução = Directory.GetCurrentDirectory();
                string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
                string pastaProcurada = Path.Combine(diretorioPai, "dados");
                string[] subpastas = Directory.GetDirectories(pastaProcurada);

                for (int i = 0; i < subpastas.Length; i++)
                {
                    stop++;
                    rest++;
                    string Caminho = subpastas[i];
                    string NameDoBanco = Path.GetFileName(Caminho);

                    RadioButton radioButtonBancos = new RadioButton();
                    radioButtonBancos.Background = Brushes.White;
                    radioButtonBancos.Content = NameDoBanco;
                    radioButtonBancos.Width = 210;
                    radioButtonBancos.Foreground = Brushes.Black;
                    radioButtonBancos.FontWeight = FontWeights.Bold;
                    radioButtonBancos.Checked += RadioButtonBancos_Checked;

                    Border border = new Border();
                    border.Background = Brushes.White;
                    border.BorderThickness = new Thickness(3);
                    border.Child = radioButtonBancos;

                    Shapes.Rectangle statusBanco = new Shapes.Rectangle();
                    statusBanco.Fill = Brushes.Green;
                    statusBanco.Width = 210;
                    statusBanco.Height = 20;

                    TextBlock textBlock = new TextBlock();
                    VersaoBanco = PegaVersao(NameDoBanco);
                    textBlock.Text = "Versão do Banco: " + VersaoBanco;
                    textBlock.Foreground = Brushes.White;
                    textBlock.FontWeight = FontWeights.Bold;

                    if (VersaoBanco != versaoDP) { statusBanco.Fill = Brushes.Red; }
                    else { statusBanco.Fill = Brushes.Green; }
                    if (VersaoBanco == "Pasta Vazia!") { statusBanco.Fill = Brushes.Black; }

                    Canvas.SetLeft(border, largura - 3);
                    Canvas.SetTop(border, altura);

                    Canvas.SetLeft(statusBanco, largura);
                    Canvas.SetTop(statusBanco, altura + 18);

                    Canvas.SetLeft(textBlock, largura);
                    Canvas.SetTop(textBlock, altura + 18);

                    RadioButton radioButtonBancosCopy = new RadioButton();
                    radioButtonBancosCopy.Background = Brushes.White;
                    radioButtonBancosCopy.Content = NameDoBanco;
                    radioButtonBancosCopy.Width = 210;
                    radioButtonBancosCopy.Foreground = Brushes.Black;
                    radioButtonBancosCopy.FontWeight = FontWeights.Bold;
                    radioButtonBancosCopy.Checked += RadioButtonBancos_Checked;

                    Border borderCopy = new Border();
                    borderCopy.Background = border.Background;
                    borderCopy.BorderThickness = border.BorderThickness;
                    borderCopy.Child = radioButtonBancosCopy;
                    borderCopy.Visibility = border.Visibility;

                    Shapes.Rectangle statusBancoCopy = new Shapes.Rectangle();
                    statusBancoCopy.Fill = statusBanco.Fill;
                    statusBancoCopy.Width = statusBanco.Width;
                    statusBancoCopy.Height = statusBanco.Height;
                    statusBancoCopy.Visibility = statusBanco.Visibility;

                    TextBlock textBlockCopy = new TextBlock();
                    textBlockCopy.Text = textBlock.Text;
                    textBlockCopy.Foreground = textBlock.Foreground;
                    textBlockCopy.FontWeight = textBlock.FontWeight;
                    textBlockCopy.Visibility = textBlock.Visibility;

                    Canvas.SetLeft(borderCopy, largura - 3);
                    Canvas.SetTop(borderCopy, altura);

                    Canvas.SetLeft(statusBancoCopy, largura);
                    Canvas.SetTop(statusBancoCopy, altura + 18);

                    Canvas.SetLeft(textBlockCopy, largura);
                    Canvas.SetTop(textBlockCopy, altura + 18);

                    largura += 220;
                    if (rest == 5)
                    {
                        rest = 0;
                        altura += 65;
                        largura = 40;
                    }

                    if (stop <= 30)
                    {
                        canvas4.Children.Add(border);
                        canvas4.Children.Add(statusBanco);
                        canvas4.Children.Add(textBlock);
                    }
                    if (stop == 30) { altura = 155; }
                    if (stop <= 60)
                    {
                        borderCopy.Visibility = Visibility.Collapsed;
                        statusBancoCopy.Visibility = Visibility.Collapsed;
                        textBlockCopy.Visibility = Visibility.Collapsed;

                        canvas4parte2.Children.Add(borderCopy);
                        canvas4parte2.Children.Add(statusBancoCopy);
                        canvas4parte2.Children.Add(textBlockCopy);
                    }
                    if (stop == 60) { break; }
                }
            }


        }


    }
}
