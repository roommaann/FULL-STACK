using Models;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class ProductService : IProductService
    {
        public List<ProductModel> GetAllData()
        {
            return CRUD.GetAllData();
        }

        public bool AddProduct(ProductModel product)
        {
            return CRUD.AddProduct(product);
        }

        public bool DeleteProduct(int id)
        {
            return CRUD.DeleteProduct(id);
        }

        public bool UpdateProduct(ProductModel product)
        {
            return CRUD.UpdateProduct(product);
        }
    }
}
