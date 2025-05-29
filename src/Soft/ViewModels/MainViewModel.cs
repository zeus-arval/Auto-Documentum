using AD.Services.Common;

namespace Soft.MVVM
{
    public class MainViewModel
    {
        private readonly DIContainer _diContainer;

        public TopBarViewModel TopBarViewModel { get; init; }

        public MainViewModel(DIContainer diContainer)
        {
            _diContainer = diContainer;
            TopBarViewModel = new TopBarViewModel();
        }
    }
}
