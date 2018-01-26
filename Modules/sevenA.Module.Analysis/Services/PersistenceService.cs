namespace sevenA.Module.Analysis.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using LiteDB;

    using sevenA.Core.Helpers;
    using sevenA.Module.Analysis.Models.DTOs;

    public class PersistenceService
    {
        private const string DbCollectionStringRiskFreeRate = "RiskFreeRate";

        private static readonly string DbLocation = Path.Combine(ApplicationHelper.GetAppDataFolder(), "DB.db");
        private static readonly string DbConnectionString = $"filename={DbLocation}; journal=false";
        private readonly LiteDatabase _db = new LiteDatabase(DbConnectionString);

        static PersistenceService()
        {
            Instance = new PersistenceService();
        }

        public static PersistenceService Instance { get; }

        public Task<List<RiskFreeRateDTO>> GetAllRiskFreeRates()
        {
            return Task.Run(() =>
                {
                    var list = new List<RiskFreeRateDTO>();

                    if (_db.CollectionExists(DbCollectionStringRiskFreeRate))
                    {
                        try
                        {
                            var dtos = _db.GetCollection<RiskFreeRateDTO>(DbCollectionStringRiskFreeRate).FindAll();
                            list.AddRange(dtos);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    return list;
                });
        }

        public Task<bool> SaveRiskFreeRate(RiskFreeRateDTO dto)
        {
            return Task.Run(
                () =>
                    {
                        try
                        {
                            var table = _db.GetCollection<RiskFreeRateDTO>(DbCollectionStringRiskFreeRate);
                            table.Upsert(dto);

                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    });
        }
    }
}