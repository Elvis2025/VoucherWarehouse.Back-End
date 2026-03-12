using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcSystem.EcfEVoucher.Dto
{
    public class ReceiveCreditNoteECFInputDto: ReceiveSalesEcfInputDto
    {
        public InformacionReferencia informacionReferencia { get; set; }
    }

    public class InformacionReferencia
    {
        public string nCFModificado { get; set; }
        public string rNCOtroContribuyente { get; set; }
        [StringLength(10)]
        public string fechaNCFModificado { get; set; }
        public int codigoModificacion { get; set; }
        [StringLength(90)]
        public string razonModificacion { get; set; }
    }
}
