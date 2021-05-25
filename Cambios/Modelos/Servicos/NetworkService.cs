namespace Cambios.Modelos.Servicos
{
    using Modelos; //como estou dentro no mesmo namespace posso apargar os cambios
    using System.Net;

    public class NetworkService
    {
        //método para verificar a conexão
        public Response CheckConnection()
        {
            var client = new WebClient(); //vou utilizar para testar se tenho ligação a internet

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204")) //ping para a google
                {
                    return new Response
                    {
                        IsSucess = true,
                    };
                }
            }
            catch //caso não consiga fazer a ligação
            {
                return new Response
                {
                    IsSucess = false,
                    Message ="Configure a sua ligação a Internet",
                };
            }
        }
    }
}
