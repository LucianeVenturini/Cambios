using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Cambios.Modelos.Servicos
{
    public class DataService
    {
        //atributo para fazer a ligação 
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService; //para comunicar com o utilizador

        //contrutor
        //vou criar na raiz uma pasta chamada Data e vai ser
        //dentro desta pasta que vou colocar a Base de dados.
        //se a pasta não existir ele vai criar. 
        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data")) //se não tiver criada, vou criar a pasta
            {
                Directory.CreateDirectory("Data");
            }

            //caminho para a base de dados

            var path = @"Data\Rates.sqlite";
            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open(); //se a base de dados não existir ele cria

                //tabela com os campos que tenho no modelo de dados
                string sqlcommand = "create table if not exists rates(RateId int, Code varchar(5), TaxRate real, Name varchar(250))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();//vai executar o comando que não é uma query
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                //aqui podia usar o response tbm. 
            }
        }
        //métodos para manipular a base de dados
        //vai receber uma lista de rates para gravar na base de dados
        public void SaveData(List<Rate> Rates)
        {
            try
            {
                foreach (var rate in Rates)
                {
                    string sql = string.Format("insert into Rates (RateId, Code, TaxRate, Name)values( {0}, '{1}', {2}, '{3}')", rate.RateId, rate.Code, rate.TaxRate, rate.Name);

                    //não posso usar o mesmo que o da linha 41
                    command = new SQLiteCommand(sql, connection);

                    command.ExecuteNonQuery();
                }
                connection.Close();                
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
        public List<Rate> GetData()
        {
            List<Rate> rates = new List<Rate>();

            try
            {
                string sql = "select RateId, Code, TaxRate, Name from Rates";

                command = new SQLiteCommand(sql, connection);

                //Lê cada registo
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    rates.Add(new Rate
                    {
                        RateId = (int)reader["RateId"],
                        Code = (string) reader ["Code"],
                        Name= (string) reader ["Name"],
                        TaxRate = (double) reader ["TaxRate"]
                    }
                    );
                }
                connection.Close();

                return rates;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null; // não retorna nada
            }
        }
        public void DeleteData()
        {
            try
            {
                //Comando de SQL
                string sql = "delete * from Rates";

                //Executar o comando
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

    }
}
