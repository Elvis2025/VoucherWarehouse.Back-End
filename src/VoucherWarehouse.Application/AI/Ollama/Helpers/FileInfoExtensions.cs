using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Helpers;

public static class FileInfoExtensions
{
    public static DateTime LastWriteTimeUtcUtc(this FileInfo fileInfo) => fileInfo.LastWriteTimeUtc;
}
