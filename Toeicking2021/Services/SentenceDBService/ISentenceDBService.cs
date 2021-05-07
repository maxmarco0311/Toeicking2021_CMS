using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Services.SentenceDBService
{
    public interface ISentenceDBService
    {
        Task<string> AddSentenceGroup(SentenceInputVM data);
        Task<string> DeleteSenetnce(int id);
        IQueryable<Sentence> TableAsQueryable();
        Task<List<GA>> GetGrammarsBySentenceId(int sentenceId);
        Task<List<VA>> GetVocAnalysesBySentenceId(int sentenceId);
        Task<List<Vocabulary>> GetVocabularyBySentenceId(int sentenceId);
        Task<List<Vocabulary>> GetAllVocabularies();
        Task<string> UpdateSentence(int sentenceId, string sentence, string chinese);
        Task<string> UpdateGrammars(List<GA> grammars);
        Task<string> UpdateVocAnalysis(List<VA> vocAnalyses);
        Task<string> UpdateVoc(List<Vocabulary> vocabularies);
        Task<string> AddCheckTime(int sentenceId);
        IQueryable<Sentence> GetSentencesWithoutVoice();
        Task<string> UpdateHasAudio(int sentenceId);


    }
}
