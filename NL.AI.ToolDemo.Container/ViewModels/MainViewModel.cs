using NL.AI.ToolDemo.Container.IViewModels;
using NL.AI.ToolDemo.Model;
using NL.CardioReader.MidEnd.VM.KeyEnum;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VMOnly.BaseLib;
using NL.CardioReader.VoidPower.VMOnly.IF;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NL.AI.ToolDemo.Container.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private const string PREFIX_IMAGESOURCE = "/NL.AI.ToolDemo.Container;component/Images/";
        private Uri _tranformPage = new Uri("/NL.AI.ToolDemo.Container;component/TransformPage.xaml", UriKind.Relative);
        private Uri _waveReviewPage = new Uri("/NL.AI.ToolDemo.Container;component/WaveReviewPage.xaml", UriKind.Relative);
        private Uri _dataListPage = new Uri("/NL.AI.ToolDemo.Container;component/DataListPage.xaml", UriKind.Relative);
        private Uri _ecgViewPage = new Uri("/NL.AI.ToolDemo.Container;component/ECGViewPage.xaml", UriKind.Relative);
        /// <summary>
        /// 头部菜单
        /// </summary>
        private ObservableCollection<MenuNode> _menuList;

        public ObservableCollection<MenuNode> MenuList
        {
            get { return _menuList; }
            set { SetPropertyNotify(ref _menuList, value); }
        }
        /// <summary>
        /// 左侧菜单
        /// </summary>
        private ObservableCollection<MenuNode> _listSource;
        public ObservableCollection<MenuNode> ListSource
        {
            get => _listSource;
            set { SetPropertyNotify(ref _listSource, value); }
        }
        /// <summary>
        /// 头部菜单选中
        /// </summary>
        private MenuNode _selectedItem;
        public MenuNode SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetPropertyNotify(ref _selectedItem, value);
                AddSource(value.Name);
            }
        }

        private MenuNode _selectedMenu;
        /// <summary>
        /// 左侧菜单选中
        /// </summary>
        public MenuNode SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                if (_selectedMenu != value && !value.IsParent)
                {
                    SetPropertyNotify(ref _selectedMenu, value);
                    OnSelectedMenuChanged(value.Name);
                }
            }
        }

        private Uri _currentPage;
        /// <summary>
        /// 页面的路径
        /// </summary>
        public Uri CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    SetPropertyNotify(ref _currentPage, value);
                }
            }
        }

        public ICommand MinCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public MainViewModel(IMessageModule iMessageModule, ICacheManager iCacheManager, ISkyCastleTrailer skyCastleTrailer, IDialogFactory dialogFactory, ICommandFactory commandFactory) : base(iMessageModule, iCacheManager, skyCastleTrailer, dialogFactory, commandFactory)
        {
            MinCommand = _commandFactory.GetCommand<object>(WinMin);
            CloseCommand = _commandFactory.GetCommand<object>(WinClose);

            iMessageModule.Register<NaviKeyEnum>(this, MessagerKeyEnum.PageNavi, Navi);
        }

        private async Task Navi(NaviKeyEnum arg)
        {
            await TaskEx.FromResult(0);
            if(arg == NaviKeyEnum.DiagnosisStation)
            {
                CurrentPage = _ecgViewPage;
            }
        }

        private async Task WinMin(object o)
        {
            await TaskEx.FromResult(0);

            _iMessageModule.Send<WindowOperateEnum>(MessagerKeyEnum.MainWinChanged, WindowOperateEnum.WinMin);
        }
        private async Task WinClose(object o)
        {
            await TaskEx.FromResult(0);

            _iMessageModule.Send<WindowOperateEnum>(MessagerKeyEnum.MainWinChanged, WindowOperateEnum.Close);
        }

        private void CommonTools()
        {
            List<MenuNode> nodes = new List<MenuNode>();
            var node11 = new MenuNode() { Name = "格式转化", Icon = PREFIX_IMAGESOURCE + "transform.png", SelectedIcon = PREFIX_IMAGESOURCE + "transform.png" };
            var node12 = new MenuNode() { Name = "波形查看", Icon = PREFIX_IMAGESOURCE + "wave.png", SelectedIcon = PREFIX_IMAGESOURCE + "wave.png" };
            var node13 = new MenuNode() { Name = "数据列表", Icon = PREFIX_IMAGESOURCE + "data.png", SelectedIcon = PREFIX_IMAGESOURCE + "data.png" };
            nodes.Add(node11);
            nodes.Add(node12);
            nodes.Add(node13);
            ListSource = new ObservableCollection<MenuNode>(nodes);
        }

        private void ECGData()
        {
            List<MenuNode> nodes = new List<MenuNode>();
            var node11 = new MenuNode() { Name = "格式转化", Icon = PREFIX_IMAGESOURCE + "transform.png", SelectedIcon = PREFIX_IMAGESOURCE + "transform.png" };
            var node12 = new MenuNode() { Name = "波形查看", Icon = PREFIX_IMAGESOURCE + "wave.png", SelectedIcon = PREFIX_IMAGESOURCE + "wave.png" };
            var node13 = new MenuNode() { Name = "数据列表", Icon = PREFIX_IMAGESOURCE + "data.png", SelectedIcon = PREFIX_IMAGESOURCE + "data.png" };
            nodes.Add(node11);
            nodes.Add(node12);
            nodes.Add(node13);
            ListSource = new ObservableCollection<MenuNode>(nodes);
        }

        private void OnSelectedMenuChanged(string value)
        {
            switch (value)
            {
                case "格式转化":
                    CurrentPage = _tranformPage;
                    break;
                case "波形查看":
                    CurrentPage = _waveReviewPage;
                    break;
                case "数据列表":
                    CurrentPage = _dataListPage;
                    break;
                default:
                    break;
            }
        }

        private void AddSource(string name)
        {
            switch (name)
            {
                case "心电数据":
                    ECGData();
                    break;
                case "常用工具":
                    CommonTools();
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            List<MenuNode> nodes = new List<MenuNode>();
            var node11 = new MenuNode() { Name = "心电数据", Icon = PREFIX_IMAGESOURCE + "data.png", SelectedIcon = PREFIX_IMAGESOURCE + "data.png" };
            var node12 = new MenuNode() { Name = "常用工具", Icon = PREFIX_IMAGESOURCE + "tool.png", SelectedIcon = PREFIX_IMAGESOURCE + "tool.png" };
            nodes.Add(node11);
            nodes.Add(node12);
            MenuList = new ObservableCollection<MenuNode>(nodes);
            AddSource("心电数据");
            SelectedMenu = ListSource[2];
        }

        protected async override Task Loaded(object o)
        {
            await TaskEx.FromResult(0);           
            Init();
        }       

        protected async override Task UnLoaded(object o)
        {
            await TaskEx.FromResult(0);
        }
    }
}
