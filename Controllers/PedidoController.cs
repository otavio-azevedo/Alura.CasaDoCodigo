using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IPedidoRepository pedidoRepository;

        public PedidoController(IProdutoRepository produtoRepository, IPedidoRepository pedidoRepository)
        {
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
        }

        public async Task<IActionResult> Carrossel()
        {
            return View(await produtoRepository.GetProdutosAsync());
        }

        public async Task<IActionResult> BuscaProdutos(string pesquisa)
        {
            return View(await produtoRepository.GetProdutosAsync(pesquisa));
        }

        [Authorize] //Exige autenticação (login) para acessar a action
        public async Task<IActionResult> Carrinho(string codigoProduto)
        {
            if (!string.IsNullOrEmpty(codigoProduto))
            {
                await pedidoRepository.AddItemAsync(codigoProduto);
            }

            var pedido = await pedidoRepository.GetPedidoAsync();
            List<ItemPedido> itens = pedido.Itens;
            CarrinhoViewModel carrinhoViewModel = new CarrinhoViewModel(itens);
            return base.View(carrinhoViewModel);
        }

        [Authorize] //Exige autenticação (login) para acessar a action
        public async Task<IActionResult> Cadastro()
        {
            var pedido = await pedidoRepository.GetPedidoAsync();

            if (pedido == null)
            {
                return RedirectToAction("Carrossel");
            }

            return View(pedido.Cadastro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Protege o metódo de chamadas externas a aplicação (Cross-site request forgery)
        [Authorize] //Exige autenticação (login) para acessar a action
        public async Task<IActionResult> Resumo(Cadastro cadastro)
        {
            if (ModelState.IsValid)
            {
                return View(await pedidoRepository.UpdateCadastroAsync(cadastro));
            }
            return RedirectToAction("Cadastro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Protege o metódo de chamadas externas a aplicação (Cross-site request forgery)
        [Authorize] //Exige autenticação (login) para acessar a action
        public async Task<UpdateQuantidadeResponse> UpdateQuantidade([FromBody] ItemPedido itemPedido)
        {
            return await pedidoRepository.UpdateQuantidadeAsync(itemPedido);
        }

    }
}
