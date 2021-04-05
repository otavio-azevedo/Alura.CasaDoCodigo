using CasaDoCodigo.Areas.Identity.Data;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppIdentityUser> userManager;

        public PedidoController(
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            UserManager<AppIdentityUser> userManager)
        {
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
            this.userManager = userManager;
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

            //Busca dados do usuário logado
            var usuario = await userManager.GetUserAsync(this.User);
            pedido.Cadastro.Email = usuario.Email;
            pedido.Cadastro.Nome = usuario.Nome;
            pedido.Cadastro.Telefone = usuario.Telefone;
            pedido.Cadastro.Endereco = usuario.Endereco;
            pedido.Cadastro.Complemento = usuario.Complemento;
            pedido.Cadastro.Bairro = usuario.Bairro;
            pedido.Cadastro.Municipio = usuario.Municipio;
            pedido.Cadastro.CEP = usuario.CEP;
            pedido.Cadastro.UF = usuario.UF;

            return View(pedido.Cadastro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Protege o metódo de chamadas externas a aplicação (Cross-site request forgery)
        [Authorize] //Exige autenticação (login) para acessar a action
        public async Task<IActionResult> Resumo(Cadastro cadastro)
        {
            if (ModelState.IsValid)
            {
                //Busca dados do usuário logado
                var usuario = await userManager.GetUserAsync(this.User);
                usuario.Email       = cadastro.Email;
                usuario.Nome        = cadastro.Nome;
                usuario.Telefone    = cadastro.Telefone;
                usuario.Endereco    = cadastro.Endereco;
                usuario.Complemento = cadastro.Complemento;
                usuario.Bairro      = cadastro.Bairro;
                usuario.Municipio   = cadastro.Municipio;
                usuario.CEP         = cadastro.CEP;
                usuario.UF          = cadastro.UF;
                //Salva as alterações no db do identity
                await userManager.UpdateAsync(usuario);

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
