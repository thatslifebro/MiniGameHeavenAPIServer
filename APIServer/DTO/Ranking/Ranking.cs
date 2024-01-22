using APIServer.Models;
using CloudStructures.Structures;
using System.Collections.Generic;

namespace APIServer.DTO.Ranking
{
    public class RankingResponse : ErrorCodeDTO
    {
        public List<RedisSortedSetEntry<int>> RankingData { get; set; }
    }

}
