using NL.CardioReader.MidEnd.BizObject;
using NL.CardioReader.MidEnd.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.BLL
{
    public class BaseConfigBusi : IBaseConfigBusi
    {
        public Task<List<BODiagCenterConfig>> FindAllDiagCenterConfig()
        {
            throw new NotImplementedException();
        }

        public Task<List<BODoctorConfig>> FindDoctorConfig(long doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BODoctorConfigCategory>> FindDoctorConfigCategory()
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetSysConfigurations(List<string> keys)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateDoctorConfig(long doctorId, List<BODoctorConfig> configInfo)
        {
            throw new NotImplementedException();
        }
    }
}
