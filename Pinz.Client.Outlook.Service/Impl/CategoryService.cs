using System.Collections.ObjectModel;
using Ninject;
using System.Collections.Generic;
using Com.Pinz.Client.Outlook.Service.DAO;
using Com.Pinz.Client.Outlook.Service.Model;

namespace Com.Pinz.Client.Outlook.Service.Impl
{
    public class CategoryService : ICategoryService
    {
        private IOutlookService outlookService;
        private ObservableCollection<OutlookCategory> categories;

        [Inject]
        public CategoryService(IOutlookService outlookService)
        {
            this.outlookService = outlookService;
            categories = new ObservableCollection<OutlookCategory>();
        }


        public ObservableCollection<OutlookCategory> ReadAllCategories()
        {
            categories.Clear();
            List<OutlookCategory> categoryList = outlookService.ReadAllCategories();
            categoryList.ForEach(categories.Add);

            return categories;
        }

        public void Create()
        {
            categories.Add(outlookService.CreateCategory());
        }

        public void Update(OutlookCategory category)
        {
            outlookService.UpdateCategory(category);
        }
    }
}
