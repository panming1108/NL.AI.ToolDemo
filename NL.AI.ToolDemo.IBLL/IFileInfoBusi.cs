using NL.AI.ToolDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.IBLL
{
    public interface IFileInfoBusi
    {
        Task<QueryList<FileDataInfo>> GetAllFileInfos(int pageNo, int pageSize, string patientName, int patientGender, int patientAge, int patientAgeUnit, string localFileUrl, string aIDiagnosis, string remark);
        Task<bool> CreateFileInfo(FileDataInfo fileInfo);
    }
}
