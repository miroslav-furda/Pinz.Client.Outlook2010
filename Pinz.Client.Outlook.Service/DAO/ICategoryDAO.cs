using Com.Pinz.Client.Outlook.Service.Model;
using System.Collections.Generic;

namespace Com.Pinz.Client.Outlook.Service.DAO
{
    public interface ICategoryDAO
    {
        List<OutlookCategory> readAll();
        OutlookCategory create(OutlookCategory category);
        OutlookCategory update(OutlookCategory category);
        void delete(OutlookCategory category);
    }
}
