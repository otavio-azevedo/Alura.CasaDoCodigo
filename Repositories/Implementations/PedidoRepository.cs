using System;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CasaDoCodigo.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {

        private readonly IHttpContextAccessor contextAccessor;
        private readonly IHttpHelper httpHelper;
        private readonly ICadastroRepository cadastroRepository;

        public PedidoRepository(IConfiguration configuration,
            ApplicationContext context,
            IHttpContextAccessor contextAccessor,
            IHttpHelper sessionHelper,
            ICadastroRepository cadastroRepository) : base(configuration, context)
        {
            this.contextAccessor = contextAccessor;
            this.httpHelper = sessionHelper;
            this.cadastroRepository = cadastroRepository;
        }

        public async Task AddItemAsync(string codigo)
        {
            var produto = await
                            context.Set<Produto>()
                            .Where(p => p.Codigo == codigo)
                            .SingleOrDefaultAsync();

            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado");
            }

            var pedido = await GetPedidoAsync();

            var itemPedido = await
                                context.Set<ItemPedido>()
                                .Where(i => i.Produto.Codigo == codigo
                                        && i.Pedido.Id == pedido.Id)
                                .SingleOrDefaultAsync();

            if (itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);
                await
                    context.Set<ItemPedido>()
                    .AddAsync(itemPedido);

                await context.SaveChangesAsync();
            }
        }

        public async Task<Pedido> GetPedidoAsync()
        {
            var pedidoId = httpHelper.GetPedidoId();
            var pedido =
                await dbSet
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(prod => prod.Categoria)
                .Include(p => p.Cadastro)
                .Where(p => p.Id == pedidoId)
                .SingleOrDefaultAsync();

            if (pedido == null)
            {
                pedido = new Pedido(httpHelper.GetCadastro());
                await dbSet.AddAsync(pedido);
                await context.SaveChangesAsync();
                httpHelper.SetPedidoId(pedido.Id);
            }

            return pedido;
        }

        public async Task<UpdateQuantidadeResponse> UpdateQuantidadeAsync(ItemPedido itemPedido)
        {
            var itemPedidoDB = await GetItemPedidoAsync(itemPedido.Id);

            if (itemPedidoDB != null)
            {
                itemPedidoDB.AtualizaQuantidade(itemPedido.Quantidade);

                if (itemPedido.Quantidade == 0)
                {
                    await RemoveItemPedidoAsync(itemPedido.Id);
                }

                await context.SaveChangesAsync();

                var pedido = await GetPedidoAsync();
                var carrinhoViewModel = new CarrinhoViewModel(pedido.Itens);

                return new UpdateQuantidadeResponse(itemPedidoDB, carrinhoViewModel);
            }

            throw new ArgumentException("ItemPedido não encontrado");
        }

        public async Task<Pedido> UpdateCadastroAsync(Cadastro cadastro)
        {
            var pedido = await GetPedidoAsync();
            await cadastroRepository.UpdateAsync(pedido.Cadastro.Id, cadastro);
            httpHelper.ResetPedidoId();
            httpHelper.SetCadastro(pedido.Cadastro);
            return pedido;
        }

        private async Task<ItemPedido> GetItemPedidoAsync(int itemPedidoId)
        {
            return
            await context.Set<ItemPedido>()
                .Where(ip => ip.Id == itemPedidoId)
                .SingleOrDefaultAsync();
        }

        private async Task RemoveItemPedidoAsync(int itemPedidoId)
        {
            context.Set<ItemPedido>()
                .Remove(await GetItemPedidoAsync(itemPedidoId));
        }
    }
}
