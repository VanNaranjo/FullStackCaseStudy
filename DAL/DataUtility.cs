using System.Text.Json;
using CaseStudy.DAL.DomainClasses;
using CaseStudy.DAL;

namespace CaseStudy.DAL
{
    public class DataUtility
    {
        private readonly AppDbContext _db;
        public DataUtility(AppDbContext context)
        {
            _db = context;
        }
        public async Task<bool> LoadGpuInfoFromWebToDb(string stringJson)
        {
            bool brandsLoaded = false;
            bool gpuLoaded = false;
            try
            {
                // an element that is typed as dynamic is assumed to support any operation
                dynamic? objectJson = JsonSerializer.Deserialize<Object>(stringJson);
                brandsLoaded = await LoadBrands(objectJson);
                gpuLoaded = await LoadMenuItems(objectJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return brandsLoaded && gpuLoaded;
        }

        private async Task<bool> LoadBrands(dynamic jsonObjectArray)
        {
            bool loadedBrands = false;
            try
            {
                // clear out the old rows
                _db.Brands?.RemoveRange(_db.Brands);
                await _db.SaveChangesAsync();
                List<String> allBrands = new();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    if (element.TryGetProperty("Brand", out JsonElement gpuDataJson))
                    {
                        allBrands.Add(gpuDataJson.GetString()!);
                    }
                }
                IEnumerable<String> brands = allBrands.Distinct<String>();
                foreach (string braname in brands)
                {
                    Brand bra = new();
                    bra.Name = braname;
                    await _db.Brands!.AddAsync(bra);
                    await _db.SaveChangesAsync();
                }
                loadedBrands = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedBrands;
        }

        private async Task<bool> LoadMenuItems(dynamic jsonObjectArray)
        {
            bool loadedItems = false;
            try
            {
                List<Brand> brands = _db.Brands!.ToList();
                // clear outthe old
                _db.Products?.RemoveRange(_db.Products);
                await _db.SaveChangesAsync();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    Product item = new();

                    item.Id = Convert.ToString(element.GetProperty("SKU").GetString());
                    item.ProductName = Convert.ToString(element.GetProperty("ProductName").GetString());
                    item.GraphicName = Convert.ToString(element.GetProperty("GraphicURL").GetString());
                    item.CostPrice = Convert.ToDecimal(element.GetProperty("CostPrice").GetDecimal());
                    item.MSRP = Convert.ToDecimal(element.GetProperty("MSRP").GetDecimal());
                    item.QtyOnHand = Convert.ToInt32(element.GetProperty("QtyOnHand").GetInt32());
                    item.QtyOnBackOrder = Convert.ToInt32(element.GetProperty("QtyOnBackOrder").GetInt32());
                    item.Description = Convert.ToString(element.GetProperty("Description").ToString());
                    string? bra = element.GetProperty("Brand").GetString();
                    item.TotalVideoMemory = Convert.ToString(element.GetProperty("TotalVideoMemory").ToString());

                    // add the FK here
                    foreach (Brand brand in brands)
                    {
                        if (brand.Name == bra)
                        {
                            item.Brand = brand;
                            break;
                        }
                    }
                    await _db.Products!.AddAsync(item);
                    await _db.SaveChangesAsync();
                }
                loadedItems = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedItems;
        }
    }
}
