namespace Cambios.Modelos
{
    public class Response
    {
        public bool IsSucess { get; set; } //true se tiver sucesso
        //correu bem ou não, tenho internet ouo não tenho, a API carregou bem ou não carregou...

        public string Message { get; set; }//caso coram mal o que se passou,
                                           //API devolve uma mensagem a dizer que não carregou bem

        public object Result { get; set; } //prop object porque não sei o que vai sair daí
        //caso carregou tudo bem carrego para o objecto result
    }
}
