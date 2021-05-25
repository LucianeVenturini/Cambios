
namespace Cambios
{
    using Cambios.Modelos;
    using Cambios.Modelos.Servicos;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {

        #region Atributos
        //definir um atributo Network service
        private NetworkService networService;

        //definir um atributo ApiService
        private ApiService apiService;

        #endregion

        public List<Rate> Rates { get; set; } = new List<Rate>();

        public Form1()
        {
            InitializeComponent();
            networService = new NetworkService(); //Estanciar objeto
            apiService = new ApiService();
            LoadRates();//para criar as taxas
        }

        private async void LoadRates()
        {
          
            lbl_resultados.Text = "A atualizar taxas...";

            var connection = networService.CheckConnection();

            if (!connection.IsSucess) // se não tiver conexão
            {
                MessageBox.Show(connection.Message);
                return; //saí do método e não entra na API
            }
            else
            {
                await LoadApiRates();
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

            ProgressBar1.Value = 100;

            lbl_resultados.Text = "Taxas carregadas...";
        }

        private async Task LoadApiRates() //porque na API tbm foi defenido como async
        {
            ProgressBar1.Value = 0; // aqui que ele vai carregar se não tiver conexão a internet

            var response = await apiService.GetRates("http://cambios.somee.com", "/api/Rates");

            Rates = (List<Rate>)response.Result;
            
        }
    }
}
