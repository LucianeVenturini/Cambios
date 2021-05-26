using Cambios.Modelos;
using Cambios.Modelos.Servicos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cambios
{
        
    public partial class Form1 : Form
    {

        #region Atributos
        private List<Rate> Rates; //para substituir -> public List<Rate> Rates { get; set; } = new List<Rate>();

        //definir um atributo Network service
        private NetworkService networService;

        //definir um atributo ApiService
        private ApiService apiService;

        private DialogService dialogService;

        private DataService dataService;

        #endregion


        public Form1()
        {
            InitializeComponent();
            networService = new NetworkService(); //Estanciar objeto
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService(); //ao fazer isto estou a chamar o contrutor e ele vai logo tratar da pasta
            LoadRates();//para criar as taxas
        }

        private async void LoadRates()
        {
            bool load;
          
            lbl_resultados.Text = "A atualizar taxas...";

            var connection = networService.CheckConnection();

            if (!connection.IsSucess) // se não tiver conexão
            {
                LoadLocalRates(); //Base de dados
                load = false; //não consegiu carregar
            }
            else
            {
                await LoadApiRates(); //ler a Api
                load = true;//load passa a true
            }
            //Pode ocorrer que a primeira vez a base de dados pode não estar preenchida
            if (Rates.Count == 0) // minhas listas não foram carregadas
            {
                lbl_resultados.Text = "Não há ligação a Internet" + Environment.NewLine +
                     "a não foram préviamente carregadas as taxas." + Environment.NewLine +
                     "Tente mais tarde!";

                lbl_status.Text = "Primeira inicialização deverá ter à ligação a internet.";

                return;
            }


            cb_origem.DataSource = Rates; //lista rates
            //para aparecer os nomes adequadamente vamos a class rates
            //criar um override ou então podemos fazer da seguinte forma:
            cb_origem.DisplayMember = "Name";

            //corrige bug da microsoft
            cb_destino.BindingContext = new BindingContext(); //limpo a ligação de uma para a outra

            //agora vamos fazer para a cb_destino
            cb_destino.DataSource = Rates;
            cb_destino.DisplayMember = "Name";

            //desta forma a origem e destino vão apresentar a mesma moeda. 
            //Para resolver isto vamos fazer um cb_destino.BindingContext na linha 56
                     
          
            lbl_resultados.Text = "Taxas atualizadas...";

            if (load)
            {
                lbl_status.Text = string.Format("Taxas carregadas da internet em {0:F}", DateTime.Now);
            }
            else
            {
                lbl_status.Text = string.Format("Taxas carregadas da Base de Dados.");
            }

            ProgressBar1.Value = 100;
            btn_converter.Enabled = true;
            btn_trocar.Enabled = true;
        }

        private void LoadLocalRates() //implementar a base de dados
        {
            //Tenho que buscar a minha lista
            Rates = dataService.GetData();
            //MessageBox.Show("Não está implementado");
        }

        private async Task LoadApiRates() //porque na API tbm foi defenido como async
        {
            ProgressBar1.Value = 0; // aqui que ele vai carregar se não tiver conexão a internet

            var response = await apiService.GetRates("http://cambios.somee.com", "/api/Rates");

            Rates = (List<Rate>)response.Result;

           //Apaga dados antigos da base de dados e coloca os novos dados
            dataService.DeleteData();
            dataService.SaveData(Rates);


        }

        private void btn_converter_Click(object sender, EventArgs e)
        {
            Converter();
        }

        private void Converter()
        {
            //validações
            if (string.IsNullOrEmpty(tb_valor.Text))
            {
                dialogService.ShowMessage("Erro", "Insira um valor a converter");
                return;
            }
            decimal valor;
            if (!decimal.TryParse(tb_valor.Text, out valor))
            {
                dialogService.ShowMessage("Erro de conversão", "valor terá que ser numérico");

                return;
            }
            //Validar que tenho as moedas escolhidas
            if (cb_origem.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem que escolher uma moeda a converter");
                return;
            }
            if (cb_destino.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem que escolher uma moeda de destino para converter");
                return;
            }

            var taxaOrigem = (Rate) cb_origem.SelectedItem;

            var taxaDestino = (Rate)cb_destino.SelectedItem;

            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            lbl_resultados.Text = string.Format("{0} {1:C2} = {2} {3:C2}", 
                taxaOrigem.Code, 
                valor, 
                taxaDestino.Code, 
                valorConvertido);
        }

        private void btn_trocar_Click(object sender, EventArgs e)
        {
            Trocar();
        }

        private void Trocar()
        {
            var aux = cb_origem.SelectedItem;

            cb_origem.SelectedItem = cb_destino.SelectedItem;

            cb_destino.SelectedItem = aux;

            Converter();
        }
    }
}
