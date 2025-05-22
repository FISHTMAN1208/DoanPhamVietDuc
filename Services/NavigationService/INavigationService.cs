using DoanPhamVietDuc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.NavigationService
{
    public interface INavigationService
    {
		BaseViewModel CurrentViewModel { get; }
		event Action CurrentViewModelChanged;
		void NavigateTo<T>() where T : BaseViewModel;
		void NavigateTo<T>(object parameter) where T : BaseViewModel;
	}
}
