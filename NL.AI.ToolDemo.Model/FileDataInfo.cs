using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Model
{
    public class FileDataInfo
    {
        public long Id { get; set; }
        public string PatientName { get; set; }
        public int PatientGender { get; set; }
        public int PatientAge { get; set; }
        public int PatientAgeUnit { get; set; }
        public string LocalFileUrl { get; set; }
        public string AIDiagnosis { get; set; }
        public string Remark { get; set; }
        public bool IsFileLost { get; set; }
        public bool IsDeleted { get; set; }
    }
}
