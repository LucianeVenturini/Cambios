
namespace Cambios
{
    using Modelos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadRates();//para criar as taxas
        }

        private async void LoadRates()
        {
            //bool load;//foi carregado ou não

            ProgressBar1.Value = 0;

            //para carregar a API via conexão
            var client = new HttpClient(); //crio http para fazer ligação externa
            client.BaseAddress = new Uri("http://cambios.somee.com"); //por o endereço
                                                                      //base da API que podemos ir buscar ao postman

            //controlador da API - uma pasta onde tenho os rates
            //tarefa assincrona, por isto o método vai passar a ser private async para
            //aplicação não deixe de estar a correr qdo carrega as taxas  
            //vamos guardar o resultado numa variavel
            var response = await client.GetAsync("/api/Rates");

            //variável objecto que vai ficar a espera da resposta que vem de cima
            //carrego os resultados no formato de uma string pra dentro do objeto result
            var result = await response.Content.ReadAsStringAsync(); //conteudo lido como uma string

            //para o caso de alguma coisa correr mal
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase); //tras o erro que se passou
                return; //se correr mal, sai
            }

            //se correu tudo bem
            var rates = JsonConvert.DeserializeObject<List<Rate>>(result);


            cb_origem.DataSource = rates; //lista rates
            //para aparecer os nomes adequadamente vamos a class rates
            //criar um override ou então podemos fazer da seguinte forma:
            cb_origem.DisplayMember = "Name";

            //corrige bug da microsoft
            cb_destino.BindingContext = new BindingContext(); //limpo a ligação de uma para a outra

            //agora vamos fazer para a cb_destino
            cb_destino.DataSource = rates;
            cb_destino.DisplayMember = "Name";

            //desta forma a origem e destino vão apresentar a mesma moeda. 
            //Para resolver isto vamos fazer um cb_destino.BindingContext na linha 56

            ProgressBar1.Value = 100;
        }
    }
}
