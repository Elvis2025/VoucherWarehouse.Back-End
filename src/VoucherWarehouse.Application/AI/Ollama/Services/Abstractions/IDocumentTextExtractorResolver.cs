using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IDocumentTextExtractorResolver : ITransientDependency
{
    IDocumentTextExtractor Resolve(string filePath);
}
