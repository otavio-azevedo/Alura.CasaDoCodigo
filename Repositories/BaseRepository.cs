using CasaDoCodigo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public class BaseRepository<T> where T : BaseModel
    {
        protected readonly IConfiguration configuration;
        protected readonly ApplicationContext context;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(IConfiguration configuration,
            ApplicationContext contexto)
        {
            this.configuration = configuration;
            this.context = contexto;
            dbSet = contexto.Set<T>();
        }
    }
}
