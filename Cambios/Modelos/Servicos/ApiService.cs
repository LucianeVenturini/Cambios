namespace Cambios.Modelos.Servicos
{
    using Newtonsoft.Json;
    using System;
    using Modelos;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    public class ApiService
    {
        //método asyncrono tarefa (task) que tem como tarefa devolver uma response
        public async Task<Response> GetRates (string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result,
                    };
                }
                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);
                return new Response
                {
                    IsSucess = true,
                    Result = rates,
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message,

                };
            }
        }
    }
}
