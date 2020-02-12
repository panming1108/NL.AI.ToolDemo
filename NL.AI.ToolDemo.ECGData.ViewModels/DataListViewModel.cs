using NL.AI.ToolDemo.ECGData.IViewModels;
using NL.AI.ToolDemo.IBLL;
using NL.AI.ToolDemo.Model;
using NL.AI.ToolDemo.Tools;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VMOnly.BaseLib;
using NL.CardioReader.VoidPower.VMOnly.IF;
using NL.CardioReader.VoidPower.VVMModule.IF;
using NL.SkyCastle.DXC.Infrastructure.Treasury.Enum;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NL.AI.ToolDemo.ECGData.ViewModels
{
    public class DataListViewModel : ViewModelBase, IDataListViewModel
    {
        private IFileInfoBusi _fileInfoBusi;
        private List<EnumDictionary> _genderSource;
        private int _selectedGender;
        private string _patientName;
        private string _aiDiagnosis;
        private int? _patientAge;
        private int _patientAgeUnit;
        private string _localFileUrl;
        private string _remark;
        private List<FileInfo> _dataInfos;
        private int _pageNo;
        private int _pageSize;
        private int _itemCount;
        private int[] _pageSizeSource;

        public int[] PageSizeSource
        {
            get => _pageSizeSource;
            set
            {
                SetPropertyNotify(ref _pageSizeSource, value);
            }
        }
        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                SetPropertyNotify(ref _itemCount, value);
            }
        }
        public int PageNo
        {
            get { return _pageNo; }
            set
            {
                if (_pageNo != value)
                {
                    _pageNo = value;
                    PageSearch(_pageSize, _pageNo).GetAwaiter();
                    SetPropertyNotify(ref _pageNo, value);
                }
            }
        }
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    int pageCount = (ItemCount - 1) / Math.Max(1, value) + 1;
                    if (PageNo <= pageCount)
                    {
                        PageSearch(_pageSize, _pageNo).GetAwaiter();
                    }
                    SetPropertyNotify(ref _pageSize, value);
                }
            }
        }
        public List<EnumDictionary> GenderSource
        {
            get => _genderSource;
            set => SetPropertyNotify(ref _genderSource, value);
        }
        public string PatientName
        {
            get => _patientName;
            set => SetPropertyNotify(ref _patientName, value);
        }
        public string AIDiagnosis
        {
            get => _aiDiagnosis;
            set => SetPropertyNotify(ref _aiDiagnosis, value);
        }
        public int SelectedGender
        {
            get => _selectedGender;
            set => SetPropertyNotify(ref _selectedGender, value);
        }
        public int? PatientAge
        {
            get => _patientAge;
            set => SetPropertyNotify(ref _patientAge, value);
        }
        public List<FileInfo> DataInfos
        {
            get => _dataInfos;
            set => SetPropertyNotify(ref _dataInfos, value);
        }
        public string LocalFileUrl
        {
            get => _localFileUrl;
            set => SetPropertyNotify(ref _localFileUrl, value);
        }
        public int PatientAgeUnit
        {
            get => _patientAgeUnit;
            set => SetPropertyNotify(ref _patientAgeUnit, value);
        }
        public string Remark
        {
            get => _remark;
            set => SetPropertyNotify(ref _remark, value);
        }

        public ICommand SearchCommand { get; set; }
        public ICommand ImportCommand { get; set; }

        public DataListViewModel(IFileInfoBusi fileInfoBusi, IMessageModule iMessageModule, ICacheManager iCacheManager, ISkyCastleTrailer skyCastleTrailer, IDialogFactory dialogFactory, ICommandFactory commandFactory) : base(iMessageModule, iCacheManager, skyCastleTrailer, dialogFactory, commandFactory)
        {
            _fileInfoBusi = fileInfoBusi;
            InitFields();
            InitCommands();
        }

        private void InitFields()
        {
            _pageNo = 1;
            _pageSize = 10;
            _pageSizeSource = new int[] { 5, 10, 20 };
            _dataInfos = new List<FileInfo>();
            _genderSource = new List<EnumDictionary>();
            _patientName = string.Empty;
            _patientAgeUnit = -1;
        }

        private void InitCommands()
        {
            SearchCommand = _commandFactory.GetCommand<object>(OnSearch);
            ImportCommand = _commandFactory.GetCommand<string>(OnImport);
        }

        private async Task OnImport(string fileUrl)
        {
            await TaskEx.FromResult(0);
            if(System.IO.File.Exists(fileUrl))
            {
                FileInfo fileInfo = new FileInfo()
                {
                    Id = IdWorker.NewDefaultId,
                    PatientName = "李四",
                    PatientGender = 1,
                    PatientAge = 30,
                    PatientAgeUnit = 1,
                    AIDiagnosis = "asdfasdfasdfasdf",
                    Remark = "asdfasdfasdf",
                    LocalFileUrl = fileUrl,
                };
                var result = await _fileInfoBusi.CreateFileInfo(fileInfo);
                var message = _dialogFactory.GetDialog<IMessageDialog>();
                if(result)
                {
                    message.ShowMessage("提示", "导入成功", null, null);
                    await Search(PageNo, PageSize, string.Empty, -1, -1, -1, string.Empty, string.Empty, string.Empty);
                }
                else
                {
                    message.ShowMessage("提示", "导入失败", null, null);
                }
            }
        }

        private async Task OnSearch(object arg)
        {
            await PageSearch(PageSize, PageNo);
        }

        protected async Task PageSearch(int pageSize, int pageNo)
        {
            var patientAge = PatientAge.HasValue ? PatientAge.Value : -1;
            await Search(pageNo, pageSize, PatientName, SelectedGender, patientAge, PatientAgeUnit, LocalFileUrl, AIDiagnosis, Remark);
        }

        private async Task Search(int pageNo, int pageSize, string patientName, int patientGender, int patientAge, int patientAgeUnit, string localFileUrl, string aIDiagnosis, string remark)
        {
            var result = await _fileInfoBusi.GetAllFileInfos(pageNo, pageSize, patientName, patientGender, patientAge, patientAgeUnit, localFileUrl, aIDiagnosis, remark);
            PageSize = result.PageSize;
            PageNo = result.PageIndex;
            ItemCount = result.ItemsCount;
            DataInfos = result.Data?.ToList();
        }

        protected async override Task Loaded(object o)
        {
            GenderSource = EnumExtensions.GetSelectList(typeof(GenderEnum));
            await Search(1, 10, string.Empty, -1, -1, -1, string.Empty, string.Empty, string.Empty);
        }

        protected async override Task UnLoaded(object o)
        {
            await TaskEx.FromResult(0);
        }
    }
}
