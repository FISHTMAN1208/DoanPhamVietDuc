using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DoanPhamVietDuc.ViewModels
{
	public class RevenueByCategoryViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<CategoryRevenue> _revenues;
		public ObservableCollection<CategoryRevenue> Revenues
		{
			get => _revenues;
			set => SetProperty(ref _revenues, value);
		}

		private DateTime? _startDate;
		public DateTime? StartDate
		{
			get => _startDate;
			set { SetProperty(ref _startDate, value); LoadRevenueAsync(); }
		}

		private DateTime? _endDate;
		public DateTime? EndDate
		{
			get => _endDate;
			set { SetProperty(ref _endDate, value); LoadRevenueAsync(); }
		}

		private ObservableCollection<ISeries> _chartSeries;
		public ObservableCollection<ISeries> ChartSeries
		{
			get => _chartSeries;
			set => SetProperty(ref _chartSeries, value);
		}

		private ObservableCollection<Axis> _xAxes;
		public ObservableCollection<Axis> XAxes
		{
			get => _xAxes;
			set => SetProperty(ref _xAxes, value);
		}

		public ICommand LoadRevenueCommand { get; }
		public ICommand CloseCommand { get; }
		public ICommand ExportPDFCommand { get; }
	

		public RevenueByCategoryViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
			_dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

			Title = "Thống kê doanh thu";
			Revenues = new ObservableCollection<CategoryRevenue>();
			ChartSeries = new ObservableCollection<ISeries>();
			XAxes = new ObservableCollection<Axis>();

			LoadRevenueCommand = new AsyncRelayCommand(async _ => await LoadRevenueAsync());
			CloseCommand = new RelayCommand(_ => CloseWindow());
			
			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadRevenueAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		private async Task LoadRevenueAsync()
		{
			try
			{
				IsBusy = true;
				var revenues = await _dataService.GetRevenueByCategoryAsync(StartDate, EndDate, "Đã thanh toán");
				Revenues = new ObservableCollection<CategoryRevenue>(revenues); // Cải thiện hiệu suất
				UpdateChart(revenues);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải dữ liệu: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void UpdateChart(List<CategoryRevenue> revenues)
		{
			ChartSeries.Clear();
			XAxes.Clear();

			ChartSeries.Add(new ColumnSeries<decimal>
			{
				Name = "Doanh thu (VNĐ)",
				Values = revenues.Select(r => r.Revenue).ToList(),
				Fill = new SolidColorPaint(SKColor.Parse("#10B981")),
				Stroke = null,
				DataLabelsPaint = new SolidColorPaint(SKColors.Black),
				DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N0} VNĐ"
			});

			XAxes.Add(new Axis
			{
				Labels = revenues.Select(r => r.CategoryName).ToList(),
				LabelsRotation = 45,
				TextSize = 12,
				LabelsPaint = new SolidColorPaint(SKColors.Black)
			});

			Console.WriteLine($"Đã tải {revenues.Count} thể loại, tổng doanh thu: {revenues.Sum(r => r.Revenue)}");
		}

		private void CloseWindow()
		{
			foreach (Window window in Application.Current.Windows)
			{
				if (window.DataContext == this)
				{
					window.Close();
					break;
				}
			}
		}
	}
}