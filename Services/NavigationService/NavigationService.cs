using DoanPhamVietDuc.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.NavigationService
{
	public class NavigationService : INavigationService
	{
		private readonly IServiceProvider _serviceProvider;
		public BaseViewModel CurrentViewModel { get; private set; }

		public event Action CurrentViewModelChanged;

		public NavigationService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		public void NavigateTo<T>() where T : BaseViewModel
		{
			CurrentViewModel = _serviceProvider.GetRequiredService<T>();
			CurrentViewModelChanged?.Invoke();
		}

		public void NavigateTo<T>(object parameter) where T : BaseViewModel
		{
			T viewModel = _serviceProvider.GetRequiredService<T>();

			if (viewModel is IParameterViewModel parameterViewModel)
			{
				parameterViewModel.Initialize(parameter);
			}

			CurrentViewModel = viewModel;
			CurrentViewModelChanged?.Invoke();
		}
	}

	public interface IParameterViewModel
	{
		void Initialize(object parameter);
	}
}

