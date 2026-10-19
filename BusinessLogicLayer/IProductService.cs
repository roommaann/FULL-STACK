using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public interface IProductService
    {
        List<ProductModel> GetAllData();
        bool AddProduct(ProductModel product);
        bool DeleteProduct(int id);
        bool UpdateProduct(ProductModel product);
    }
}
