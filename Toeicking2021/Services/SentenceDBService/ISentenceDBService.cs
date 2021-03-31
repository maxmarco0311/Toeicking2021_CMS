using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Services.SentenceDBService
{
    public interface ISentenceDBService
    {
        Task<string> AddSentenceGroup(SentenceInputVM data);
        Task<string> DeleteSenetnce(int id);

    }
}
