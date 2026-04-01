using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.BackgroundWorker;

public interface IEcfVoucherRowProcessor : ITransientDependency
{
    Task ProcessAsync(DgiiExcelImportDto row, int rowNumber);
}
