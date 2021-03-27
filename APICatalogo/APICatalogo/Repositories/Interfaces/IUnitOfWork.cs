namespace APICatalogo.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        //propriedades
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        
        //metodo
        void Commit();
    }
}
