using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.Servicos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProdutosController( IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitofwork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        //=============================================TESTE================================================
        //acessar arquivo de configuração
        [HttpGet("Autor")]
        public ActionResult<string> GetAutor()
        {
            var autor = _configuration["autor"];
            return $"Autor {autor}";
        }

        //https://localhost:5001/api/Produtos/testeBind/1/
        [HttpGet("testeBind/{id}")]
        public ActionResult<Produto> Get(int id, [BindRequired] string nome) //nome obrigatorio
        {
            return Ok();
        }

        //Fonte de dados dos parametros
        //https://localhost:5001/api/Produtos/saudacao/Ana/
        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> Get([FromServices] IMeuServico meuservico, string nome)
        {
            return Ok(meuservico.Saudacao(nome));
        }
        //=================================================================================================


        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            var produtos = _unitofwork.ProdutoRepository.Get().ToList();
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDTO;
        }

        [HttpGet("Paginado")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetPaginado([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = _unitofwork.ProdutoRepository.GetProdutos(produtosParameters);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevius
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDTO;
        }


        [HttpGet("{id:int:min(1)}", Name ="ObterProduto")] //restrição de rotas
        public ActionResult<ProdutoDTO> Get(int id)
        {
            try
            {
                var produto = _unitofwork.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto == null)
                {
                    return NotFound($"O produto com o id = {id} não foi encontrado");
                }
                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

                return produtoDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar obter o produto  com id = {id} do banco de dados");
            }
        }

        [HttpGet("menorpreco")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutoPorPreco()
        {
            var produtos = _unitofwork.ProdutoRepository.GetProdutosProPreco().ToList();
            var produtoDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtoDTO;
        }

        [HttpPost]
        public ActionResult Post([FromBody] ProdutoDTO produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);

            _unitofwork.ProdutoRepository.Add(produto);
            _unitofwork.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            //por convenção um metodo post deve retornar um header location, onde coloca a localizão do recurso
            //passa o nome da rota, os parametros da action, no corpo da requisição o produto
            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produtoDTO);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produtoDto)
        {
            if(id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            _unitofwork.ProdutoRepository.Update(produto);
            _unitofwork.Commit(); //persiste no bd
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _unitofwork.ProdutoRepository.GetById(prod => prod.ProdutoId == id);

            if(produto == null)
            {
                return NotFound();
            }

            _unitofwork.ProdutoRepository.Delete(produto);
            _unitofwork.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }
    }
}
