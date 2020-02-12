using NL.AI.ToolDemo.IBLL;
using NL.AI.ToolDemo.IDAL;
using NL.AI.ToolDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.BLL
{
    public class FileInfoBusi : IFileInfoBusi
    {
        private IFileInfoDAL _fileInfoDAL;
        public FileInfoBusi(IFileInfoDAL fileInfoDAL)
        {
            _fileInfoDAL = fileInfoDAL;
        }

        public async Task<bool> CreateFileInfo(FileInfo fileInfo)
        {
            return await _fileInfoDAL.InsertFileInfo(fileInfo);
        }

        public async Task<QueryList<FileInfo>> GetAllFileInfos(int pageNo, int pageSize, string patientName, int patientGender, int patientAge, int patientAgeUnit, string localFileUrl, string aIDiagnosis, string remark)
        {
            return await _fileInfoDAL.GetAllFileInfos(pageNo, pageSize, patientName, patientGender, patientAge, patientAgeUnit, localFileUrl, aIDiagnosis, remark);      
        }
    }
}
