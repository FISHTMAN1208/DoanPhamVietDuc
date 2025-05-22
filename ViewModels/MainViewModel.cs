using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Services.NavigationService;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private readonly INavigationService _navigationService;

		public BaseViewModel CurrentViewModel => _navigationService.CurrentViewModel;

		public ICommand NavigateToBookListCommand { get; }
		public ICommand NavigateToCategoryManagerCommand { get; }
		public ICommand NavigateToSupplierManagerCommand { get; }
		public ICommand NavigateToLanguageManagerCommand { get; }
		public ICommand NavigateToBookCoverTypeManagerCommand { get; }
		public ICommand NavigateToImportManagerCommand { get; }


		public MainViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
			Title = "Quản lý sách";

			//Command chuyển danh sách sách
			NavigateToBookListCommand = new RelayCommand(_ => _navigationService.NavigateTo<BookListViewModel>());

			//Command chuyển danh sách danh mục
			NavigateToCategoryManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<CategoryListViewModel>());

			//Command chuyển danh sách nhà cung cấp
			NavigateToSupplierManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<SupplierListViewModel>());

			//Command chuyển danh sách ngôn ngữ
			NavigateToLanguageManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<LanguageListViewModel>());

			//Command chuyển danh sách bìa sách
			NavigateToBookCoverTypeManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<BookCoverTypeListViewModel>());

			//Command chuyển danh sách phiếu nhập
			NavigateToImportManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<ImportListViewModel>());


			//Khởi tạo với BookListViewModel
			_navigationService.NavigateTo<BookListViewModel>();

			// Đăng ký sự kiện khi ViewModel thay đổi
			_navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
		}
		private void OnCurrentViewModelChanged()
		{
			OnPropertyChanged(nameof(CurrentViewModel));
		}
	}
}
