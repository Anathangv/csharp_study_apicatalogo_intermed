using APICatalogo.Context;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories.Implementacoes
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProdutoRepository _produtoRepo;
        private CategoriaRepository _categoriaRepo;
        public AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context);
                //?? indica se _produtoRepo ja existe então passa uma instancia do _produtoRepo e se for nulo então instancia _produtoRepo
            }
        }

        public ICategoriaRepository CategoriaRepository {
            get
            {
                return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context);
            }
        }

        //persistir as informações no banco de dados
        public void Commit()
        {
            _context.SaveChanges();
        }

        //liberando recursos usados 
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
