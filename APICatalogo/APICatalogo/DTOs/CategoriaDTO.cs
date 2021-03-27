using System.Collections.Generic;

namespace APICatalogo.DTOs
{
    public class CategoriaDTO
    {
        public int CategoriaID { get; set; }
        public string Nome { get; set; }
        public string ImagemUrl { get; set; }

        public ICollection<ProdutoDTO> Produtos { get; set; }
    }
}
