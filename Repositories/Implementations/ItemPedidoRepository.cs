using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public class ItemPedidoRepository : BaseRepository<ItemPedido>, IItemPedidoRepository
    {
        public ItemPedidoRepository(ApplicationContext context) : base(context)
        {
        }

        public ItemPedido GetItemPedido(int itemPedidoId)
        {
            return dbSet.Where(x => x.Id == itemPedidoId).SingleOrDefault();
        }

        public void RemoveItemPedido(int itemPedidoId)
        {
            dbSet.Remove(GetItemPedido(itemPedidoId));
            
        }
    }
}
