using APICatalogo.Models;
using System.Collections.Generic;

namespace APICatalogo.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria> 
    {
        IEnumerable<Categoria> GetCategoriasProdutos();
    }
}
