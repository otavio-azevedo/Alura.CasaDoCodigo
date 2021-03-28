using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> GetPedidoAsync();
        Task AddItemAsync(string codigo);
        Task<UpdateQuantidadeResponse> UpdateQuantidadeAsync(ItemPedido itemPedido);
        Task<Pedido> UpdateCadastroAsync(Cadastro cadastro);
    }
}
