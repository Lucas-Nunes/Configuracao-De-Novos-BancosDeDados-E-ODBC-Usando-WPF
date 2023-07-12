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
        private string BancoAtual;
        private string BancoAtualPaginas;
        private int PaginaAtual;
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
            try
            {
                Application app = new Application();
                MainWindow mainWindow = new MainWindow();
                app.Run(mainWindow);
            }
            catch (Exception E) { MessageBox.Show("Erro ao abrir o programa:\n\n" + "Erro:\n" + E.Message); }
        }

        private void SetWindowSize()
        {
            ResizeMode = ResizeMode.CanMinimize;
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
            Canvas.SetTop(desc1text, 585);
            Canvas.SetZIndex(desc1text, 9999);

            Canvas.SetLeft(desc1, 800);
            Canvas.SetTop(desc1, 585);
            Canvas.SetZIndex(desc1, 9999);

            Rodape.Children.Add(desc1);
            Rodape.Children.Add(desc1text);


            Shapes.Rectangle desc3 = new Shapes.Rectangle();
            desc3.Fill = Brushes.Purple;
            desc3.Width = 150;
            desc3.Height = 20;

            TextBlock desc3text = new TextBlock();
            desc3text.Text = "Banco Atual";
            desc3text.Foreground = Brushes.White;
            desc3text.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(desc3text, 630);
            Canvas.SetTop(desc3text, 585);
            Canvas.SetZIndex(desc3text, 9999);

            Canvas.SetLeft(desc3, 630);
            Canvas.SetTop(desc3, 585);
            Canvas.SetZIndex(desc3, 9999);
            
            Rodape.Children.Add(desc3);
            Rodape.Children.Add(desc3text);//canvas3

            CheckBox checkboxatualizador = new CheckBox();
            checkboxatualizador.Background = Brushes.White;
            checkboxatualizador.Content = "Atualizador de Banco Automático";
            checkboxatualizador.Width = 230;
            checkboxatualizador.Foreground = Brushes.Black;
            checkboxatualizador.FontWeight = FontWeights.Bold;
            checkboxatualizador.Checked += Atualizador_CheckedChanged;
            checkboxatualizador.Unchecked += Atualizador_CheckedChanged;
            checkboxatualizador.IsChecked = true;
            Canvas.SetLeft(checkboxatualizador, 210);
            Canvas.SetTop(checkboxatualizador, 72);

            Shapes.Rectangle descAt = new Shapes.Rectangle();
            descAt.Fill = Brushes.White;
            descAt.Width = 220;//150
            descAt.Height = 20;
            Canvas.SetLeft(descAt, 210);
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
            Canvas.SetTop(desc2text, 585);
            Canvas.SetZIndex(desc2text, 9999);

            Canvas.SetLeft(desc2, 970);
            Canvas.SetTop(desc2, 585);
            Canvas.SetZIndex(desc2, 9999);

            Rodape.Children.Add(desc2);
            Rodape.Children.Add(desc2text);

            BancosTelaInicial();
            foreach (UIElement elemento in canvas4parte2.Children)
            {
                elemento.Visibility = Visibility.Collapsed;
            }
        }

        private void VerificarBancoAtual(string NameDoBanco)
        {
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string ConfigBanco = Path.Combine(DiretorioDeExecução, "BancoAtual.txt");

            BancoAtual = File.ReadAllText(ConfigBanco);

            if (BancoAtual != NameDoBanco)
            {
                try
                {
                    using (StreamWriter arquivo = File.CreateText(ConfigBanco))
                    {
                        arquivo.WriteLine(NameDoBanco);
                    }
                }
                catch (Exception E) { MessageBox.Show("Erro ao gravar banco atual:\n\n" + "Erro:\n" + E.Message); }
            }
        }

        private void VerificarTextoHint(object sender, MouseButtonEventArgs e)
        {
            TextBox textbox = FindName("text1") as TextBox;
            string texto = canvas1.Children.OfType<TextBox>().FirstOrDefault()?.Text;
            if (texto.Length > 0)
            {
                textbox.Text = string.Empty;
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
            PaginaAtual = 2;
            if(BancoAtualPaginas != "Pagina2"){canvas6.Children.Clear();}

            foreach (UIElement elemento in canvas6.Children)
            {
                elemento.Visibility = Visibility.Visible;
            }

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
            PaginaAtual = 1;
            if(BancoAtualPaginas == "Pagina2"){canvas6.Children.Clear();}
            foreach (UIElement elemento in canvas4parte2.Children)
            {
                elemento.Visibility = Visibility.Collapsed;
            }

            foreach (UIElement elemento in canvas4.Children)
            {
                elemento.Visibility = Visibility.Visible;
            }
            BancosTelaInicial();
        }

        private void RadioButtonBancos_MouseRight(object sender, MouseButtonEventArgs e, string NameDoBanco)
        {
            canvas5.Children.Clear();

            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);

            Button desc1text = new Button();
            desc1text.Content = "Copiar (BACKUP)";
            desc1text.Foreground = Brushes.Black;
            desc1text.FontWeight = FontWeights.Bold;
            desc1text.Width = 150;
            desc1text.PreviewMouseLeftButtonDown += (sender, e) => CopiarBancos(NameDoBanco);

            Canvas.SetLeft(desc1text, mousePosition.X);
            Canvas.SetTop(desc1text, mousePosition.Y);
            canvas5.Children.Add(desc1text);

            Button desc2text = new Button();
            desc2text.Content = "DELETAR";
            desc2text.Width = 150;
            desc2text.Foreground = Brushes.Black;
            desc2text.FontWeight = FontWeights.Bold;
            desc2text.PreviewMouseLeftButtonDown += (sender, e) => ExcluirBanco(NameDoBanco);

            Canvas.SetLeft(desc2text, mousePosition.X);
            Canvas.SetTop(desc2text, mousePosition.Y + 20);
            canvas5.Children.Add(desc2text);

            double x = mousePosition.X;
            double y = mousePosition.Y + 40;

            Button renomear = new Button();
            renomear.Content = "RENOMEAR";
            renomear.Width = 150;
            renomear.Foreground = Brushes.Black;
            renomear.FontWeight = FontWeights.Bold;
            renomear.PreviewMouseLeftButtonDown += (sender, e) => RenomearBanco(NameDoBanco, x, y);

            Canvas.SetLeft(renomear, x);
            Canvas.SetTop(renomear, y);
            canvas5.Children.Add(renomear);
        }

        private void RenomearBanco(string NameDoBanco, double x, double y)
        {
            FbConnection.ClearAllPools();
            TextBox renomearTexto = new TextBox();
            renomearTexto.Width = 150;
            renomearTexto.Text = NameDoBanco;
            renomearTexto.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    RenomearBanco2(NameDoBanco, renomearTexto.Text);
                }
            };

            Canvas.SetLeft(renomearTexto, x + 150);
            Canvas.SetTop(renomearTexto, y + 20);
            canvas5.Children.Add(renomearTexto);
        }

        private void RenomearBanco2(string NameDoBanco, string novoNome)
        {
            FbConnection.ClearAllPools();
            canvas5.Children.Clear();
            if (!string.IsNullOrWhiteSpace(novoNome))
            {
                try
                {
                    string DiretorioDeExecução = Directory.GetCurrentDirectory();
                    string DiretorioRaiz = Path.Combine(DiretorioDeExecução, "..");
                    string CaminhoDaDados = Path.Combine(DiretorioRaiz, "dados");
                    string CaminhoDoBanco = Path.Combine(CaminhoDaDados, NameDoBanco);
                    string CaminhoDoBancoNomeNovo = Path.Combine(CaminhoDaDados, novoNome);
                    Directory.Move(CaminhoDoBanco, CaminhoDoBancoNomeNovo);

                    MessageBox.Show("Banco renomeado com sucesso!");

                    string ConfigBanco = Path.Combine(DiretorioDeExecução, "BancoAtual.txt");
                    string bancoAtualStatus = File.ReadAllText(ConfigBanco);
                    if (bancoAtualStatus.Trim() == NameDoBanco.Trim())
                    {
                        try
                        {
                            using (StreamWriter arquivo = File.CreateText(ConfigBanco))
                            {
                                arquivo.WriteLine(" ");
                            }
                        }
                        catch (Exception E) { MessageBox.Show("Erro ao remover banco atual:\n\n" + "Erro:\n" + E.Message); }
                    }

                    BancosTelaInicial();
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Source and destination path must be different.")
                    {
                        MessageBox.Show("Erro ao renomear! \nEsse Nome já existe! ");
                        BancosTelaInicial();
                    }
                    else
                    {
                        MessageBox.Show("Erro ao renomear!\n " + ex.Message);
                        BancosTelaInicial();
                    }
                }
            }
            else { MessageBox.Show("Erro ao renomear!\n Nome vazio!"); }
        }

        private void ExcluirBanco(string NameDoBanco)
        {
            canvas5.Children.Clear();
            MessageBoxResult resultado = MessageBox.Show("Deseja realmente excluir o banco de dados?", "Confirmação", MessageBoxButton.YesNo);
            if (resultado == MessageBoxResult.Yes)
            {
                FbConnection.ClearAllPools();
                string DiretorioDeExecução = Directory.GetCurrentDirectory();
                string DiretorioRaiz = Path.Combine(DiretorioDeExecução, "..");
                string CaminhoDaDados = Path.Combine(DiretorioRaiz, "dados");
                string Banco = Path.Combine(CaminhoDaDados, NameDoBanco);
                try
                {
                    if (Directory.Exists(Banco))
                    {
                        Directory.Delete(Banco, true);
                        MessageBox.Show("Banco excluído com sucesso!");

                        string ConfigBanco = Path.Combine(DiretorioDeExecução, "BancoAtual.txt");
                        string bancoAtualStatus = File.ReadAllText(ConfigBanco);
                        if (bancoAtualStatus.Trim() == NameDoBanco.Trim())
                        {
                            try
                            {
                                using (StreamWriter arquivo = File.CreateText(ConfigBanco))
                                {
                                    arquivo.WriteLine(" ");
                                }
                            }
                            catch (Exception E) { MessageBox.Show("Erro ao remover banco atual:\n\n" + "Erro:\n" + E.Message); }
                        }

                        canvas4.Children.Clear();
                        canvas4parte2.Children.Clear();
                        BancosTelaInicial();
                    }
                    else
                    {
                        MessageBox.Show("O Banco não existe.");
                        BancosTelaInicial();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir a Banco: {ex.Message}");
                    BancosTelaInicial();
                }
            }
            else if (resultado == MessageBoxResult.No)
            {
                BancosTelaInicial();
            }
        }

        private void CopiarBancos(string NameDoBanco)
        {
            canvas5.Children.Clear();
            Boolean stop = true;
            try
            {
                string DiretorioDeExecução = Directory.GetCurrentDirectory();
                string DiretorioRaiz = Path.Combine(DiretorioDeExecução, "..");
                string CaminhoDaDados = Path.Combine(DiretorioRaiz, "dados");
                string DadosCopiado = Path.Combine(CaminhoDaDados, NameDoBanco);
                string NomeEDestino = Path.Combine(CaminhoDaDados, NameDoBanco + " COPIA");

                while (stop == true)
                {
                    if (!Directory.Exists(NomeEDestino))
                    {
                        Directory.CreateDirectory(NomeEDestino);
                        stop = false;
                    }
                    else
                    {
                        NomeEDestino = Path.Combine(NomeEDestino, NomeEDestino + " COPIA");
                        stop = true;
                    }
                }

                string[] arquivos = Directory.GetFiles(DadosCopiado);

                foreach (string arquivo in arquivos)
                {
                    string nomeArquivo = Path.GetFileName(arquivo);
                    string caminhoDestino = Path.Combine(NomeEDestino, nomeArquivo);
                    File.Copy(arquivo, caminhoDestino, true);
                }

                MessageBox.Show("Banco copiado com sucesso!");
                canvas5.Children.Clear();
                canvas4.Children.Clear();
                canvas4parte2.Children.Clear();
                BancosTelaInicial();
            }
            catch
            {
                MessageBox.Show("Erro ao copiar Banco!");
                canvas5.Children.Clear();
                BancosTelaInicial();
            }

        }

        private void RadioButtonMouseRightClear(object sender, MouseEventArgs e)
        { canvas5.Children.Clear(); }

        private void BancosTelaInicial()
        {
            Canvas canvas4 = FindName("canvas4") as Canvas;
            Canvas canvas4parte2 = FindName("canvas4parte2") as Canvas;

            PaginaAtual = 1;
            canvas6.Children.Clear();
            canvas5.Children.Clear();
            canvas4parte2.Children.Clear();
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string DiretorioRaiz = Path.Combine(DiretorioDeExecução, "..");
            string CaminhoDaDados = Path.Combine(DiretorioRaiz, "dados");
            string[] subpastas = Directory.GetDirectories(CaminhoDaDados);
            string versaoDP = PegaVersaoDP();
            
            int largura = 40;
            int altura = 155;
            int rest = 0;
            int stop = 0;
            int Qdado = 0;

            string ConfigBancoStatus = Path.Combine(DiretorioDeExecução, "BancoAtual.txt");
            Shapes.Rectangle BancoAtualDesc = new Shapes.Rectangle();
            BancoAtualDesc.Fill = Brushes.White;
            BancoAtualDesc.Width = 250;
            BancoAtualDesc.Height = 20;

            TextBlock textBancoAtualDesc = new TextBlock();
            string ConfigBanco = Path.Combine(DiretorioDeExecução, "BancoAtual.txt");
            string bancoAtualStatus = File.ReadAllText(ConfigBancoStatus);
            if(bancoAtualStatus.Trim() == ""){bancoAtualStatus = "Nenhum";}
            textBancoAtualDesc.Text = "Banco Atual: " + bancoAtualStatus;
            textBancoAtualDesc.Foreground = Brushes.Black;
            textBancoAtualDesc.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(textBancoAtualDesc, 460);
            Canvas.SetTop(textBancoAtualDesc, 70);

            Canvas.SetLeft(BancoAtualDesc, 460);
            Canvas.SetTop(BancoAtualDesc, 70);

            canvas3.Children.Add(BancoAtualDesc);
            canvas3.Children.Add(textBancoAtualDesc);
            
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
                radioButtonBancos.PreviewMouseRightButtonDown += (sender, e) =>
                {
                    RadioButtonBancos_MouseRight(sender, e, NameDoBanco);
                };

                Border border = new Border();
                border.Background = Brushes.White;
                border.BorderThickness = new Thickness(3);
                border.Child = radioButtonBancos;

                Canvas.SetLeft(border, largura - 3);
                Canvas.SetTop(border, altura);

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

                Canvas.SetLeft(statusBanco, largura);
                Canvas.SetTop(statusBanco, altura + 18);

                Canvas.SetLeft(textBlock, largura);
                Canvas.SetTop(textBlock, altura + 18);
                
                if(Qdado < 25)
                {
                    BancoAtual = File.ReadAllText(ConfigBanco);
                    if (BancoAtual.Trim() == NameDoBanco.Trim())
                    {
                        BancoAtualPaginas = "Pagina1";
                        Shapes.Rectangle statusBancoAtual = new Shapes.Rectangle();
                        statusBancoAtual.Fill = Brushes.Purple;
                        statusBancoAtual.Width = 210;
                        statusBancoAtual.Height = 5;

                        Canvas.SetLeft(statusBancoAtual, largura);
                        Canvas.SetTop(statusBancoAtual, altura-3);
                        canvas6.Children.Add(statusBancoAtual);
                    }
                }

                largura += 220;
                if (rest == 5)
                {
                    rest = 0;
                    altura += 65;
                    largura = 40;
                }

                if (stop <= 25)
                {
                    canvas4.Children.Add(border);
                    canvas4.Children.Add(statusBanco);
                    canvas4.Children.Add(textBlock);
                }
                if (stop == 25)
                {
                    largura -= 220;
                    altura = 155; 
                    rest -= 1;
                }

                if (stop <= 50 && stop > 25)
                {
                    RadioButton radioButtonBancosCopy = new RadioButton();
                    radioButtonBancosCopy.Background = Brushes.White;
                    radioButtonBancosCopy.Content = NameDoBanco;
                    radioButtonBancosCopy.Width = 210;
                    radioButtonBancosCopy.Foreground = Brushes.Black;
                    radioButtonBancosCopy.FontWeight = FontWeights.Bold;
                    radioButtonBancosCopy.Checked += RadioButtonBancos_Checked;
                    radioButtonBancosCopy.PreviewMouseRightButtonDown += (sender, e) =>
                    {
                        RadioButtonBancos_MouseRight(sender, e, NameDoBanco);
                    };

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

                    BancoAtual = File.ReadAllText(ConfigBanco);
                    if (BancoAtual.Trim() == NameDoBanco.Trim())
                    {
                        BancoAtualPaginas = "Pagina2";
                        Shapes.Rectangle statusBancoAtual = new Shapes.Rectangle();
                        statusBancoAtual.Fill = Brushes.Purple;
                        statusBancoAtual.Width = 210;
                        statusBancoAtual.Height = 5;

                        Canvas.SetLeft(statusBancoAtual, largura);
                        Canvas.SetTop(statusBancoAtual, altura-3);
                        canvas6.Children.Add(statusBancoAtual);
                    }

                    borderCopy.Visibility = Visibility.Collapsed;
                    statusBancoCopy.Visibility = Visibility.Collapsed;
                    textBlockCopy.Visibility = Visibility.Collapsed;

                    canvas4parte2.Children.Add(borderCopy);
                    canvas4parte2.Children.Add(statusBancoCopy);
                    canvas4parte2.Children.Add(textBlockCopy);
                }

                Qdado++;
                if(Qdado > 25){paginas();}

                var statusBar = new StatusBar();
                statusBar.Height = 30;
                statusBar.Width = 1200;

                var statusBarItem = new StatusBarItem();
                statusBarItem.VerticalAlignment = VerticalAlignment.Bottom;
                statusBarItem.HorizontalAlignment = HorizontalAlignment.Stretch;

                var DescRodape = new TextBlock();
                int BancosNaoAdicionados;
                if(Qdado < subpastas.Length ) 
                {
                    BancosNaoAdicionados = subpastas.Length - Qdado;
                    DescRodape.Text = "Total de bancos de Dados: " + subpastas.Length + "/50 Max" +" | Bancos não adicionados: " + BancosNaoAdicionados; 
                }
                else{DescRodape.Text = "Total de bancos de Dados: " + subpastas.Length + "/50 Max";}
                DescRodape.Foreground = Brushes.Black;
                DescRodape.FontWeight = FontWeights.Bold;
                DescRodape.FontSize = 15;//

                statusBarItem.Content = DescRodape;
                statusBar.Items.Add(statusBarItem);
                Rodape.Children.Add(statusBar);

                Canvas.SetLeft(statusBar, 0);
                Canvas.SetTop(statusBar, 580);
                if (stop == 50) { break; }

            }
            if (PaginaAtual == 1)
            {
                if(BancoAtualPaginas == "Pagina2")
                {
                    foreach (UIElement elemento in canvas6.Children)
                    {
                        elemento.Visibility = Visibility.Collapsed;
                    }
                }
            }
            if (PaginaAtual == 2)
            {
                if(BancoAtualPaginas != "Pagina2"){canvas6.Children.Clear();}
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
                            if (rest == 1) { ListaUniDados.Add("UniBanco"); }
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

        private List<string> PegaDadosUniBancoSegundaVerificação(object sender, EventArgs e)
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
                    string query = "SELECT COUNT(distinct codemp) FROM produtos;";
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
                            if (rest == 1 && i == 0) { 
                                ListaUniDados.Add("UniBanco");}
                            else
                            { 
                                if( rest != 1 ){ListaUniDados.Add("0" + rest); }
                                else {ListaUniDados.Add("UniBanco");}
                            }
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
            List<string> DadosUni;
            if(PegaDadosUniBanco(sender, e).Count > PegaDadosUniBancoSegundaVerificação(sender, e).Count )
            {DadosUni = PegaDadosUniBanco(sender, e);}
            else
            {DadosUni = PegaDadosUniBancoSegundaVerificação(sender, e);}

            for (int i = 0; i < 10; i++)
            {
                if (i >= DadosUni.Count)
                {
                    DadosUni.Add("sem Dados");
                }
                else if (string.IsNullOrEmpty(DadosUni[i]))
                {
                    if(DadosUni[i] == "UniBanco"){DadosUni[i] = "";}
                    else {DadosUni.Add("sem Dados");}
                }
                else
                {
                    if(DadosUni[i] == "UniBanco"){DadosUni[i] = "";}
                    else {DadosUni.Add("sem Dados");}
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
            string DadosDoArquivo = @"[SISTEMA]
DADOS01=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados.fdb\n";

            if (DadosUni[1] != "sem Dados") { DadosDoArquivo += @"DADOS02=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[1] + ".fdb\n"; };
            if (DadosUni[2] != "sem Dados") { DadosDoArquivo += @"DADOS03=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[2] + ".fdb\n"; };
            if (DadosUni[3] != "sem Dados") { DadosDoArquivo += @"DADOS04=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[3] + ".fdb\n"; };
            if (DadosUni[4] != "sem Dados") { DadosDoArquivo += @"DADOS05=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[4] + ".fdb\n"; };
            if (DadosUni[5] != "sem Dados") { DadosDoArquivo += @"DADOS06=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[5] + ".fdb\n"; };
            if (DadosUni[6] != "sem Dados") { DadosDoArquivo += @"DADOS07=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[6] + ".fdb\n"; };
            if (DadosUni[7] != "sem Dados") { DadosDoArquivo += @"DADOS08=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[7] + ".fdb\n"; };
            if (DadosUni[8] != "sem Dados") { DadosDoArquivo += @"DADOS09=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + @"\Dados" + DadosUni[8] + ".fdb\n"; };
            DadosDoArquivo += @"DADOSEMP=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\DadosEmp.fdb\n";
            DadosDoArquivo += @"DADOSLOG=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Log.fdb\n";
            DadosDoArquivo += "SERVIDOR=LOCALHOST\n";
            DadosDoArquivo += "\n";

            DadosDoArquivo += @"DADOSREDE01=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados.fdb\n";
            if (DadosUni[1] != "sem Dados") { DadosDoArquivo += @"DADOSREDE02=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[1] + ".fdb\n"; }
            if (DadosUni[2] != "sem Dados") { DadosDoArquivo += @"DADOSREDE03=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[2] + ".fdb\n"; }
            if (DadosUni[3] != "sem Dados") { DadosDoArquivo += @"DADOSREDE04=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[3] + ".fdb\n"; }
            if (DadosUni[4] != "sem Dados") { DadosDoArquivo += @"DADOSREDE05=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[4] + ".fdb\n"; }
            if (DadosUni[5] != "sem Dados") { DadosDoArquivo += @"DADOSREDE06=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[5] + ".fdb\n"; }
            if (DadosUni[6] != "sem Dados") { DadosDoArquivo += @"DADOSREDE07=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[6] + ".fdb\n"; }
            if (DadosUni[7] != "sem Dados") { DadosDoArquivo += @"DADOSREDE08=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[7] + ".fdb\n"; }
            if (DadosUni[8] != "sem Dados") { DadosDoArquivo += @"DADOSREDE09=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Dados" + DadosUni[8] + ".fdb\n"; }
            DadosDoArquivo += @"DADOSREDEEMP=C:\RENOVAR\DADOS\" + radioButtonBancos.Content + "\\DadosEmp.fdb\n";
            DadosDoArquivo += @"DADOSREDELOG=C:\RENOVAR\DADOS\" + (string)radioButtonBancos.Content + "\\Log.fdb\n";
            DadosDoArquivo += "SERVIDORREDE=LOCALHOST\n";
            DadosDoArquivo += "\n";

            if (cnpjResult[0] != "sem CNPJ") { DadosDoArquivo += "CNPJ01=" + cnpjResult[0] + "\n"; };
            if (cnpjResult[1] != "sem CNPJ") { DadosDoArquivo += "CNPJ02=" + cnpjResult[1] + "\n"; };
            if (cnpjResult[2] != "sem CNPJ") { DadosDoArquivo += "CNPJ03=" + cnpjResult[2] + "\n"; };
            if (cnpjResult[3] != "sem CNPJ") { DadosDoArquivo += "CNPJ04=" + cnpjResult[3] + "\n"; };
            if (cnpjResult[4] != "sem CNPJ") { DadosDoArquivo += "CNPJ05=" + cnpjResult[4] + "\n"; };
            if (cnpjResult[5] != "sem CNPJ") { DadosDoArquivo += "CNPJ06=" + cnpjResult[5] + "\n"; };
            if (cnpjResult[6] != "sem CNPJ") { DadosDoArquivo += "CNPJ07=" + cnpjResult[6] + "\n"; };
            if (cnpjResult[7] != "sem CNPJ") { DadosDoArquivo += "CNPJ08=" + cnpjResult[7] + "\n"; };
            if (cnpjResult[8] != "sem CNPJ") { DadosDoArquivo += "CNPJ09=" + cnpjResult[8] + "\n"; };

            DadosDoArquivo += @"
[SQL]
DADOS_SQL01=Dados01
DADOS_SQLEMP=DadosEmp
DADOS_SQLLOG=DadosLog" + "\n" +

            @"
[HOST]
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
UNIFICADA=" + uniBanco + @"

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

        private int VerificarQuantidadeDeEmpresas(object sender, EventArgs e, int rest)
        {
            int Result = 0;
            try
            {
                RadioButton radioButtonBancos = (RadioButton)sender;
                string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                string pastaComBanco = Path.Combine(pastaDados, (string)radioButtonBancos.Content);
                string[] arquivos = Directory.GetFiles(pastaComBanco, "*DADOS*").Where(arquivo => !Path.GetFileName(arquivo).Contains("DADOSEMP")).ToArray();
                string connectionString = "";
                    if (rest == 1) { connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOS.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;"; }
                    else { connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOS0" + rest + ".fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;"; }
                    string query = "SELECT COUNT(distinct codemp) FROM produtos;";
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
                        return Result;
                    }
                    return Result;
            }
            catch
            {
                MessageBox.Show("Erro: Falha na consulta ao banco de dados durante a verificação do banco unificado. :03");
                return Result;
            }
        }

        private int VerificarQuantidadeDeEmpresasEtapa2(object sender, EventArgs e, int rest)
        {
            int Result = 0;
            try
            {
                RadioButton radioButtonBancos = (RadioButton)sender;
                string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                string pastaComBanco = Path.Combine(pastaDados, (string)radioButtonBancos.Content);
                string[] arquivos = Directory.GetFiles(pastaComBanco, "*DADOS*").Where(arquivo => !Path.GetFileName(arquivo).Contains("DADOSEMP")).ToArray();
                string connectionString = "";
                if (rest == 1) { connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOS.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;"; }
                else { connectionString = "User=SYSDBA;Password=masterkey;Database=" + pastaComBanco + "\\DADOS0" + rest + ".fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;"; }
                string query = "SELECT COUNT(distinct codemp) FROM pessoa;";
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
                    return Result;
                }
                return Result;
            }
            catch
            {
                MessageBox.Show("Erro: Falha na consulta ao banco de dados durante a verificação do banco unificado. :03");
                return Result;
            }
        }

        private void RadioButtonBancos_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton RadioButtonBanco = (RadioButton)sender;
            TextBox textbox = FindName("text1") as TextBox;
            textbox.Text = "Filtrar por nome";
            if (RadioButtonBanco.IsChecked == true)
            {
                int i = 0;
                int j = 0;
                string pastaComBanco;
                IEnumerable<string> arquivosFiltrados = BuscarArquivosDados(sender, e);
                if (arquivosFiltrados.Any()) { }
                else
                {
                    MessageBox.Show("Nenhum Arquivo DADOS.FDB encontrado!");
                    return;
                }
                Criarini(sender, e);

                VerificarBancoAtual((string)RadioButtonBanco.Content);

                string versaoDP = PegaVersaoDP();
                string VersaoBanco = PegaVersao((string)RadioButtonBanco.Content);
                if (AutoAtualizador == 1) { if (versaoDP != VersaoBanco) { StatusCheckBox2 = 1; } }

                int DadosUni;
                try
                {
                    foreach (string arquivos in arquivosFiltrados)
                    {
                        j++;
                        if (VerificarQuantidadeDeEmpresas(sender, e, j) > VerificarQuantidadeDeEmpresasEtapa2(sender, e, j))
                        { DadosUni = VerificarQuantidadeDeEmpresas(sender, e, j); }
                        else
                        { DadosUni = VerificarQuantidadeDeEmpresasEtapa2(sender, e, j); }
                        while ( DadosUni != 0) 
                        {
                            i++;
                            string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                            string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                            string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                            if (j > 1) { pastaComBanco = Path.Combine(pastaDados, (string)RadioButtonBanco.Content, "dados0" + j + ".fdb"); }
                            else { pastaComBanco = Path.Combine(pastaDados, (string)RadioButtonBanco.Content, "dados.fdb"); }
                            string dsnName = "RENOVARFB0" + i;
                            string driverName = "Firebird/InterBase(r) driver";
                            string databasePath = pastaComBanco;
                            string username = "SYSDBA";
                            string password = "masterkey";
                            string Descrição = "Dados0" + j;
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
                            DadosUni--;
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
                canvas6.Children.Clear();

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
                        radioButtonBancos.PreviewMouseRightButtonDown += (sender, e) =>
                        {
                            RadioButtonBancos_MouseRight(sender, e, NameDoBanco);
                        };

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

                        string ConfigBanco = Path.Combine(DiretorioDeExecução, "BancoAtual.txt");
                        BancoAtual = File.ReadAllText(ConfigBanco);
                        if (BancoAtual.Trim() == NameDoBanco.Trim())
                        {
                            Shapes.Rectangle statusBancoAtual = new Shapes.Rectangle();
                            statusBancoAtual.Fill = Brushes.Purple;
                            statusBancoAtual.Width = 210;
                            statusBancoAtual.Height = 5;

                            Canvas.SetLeft(statusBancoAtual, largura);
                            Canvas.SetTop(statusBancoAtual, altura-3);
                            canvas6.Children.Add(statusBancoAtual);
                        }

                        Canvas.SetLeft(border, largura - 3);
                        Canvas.SetTop(border, altura);

                        Canvas.SetLeft(statusBanco, largura);
                        Canvas.SetTop(statusBanco, altura + 18);

                        Canvas.SetLeft(textBlock, largura);
                        Canvas.SetTop(textBlock, altura + 18);

                        largura += 220;
                        if (rest == 5)
                        {
                            rest = 0;
                            altura += 65;
                            largura = 40;
                        }
                        if (stop <= 25)
                        {
                            canvas4.Children.Add(border);
                            canvas4.Children.Add(statusBanco);
                            canvas4.Children.Add(textBlock);
                        }
                        if (stop == 25) { altura = 155; }
                        if (stop <= 50 && stop >= 25)
                        {
                            RadioButton radioButtonBancosCopy = new RadioButton();
                            radioButtonBancosCopy.Background = Brushes.White;
                            radioButtonBancosCopy.Content = NameDoBanco;
                            radioButtonBancosCopy.Width = 210;
                            radioButtonBancosCopy.Foreground = Brushes.Black;
                            radioButtonBancosCopy.FontWeight = FontWeights.Bold;
                            radioButtonBancosCopy.Checked += RadioButtonBancos_Checked;
                            radioButtonBancosCopy.PreviewMouseRightButtonDown += (sender, e) =>
                            {
                                RadioButtonBancos_MouseRight(sender, e, NameDoBanco);
                            };
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
            }else{BancosTelaInicial();}
        }
    }
}
