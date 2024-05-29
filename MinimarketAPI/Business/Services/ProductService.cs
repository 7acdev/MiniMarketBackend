using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minimarket.Business.Service;
using Minimarket.Data.Repo;
using Minimarket.DTO;
using Minimarket.Model;

namespace Minimarket.Business.Service;

public class ProductService : IProductService
{
    private readonly IGenericRepo<Product> _model;
    private readonly IMapper _mapper;

    public ProductService(IGenericRepo<Product> model, IMapper mapper)
    {
        _model = model;
        _mapper = mapper;
    }

    public async Task<ProductDTO> Create(ProductDTO model)
    {
        var dbModel = _mapper.Map<Product>(model);
        var res = await _model.Create(dbModel);
        return res.Id != 0 ? _mapper.Map<ProductDTO>(res) 
            : throw new TaskCanceledException("No se pudo crear el producto");
    }

    public async Task<bool> Edit(ProductDTO model)
    {
        var query = await _model.Query(p => p.Id == model.Id)
        .FirstOrDefaultAsync();

        if (query != null)
        {
            query.Name = model.Name;
            query.Category = model.Category;
            query.Stock = model.Stock;
            query.Price = model.Price;
        }
        else
        {
            throw new TaskCanceledException("Producto no encontrado");
        }

        var res = await _model.Edit(query);
        if (!res)
        {
            throw new TaskCanceledException("No se pudo editar el producto");
        }
        return res;
    }

    public async Task<bool> Delete(int id)
    {
        var query = await _model.Query(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (query != null)
        {
            var res = await _model.Delete(query);
            if (!res)
            {
                throw new TaskCanceledException("No se pudo eliminar");
            }
            return res;
        }
        else
        {
            throw new TaskCanceledException("No se encontro el producto");
        }
    }

    public async Task<ProductDTO> GetProduct(int id)
    {
        var query = await _model.Query(p => p.Id == id).FirstOrDefaultAsync();
        return query != null ? _mapper.Map<ProductDTO>(query) 
            : throw new TaskCanceledException("Producto no encontrado");
    }

    public async Task<List<ProductDTO>> ProductList(string seach)
    {
        var query = _model.Query(p => p.Name!.ToLower()
            .Contains(seach.ToLower())
        );

        List<ProductDTO> list = _mapper.Map<List<ProductDTO>>(
            await query.ToListAsync()
        );
        return list;
    }
}