using System.Collections.ObjectModel;
using Ninject;
using System.Collections.Generic;
using Com.Pinz.Client.Outlook.Service.DAO;
using Com.Pinz.Client.Outlook.Service.Model;

namespace Com.Pinz.Client.Outlook.Service.Impl
{
    public class CategoryService : ICategoryService
    {
        private ICategoryDAO categoryDAO;
        private ObservableCollection<OutlookCategory> categories;

        [Inject]
        public CategoryService(ICategoryDAO categoryDAO)
        {
            this.categoryDAO = categoryDAO;
            categories = new ObservableCollection<OutlookCategory>();
        }


        public ObservableCollection<OutlookCategory> readAll()
        {
            categories.Clear();
            List<OutlookCategory> categoryList = categoryDAO.readAll();
            categoryList.ForEach(categories.Add);

            return categories;
        }

        public void create(OutlookCategory category)
        {
            categories.Add(categoryDAO.create(category));
        }

        public void update(OutlookCategory category)
        {
            categoryDAO.update(category);
        }
    }
}
