using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.ViewModels
{
    public class SearchAccessionPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }
        private readonly RestClient _restClient;

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }
        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }
        private List<AccessionThumbnail> _accessionThumbnailList;
        public List<AccessionThumbnail> AccessionThumbnailList
        {
            get { return _accessionThumbnailList; }
            set { SetProperty(ref _accessionThumbnailList, value); }
        }


        public SearchAccessionPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Search Accession";

            _pageDialogService = pageDialogService;
            _restClient = new RestClient();


            FindCommand = new DelegateCommand(OnFindCommandExecuted);
            ItemTappedCommand = new DelegateCommand(OnItemTappedCommandExecuted);
        }

        public DelegateCommand FindCommand { get; }

        public DelegateCommand ItemTappedCommand { get; }

        private async void OnItemTappedCommandExecuted()
        {
            if (AccessionThumbnailList != null)
            {
                var navigationParams = new NavigationParameters();
                navigationParams.Add("accessionThumbnail", AccessionThumbnailList.FirstOrDefault());
                await NavigationService.GoBackAsync(navigationParams);
            }
        }

        private async void OnFindCommandExecuted()
        {
            Settings.SearchAccessionSearchText = SearchText;
            Settings.SearchAccessionFilter = Filter;

            try
            {
                string query = string.Empty;
                switch (Filter)
                {
                    case "Accession Number":
                        string[] acc_number_parts = _searchText.Trim().Replace("CIP ", "").Split('.');
                        string acc_number_part2 = string.IsNullOrEmpty(acc_number_parts[0]) ? string.Empty : acc_number_parts[0];
                        query = string.Format("@accession.accession_number_part2 like '%{0}%'", acc_number_part2);
                        if ((acc_number_parts.Count() > 1))
                        {
                            query += " and @accession.accession_number_part3 = '" + acc_number_parts[1] + "'";
                        }
                        break;
                    case "Collecting Number":
                        query = string.Format("@accession_inv_name.category_code = 'COLLECTOR' AND @accession_inv_name.plant_name LIKE '%{0}%'", _searchText);
                        break;
                    case "Cultivar Name":
                        query = string.Format("@accession_inv_name.category_code = 'CULTIVAR' AND @accession_inv_name.plant_name LIKE '%{0}%'", _searchText);
                        break;
                    case "Accession Id":
                        query = string.Format("@accession.accession_id = '{0}'", _searchText);
                        break;
                }
                AccessionThumbnailList = await _restClient.SearchAccession(query, "get_accession_thumbnail", "accession");

                if (AccessionThumbnailList == null || AccessionThumbnailList.Count == 0)
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "Sin resultados", "OK");
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(SearchText))
                    SearchText = Settings.SearchAccessionSearchText;

                if (string.IsNullOrEmpty(Filter))
                    Filter = Settings.SearchAccessionFilter;
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}
