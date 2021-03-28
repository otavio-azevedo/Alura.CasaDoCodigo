using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface ICadastroRepository
    {
        Task<Cadastro> UpdateAsync(int cadastroId, Cadastro novoCadastro);
    }
}
