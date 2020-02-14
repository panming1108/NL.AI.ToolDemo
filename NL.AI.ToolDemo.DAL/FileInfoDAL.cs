using NL.AI.ToolDemo.DAL.DBConnect;
using NL.AI.ToolDemo.IDAL;
using NL.AI.ToolDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.DAL
{
    public class FileInfoDAL : DbBaseManagement, IFileInfoDAL
    {
        public async Task<QueryList<FileDataInfo>> GetAllFileInfos(int pageNo, int pageSize, string patientName, int patientGender, int patientAge, int patientAgeUnit, string localFileUrl, string aIDiagnosis, string remark)
        {
            int iCurrentCount = 0;
            while (iCurrentCount < 1)
            {
                var sqlCurrentCount = new StringBuilder();
                sqlCurrentCount.Append("select count(1) from (");
                sqlCurrentCount.Append("select * from FileInfo where 1=1 and IsDeleted = 0 ");
                sqlCurrentCount.Append(GetConditionSql(patientName, patientGender, patientAge, patientAgeUnit, localFileUrl, aIDiagnosis, remark));

                if (pageNo > 0 && pageSize > 0)
                {
                    sqlCurrentCount.Append($" limit {pageSize} offset {pageSize}*{(pageNo - 1)} ");
                }
                sqlCurrentCount.Append(") tmp");
                var sqlString = sqlCurrentCount.ToString();
                iCurrentCount = await ExecuteScalar<int>(sqlString);
                if (iCurrentCount < 1)
                {
                    pageNo--;
                    if (pageNo < 1)
                    {
                        pageNo = 1;
                        break;
                    }
                }
            }
            var sql = new StringBuilder();
            var sqlCount = new StringBuilder();
            sqlCount.Append(" select Count(1) from FileInfo where 1=1  and IsDeleted = 0 ");
            sql.Append(" select * from FileInfo where 1=1  and IsDeleted = 0 ");
            sql.Append(GetConditionSql(patientName, patientGender, patientAge, patientAgeUnit, localFileUrl, aIDiagnosis, remark));
            sqlCount.Append(GetConditionSql(patientName, patientGender, patientAge, patientAgeUnit, localFileUrl, aIDiagnosis, remark));

            if (pageNo != 0 && pageSize != 0)
            {
                sql.Append($" limit {pageSize} offset {pageSize}*{(pageNo - 1)} ");
            }

            var countResult = await ExecuteScalar<int>(sqlCount.ToString());
            var result = await SqlQueryList<FileDataInfo>(sql.ToString());

            return new QueryList<FileDataInfo>() { ItemsCount = countResult, Data = result.ToList(), PageIndex = pageNo, PageSize = pageSize };
        }

        public async Task<bool> InsertFileInfo(FileDataInfo fileInfo)
        {
            string sql =
                @"insert into FileInfo(Id,PatientName,PatientGender,PatientAge,PatientAgeUnit
                            ,LocalFileUrl,AIDiagnosis,Remark,IsFileLost,IsDeleted) values(
                            @Id,@PatientName,@PatientGender,@PatientAge,@PatientAgeUnit
                            ,@LocalFileUrl,@AIDiagnosis,@Remark,false,false)";
            return (await SqlExcute(sql, fileInfo)) > 0;
        }

        private StringBuilder GetConditionSql(string patientName, int patientGender, int patientAge, int patientAgeUnit, string localFileUrl, string aIDiagnosis, string remark)
        {
            var conditionSql = new StringBuilder("");
            if (!string.IsNullOrWhiteSpace(patientName))
            {
                conditionSql.Append($" and PatientName like '%{patientName}%'");
            }
            if (patientGender > 0)
            {
                conditionSql.Append($" and PatientGender = {patientGender}");
            }
            if (patientAge >= 0)
            {
                conditionSql.Append($" and PatientAge = {patientAge}");
            }
            if (patientAgeUnit >= 0)
            {
                conditionSql.Append($" and PatientAgeUnit = {patientAgeUnit}");
            }
            if (!string.IsNullOrWhiteSpace(localFileUrl))
            {
                conditionSql.Append($" and LocalFileUrl like '%{localFileUrl}%'");
            }
            if (!string.IsNullOrWhiteSpace(aIDiagnosis))
            {
                conditionSql.Append($" and AIDiagnosis like '%{aIDiagnosis}%'");
            }
            if (!string.IsNullOrWhiteSpace(remark))
            {
                conditionSql.Append($" and Remark like '%{remark}%'");
            }
            return conditionSql;
        }
    }
}
